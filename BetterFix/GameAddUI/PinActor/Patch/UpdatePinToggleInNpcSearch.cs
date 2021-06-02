using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// ui_NpcSearch渲染人物关注按钮
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "OnRenderNpc")]
    public static class UpdatePinToggleInNpcSearchOnRenderNpc
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnRenderActorPostfix(ui_NpcSearch __instance, int index, Refers obj)
        //原方法签名
        //private void OnRenderNpc(int index, Refers obj)
        {
            if (Main.Setting.UiPinActors.Value)
            {
                //若能找到actorId（即该人物图标已被正确渲染）
                if (PinAcotrSupport.TryGetActorIdFromName(obj.name, out int actorId))
                {
                    Transform pinToggle = obj.transform.Find("BetterFix.PinToggle");
                    //若该人物图标上未含有人物关注按钮
                    if (pinToggle == null)
                    {
                        pinToggle = PinAcotrSupport.NewPinToggle(20, -20);
                        pinToggle.SetParent(obj.transform, false);
                    }

                    //关注按钮的Toggle组件
                    UnityEngine.UI.Toggle unityToggle = pinToggle.GetComponent<UnityEngine.UI.Toggle>();
                    //先清空
                    unityToggle.onValueChanged.RemoveAllListeners();
                    //设置组件的开关状态
                    unityToggle.isOn = PinAcotrSupport.IsActorAlreadyPined(actorId);
                    //再加入Listener
                    unityToggle.onValueChanged.AddListener((bool value) => PinAcotrSupport.PinOnValueChanged_Invoke(value, unityToggle));
                }
            }
            else
            {
                Transform pinToggle = obj.transform.Find("BetterFix.PinToggle");
                if (pinToggle != null && pinToggle.gameObject.activeSelf)
                {
                    //关注按钮的Toggle组件
                    UnityEngine.UI.Toggle unityToggle = pinToggle.GetComponent<UnityEngine.UI.Toggle>();
                    //先清空
                    unityToggle.onValueChanged.RemoveAllListeners();
                    //设置组件的开关状态
                    unityToggle.isOn = false;
                    //
                    pinToggle.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// ui_NpcSearch更新关注人物列表
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "UpdateNpcList")]
    public static class UpdatePinedActorList
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool UpdateNpcListPrefix(ui_NpcSearch __instance, bool scrollToTop)
        //原方法签名
        //public void UpdateNpcList(bool scrollToTop = true)
        {
            Transform showOnlyPinedActor = __instance.transform.Find("MainWindow/BetterFix.NpcSearchPinOptions/BetterFix.ShowOnlyPinedActor");

            //
            if (showOnlyPinedActor != null && showOnlyPinedActor.GetComponent<UnityEngine.UI.Toggle>().isOn)
            {
                if (!scrollToTop)
                {
                    List<int> uiNpcSearchShowingActorIdList = Traverse.Create(__instance).Field<List<int>>("showingActorIdList").Value;

                    uiNpcSearchShowingActorIdList.Clear();
                    uiNpcSearchShowingActorIdList.AddRange(PinAcotrSupport.GetThisSaveSortValidPinedActors());
                    uiNpcSearchShowingActorIdList.Sort(new Comparison<int>(PinAcotrSupport.NormalCompareActor));

                    InfinityScroll villageNpcScroll = __instance.CGet<InfinityScroll>("VillageNpcScroll");

                    villageNpcScroll.UpdateData(uiNpcSearchShowingActorIdList.Count);
                    villageNpcScroll.gameObject.SetActive(true);
                }

                return false;
            }

            return true;
        }
    }
}
