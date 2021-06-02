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
    /// 更新仅限商人的人物列表，并以此更新商人提示按钮的显示/隐藏
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "UpdateActorList")]
    public static class UpdateOnlyMerchantList
    {

        /// <summary>
        /// 
        /// </summary>
        [HarmonyPostfix]
        private static void UpdateActorListPostfix()
        //原方法签名
        //public static void UpdateActorList()
        {
            if (Main.Setting.UiOnlyMerchantToggle.Value)
            {
                GameObject ui_PlaceActorWindowGO = GameObject.Find("UIRoot/Canvas/UIWindow/ui_PlaceActorWindow");
                if (ui_PlaceActorWindowGO != null)
                {
                    ui_PlaceActorWindow instance = ui_PlaceActorWindowGO.GetComponent<ui_PlaceActorWindow>();
                    OnlyMerchantSupport.OnlyMerchantList = OnlyMerchantSupport.GetOnlyMerchantList(Traverse.Create(instance).Field<List<int>>("normalList").Value, true);

                    Transform onlyMerchantToggleTransform = ui_PlaceActorWindowGO.transform.Find("BetterFix.OnlyMerchantToggle");
                    if (onlyMerchantToggleTransform != null)
                    {
                        bool needShow = OnlyMerchantSupport.OnlyMerchantList.Count > 0;
                        if (onlyMerchantToggleTransform.gameObject.activeSelf != needShow)
                        {
                            onlyMerchantToggleTransform.gameObject.SetActive(OnlyMerchantSupport.OnlyMerchantList.Count > 0);
                        }
                        //OnlyMerchantSupport.IsResetting = true;
                        onlyMerchantToggleTransform.GetComponent<Toggle>().isOn = false;
                    }
                }
            }
            //else
            //{
            //    GameObject ui_PlaceActorWindowGO = GameObject.Find("UIRoot/Canvas/UIWindow/ui_PlaceActorWindow");
            //    if (ui_PlaceActorWindowGO != null)
            //    {
            //        Transform onlyMerchantToggleTransform = ui_PlaceActorWindowGO.transform.Find("BetterFix.OnlyMerchantToggle");
            //        if (onlyMerchantToggleTransform != null && onlyMerchantToggleTransform.gameObject.activeSelf)
            //        {
            //            onlyMerchantToggleTransform.GetComponent<Toggle>().isOn = false;
            //            onlyMerchantToggleTransform.gameObject.SetActive(false);
            //        }
            //    }
            //}
        }
    }
}
