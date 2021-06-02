using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 对话窗口追加显示人物的所属势力和身份称呼信息，若为商人则继续追加商人所属商队信息
    /// </summary>
    [HarmonyPatch(typeof(ui_MessageWindow), "SetMassageWindow")]
    public static class DisplayDialogActorIdentity
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="baseEventDate"></param>
        /// <param name="chooseId"></param>
        [HarmonyPostfix]
        private static void SetMassageWindowPostfix(ui_MessageWindow __instance, int[] baseEventDate)
        //原方法签名
        //private void SetMassageWindow(int[] baseEventDate, int chooseId)
        {
            if (Main.Setting.DisplayDialogActorIdentity.Value)
            {
                int eventId = baseEventDate[2];
                int eventActorPresetId = int.Parse(DateFile.instance.eventDate[eventId][2]);
                int taiwuActorId = DateFile.instance.MianActorID();
                //显示的对话人物ID
                int eventActorId = (eventActorPresetId == 0) ? baseEventDate[1] : ((eventActorPresetId == -1) ? taiwuActorId : eventActorPresetId);

                //调试信息
                //if (Main.Setting.debugMode.Value)
                //{
                //    QuickLogger.Log(LogLevel.Warning, "对话窗口 对话人物ID:{0} (baseEventDate[2]:{1} DateFile.instance.eventDate[num][2]:{2} taiwuActorId:{3}) ", eventActorId, eventId, eventActorPresetId, taiwuActorId);
                //}

                //若对话人物身份已判明（ID为正数）
                if (eventActorId > 0)
                {
                    //对话人物的baseActorId
                    int baseActorId = int.Parse(DateFile.instance.GetActorDate(eventActorId, 997, false));

                    //对话人物为实际人物（baseActorId在1～32之间）
                    if (baseActorId >= 1 && baseActorId <= 32)
                    {
                        //调整Text组件所对应的RectTransform的绑定位置（左移80f，下移15f）原值(-40f, 0f);
                        __instance.mianActorNameText.rectTransform.anchoredPosition = new Vector2(-120f, -15f);
                        //调整Text组件所对应的RectTransform的sizeDelta大小（宽度增大20f）原值(-100f, 0f)（因为有anchorMin、anchorMax的设置，这里是相对于父级组件的值）;
                        __instance.mianActorNameText.rectTransform.sizeDelta = new Vector2(-80f, 0f);
                        //调整Text组件的对其方式为（上下居中，左右居右）原值MiddleCenter
                        __instance.mianActorNameText.alignment = TextAnchor.MiddleRight;
                        //调整Text组件的纵向溢出模式为允许溢出（超出组件范围的文本也依然显示）原值Truncate（阶段：超出组件范围的文本不予显示）
                        __instance.mianActorNameText.verticalOverflow = VerticalWrapMode.Overflow;

                        //采用stringBuilder来整合文本（第二行加上所属势力和身份称呼；若为商人，则在第三行加上商人所属商队）
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("{0}\n<color=#DDCCBB>{1}</color>{2}", __instance.mianActorNameText.text, DisplaySupport.GetActorGangText(eventActorId), DisplaySupport.GetGangLevelColorText(eventActorId));

                        //若对话人物为商人，追加所属商队名称
                        if (DisplaySupport.IsActorMerchant(eventActorId))
                        {
                            stringBuilder.AppendFormat("·<color=#80A0E0>{0}</color>", DisplaySupport.GetShopName(eventActorId));
                        }

                        //重设人物名字文本
                        __instance.mianActorNameText.text = stringBuilder.ToString();
                    }
                }
            }
        }
    }
}
