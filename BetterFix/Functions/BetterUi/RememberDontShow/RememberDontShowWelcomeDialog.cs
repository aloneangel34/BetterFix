using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 记忆不显示进入游戏时的版本信息公告
    /// </summary>
    [HarmonyPatch(typeof(MainMenu), "ShowWelcomeDialog")]
    public static class RememberDontShowWelcomeDialog
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool ShowWelcomeDialogPrefix(MainMenu __instance)
        //原方法签名
        //private void ShowWelcomeDialog()
        {
            if (Main.Setting.RememberDontShowWelcomeDialog.Value)
            {
                Traverse.Create(__instance).Field<bool>("showStartMassage").Value = false;
                return false;
            }

            return true;
        }
    }
}
