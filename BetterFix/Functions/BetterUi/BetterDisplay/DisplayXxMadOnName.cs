using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 名字一栏显示入魔人标记：地点人物列表
    /// </summary>
    [HarmonyPatch(typeof(SetPlaceActor), "SetActor")]
    public static class XxMadOnNameInPlaceActor
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
            if (Main.Setting.DisplayXxMadOnName.Value)
            {
                //若人物为入魔人
                if (int.Parse(DateFile.instance.GetActorDate(key, 6, false)) == 1)
                {
                    //采用stringBuilder来整合文本（加上商人所属商队）
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("{0}魔</color>·{1}", DateFile.instance.massageDate[20009][0], __instance.listActorNameText.text);

                    //重设人物名字文本
                    __instance.listActorNameText.text = stringBuilder.ToString();
                }
            }
        }
    }
    /// <summary>
    /// 名字一栏显示入魔人标记：人物名册（NPC搜索）界面
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "OnRenderNpc")]
    public static class XxMadOnNameInNpcSearch
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="key">人物ID</param>
        /// <param name="show">是否显示具体信息</param>
        [HarmonyPostfix]
        private static void OnRenderNpcPostfix(ui_NpcSearch __instance, int index, Refers obj)
        //原方法签名
        //private void OnRenderNpc(int index, Refers obj)
        {
            if (Main.Setting.DisplayXxMadOnName.Value)
            {
                try
                {
                    int actorId = int.Parse(obj.name.Split(',')[1]);

                    //若人物为入魔人
                    if (int.Parse(DateFile.instance.GetActorDate(actorId, 6, false)) == 1)
                    {
                        //采用stringBuilder来整合文本（加上商人所属商队）
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("{0}魔</color>·{1}", DateFile.instance.massageDate[20009][0], obj.CGet<CText>("NameText").text);

                        //重设人物名字文本
                        obj.CGet<CText>("NameText").text = stringBuilder.ToString();
                    }
                }
                catch (Exception ex)
                {
                    QuickLogger.Log(BepInEx.Logging.LogLevel.Error, "ui_NpcSearch获取人物ID失败 name:{0}\n{1}", obj.name, ex);
                }
            }
        }
    }
}
