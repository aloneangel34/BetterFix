using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 在进入游戏初始化地图后，建立/重设地点建筑界面UI
    /// </summary>
    [HarmonyPatch(typeof(WorldMapSystem), "InitWorldMap")]
    public static class CreatAndAttachUI
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void InitWorldMapPostfix(WorldMapSystem __instance)
        //原方法签名
        //public IEnumerator InitWorldMap()
        {
            //SetPlaceIcon.StoryIcon = __instance.placePrefab.storyImage;
            if (Main.Setting.UiAddPlaceBuildUI.Value && !DateFile.instance.doMapMoveing)
            {
                Main.InGameBuildingUI = new PlaceHomeBuildingUI();
                Main.InGameBuildingUI.AttachUI();
            }
        }
    }
}
