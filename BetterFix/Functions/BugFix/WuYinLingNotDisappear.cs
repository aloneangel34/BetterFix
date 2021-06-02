using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 修复修复持有无影令的NPC被暗杀后，无影令不会消失、而带入该NPC墓中（然后又可能被其他NPC盗墓获得）的BUG
    /// </summary>
    [HarmonyPatch(typeof(WuLinGeneralAssembly), "IsAssassination")]
    public static class WuYinLingNotDisappear
    {
        /// <summary>
        /// 当移除人物时，同时移除人物身上所有的无影令
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值（是否为无影令暗杀对象）</param>
        /// <param name="actorId">要检测的人物ID</param>
        /// <param name="remove">（原方法未实际使用的参数，只有在移除人物时，传入的才为true）疑似为是否移除无影令</param>
        [HarmonyPostfix]
        private static void IsAssassinationPostfix(WuLinGeneralAssembly __instance, bool __result, int actorId, bool remove)
        //原方法签名
        //public bool IsAssassination(int actorId, bool remove = false)
        {
            //如果需要移除无影令 且 人物持有无影令 且 开启了对该BUG的修正
            if (remove && __result && Main.Setting.BugFixWuYinLingNotDisappear.Value)
            {
                //获取人物物品列表
                List<int> actorHaveItemIdList = DateFile.instance.actorItemsDate[actorId].Keys.ToList<int>();

                //遍历物品列表
                for (int i = 0; i < actorHaveItemIdList.Count; i++)
                {
                    //若该物品为无影令
                    if (DateFile.instance.GetItemDate(actorHaveItemIdList[i], 999, true, -1).ParseInt() == 33)
                    {
                        //调试信息
                        if (Main.Setting.debugMode.Value)
                        {
                            QuickLogger.Log(LogLevel.Info, "检测到无影令击杀 移除被击杀者:{0}({1})身上的无影令",
                                //{0}人物姓名
                                DateFile.instance.GetActorName(actorId),
                                //{1}人物ID
                                actorId
                                );
                        }

                        //人物丢失该无影令物品（无影令可堆叠，所以数量要获取总数）
                        DateFile.instance.LoseItem(actorId, actorHaveItemIdList[i], DateFile.instance.GetItemNumber(actorId, actorHaveItemIdList[i]), true);
                        //结束补丁（无影令可堆叠，所以找到一次以后，就不必继续循环了）
                        return;
                    }
                }
            }
        }
    }
}
