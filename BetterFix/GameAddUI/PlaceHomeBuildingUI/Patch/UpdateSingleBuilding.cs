using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 在产业建筑更新时，更新对应的地点建筑
    /// </summary>
    [HarmonyPatch(typeof(HomeSystemWindow), "UpdateHomePlace")]
    public static class UpdateSingleBuilding
    {
#if false
        /// <summary>
        /// 防止在产业建筑窗口未开启时的无效刷新
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool UpdateHomePlacePrefix(HomeSystemWindow __instance, int partId, int placeId, int buildingIndex)
        //原方法签名
        {
            Patches patches1 = Harmony.GetPatchInfo(typeof(HomeSystemWindow).GetMethod("UpdateHomePlace"));
            Patches patches2 = Harmony.GetPatchInfo(typeof(HomeBuilding).GetMethod("UpdateBuilding"));

            Main.Logger.LogDebug("HomeSystemWindow.UpdateHomePlace 的所有前置补丁列表:");
            for (int i = 0; i < patches1.Prefixes.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches1.Prefixes[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches1.Prefixes[i].owner);
                Main.Logger.LogDebug("优先级: " + patches1.Prefixes[i].priority);
                Main.Logger.LogDebug("--------");
            }
            Main.Logger.LogDebug("HomeSystemWindow.UpdateHomePlace 的所有后置补丁列表:");
            for (int i = 0; i < patches1.Postfixes.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches1.Postfixes[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches1.Postfixes[i].owner);
                Main.Logger.LogDebug("优先级: " + patches1.Postfixes[i].priority);
                Main.Logger.LogDebug("--------");
            }
            Main.Logger.LogDebug("HomeSystemWindow.UpdateHomePlace 的所有最终补丁列表:");
            for (int i = 0; i < patches1.Finalizers.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches1.Finalizers[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches1.Finalizers[i].owner);
                Main.Logger.LogDebug("优先级: " + patches1.Finalizers[i].priority);
                Main.Logger.LogDebug("--------");
            }

            Main.Logger.LogDebug("\n\nHomeBuilding.UpdateBuilding 的所有前置补丁列表:");
            for (int i = 0; i < patches2.Prefixes.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches2.Prefixes[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches2.Prefixes[i].owner);
                Main.Logger.LogDebug("优先级: " + patches2.Prefixes[i].priority);
                Main.Logger.LogDebug("--------");
            }
            Main.Logger.LogDebug("HomeBuilding.UpdateBuilding 的所有后置补丁列表:");
            for (int i = 0; i < patches1.Postfixes.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches2.Postfixes[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches2.Postfixes[i].owner);
                Main.Logger.LogDebug("优先级: " + patches2.Postfixes[i].priority);
                Main.Logger.LogDebug("--------");
            }
            Main.Logger.LogDebug("HomeBuilding.UpdateBuilding 的所有最终补丁列表:");
            for (int i = 0; i < patches1.Finalizers.Count; i++)
            {
                Main.Logger.LogDebug("补丁序号: " + patches2.Finalizers[i].index);
                Main.Logger.LogDebug("所属HarmonyID: " + patches2.Finalizers[i].owner);
                Main.Logger.LogDebug("优先级: " + patches2.Finalizers[i].priority);
                Main.Logger.LogDebug("--------");
            }

            //if (partId == HomeSystem.instance.homeMapPartId && placeId == HomeSystem.instance.homeMapPlaceId)
            //{
            //    QuickLogger.Log(LogLevel.Info, "更新产业建筑 partId:{0} placeId:{1} isShowHomeSystem:{2} WindowExists:{3} homeBuldingCount:{4}",
            //        HomeSystem.instance.homeMapPartId,
            //        HomeSystem.instance.homeMapPlaceId,
            //        Traverse.Create(HomeSystem.instance).Field<bool>("isShowHomeSystem").Value,
            //        HomeSystemWindow.Exists,
            //        HomeSystemWindow.Instance.allHomeBulding.Length
            //        );
            //    // :{5} :{6} :{7} :{8} :{9} :{10}
            //}

            //if (Main.Setting.UiAddPlaceBuildUI.Value && Main.InGameBuildingUI != null)
            //{
            //    if (!Traverse.Create(HomeSystem.instance).Field<bool>("isShowHomeSystem").Value)
            //    {
            //        __instance.gameObject.SetActive(false);
            //        return false;
            //    }
            //}

            return true;
        }
#endif
        /// <summary>
        /// 在产业建筑更新时，更新对应的地点建筑
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateHomePlacePostfix(HomeSystemWindow __instance, int partId, int placeId, int buildingIndex)
        //原方法签名
        {
            if (Main.Setting.UiAddPlaceBuildUI.Value && Main.InGameBuildingUI != null)
            {
                if (Main.InGameBuildingUI._showBuildingType != 9 && PlaceHomeBuildingUI._partId == partId && PlaceHomeBuildingUI._placeId == placeId)
                {
                    if (Main.InGameBuildingUI._showBuildingList != null && Main.InGameBuildingUI._showBuildingList.Contains(buildingIndex))
                    {
                        foreach (var placeBuilding in Main.InGameBuildingUI.buildingInfiniteScrollView.dataList)
                        {
                            string[] nameTexts = placeBuilding.GetComponent<SetPlaceIcon>().buildingButton.name.Split(',');
                            if (nameTexts.Length == 4 && int.TryParse(nameTexts[3], out int index) && index == buildingIndex)
                            {
                                //QuickLogger.Log(LogLevel.Info, "确认需要同步更新地点建筑 name:{0}", placeBuilding.name);
                                placeBuilding.GetComponent<SetPlaceIcon>().SetBuilding(partId, placeId, buildingIndex);
                            }
                        }
                    }
                }
            }
        }
    }
}
