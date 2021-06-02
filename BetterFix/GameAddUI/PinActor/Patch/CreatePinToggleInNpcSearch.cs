using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;
using UnityUIKit.Components;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// ui_NpcSearch创建人物关注按钮：UI主体部分
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "Awake")]
    public static class CreatePinToggleNpcSearchAwake
    {
        internal static ui_NpcSearch RecordInstance = null;
        public const int ClearAllPinedClickId = -1314;

        #region 弃用
        //internal static Transform VillageNpcTemp = null;
        //internal static Transform KnownNpcTemp = null;
        //internal static Transform WorldPartNpcTemp = null;
        //internal static UnityEngine.UI.Toggle ShowOnlyPinedActorUnityToggle = null;
        #endregion

        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static void AwakePrefix(ui_NpcSearch __instance)
        //原方法签名
        {
            RecordInstance = __instance;

            if (Main.Setting.UiPinActors.Value)
            {
                //向预制体中加入关注按钮UI
                Transform villageNpcTemp = __instance.transform.Find("MainWindow/NpcHolder/VillageNpcScroll/Viewport/Npc");
                Transform knownNpcTemp = __instance.transform.Find("MainWindow/NpcHolder/KnownNpcScroll/Viewport/Npc");
                Transform worldPartNpcTemp = __instance.transform.Find("MainWindow/NpcHolder/WorldPartNpc/WorldPartNpcScroll/Viewport/Npc");

                foreach (Transform npcTemp in new Transform[] { villageNpcTemp , knownNpcTemp, worldPartNpcTemp })
                {
                    if (npcTemp != null && npcTemp.transform.Find("BetterFix.PinToggle") == null)
                    {
                        RectTransform pinToggle = PinAcotrSupport.NewPinToggle(20, -20);
                        pinToggle.SetParent(npcTemp.transform, false);
                    }
                }

                //向ui_NpcSearch界面，加入两个按钮（“显示本存档栏位所有已关注人物”和“清空本存档栏位所有关注人物”）
                CToggle knownNpcCToggle = __instance.CGet<CToggle>("KnownToggle");
                if (knownNpcCToggle != null && __instance.transform.Find("MainWindow/BetterFix.NpcSearchPinOptions") == null)
                {
                    Container npcSearchPinOptions = new Container
                    {
                        Name = "BetterFix.NpcSearchPinOptions",
                        Group =
                        {
                            Direction = UnityUIKit.Core.Direction.Vertical,
                            Spacing = 5,
                        },
                        Element =
                        {
                            PreferredSize = { 50, 650 }
                        }
                    };
                    npcSearchPinOptions.RectTransform.anchorMin = new Vector2(0, 0.5f);
                    npcSearchPinOptions.RectTransform.anchorMax = new Vector2(0, 0.5f);
                    npcSearchPinOptions.RectTransform.anchoredPosition = new Vector2(-28, -185);
                    npcSearchPinOptions.RectTransform.sizeDelta = new Vector2(50, 600);
                    npcSearchPinOptions.RectTransform.SetParent(__instance.transform.Find("MainWindow"), false);

                    VerticalTaiwuToggle showOnlyPinedActor = new VerticalTaiwuToggle
                    {
                        Name = "BetterFix.ShowOnlyPinedActor",
                        IsTitel = true,
                        TextString = "显示所有已关注人物　",
                        TipTitle = "功能说明",
                        TipContant = "显示所有已关注的人物\n（不受上方搜索条件影响）\n\n本增强优化MOD提供的关注人物的记录只对应存档栏位，不对应具体存档（不向存档写入额外数据、以免坏档）",
                        Element =
                        {
                            PreferredSize = { 0 }
                        },
                        onValueChanged = (bool value, VerticalToggle tg) =>
                        {
                            if (Main.Setting.ModEnable.Value && Main.Setting.UiPinActors.Value && value)
                            {
                                __instance.UpdateNpcList(false);

                                #region 弃用
                                /*
                                NpcSearchPinedActorSort.ShowOnlyPinedActor = true;
                                knownNpcCToggle.group.SetAllTogglesOff();
                                //var onValueChangedBackup = knownNpcCToggle.onValueChanged;
                                //knownNpcCToggle.onValueChanged = null;
                                knownNpcCToggle.isOn = true;
                                //__instance.UpdateNpcList(true);
                                //knownNpcCToggle.isOn = false;
                                //knownNpcCToggle.onValueChanged = onValueChangedBackup;
                                tg.isOn = false;
                                */
                                #endregion
                            }
                            else if (!__instance.CGet<CToggle>("VillageToggle").isOn)
                            {
                                __instance.CGet<InfinityScroll>("VillageNpcScroll").gameObject.SetActive(false);
                            }
                        }
                    };
                    showOnlyPinedActor.SetParent(npcSearchPinOptions, false);
                    UnityEngine.UI.Toggle showOnlyPinedActorUnityToggle = showOnlyPinedActor.GameObject.GetComponent<UnityEngine.UI.Toggle>();
                    showOnlyPinedActorUnityToggle.group = knownNpcCToggle.group;


                    VerticalTaiwuToggle clearAllPinedActor = new VerticalTaiwuToggle
                    {
                        Name = "BetterFix.ClearAllPinedActor",
                        IsTitel = true,
                        TextString = "<color=#FF5050>清空所有关注　</color>",
                        TipTitle = "功能说明",
                        TipContant = "一般在新开档后使用\n\n由于关注人物的记录只对应存档栏位、不对应具体存档，难免会出现关注列表和具体存档不一致的情况。\n此时就需要清空关注列表来消除这种不一致。\n（记录进存档的关注还是期待官方原生的功能吧）",
                        Element =
                        {
                            PreferredSize = { 0, 200 }
                        },
                        onValueChanged = (bool value, VerticalToggle tg) =>
                        {
                            if (value)
                            {
                                if (Main.Setting.ModEnable.Value && Main.Setting.UiPinActors.Value)
                                {
                                    //public void SetYesOrNoWindow(int clickId, string title, string massage, bool backMask = false, bool canClose = true)
                                    YesOrNoWindow.instance.SetYesOrNoWindow(ClearAllPinedClickId, "确认清空所有关注人物？", "是否真的要清空本存档栏位所对应的所有关注人物？\n\n关注人物的记录只对应存档栏位、不对应具体存档（不向存档写入额外数据、以免坏档）\n最后一次被清空的记录将会备份在“游戏目录\\BepInEx\\config\\TaiwuMOD.BetterFix.cfg”文件中的LastClearedPinActorIdsBackup选项下，如有需要可手动还原", false, true);
                                }

                                tg.isOn = false;
                            }
                        }
                    };
                    clearAllPinedActor.SetParent(npcSearchPinOptions, false);
                }
            }
        }
    }
}
