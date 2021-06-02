using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;
using HarmonyLib;
using UnityUIKit.GameObjects;

namespace BetterFix
{
    internal static class RecoverUiChange
    {
        /// <summary>
        /// 还原所有UI改动
        /// </summary>
        internal static void RecoverAll(bool isActive)
        {
            if (isActive)
            {
                if (Main.Setting.UiQuickSurrender.Value)
                {
                    SetQuickSurrenderButton(true);
                }

                if (Main.Setting.UiPinActors.Value)
                {
                    SetPinActorToggleActive(true);
                }

                if (Main.Setting.UiOnlyMerchantToggle.Value)
                {
                    SetPlaceMerchantButtonActive(true);
                }

                if (Main.Setting.UiQuickChangeEquipedGongFa.Value)
                {
                    RefreshEquipedGongFaButtonInteractable();
                }

                if (Main.Setting.UiAddPlaceBuildUI.Value)
                {
                    if (Main.IsInGame())
                    {
                        if (Main.InGameBuildingUI == null)
                        {
                            Main.InGameBuildingUI = new PlaceHomeBuildingUI();
                        }
                        Main.InGameBuildingUI.AttachUI(true);
                    }
                }
            }
            else
            {
                RecoverBattleGongFaUi();
                RecoverMianQiText();
                RecoverMouseActorSimpleAdditionInfo();
                RefreshEquipedGongFaButtonInteractable();
                SetQuickSurrenderButton(false);
                SetPinActorToggleActive(false);
                SetPlaceMerchantButtonActive(false);

                //RecoverUiChange.SetPlaceBuildingUiActive(false);
                PlaceHomeBuildingUI.DetachUI();
            }
        }

        /// <summary>
        /// 还原所有战斗中图标栏位的UI变动
        /// </summary>
        internal static void RecoverBattleGongFaUi()
        {
            RecoverMoveGongFaHolderUi();
            RecoverDefGongFaHolderUi();
        }

        /// <summary>
        /// 还原身法图标栏位的UI变动
        /// </summary>
        internal static void RecoverMoveGongFaHolderUi()
        {
            //获取MoveGongFaHolder这个GameObject的Transform组件
            Transform moveGongFaHolderTransform = BattleSystem.instance.actorGongFaHolder[1];
            //获取MoveGongFaHolder这个GameObject的GridLayoutGroup组件
            UnityEngine.UI.GridLayoutGroup moveGongFaHolderGridLayout = moveGongFaHolderTransform.gameObject.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            //还原RectTransform的anchoredPosition位置
            moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            //还原GridLayoutGroup的受约束的轴（游戏为X轴/水平方向）上应存在的单元格数
            moveGongFaHolderGridLayout.constraintCount = 9;

            #region 弃用
            //if (GongFaIconOverScreen.IsMoveGongFaHolderAnchoredPositionModified)
            //{
            //    //还原RectTransform的anchoredPosition位置（Y轴上移15f）
            //    Vector2 anchoredPositionXY = moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition;
            //    anchoredPositionXY.y += 15f;
            //    moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPositionXY;

            //    GongFaIconOverScreen.IsMoveGongFaHolderAnchoredPositionModified = false;
            //}
            #endregion
        }

        /// <summary>
        /// 还原护体图标栏位的UI变动
        /// </summary>
        internal static void RecoverDefGongFaHolderUi()
        {
            //获取DefGongFaHolder这个GameObject的Transform组件
            Transform defGongFaHolderTransform = BattleSystem.instance.actorGongFaHolder[2];
            //获取DefGongFaHolder这个GameObject的GridLayoutGroup组件
            UnityEngine.UI.GridLayoutGroup defGongFaHolderGridLayout = defGongFaHolderTransform.gameObject.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            //还原RectTransform的anchoredPosition位置
            defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            //还原GridLayoutGroup的受约束的轴（游戏为X轴/水平方向）上应存在的单元格数
            defGongFaHolderGridLayout.constraintCount = 9;

            #region 弃用
            //if (GongFaIconOverScreen.IsDefGongFaHolderAnchoredPositionModified)
            //{
            //    //还原RectTransform的anchoredPosition位置（Y轴上移15f）
            //    Vector2 anchoredPositionXY = defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition;
            //    anchoredPositionXY.y += 15f;
            //    defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPositionXY;

            //    GongFaIconOverScreen.IsDefGongFaHolderAnchoredPositionModified = false;
            //}
            #endregion
        }

