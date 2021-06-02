using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 显示地格ID：在地格浮动信息窗口标题中
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "UpdateMianPlaceMassage")]
    public static class DisplayPlaceIdInMianPlaceMassage
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateMianPlaceMassagePostfix(WindowManage __instance, int mianPartId, int minaPlaceId)
        //原方法签名
        //public void UpdateMianPlaceMassage(int mianPartId, int minaPlaceId)
        {
            if (Main.Setting.DisplayPlaceId.Value)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}{1}(地格.{2})</color>",
                    //{0}原文本
                    __instance.informationName.text,
                    //{1}颜色前缀（蓝色）
                    DateFile.instance.massageDate[20005][0],
                    //{2}地格ID
                    minaPlaceId
                    );

                __instance.informationName.text = stringBuilder.ToString();
            }
        }
    }

    /// <summary>
    /// 显示地格ID：在所选地点信息界面上
    /// </summary>
    [HarmonyPatch(typeof(ChoosePlaceWindow), "SetPlaceMassageDate")]
    public static class DisplayPlaceIdInChoosePlaceWindow
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void SetPlaceMassageDatePostfix(ChoosePlaceWindow __instance, int partId, int placeId)
        //原方法签名
        //public void SetPlaceMassageDate(int partId, int placeId)
        {
            if (Main.Setting.DisplayPlaceId.Value)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}\n{1}(地格.{2})</color>",
                    //{0}原文本
                    __instance.moveMapPlaceName.text,
                    //{1}颜色前缀（蓝色）
                    DateFile.instance.massageDate[20005][0],
                    //{2}地格ID
                    placeId
                    );

                __instance.moveMapPlaceName.text = stringBuilder.ToString();
                __instance.moveMapPlaceName.verticalOverflow = VerticalWrapMode.Overflow;
            }
        }
    }

    /// <summary>
    /// 显示地格ID：人物信息界面（人物关系页面）
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "SetActorPeopleAttr")]
    public static class DisplayPlaceIdInActorMeumPeople
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="key">人物ID</param>
        [HarmonyPostfix]
        private static void SetActorPeopleAttrPostfix(ActorMenu __instance, int key)
        //原方法签名
        //public void SetActorPeopleAttr(int key)
        {
            if (Main.Setting.DisplayPlaceId.Value)
            {
                __instance.PeopleActorPlaceText.text = AdditionInfo.ShowActorPlaceInfo(key, AdditionInfo.PlaceInfoType.ActorMeumPeoplePlace);
            }
        }
    }


    /// <summary>
    /// 显示地格ID：人物信息界面（人物关系页面）
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "SetActorAttr")]
    public static class DisplayPlaceIdInActorMeumAttr
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="key">人物ID</param>
        [HarmonyPostfix]
        private static void SetActorAttrPostfix(ActorMenu __instance, int key)
        //原方法签名
        //public void SetActorAttr(int key)
        {
            if (Main.Setting.DisplayPlaceId.Value)
            {
                if (__instance.actorMenuIndex == 1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}\n\n{1}",
                        //{0}原年龄文本
                        __instance.ageText.text,
                        //{1}人物所在地点信息
                        AdditionInfo.ShowActorPlaceInfo(key, AdditionInfo.PlaceInfoType.ActorMeumMainAttr)
                        );

                    __instance.ageText.text = stringBuilder.ToString();
                    __instance.ageText.alignment = TextAnchor.UpperCenter;
                    __instance.ageText.verticalOverflow = VerticalWrapMode.Overflow;
                    __instance.ageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.5f, -8f);
                }
            }
        }
    }
}
