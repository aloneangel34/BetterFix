using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// ui_PlaceActorWindow创建人物关注按钮：监听事件部分
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "OnRenderActor")]
    public static class UpdatePinToggleInPlaceActorOnRenderActor
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnRenderActorPostfix(ui_PlaceActorWindow __instance, int index, Refers refer)
        //原方法签名
        //private void OnRenderActor(int index, Refers refer)
        {
            if (Main.Setting.UiPinActors.Value)
            {
                //若能找到actorId（即该人物图标已被正确渲染）
                if (PinAcotrSupport.TryGetActorIdFromName(refer.CGet<SetPlaceActor>("SetPlaceActor").gameObject.name, out int actorId))
                {
                    Transform pinToggle = refer.CGet<SetPlaceActor>("SetPlaceActor").transform.Find("BetterFix.PinToggle");
                    //若该人物图标上未含有人物关注按钮
                    if (pinToggle == null)
                    {
                        //创建按钮
                        pinToggle = PinAcotrSupport.NewPinToggle();
                        pinToggle.SetParent(refer.CGet<SetPlaceActor>("SetPlaceActor").transform, false);
                    }

                    //是否显示关注按钮：远距离不显示人物 或 该人物非实在人物（比如外道侠客之类的就不算实际人物）
                    bool showToggle = Traverse.Create(__instance).Field<bool>("showActors").Value && Characters.HasChar(actorId);

                    //若按钮的现在的显示/隐藏状态不符合需求
                    if (pinToggle.gameObject.activeSelf != showToggle)
                    {
                        //设置按钮的显示/隐藏
                        pinToggle.gameObject.SetActive(showToggle);
                    }

                    if (showToggle)
                    {
                        //关注按钮的Toggle组件
                        UnityEngine.UI.Toggle unityToggle = pinToggle.GetComponent<UnityEngine.UI.Toggle>();
                        //先清空Listener
                        unityToggle.onValueChanged.RemoveAllListeners();
                        //设置组件的开关状态
                        unityToggle.isOn = PinAcotrSupport.IsActorAlreadyPined(actorId);
                        //再加入Listener
                        unityToggle.onValueChanged.AddListener((bool value) => PinAcotrSupport.PinOnValueChanged_Invoke(value, unityToggle));
                    }
                }
            }
            else
            {
                Transform pinToggle = refer.CGet<SetPlaceActor>("SetPlaceActor").gameObject.transform.Find("BetterFix.PinToggle");
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
}
