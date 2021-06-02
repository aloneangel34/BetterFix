using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 在主界面隐藏时，隐藏更新本MOD地点建筑UI
    /// </summary>
    [HarmonyPatch(typeof(ui_MainUI), "OnHide")]
    public static class HideBuildingUiInOnHide
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnHidePostfix(ui_MainUI __instance)
        //原方法签名
        //public override void OnHide()
        {
            //无需关闭，一直保持开启
            if (Main.InGameBuildingUI != null)
            {
                //QuickLogger.Log(LogLevel.Info, "ui_MainUI_OnHide_HideUI");

                Main.InGameBuildingUI.SetUIActive(false, false, true);
            }
        }
    }

    /// <summary>
    /// 在显示HomeSystem时，隐藏更新本MOD地点建筑UI
    /// </summary>
    [HarmonyPatch(typeof(HomeSystem), "ShowHomeSystem")]
    public static class HideBuildingUiInShowHomeSystem
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void ShowHomeSystemPostfix(HomeSystem __instance)
        //原方法签名
        //public void ShowHomeSystem(bool baseHome)
        {
            //因为Traverse挖掘私有字段比较耗资源，所以放到最后判定，这样若前面已判定为true就不用调用了
            bool noNeedChange = DateFile.instance.playerMoveing || Main.InGameBuildingUI == null || Traverse.Create(__instance).Field<bool>("isShowHomeSystem").Value;
            
            if (!noNeedChange)
            {
                //QuickLogger.Log(LogLevel.Info, "HomeSystem_ShowHomeSystem_HideUI playerMoveing:{0} isShowHomeSystem:{1}", DateFile.instance.playerMoveing, Traverse.Create(__instance).Field<bool>("isShowHomeSystem").Value);

                Main.InGameBuildingUI.SetUIActive(false);
            }
        }
    }

    /// <summary>
    /// 在显示HomeSystem时，隐藏更新本MOD地点建筑UI
    /// </summary>
    [HarmonyPatch(typeof(HomeSystemWindow), "ShowHomeView")]
    public static class HideBuildingUiInShowHomeView
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void ShowHomeViewPostfix(HomeSystemWindow __instance)
        //原方法签名
        //public void ShowHomeView(int partID, int placeID)
        {
            if (Main.InGameBuildingUI != null)
            {
                //QuickLogger.Log(LogLevel.Info, "HomeSystemWindow_ShowHomeView_HideUI");

                Main.InGameBuildingUI.SetUIActive(false);
            }
        }
    }
}
