using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 记忆不显示走火入魔提示
    /// </summary>
    [HarmonyPatch(typeof(StudyWindow), "SetStudyButton")]
    public static class RememberDontShowStudyMad
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static void SetStudyButtonPrefix(StudyWindow __instance)
        //原方法签名
        //public void SetStudyButton(int index)
        {
            if (Main.Setting.RememberDontShowStudyMad.Value)
            {
                Traverse.Create(__instance).Field<bool>("showDanger").Value = true;
                //QuickLogger.Log(LogLevel.Info, "记忆不显示走火入魔提示 showDanger:{0}", Traverse.Create(__instance).Field<bool>("showDanger").Value);
            }
        }
    }
}
