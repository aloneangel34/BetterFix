using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUIKit.GameObjects;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 商人提示按钮支援方法集
    /// </summary>
    public static class OnlyMerchantSupport
    {
        //internal static GameObject CreatedOnlyMerchantToggle = null;
        internal static List<int> OnlyMerchantList = new List<int>();
        //internal static bool IsResetting = false;

        /// <summary>
        /// 新建一个商人提示按钮
        /// </summary>
        /// <returns>商人提示按钮的RectTransform</returns>
        public static RectTransform NewOnlyMerchantToggle()
        {
            OnlyMerchantToggle onlyMerchantToggle = new OnlyMerchantToggle
            {
                Name = "BetterFix.OnlyMerchantToggle",
                TipTitle = "商人提示按钮",
                TipContant = "仅当选择地点有商人时出现\n（婴儿除外）\n\n点击以单独显示该地点的商人\n如要显示全部人物请点击左侧游戏原生按钮",
                DefaultActive = false,
            };

            return onlyMerchantToggle.RectTransform;
        }

        /// <summary>
        /// 商人提示按钮的点击事件
        /// </summary>
        /// <param name="isOn">本次点击后的开关状态</param>
        /// <param name="tg">商人提示按钮所对应的UnityToggle</param>
        /// <param name="instance">ui_PlaceActorWindow实例</param>
        public static void MerchantOnValueChanged_Invoke(bool isOn, UnityEngine.UI.Toggle tg, ui_PlaceActorWindow instance)
        {
            InfinityScroll actorScroll = instance.CGet<InfinityScroll>("ActorScroll");

            if (isOn)
            {
                List<CToggle> allCToggles = instance.CGet<CToggleGroup>("TypeTogGroup").GetAll();
                for (int i = 0; i < allCToggles.Count; i++)
                {
                    allCToggles[i].isOn = false;
                }

                Traverse.Create(instance).Field<List<int>>("showingActorList").Value = OnlyMerchantSupport.OnlyMerchantList;

                actorScroll.SetDataCount(OnlyMerchantSupport.OnlyMerchantList.Count);
                if (OnlyMerchantSupport.OnlyMerchantList.Count > 0 && actorScroll.curTopIndex != 0)
                {
                    actorScroll.Refresh(0);
                }

                Traverse.Create(instance).Field<int>("actorType").Value = -1;
                tg.isOn = false;
            }
            //else if (OnlyMerchantSupport.IsResetting)
            //{
            //    OnlyMerchantSupport.IsResetting = false;
            //}
            //else
            //{
            //    CToggleGroup typeTogGroup = instance.CGet<CToggleGroup>("TypeTogGroup");
            //    for (int i = 0; i < typeTogGroup.Count(); i++)
            //    {
            //        CToggle ctoggle = typeTogGroup.Get(i);
            //        if (ctoggle.interactable)
            //        {
            //            //Traverse.Create(instance).Field<int>("actorType").Value = ctoggle.key;
            //            //ctoggle.isOn = true;
            //            //ui_PlaceActorWindow.UpdateActorList();

            //            typeTogGroup.Set(ctoggle, true);
            //            break;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 从给定的人物ID列表中获取身份仅限为商人（有“浏览货物…”互动）的人物ID列表
        /// </summary>
        /// <param name="actorIdList">给定的人物ID列表</param>
        /// <param name="excludeBaby">是否排除婴儿（无法实际对话）</param>
        /// <returns>身份仅限为商人的人物ID列表</returns>
        public static List<int> GetOnlyMerchantList(List<int> actorIdList, bool excludeBaby = false)
        {
            List<int> result = new List<int>();
            if (actorIdList == null)
            {
                return result;
            }

            foreach (int actorId in actorIdList)
            {
                if (DisplaySupport.IsActorMerchant(actorId, excludeBaby))
                {
                    result.Add(actorId);
                }
            }
            return result;
        }

    }
}
