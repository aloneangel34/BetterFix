using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 显示商人所属商队：地点人物列表
    /// </summary>
    [HarmonyPatch(typeof(SetPlaceActor), "SetActor")]
    public static class MerchantTypeInPlaceActor
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="key">人物ID</param>
        /// <param name="show">是否显示具体信息</param>
        [HarmonyPostfix]
        private static void SetActorPostfix(SetPlaceActor __instance, int key, bool show)
        //原方法签名
        //public void SetActor(int key, bool show)
        {
            if (Main.Setting.DisplayMerchantType.Value)
            {
                //若人物为商人
                if (DisplaySupport.IsActorMerchant(key))
                {
                    //采用stringBuilder来整合文本（加上商人所属商队）
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}·{1}{2}</color>",
                        //{0}原身份文本
                        __instance.gangNameText.text,
                        //{1}颜色前缀（蓝色）
                        DateFile.instance.massageDate[20005][0],
                        //{2}所属商队名称
                        DisplaySupport.GetShopName(key)
                        );

                    //重设人物身份信息文本
                    __instance.gangNameText.text = stringBuilder.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 显示商人所属商队：人物信息窗口（人物属性页面）
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "SetActorAttr")]
    public static class MerchantTypeInActorMeumMain
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
            if (Main.Setting.DisplayMerchantType.Value)
            {
                //若人物为商人
                if (DisplaySupport.IsActorMerchant(key))
                {
                    //采用stringBuilder来整合文本（加上商人所属商队）
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}·{1}{2}</color>",
                        //{0}原身份文本
                        __instance.gangLevelText.text,
                        //{1}颜色前缀（蓝色）
                        DateFile.instance.massageDate[20005][0],
                        //{2}所属商队名称
                        DisplaySupport.GetShopName(key)
                        );

                    //重设人物身份信息文本
                    __instance.gangLevelText.text = stringBuilder.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 显示商人所属商队：人物信息窗口（人物关系页面）
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "SetActorPeopleAttr")]
    public static class MerchantTypeInActorMeumPeople
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
            if (Main.Setting.DisplayMerchantType.Value)
            {
                //若人物为商人
                if (DisplaySupport.IsActorMerchant(key))
                {
                    //采用stringBuilder来整合文本（加上商人所属商队）
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}·{1}{2}</color>",
                        //{0}原身份文本
                        __instance.peopleGangLevelText.text,
                        //{1}颜色前缀（蓝色）
                        DateFile.instance.massageDate[20005][0],
                        //{2}所属商队名称
                        DisplaySupport.GetShopName(key)
                        );

                    //重设人物身份信息文本
                    __instance.peopleGangLevelText.text = stringBuilder.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 显示商人所属商队：简易人物浮动信息窗口（比如搜索NPC界面）
    /// </summary>
    [HarmonyPatch(typeof(MouseTipActorInfoSimple), "UpdateContentAndSize")]
    public static class MerchantTypeInMouseTipActorInfoSimple
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="args">鼠标滑过传入的参数组</param>
        [HarmonyPostfix]
        private static void UpdateContentAndSizePostfix(MouseTipActorInfoSimple __instance, params object[] args)
        //原方法签名
        //protected override void UpdateContentAndSize(params object[] args)
        {
            if (Main.Setting.DisplayMerchantType.Value)
            {
                //人物ID
                int actorId = (int)args[0];
                //若人物为商人
                if (DisplaySupport.IsActorMerchant(actorId))
                {
                    CText gangLevelText = __instance.CGet<CText>("GangLevel");

                    //采用stringBuilder来整合文本（加上商人所属商队）
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}·{1}{2}</color>",
                        //{0}原身份文本
                        gangLevelText.text,
                        //{1}颜色前缀（蓝色）
                        DateFile.instance.massageDate[20005][0],
                        //{2}所属商队名称
                        DisplaySupport.GetShopName(actorId)
                        );

                    //重设人物身份信息文本
                    gangLevelText.text = stringBuilder.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 显示商人所属商队：人物浮动信息窗口
    /// </summary>
    [HarmonyPatch(typeof(MouseTipActorInfo), "UpdateContentAndSize")]
    public static class MerchantTypeInMouseTipActorInfo
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="args">鼠标滑过传入的参数组</param>
        [HarmonyPostfix]
        private static void UpdateContentAndSizePostfix(MouseTipActorInfo __instance, params object[] args)
        //原方法签名
        //protected override void UpdateContentAndSize(params object[] args)
        {
            if (Main.Setting.DisplayMerchantType.Value)
            {
                Dictionary<int, string> actorData = args[0] as Dictionary<int, string>;

                //特别要小心，传过来的人物数据不一定是完整的（点名批评轮回台）本来直接传actorId过来多好（也许是为了兼容死者/铭刻人物的数据？）
                if (actorData != null && actorData.ContainsKey(9))
                {
                    //人物的所属势力
                    int actorGang = int.Parse(actorData[19]);
                    //人物的身份品阶
                    int actorLevelNumber = int.Parse(actorData[20]);
                    //人物的GangValueId
                    int actorGangValueId = DateFile.instance.GetGangValueId(actorGang, actorLevelNumber);
                    //对话人物的追加互动事件列表
                    string[] additionalDialogEvents = DateFile.instance.presetGangGroupDateValue[actorGangValueId][812].Split('|');
                    //
                    bool isMerchant = false;

                    //因为没有传过来actorId，所以无法调用DisplaySupport.IsActorMerchant()方法……
                    //若人物的对话追加互动事件中有“（浏览货物……）”选项，则人物为商人
                    for (int i = 0; i < additionalDialogEvents.Length; i++)
                    {
                        int additionalDialogEventId = int.Parse(additionalDialogEvents[i]);

                        if (additionalDialogEventId == 901300005)
                        {
                            isMerchant = true;
                            break;
                        }
                    }

                    //若人物为商人
                    if (isMerchant)
                    {
                        int key = int.Parse(DateFile.instance.GetGangDate(int.Parse(actorData[9]), 16));

                        CText gangLevelText = __instance.CGet<CText>("GangLevel");

                        //采用stringBuilder来整合文本（加上商人所属商队）
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("{0}·{1}{2}</color>",
                            //{0}原身份文本
                            gangLevelText.text,
                            //{1}颜色前缀（蓝色）
                            DateFile.instance.massageDate[20005][0],
                            //{2}所属商队名称
                            (DateFile.instance.storyShopDate[key][0] ?? "？")
                            );

                        //重设人物身份信息文本
                        gangLevelText.text = stringBuilder.ToString();
                    }
                }
            }
        }
    }
}
