using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 修复武林大会金刚宗灌顶不会随机到定力的BUG
    /// </summary>
    [HarmonyPatch(typeof(MessageEventManager), "EndEvent20979_1")]
    public static class Event20979WrongRandowRange
    {
        /// <summary>
        /// 复制原方法（武林大会金刚宗灌顶），修正主要属性序号的取值范围（跳过原方法不再执行）
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool EndEvent20979_1Prefix(MessageEventManager __instance)
        //原方法签名
        //public void EndEvent20979_1()
        {
            if (Main.Setting.BugFixEvent20979WrongRandowRange.Value)
            {
                //（未变动）
                //太吾人物ID
                int mianActorId = DateFile.instance.mianActorId;
                //（有修改）
                //原代码：取值范围为 61～65（因为Range方法是左开右闭的）不会为66、所以原本游戏中武林大会金刚宗灌顶才不会随机到定力
                //int actorMainAttributeIndex = 61 + UnityEngine.Random.Range(0, 5);
                //现在修改为：取值范围为 61～66
                //人物主要属性的属性序号：61膂力 62体质 63灵敏 64根骨 65悟性 66定力
                int actorMainAttributeIndex = 61 + UnityEngine.Random.Range(0, 6);

                //调试信息
                if (Main.Setting.debugMode.Value)
                {
                    string attName = DateFile.instance.massageDate[1002][1].Split('|')[actorMainAttributeIndex - 61 + 1];
                    QuickLogger.Log(LogLevel.Info, "本次金刚宗灌顶随机到的属性:{0} 序号:{1}", attName, actorMainAttributeIndex);
                }

                //（未变动）
                //设置太吾该项主要属性的基础值为(基础值+当前固定值加成)的1.35倍
                Characters.SetCharProperty(mianActorId, actorMainAttributeIndex, string.Format("{0}", (int)((double)DateFile.instance.GetActorDate(mianActorId, actorMainAttributeIndex, true).ParseInt() * 1.35)));

                return false;
            }

            return true;
        }
    }
}