        /// <summary>
        /// 还原快速装备已装备的功法的按钮可互动性
        /// </summary>
        internal static void RefreshEquipedGongFaButtonInteractable()
        {
            if (ActorMenu.Exists)
            {
                ActorMenu.instance.UpdateGongFaInformation(ActorMenu.instance.equipGongFaId);
            }
        }

        /// <summary>
        /// 还原内息紊乱文本框的UI变动
        /// </summary>
        internal static void RecoverMianQiText()
        {
            //!---------------- ActorMenu界面的设定部分 ----------------!
            #region ActorMenu界面的设定部分

            GameObject actorMenuGO1 = GameObject.Find("UIRoot/Canvas/UIWindow/ActorMenu");
            GameObject actorMenuGO2 = GameObject.Find("ActorMenu");

            foreach (GameObject actorMenuGO in new GameObject[] { actorMenuGO1, actorMenuGO2 })
            {
                if (actorMenuGO != null)
                {
                    ActorMenu instance = actorMenuGO.GetComponent<ActorMenu>();

                    //还原UI布局
                    instance.actorMianQiBack.sizeDelta = new Vector2(80f, 30f);
                    instance.mianQiArrowLeft.transform.localPosition = new Vector3(-40f, 0f, 0f);
                    instance.mianQiArrowRight.transform.localPosition = new Vector3(40f, 0f, 0f);

                    //重设文本
                    if (instance.actorId > 0)
                    {
                        int mianQiType = DateFile.instance.GetActorMianQi(instance.actorId) / 2000;
                        instance.actorMianQiText.text = DateFile.instance.SetColoer(int.Parse(DateFile.instance.ageDate[mianQiType][303]), DateFile.instance.ageDate[mianQiType][301], false);
                    }
                }
            }
            #endregion
            #region 弃用
            /*
            if (DisplayActorMianQiNumberInActorMenu.IsModified)
            {
                if (DisplayActorMianQiNumberInActorMenu.ActorMenuActorMianQiBack != null)
                {
                    //还原RectTransform组件sizeDelta的宽度（重设为80f）
                    DisplayActorMianQiNumberInActorMenu.ActorMenuActorMianQiBack.sizeDelta = new Vector2(80f, 30f);

                    #region 弃用
                    //还原RectTransform组件sizeDelta的宽度（-20f）
                    //Vector2 xy = DisplayActorMianQiNumberInActorMenu.ActorMenuActorMianQiBack.sizeDelta;
                    //xy.x -= 20f;
                    //DisplayActorMianQiNumberInActorMenu.ActorMenuActorMianQiBack.sizeDelta = xy;
                    #endregion
                }

                if (DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowLeft != null)
                {
                    //还原Transform组件localPosition的横坐标（重设为-40f）
                    DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowLeft.localPosition = new Vector3(-40f, 0f, 0f);

                    #region 弃用
                    //还原Transform组件localPosition的横坐标（+10f）
                    //Vector3 xyz = DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowLeft.localPosition;
                    //xyz.x += 10f;
                    //DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowLeft.localPosition = xyz;
                    #endregion
                }

                if (DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowRight != null)
                {
                    //还原Transform组件localPosition的横坐标（重设为40f）
                    DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowRight.localPosition = new Vector3(40f, 0f, 0f);

                    #region 弃用
                    //还原Transform组件localPosition的横坐标（-10f）
                    //Vector3 xyz = DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowRight.localPosition;
                    //xyz.x -= 10f;
                    //DisplayActorMianQiNumberInActorMenu.ActorMenumianQiArrowRight.localPosition = xyz;
                    #endregion
                }

                DisplayActorMianQiNumberInActorMenu.IsModified = false;
            }
            */
            #endregion
        }

