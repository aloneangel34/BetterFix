using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace BetterFix
{
    /// <summary>
    /// 讨要事件惩罚降低：判断部分
    /// </summary>
    [HarmonyPatch(typeof(MessageEventManager), "EndEvent207_2")]
    public static class ReduceRefusePenaltyCheckInEndEvent
    {
        internal static bool AllAskingNeedFix = false;

        /// <summary>
        /// 原方法执行前，若满足条件则开启相关修正
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static void EndEvent207_2Prefix(MessageEventManager __instance)
        //原方法签名
        //private void EndEvent207_2()
        {
            if (Main.Setting.NpcAskingEventReduceRefusePenalty.Value)
            {
                AllAskingNeedFix = true;
            }
        }

        /// <summary>
        /// 原方法执行完后关闭修正
        /// </summary>
        [HarmonyPostfix]
        private static void EndEvent207_2Postfix()
        //原方法签名
        //private void EndEvent207_2()
        {
            AllAskingNeedFix = false;
        }
    }
}
