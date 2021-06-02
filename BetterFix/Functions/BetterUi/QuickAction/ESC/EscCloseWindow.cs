using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 太吾经历记录界面允许使用ESC键关闭
    /// </summary>
    [HarmonyPatch(typeof(TipsWindow), "LateUpdate")]
    public static class EscCloseWindowInTipsWindow
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void LateUpdatePostfix(TipsWindow __instance)
        //原方法签名
        {
            if (__instance.tipsReadWindow.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape) && Main.Setting.UiEscQuickAction.Value)
            {
                __instance.ColseTipsRead();
            }
        }
    }
}