        internal static void RecoverMouseActorSimpleAdditionInfo()
        {
            //!---------------- ui_MouseTipActorInfoSimple界面的设定部分 ----------------!
            #region ui_MouseTipActorInfoSimple界面的设定部分

            GameObject actorMenuGO1 = GameObject.Find("UIRoot/Canvas/UITips/ui_MouseTipActorInfoSimple");
            GameObject actorMenuGO2 = GameObject.Find("ui_MouseTipActorInfoSimple");

            foreach (GameObject actorMenuGO in new GameObject[] { actorMenuGO1, actorMenuGO2 })
            {
                if (actorMenuGO != null)
                {
                    Transform additionInfoUI = actorMenuGO.transform.Find("BetterFix.AdditionInfoUI");

                    if (additionInfoUI != null && additionInfoUI.gameObject.activeSelf)
                    {
                        additionInfoUI.gameObject.SetActive(false);
                    }
                }
            }
            #endregion
        }



        /// <summary>
        /// 设置快速投降的显示隐藏
        /// </summary>
        /// <param name="isActive">true 显示 / false 隐藏</param>
        internal static void SetQuickSurrenderButton(bool isActive)
        {
            //TODO不需要检测是否存在，开启需要检测、关闭必须强制

            //!---------------- SkillBattleWindow界面的设定部分 ----------------!
            #region SkillBattleWindow界面的设定部分

            GameObject skillBattleWindowGO1 = GameObject.Find("UIRoot/Canvas/UIWindow/SkillBattleWindow");
            GameObject skillBattleWindowGO2 = GameObject.Find("SkillBattleWindow");

            foreach (GameObject skillBattleWindowGO in new GameObject[] { skillBattleWindowGO1, skillBattleWindowGO2 })
            {
                if (skillBattleWindowGO != null)
                {
                    //较艺中我方左侧的白旗图标
                    Transform skillSurrenderIcon = skillBattleWindowGO.transform.Find("SkillBattleMain/BattleActorBack/ActorHpBack/ActorHp/ActorBattleLoseIcon,831");

                    if (skillSurrenderIcon != null)
                    {
                        //!----------------白旗图标上的按钮组件----------------!
                        UnityEngine.UI.Button skillSurrenderButton = skillSurrenderIcon.GetComponent<UnityEngine.UI.Button>();

                        //已创建按钮组件
                        if (skillSurrenderButton != null)
                        {
                            skillSurrenderButton.onClick.RemoveAllListeners();
                            skillSurrenderButton.interactable = isActive;
                            if (isActive && SkillBattleSystem.Exists && !Traverse.Create(SkillBattleSystem.instance).Field<bool>("battleEnd").Value)
                            {
                                skillSurrenderButton.onClick.AddListener(delegate ()
                                {
                                    YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.SkillBattleSurrenderClickId, "较艺认输", "要向对手认输吗？", false, true);
                                });
                            }
                        }
                        //未创建按钮组件
                        else if (isActive && SkillBattleSystem.Exists && !Traverse.Create(SkillBattleSystem.instance).Field<bool>("battleEnd").Value)
                        {
                            skillSurrenderIcon.gameObject.AddComponent<UnityEngine.UI.Button>();
                            skillSurrenderButton = skillSurrenderIcon.GetComponent<UnityEngine.UI.Button>();

                            skillSurrenderButton.onClick.RemoveAllListeners();
                            skillSurrenderButton.interactable = isActive;
                            skillSurrenderButton.onClick.AddListener(delegate ()
                            {
                                YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.SkillBattleSurrenderClickId, "较艺认输", "要向对手认输吗？", false, true);
                            });
                        }
                    }
                }
            }
            #endregion

            //!---------------- BattleSystem界面的设定部分 ----------------!
            #region BattleSystem界面的设定部分

            GameObject battleSystemGO1 = GameObject.Find("UIRoot/Canvas/UIBackGround/BattleSystem");
            GameObject battleSystemGO2 = GameObject.Find("BattleSystem");

