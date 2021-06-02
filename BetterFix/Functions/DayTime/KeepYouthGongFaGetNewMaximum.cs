using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using System.Threading;

namespace BetterFix
{
    /// <summary>
    /// 修正人物当月的最大行动力
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "GetMaxDayTime")]
    public static class KeepYouthGongFaGetNewMaximum
    {
        /// <summary>
        /// 在原方法结束后检测是否满足不老功法的特效要求，若满足则最大行动力修正为18岁人物的行动力
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        [HarmonyPostfix]
        private static void GetMaxDayTimePostfix(DateFile __instance, ref int __result)
        //原方法签名
        //public int GetMaxDayTime()
        {
            //仅在主线程（CurrentThread.ManagedThreadId == 1）时进行修正
            //（因为涉及到读取人物特性->触发读取人物特性缓存->由于线程ID不为1，缓存字典中没有对应的Key而报错）
            //已知冲突：“太吾修改器MOD”的“行动力不减”功能是通过额外线程来调用“GetMaxDayTime”方法的。原方法不涉及读取人物特性所以没问题，但本功能需要读取特性就会有问题
            if (Thread.CurrentThread.ManagedThreadId == 1 && Main.Setting.DayTimeKeepYouthGongFaGetNewMaximum.Value && __instance.teachingOpening / 100 != 4)
            {
                //太吾的人物ID
                int taiwuId = __instance.MianActorID();
                if (Characters.HasChar(taiwuId))
                {
                    try
                    {
                        //太吾性别
                        int taiwuGender = int.Parse(__instance.GetActorDate(taiwuId, 14, false));

                        //若太吾性别ID为正
                        if (taiwuGender > 0)
                        {
                            //若 太吾已成年 且 太吾为童男/童女
                            if (__instance.MianAge() > 14 && HealthFeatureSupport.GetFeatureListFromString(__instance.GetActorDate(taiwuId, 101, false)).Contains(4001))
                            {
                                //遍历太吾装备的内功
                                foreach (int equipNeigongGongFaId in __instance.GetActorEquipGongFa(taiwuId)[0])
                                {
                                    //若遍历到的内功ID为正
                                    if (equipNeigongGongFaId > 0)
                                    {
                                        //该内功的心法类型（0正练 1逆练 2冲解）
                                        int gongFaFTyp = __instance.GetGongFaFTyp(taiwuId, equipNeigongGongFaId);

                                        //若该内功的心法类型为正，且不为冲解
                                        if (gongFaFTyp >= 0 && gongFaFTyp != 2)
                                        {
                                            //获取该内功的正/逆练特效ID
                                            int gongFaPowerId = int.Parse(__instance.gongFaDate[equipNeigongGongFaId][103 + gongFaFTyp]);

                                            //若太吾的性别满足该功法特效的所需性别
                                            if (int.Parse(__instance.gongFaFPowerDate[gongFaPowerId][12]) == taiwuGender)
                                            {
#if false
                                                //调试信息
                                                if (Main.Setting.debugMode.Value)
                                                {
                                                    //string gongfaNameText = __instance.gongFaDate[equipNeigongGongFaId][0];
                                                    //gongfaNameText = gongfaNameText.Substring((gongfaNameText.IndexOf('>') + 1), (gongfaNameText.LastIndexOf('<') - gongfaNameText.IndexOf('>') - 1));
                                                    
                                                    QuickLogger.Log(LogLevel.Info, "修正最大行动力 原本:{0} -> 18岁:{1} 太吾年龄:{2} 不老功法:{3} ",
                                                        //{0}原本最大行动力
                                                        __result,
                                                        //{1}18岁最大行动力
                                                        __instance.ageDate[18][1],
                                                        //{2}太吾年龄
                                                        __instance.MianAge(),
                                                        //{3}不老功法名称
                                                        DisplaySupport.RemoveColorCodeInRichtext(__instance.gongFaDate[equipNeigongGongFaId][0])
                                                        );
                                                }
#endif
                                                //将原方法返回的当月最大行动力设为18岁人物的最大行动力
                                                __result = int.Parse(__instance.ageDate[18][1]);

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //调试信息
                        if (Main.Setting.debugMode.Value)
                        {
                            QuickLogger.Log(LogLevel.Info, "修正最大行动力出错 taiwuId:{0} {1}\n{2}", taiwuId, Thread.CurrentThread.ManagedThreadId, ex);
                        }
                    }
                }
            }
        }
    }
}
