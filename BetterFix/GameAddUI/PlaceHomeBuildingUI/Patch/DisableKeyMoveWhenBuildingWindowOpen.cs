using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 在打开建筑窗口时，禁用键盘快捷移动
    /// </summary>
    [HarmonyPatch(typeof(WorldMapSystem), "GetMoveKey")]
    public static class DisableKeyMoveWhenBuildingWindowOpen
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static bool GetMoveKeyPrefix(WorldMapSystem __instance)
        //原方法签名
        //private void GetMoveKey(int typ)
        {
            if (BuildingWindow.instance != null && BuildingWindow.instance.buildingWindowOpend)
            {
                Traverse.Create(__instance).Field<bool>("moveButtonDown").Value = false;
                return false;
            }

            return true;
        }
    }
}