            foreach (GameObject battleSystemGO in new GameObject[] { battleSystemGO1, battleSystemGO2 })
            {
                if (battleSystemGO != null)
                {
                    //战斗中我方左侧的白旗图标
                    Transform battleSurrenderIcon = battleSystemGO.transform.Find("ActorBack/BattleActorBack/ActorDp/ActorBattleLoseIcon,814");

                    if (battleSurrenderIcon != null)
                    {
                        //!----------------白旗图标上的按钮组件----------------!
                        UnityEngine.UI.Button battleSurrenderButton = battleSurrenderIcon.GetComponent<UnityEngine.UI.Button>();

                        //已创建按钮组件
                        if (battleSurrenderButton != null)
                        {
                            battleSurrenderButton.onClick.RemoveAllListeners();
                            battleSurrenderButton.interactable = isActive;
                            if (isActive && BattleSystem.Exists && !BattleSystem.instance.battleEnd && (BattleSystem.instance.battleTyp == 1 || BattleSystem.instance.battleTyp == 2))
                            {
                                battleSurrenderButton.onClick.AddListener(delegate ()
                                {
                                    YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.ExerciseSurrenderClickId, (BattleSystem.instance.battleTyp == 1) ? "切磋战认输" : "接招战认输", "要向对手认输吗？\n（算落败、不算逃脱）", false, true);
                                });
                            }
                        }
                        //未创建按钮组件
                        else if (isActive && BattleSystem.Exists && !BattleSystem.instance.battleEnd && (BattleSystem.instance.battleTyp == 1 || BattleSystem.instance.battleTyp == 2))
                        {
                            battleSurrenderIcon.gameObject.AddComponent<UnityEngine.UI.Button>();
                            battleSurrenderButton = battleSurrenderIcon.GetComponent<UnityEngine.UI.Button>();

                            battleSurrenderButton.onClick.RemoveAllListeners();
                            battleSurrenderButton.interactable = isActive;
                            battleSurrenderButton.onClick.AddListener(delegate ()
                            {
                                YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.ExerciseSurrenderClickId, (BattleSystem.instance.battleTyp == 1) ? "切磋战认输" : "接招战认输", "要向对手认输吗？\n（算落败、不算逃脱）", false, true);
                            });
                        }
                    }
                }
            }
            #endregion
        }

        #region 弃用
        //internal static void SetPlaceBuildingUiActive(bool isActive)
        //{
        //    if (Main.InGameBuildingUI != null)
        //    {
        //        Main.InGameBuildingUI.SetUIActive(isActive);
        //    }
        //}
        #endregion

        /// <summary>
        /// 设置关注人物锁（模板）的显示隐藏（因为设定的是模板，所以将在地点人物刷新后生效）
        /// </summary>
        /// <param name="isActive">true 显示 / false 隐藏</param>
        internal static void SetPinActorToggleActive(bool isActive)
        {
            //!---------------- ui_PlaceActorWindow界面的设定部分 ----------------!
            #region ui_PlaceActorWindow界面的设定部分

            GameObject ui_PlaceActorWindowGO1 = GameObject.Find("UIRoot/Canvas/UIWindow/ui_PlaceActorWindow");
            GameObject ui_PlaceActorWindowGO2 = GameObject.Find("ui_PlaceActorWindow");

            foreach (GameObject ui_PlaceActorWindowGO in new GameObject[] { ui_PlaceActorWindowGO1, ui_PlaceActorWindowGO2 })
            {
                if (ui_PlaceActorWindowGO != null)
                {
                    //!---------------- 预制体上的人物关注按钮 ----------------!
                    Transform pinActorToggle = ui_PlaceActorWindowGO.transform.Find("ActorView/ActorViewport/PlaceActor/Actor,id/BetterFix.PinToggle");

                    //已创建人物关注按钮
                    if (pinActorToggle != null)
                    {
                        if (pinActorToggle.gameObject.activeSelf != isActive)
                        {
                            pinActorToggle.gameObject.SetActive(isActive);
                        }
                    }
                    //未创建人物关注按钮
                    else if (isActive)
                    {
                        Transform placeActorButton = ui_PlaceActorWindowGO.transform.Find("ActorView/ActorViewport/PlaceActor/Actor,id");

                        pinActorToggle = PinAcotrSupport.NewPinToggle();
                        pinActorToggle.SetParent(placeActorButton, false);
                    }
                }
            }
            #endregion

            //!---------------- ui_NpcSearch界面的设定部分 ----------------!
            #region ui_NpcSearch界面的设定部分
            GameObject ui_NpcSearchGO1 = GameObject.Find("UIRoot/Canvas/UIPopup/ui_NpcSearch");
            GameObject ui_NpcSearchGO2 = GameObject.Find("ui_NpcSearch");

            foreach (GameObject ui_NpcSearchGO in new GameObject[] { ui_NpcSearchGO1, ui_NpcSearchGO2 })
            {
                if (ui_NpcSearchGO != null)
                {
                    //!---------------- 预制体上的人物关注按钮 ----------------!
                    #region 预制体上的人物关注按钮
                    Transform VillageNpcTemp = ui_NpcSearchGO.transform.Find("MainWindow/NpcHolder/VillageNpcScroll/Viewport/Npc");
                    Transform KnownNpcTemp = ui_NpcSearchGO.transform.Find("MainWindow/NpcHolder/KnownNpcScroll/Viewport/Npc");
                    Transform WorldPartNpcTemp = ui_NpcSearchGO.transform.Find("MainWindow/NpcHolder/WorldPartNpc/WorldPartNpcScroll/Viewport/Npc");

                    foreach (Transform npcTemp in new Transform[] { VillageNpcTemp, KnownNpcTemp, WorldPartNpcTemp })
                    {
                        if (npcTemp != null)
                        {
                            Transform pinToggle = npcTemp.transform.Find("BetterFix.PinToggle");

                            //已创建人物关注按钮
                            if (pinToggle != null)
                            {
                                if (pinToggle.gameObject.activeSelf != isActive)
                                {
                                    pinToggle.gameObject.SetActive(isActive);
                                }
                            }
                            //未创建人物关注按钮，且要求开启
                            else if (isActive)
                            {
                                //!---------------- 创建人物关注按钮 ----------------!
                                pinToggle = PinAcotrSupport.NewPinToggle(20, -20);
                                pinToggle.SetParent(npcTemp.transform, false);
                            }
                        }
                    }
                    #endregion

                    //!---------------- 界面左侧的追加选项按钮 ----------------!
                    #region 界面左侧的追加选项按钮
                    Transform npcSearchPinOptions = ui_NpcSearchGO.transform.Find("MainWindow/BetterFix.NpcSearchPinOptions");

                    //已创建追加选项按钮
                    if (npcSearchPinOptions != null)
                    {
                        if (!isActive)
                        {
                            Transform showOnlyPinedActor = npcSearchPinOptions.Find("BetterFix.ShowOnlyPinedActor");
                            if (showOnlyPinedActor != null)
                            {
                                UnityEngine.UI.Toggle showOnlyPinedActorUnityToggle = showOnlyPinedActor.GetComponent<UnityEngine.UI.Toggle>();
                                if (showOnlyPinedActorUnityToggle.isOn)
                                {
                                    showOnlyPinedActorUnityToggle.isOn = false;
                                }
                            }
                        }

                        if (npcSearchPinOptions.gameObject.activeSelf != isActive)
                        {
                            npcSearchPinOptions.gameObject.SetActive(isActive);
                        }
                    }
                    //未创建追加选项按钮，且要求开启
                    else if (isActive)
                    {
                        //!---------------- 创建追加选项按钮 ----------------!
                        #region 创建追加选项按钮

                        ui_NpcSearch instance = ui_NpcSearchGO.GetComponent<ui_NpcSearch>();

                        Container NpcSearchPinOptions = new Container
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
                        NpcSearchPinOptions.RectTransform.anchorMin = new Vector2(0, 0.5f);
                        NpcSearchPinOptions.RectTransform.anchorMax = new Vector2(0, 0.5f);
                        NpcSearchPinOptions.RectTransform.anchoredPosition = new Vector2(-28, -185);
                        NpcSearchPinOptions.RectTransform.sizeDelta = new Vector2(50, 600);
                        NpcSearchPinOptions.RectTransform.SetParent(instance.transform.Find("MainWindow"), false);

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
                                if (value)
                                {
                                    instance.UpdateNpcList(false);
                                }
                                else if (!instance.CGet<CToggle>("VillageToggle").isOn)
                                {
                                    instance.CGet<InfinityScroll>("VillageNpcScroll").gameObject.SetActive(false);
                                }
                            }
                        };
                        showOnlyPinedActor.SetParent(NpcSearchPinOptions, false);
                        UnityEngine.UI.Toggle showOnlyPinedActorUnityToggle = showOnlyPinedActor.GameObject.GetComponent<UnityEngine.UI.Toggle>();
                        showOnlyPinedActorUnityToggle.group = instance.CGet<CToggle>("VillageToggle").group;

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
                                    //public void SetYesOrNoWindow(int clickId, string title, string massage, bool backMask = false, bool canClose = true)
                                    YesOrNoWindow.instance.SetYesOrNoWindow(CreatePinToggleNpcSearchAwake.ClearAllPinedClickId, "确认清空所有关注人物？", "是否真的要清空本存档栏位所对应的所有关注人物？\n\n关注人物的记录只对应存档栏位、不对应具体存档（不向存档写入额外数据、以免坏档）\n最后一次被清空的记录将会备份在“游戏目录\\BepInEx\\config\\TaiwuMOD.BetterFix.cfg”文件中的LastClearedPinActorIdsBackup选项下，如有需要可手动还原", false, true);
                                    tg.isOn = false;
                                }
                            }
                        };
                        clearAllPinedActor.SetParent(NpcSearchPinOptions, false);
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// 设置商人提示按钮的显示隐藏（刷新后生效）
        /// </summary>
        internal static void SetPlaceMerchantButtonActive(bool isActive)
        {
            //!---------------- ui_PlaceActorWindow界面的设定部分 ----------------!
            GameObject ui_PlaceActorWindow1 = GameObject.Find("UIRoot/Canvas/UIWindow/ui_PlaceActorWindow");
            GameObject ui_PlaceActorWindow2 = GameObject.Find("ui_PlaceActorWindow");

            foreach (GameObject ui_PlaceActorWindowGO in new GameObject[] { ui_PlaceActorWindow1, ui_PlaceActorWindow2 })
            {
                if (ui_PlaceActorWindowGO != null)
                {
                    //!---------------- 商人提示按钮 ----------------!
                    Transform onlyMerchantToggle = ui_PlaceActorWindowGO.transform.Find("BetterFix.OnlyMerchantToggle");

                    //商人提示按钮已创建
                    if (onlyMerchantToggle != null)
                    {
                        if (isActive)
                        {
                            if (OnlyMerchantSupport.OnlyMerchantList.Count > 0 && !onlyMerchantToggle.gameObject.activeSelf)
                            {
                                onlyMerchantToggle.gameObject.SetActive(true);
                            }
                        }
                        else if (onlyMerchantToggle.gameObject.activeSelf)
                        {
                            onlyMerchantToggle.gameObject.SetActive(false);
                        }
                    }
                    //商人提示按钮未创建，且要求开启
                    else if (isActive)
                    {
                        //!---------------- 创建商人提示按钮 ----------------!
                        RectTransform onlyMerchantToggleRect = OnlyMerchantSupport.NewOnlyMerchantToggle();
                        UnityEngine.UI.Toggle onlyMerchantUnityToggle = onlyMerchantToggleRect.GetComponent<UnityEngine.UI.Toggle>();
                        onlyMerchantUnityToggle.onValueChanged.AddListener((bool value) => OnlyMerchantSupport.MerchantOnValueChanged_Invoke(value, onlyMerchantUnityToggle, ui_PlaceActorWindowGO.GetComponent<ui_PlaceActorWindow>()));
                        onlyMerchantToggleRect.SetParent(ui_PlaceActorWindowGO.transform, false);
                    }
                }
            }
        }
    }
}
