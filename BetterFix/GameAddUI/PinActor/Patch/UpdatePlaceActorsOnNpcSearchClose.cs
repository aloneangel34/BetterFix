using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace BetterFix
{
#if true
    /// <summary>
    /// 地点人物上的人物关注按钮更新：ui_NpcSearch界面关闭时
    /// </summary>
    [HarmonyPatch(typeof(UIBase), "OnHide")]
    public static class UpdatePlaceActorsOnNpcSearchClose
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnHidePostfix(UIBase __instance)
        //原方法签名
        //private void OnHide()
        {
            if (Main.Setting.UiPinActors.Value && __instance is ui_NpcSearch && ui_PlaceActorWindow.Exists)
            {
                GameObject ui_PlaceActorWindowGO = GameObject.Find("UIRoot/Canvas/UIWindow/ui_PlaceActorWindow");
                if (ui_PlaceActorWindowGO != null)
                {
                    //QuickLogger.Log(LogLevel.Info, "尝试在NPC界面关闭时刷新地点人物");
                    ui_PlaceActorWindow instance = ui_PlaceActorWindowGO.GetComponent<ui_PlaceActorWindow>();
                    InfinityScroll actorScroll = instance.CGet<InfinityScroll>("ActorScroll");
                    List<int> showingActorList = Traverse.Create(instance).Field<List<int>>("showingActorList").Value;

                    if (showingActorList !=null)
                    {
                        actorScroll.SetDataCount(showingActorList.Count);
                    }

                    //int displayCount = Traverse.Create(instance).Field<List<int>>("showingActorList").Value.Count;
                    //actorScroll.SetDataCount(displayCount);
                }

                //刷新地点人物列表（以便更新关注按钮）
                //ui_PlaceActorWindow.UpdateActorList();
            }
        }
    }
#endif
}
