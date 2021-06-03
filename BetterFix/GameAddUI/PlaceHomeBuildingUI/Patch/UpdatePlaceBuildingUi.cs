using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 在选择地点菜单更新时，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(ChoosePlaceWindow), "UpdateChoosePlaceMenu", new Type[] { } )]
    public static class UpdateBuildingUiPlaceInUpdateChoosePlaceMenu
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateChoosePlaceMenuPostfix(ChoosePlaceWindow __instance)
        //原方法签名
        //public void UpdateChoosePlaceMenu()
        {
            //QuickLogger.Log(LogLevel.Info, "ChoosePlaceWindow_UpdateChoosePlaceMenu choosePartId:{0} choosePlaceId:{1}", __instance.choosePartId, __instance.choosePlaceId);
            //无需关闭，一直保持开启
            PlaceHomeBuildingUI.SetPlace(__instance.choosePartId, __instance.choosePlaceId);
        }
    }

    /// <summary>
    /// 在过月后，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(UIDate), "DoChangeTrun")]
    public static class UpdateBuildingUiPlaceInDoChangeTrun
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void DoChangeTrunPostfix(UIDate __instance)
        //原方法签名
        //private IEnumerator DoChangeTrun(float waitTime)
        {
            //QuickLogger.Log(LogLevel.Info, "UIDate_DoChangeTrun choosePartId:{0} choosePlaceId:{1} trunChangeing:{2}", WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, UIDate.instance.trunChangeing);

            //无需关闭，一直保持开启
            if (!UIDate.instance.trunChangeing)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }

    /// <summary>
    /// 在主界面显示时，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(HomeSystem), "CloseHomeSystem")]
    public static class UpdateBuildingUiPlaceInCloseHomeSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void CloseHomeSystemPostfix(HomeSystem __instance)
        //原方法签名
        //public void CloseHomeSystem()     
        {
            //QuickLogger.Log(LogLevel.Info, "HomeSystem_CloseHomeSystem choosePartId:{0} choosePlaceId:{1} trunChangeing:{2}", WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, UIDate.instance.trunChangeing);

            //无需关闭，一直保持开启
            if (!UIDate.instance.trunChangeing)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }

    /// <summary>
    /// 在主界面显示时，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(ui_MainUI), "OnShow")]
    public static class UpdateBuildingUiPlaceInOnShow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnShowPostfix(ui_MainUI __instance)
        //原方法签名
        //public override void OnShow()
        {
            //QuickLogger.Log(LogLevel.Info, "ui_MainUI_OnShow choosePartId:{0} choosePlaceId:{1} trunChangeing:{2} Exists:{3}", WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, UIDate.instance.trunChangeing, PlaceHomeBuildingUI.Exists);
            
            //无需关闭，一直保持开启
            if (!UIDate.instance.trunChangeing && !PlaceHomeBuildingUI.Exists)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }

    /// <summary>
    /// 传剑后，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "SetNewMianActor")]
    public static class UpdateBuildingUiPlaceInSetNewMianActor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void SetNewMianActorPostfix(DateFile __instance)
        //原方法签名
        //public void SetNewMianActor(bool randActor)
        {
            //QuickLogger.Log(LogLevel.Info, "DateFile_SetNewMianActor choosePartId:{0} choosePlaceId:{1} trunChangeing:{2}", WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, UIDate.instance.trunChangeing);

            //无需关闭，一直保持开启
            if (!UIDate.instance.trunChangeing)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }

    /// <summary>
    /// 当地点的产业建筑纳入太吾治下时（允许建设），同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "SetHomeMapShow")]
    public static class UpdateBuildingUiPlaceInSetHomeMapShow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void SetHomeMapShowPostfix(DateFile __instance, int partId, int placeId)
        //原方法签名
        //public void SetHomeMapShow(int partId, int placeId, bool open)
        {
            //无需关闭，一直保持开启
            //若纳入太吾治下的地点为选择地点时，更新地点建筑列表
            if (UIManager.Instance.currentUI is ui_MainUI && WorldMapSystem.instance.choosePartId == partId && WorldMapSystem.instance.choosePlaceId == placeId)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }

    /// <summary>
    /// 当创建奇遇时，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "SetStory")]
    public static class UpdateBuildingUiPlaceIn
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void SetStoryPostfix(DateFile __instance, int partId, int placeId)
        //原方法签名
        //public void SetStory(bool show, int partId, int placeId, int storyId, int time = -1, int value1 = 0, int value2 = 0)
        {
            //无需关闭，一直保持开启
            //若不处于过月运算中 且 奇遇的创建地点为选择地点时，更新地点建筑列表
            if (UIManager.Instance.currentUI is ui_MainUI && !UIDate.instance.trunChangeing && WorldMapSystem.instance.choosePartId == partId && WorldMapSystem.instance.choosePlaceId == placeId)
            {
                PlaceHomeBuildingUI.SetPlace(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, true);
            }
        }
    }


#if false
    /// <summary>
    /// 在游戏更新地点（用于获取地点人物）时，同步更新本MOD地点建筑所使用的地点
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "SetPlace")]
    public static class UpdatePlaceBuildingUiPlace
    {
        internal static bool NotChangeShowType = false;
        
        /// <summary>
        /// 在游戏更新地点（用于获取地点人物）时，同步更新本MOD地点建筑所使用的地点
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        [HarmonyPostfix]
        private static void SetPlacePostfix(ui_PlaceActorWindow __instance, int partId, int placeId)
        //原方法签名
        //public static void SetPlace(int partId, int placeId)
        {
            //无需关闭，一直保持开启
            PlaceHomeBuildingUI.SetPlace(partId, placeId, NotChangeShowType);
        }
    }

    /// <summary>
    /// 用于确认是否不切换地点建筑的显示类型
    /// </summary>
    [HarmonyPatch(typeof(WorldMapSystem), "UpdatePlaceActor")]
    public static class NotChangeCheck
    {
        /// <summary>
        /// 原方法执行前开启“不切换”
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        /// <param name="scrollToTop">地点人物列表是否滑动至顶端（原方法未使用，无效）</param>
        [HarmonyPrefix]
        private static void UpdatePlaceActorPrefix(WorldMapSystem __instance)
        //原方法签名
        //public void UpdatePlaceActor(int partId, int placeId, bool scrollToTop = false)
        {
            UpdatePlaceBuildingUiPlace.NotChangeShowType = true;
        }

        /// <summary>
        /// 原方法执行后关闭“不切换”
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdatePlaceActorPostfix(WorldMapSystem __instance)
        //原方法签名
        //public void UpdatePlaceActor(int partId, int placeId, bool scrollToTop = false)
        {
            UpdatePlaceBuildingUiPlace.NotChangeShowType = false;
        }
    }
#endif
}
