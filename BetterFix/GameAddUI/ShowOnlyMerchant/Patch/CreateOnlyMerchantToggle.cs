using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 在地点人物界面创建时加入“商人提示按钮”
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "Awake")]
    public static class CreateOnlyMerchantToggle
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void AwakePostfix(ui_PlaceActorWindow __instance)
        //原方法签名
        {
            if (Main.Setting.UiOnlyMerchantToggle.Value)
            {
                if (__instance.transform.Find("BetterFix.OnlyMerchantToggle") == null)
                {
                    RectTransform onlyMerchantToggleRect = OnlyMerchantSupport.NewOnlyMerchantToggle();
                    Toggle onlyMerchantUnityToggle = onlyMerchantToggleRect.GetComponent<Toggle>();
                    onlyMerchantUnityToggle.onValueChanged.AddListener((bool value) => OnlyMerchantSupport.MerchantOnValueChanged_Invoke(value, onlyMerchantUnityToggle, __instance));
                    onlyMerchantToggleRect.SetParent(__instance.transform, false);
                }
            }
        }
    }
}
