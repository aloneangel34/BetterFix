using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;

namespace BetterFix
{
    /// <summary>
    /// ui_PlaceActorWindow创建人物关注按钮：UI主体部分
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "Awake")]
    public static class CreatePinTogglePlaceActorAwake
    {
        //internal static Transform Button = null;
        //internal static GameObject PinActorToggle = null;

        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static void AwakePrefix(ui_PlaceActorWindow __instance)
        //原方法签名
        {
            if (Main.Setting.UiPinActors.Value)
            {
                Transform placeActorButton = __instance.transform.Find("ActorView/ActorViewport/PlaceActor/Actor,id");

                if (placeActorButton != null && placeActorButton.transform.Find("BetterFix.PinToggle") == null)
                {
                    RectTransform pinToggle = PinAcotrSupport.NewPinToggle();
                    pinToggle.SetParent(placeActorButton.transform, false);
                    //placeActorButton.gameObject.AddComponent<PinDisplayPointerEnter>();
                    //PinActorToggle = pinToggle.gameObject;
                }
            }
        }
    }
}
