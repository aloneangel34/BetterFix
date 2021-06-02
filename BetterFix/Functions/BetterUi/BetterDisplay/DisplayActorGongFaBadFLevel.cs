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
    /// 显示心法逆练等级：功法浮动信息窗口
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "SetGongFaTypText")]
    public static class DisplayActorGongFaBadFLevelInWindowManage
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        /// <param name="gongFaId">功法ID</param>
        /// <param name="gongFaLevel">功法修炼度</param>
        /// <param name="gongFaFLevel">功法心法等级</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="gongFaUsePower">功法使用威力</param>
        /// <param name="gongFaMaxUsePower">功法最大使用威力</param>
        /// <returns></returns>
        [HarmonyPrefix]
        private static bool SetGongFaTypTextPrefix(WindowManage __instance, ref string __result, int gongFaId, int gongFaLevel, int gongFaFLevel, int actorId, int gongFaUsePower, int gongFaMaxUsePower)
        //原方法签名
        //private string SetGongFaTypText(int gongFaId, int gongFaLevel, int gongFaFLevel, int actorId, int gongFaUsePower, int gongFaMaxUsePower)
        {
            if (Main.Setting.DisplayActorGongFaBadFLevel.Value)
            {
                //这个功法的数据
                Dictionary<int, string> thisGongFaData = DateFile.instance.gongFaDate[gongFaId];
                //功法类型（心法、身法、绝技、以及所有催破的详细分类）
                int gongFaType = Mathf.Max(int.Parse(thisGongFaData[1]), 0);
                //功法的内力五行属性
                int neiLiWuXingType = int.Parse(thisGongFaData[4]);
                //功法修炼度100%时提供的基础威力加成
                int gongFaBasePower = DateFile.instance.GetGongFaBasePower(actorId, gongFaId) - 100;
                //技艺提供的的心法等级
                int skillAddGongFaFLevel = DateFile.instance.GetGongFaFLevel(actorId, gongFaId, true);


                StringBuilder stringBuilder = new StringBuilder();

                #region 功法类型文本设定
                //功法属性
                stringBuilder.AppendFormat("{0}{1}{2}{3}</color>",
                    //{0}“·”
                    __instance.Dit(),
                    //{1}“属性：”
                    DateFile.instance.massageDate[8007][4].Split('|')[1],
                    //{2}属性颜色
                    DateFile.instance.massageDate[int.Parse(DateFile.instance.massageDate[2004][1].Split('|')[neiLiWuXingType])][0],
                    //{3}属性文本
                    DateFile.instance.massageDate[2004][0].Split('|')[neiLiWuXingType]
                    );
                __instance.itemMoneyText.text = stringBuilder.ToString();
                //__instance.itemMoneyText.text = string.Format("{0}{1}{2}", __instance.Dit(), DateFile.instance.massageDate[8007][4].Split('|')[1], DateFile.instance.SetColoer(20003, DateFile.instance.SetColoer(int.Parse(DateFile.instance.massageDate[2004][1].Split('|')[neiLiWuXingType]), DateFile.instance.massageDate[2004][0].Split('|')[neiLiWuXingType], false), false));
                #endregion

                #region 功法发挥度文本设定
                //功法发挥度
                stringBuilder.Clear();
                stringBuilder.AppendFormat("{0}{1}",
                    //{0}“·”
                    __instance.Dit(),
                    //{1}“发挥：”
                    DateFile.instance.massageDate[8007][2].Split('|')[3]
                    );

                if (actorId == 0)
                {
                    stringBuilder.AppendFormat("{0}{1}</color>",
                        //{0}发挥度颜色前缀
                        DateFile.instance.massageDate[20002][0],
                        //{1}“-”
                        DateFile.instance.massageDate[303][2]
                        );
                }
                else
                {
                    stringBuilder.AppendFormat("{0}{1}%</color>",
                        //{0}发挥度颜色前缀
                        DateFile.instance.massageDate[DisplaySupport.Color6(gongFaUsePower, gongFaMaxUsePower, 100)][0],
                        //{1}发挥度数值
                        gongFaUsePower
                        );
                }

                __instance.itemLevelText.text = stringBuilder.ToString();
                //__instance.itemLevelText.text = string.Format("{0}{1}{2}", __instance.Dit(), DateFile.instance.massageDate[8007][2].Split('|')[3], (actorId == 0) ? DateFile.instance.SetColoer(20002, DateFile.instance.massageDate[303][2], false) : DateFile.instance.SetColoer(WindowManageSupport.Color6(gongFaUsePower, gongFaMaxUsePower, 100), gongFaUsePower + "%", false));
                #endregion

                #region 设定内容
                stringBuilder.Clear();
                stringBuilder.AppendFormat("{0}{1}{2} / {3}\n{4}{5}{6}{7}</color>\n",
                    //{0}“功法类型”
                    __instance.SetMassageTitle(8007, 3, 0, 10002),
                    //{1}“·”
                    __instance.Dit(),
                    //{2}“武学”
                    DateFile.instance.massageDate[8007][4].Split('|')[7],
                    //{3}技能种类名称
                    DateFile.instance.baseSkillDate[101 + gongFaType][0],
                    //{4}“·”
                    __instance.Dit(),
                    //{5}“传自：”
                    DateFile.instance.massageDate[8007][4].Split('|')[0],
                    //{6}传承名称颜色前缀
                    DateFile.instance.massageDate[20003][0],
                    //{7}传承名称文本
                    DateFile.instance.GetGangDate(int.Parse(thisGongFaData[3]), 0)
                    );

                //若为实际人物
                if (actorId != 0)
                {
                    //修习度
                    stringBuilder.AppendFormat("{0}{1}{2}{3}%</color>",
                        //{0}“·”
                        __instance.Dit(),
                        //{1}“修习程度：”
                        DateFile.instance.massageDate[2001][0],
                        //{2}修习度颜色前缀
                        DateFile.instance.massageDate[20004][0],
                        //{3}修习度数字
                        gongFaLevel
                        );

                    #region 弃用
                    ////{7}{0}“·”
                    //stringBuilder.Append(__instance.Dit());
                    ////{8}{1}“修习程度：”
                    //stringBuilder.Append(DateFile.instance.massageDate[2001][0]);
                    ////{9}{2}“实际修习数字%”
                    //stringBuilder.Append(DateFile.instance.SetColoer(20004, gongFaLevel + "%", false));
                    #endregion

                    //功法威力加成（若功法威力加成不为0）
                    if (gongFaBasePower != 0)
                    {
                        #region 弃用
                        //StringBuilder stringBuilderNext1 = new StringBuilder();
                        ////“  (功法威力+/-加成数值%）”
                        //stringBuilderNext1.AppendFormat("  ({0}{1}%)", DateFile.instance.massageDate[8007][4].Split('|')[2], (gongFaBasePower > 0) ? ("+" + gongFaBasePower) : ("-" + Mathf.Abs(gongFaBasePower)));
                        ////上色
                        //stringBuilder.Append(DateFile.instance.SetColoer((gongFaBasePower > 0) ? 20005 : 20010, stringBuilderNext1.ToString(), false));
                        #endregion

                        stringBuilder.AppendFormat("{0}  ({1}{2}{3}%)</color>",
                            //{0}功法威力加成颜色前缀
                            (gongFaBasePower > 0) ? DateFile.instance.massageDate[20005][0] : DateFile.instance.massageDate[20010][0],
                            //{1}“功法威力”
                            DateFile.instance.massageDate[8007][4].Split('|')[2],
                            //{2}+/- 符号
                            (gongFaBasePower > 0) ? "+" : "-",
                            //{3}加成数值
                            (gongFaBasePower > 0) ? gongFaBasePower : Mathf.Abs(gongFaBasePower)
                            );
                    }
                    
                    stringBuilder.Append("\n");

                    //人物心法等级相关
                    //  {0}“·”
                    stringBuilder.Append(__instance.Dit());
                    //  {1}“心法等级：”
                    stringBuilder.Append(DateFile.instance.massageDate[2001][1]);
                    //人物是太吾
                    if (actorId == DateFile.instance.MianActorID())
                    {
                        //人物对该功法的读书逆练等级
                        int gongFaReadingBadFLevel = DateFile.instance.GetGongFaLevel(actorId, gongFaId, 2);

                        //心法等级文本
                        stringBuilder.AppendFormat("{0}{1}</color> {2}(逆.{3})</color>",
                            //{0}心法读书等级颜色前缀
                            DateFile.instance.massageDate[20009][0],
                            //{1}心法读书等级数字
                            gongFaFLevel - skillAddGongFaFLevel,
                            //{2}心法读书逆练等级颜色前缀
                            DateFile.instance.massageDate[20010][0],
                            //{3}心法读书逆练等级数字
                            gongFaReadingBadFLevel
                            );

                        #region 弃用
                        ////读书心法等级数字
                        //stringBuilder.Append(DateFile.instance.SetColoer(20009, (gongFaFLevel - skillAddGongFaFLevel).ToString(), false));
                        ////读书逆练等级数字
                        //stringBuilder.AppendFormat("（{0}逆练 {1}</color>）", DateFile.instance.massageDate[20010][0], gongFaReadingBadFLevel);
                        #endregion

                        //技艺补全部分（若技艺补全了心法等级）
                        if (skillAddGongFaFLevel > 0)
                        {
                            //人物的处世立场
                            int actorGoodness = DateFile.instance.GetActorGoodness(actorId);
                            //技艺提供的心法补全等级颜色：冲解（黄色）
                            int skillAddLevelColoer = 20008;
                            int skillAddType = 0;
                            //根据处世立场决定补全颜色
                            switch (actorGoodness)
                            {
                                //中庸
                                //case 0:
                                //    break;
                                //仁善
                                case 1:
                                //刚正
                                case 2:
                                    //逆练（红色）
                                    skillAddLevelColoer = 20005;
                                    skillAddType = 1;
                                    break;
                                //叛逆
                                case 3:
                                //唯我
                                case 4:
                                    //正练（浅蓝）
                                    skillAddLevelColoer = 20010;
                                    skillAddType = 2;
                                    break;
                            }

                            //技艺补全心法等级数字
                            #region 弃用
                            //stringBuilder.Append(DateFile.instance.SetColoer(skillAddLevelColoer, "+" + skillAddGongFaFLevel, false));
                            //stringBuilder.Append("技艺补全：");
                            //stringBuilder.Append(DateFile.instance.SetColoer(skillAddLevelColoer, "+" + skillAddGongFaFLevel + (skillAddType == 1 ? "正" : skillAddType == 2 ? "逆" : "冲解"), false));
                            #endregion

                            stringBuilder.AppendFormat(" {0}(技艺+{1}{2})</color>",
                                //{0}技艺补全颜色前缀
                                DateFile.instance.massageDate[skillAddLevelColoer][0],
                                //{1}技艺补全等级数字
                                skillAddGongFaFLevel,
                                //{1}技艺补全类型文本
                                (skillAddType == 1) ? "正练" : ((skillAddType == 2) ? "逆练" : "冲解")
                                );
                        }

                        //功法对造诣的提升
                        stringBuilder.AppendFormat(" {0}({1}{2}+{3})</color>\n",
                                //{0}造诣提升颜色前缀
                                DateFile.instance.massageDate[20006][0],
                                //{1}造诣种类名称
                                DateFile.instance.baseSkillDate[101 + gongFaType][0],
                                //{2}“造诣”
                                DateFile.instance.massageDate[8007][1].Split('|')[39],
                                //{3}造诣提升数字
                                DateFile.instance.GetGongFaAchievement(actorId, gongFaId) * gongFaFLevel
                                );
                        #region 弃用
                        //StringBuilder stringBuilderNext2 = new StringBuilder();
                        //stringBuilderNext2.AppendFormat(" ({0}{1}+{2})", new object[]
                        //                    {
                        //                    //{0}功法种类名称
                        //                    DateFile.instance.baseSkillDate[101 + gongFaType][0],
                        //                    //{1}“造诣”
                        //                    DateFile.instance.massageDate[8007][1].Split('|')[39],
                        //                    //{2}造诣增加值数字
                        //                    DateFile.instance.GetGongFaAchievement(actorId, gongFaId) * gongFaFLevel
                        //                    });
                        ////上色
                        //stringBuilder.Append(DateFile.instance.SetColoer(20006, stringBuilderNext2.ToString(), false));
                        //stringBuilder.Append("\n");
                        #endregion
                    }
                    //人物不是太吾
                    else
                    {
                        //人物对该功法的总逆练等级
                        int gongFaBadFLevel = DateFile.instance.GetGongFaBadFLevel(actorId, gongFaId);
                        //心法等级文本
                        stringBuilder.AppendFormat("{0}{1}</color> {2}(逆.{3})</color>\n",
                            //{0}心法总等级颜色前缀
                            DateFile.instance.massageDate[20009][0],
                            //{1}心法总等级数字
                            gongFaFLevel,
                            //{2}心法总逆练等级颜色前缀
                            DateFile.instance.massageDate[20010][0],
                            //{3}心法总逆练等级数字
                            gongFaBadFLevel
                            );

                        #region 弃用
                        ////  {2}心法等级数字
                        //stringBuilder.Append(DateFile.instance.SetColoer(20009, gongFaFLevel.ToString(), false));
                        ////总逆练等级数字
                        //stringBuilder.AppendFormat("（C_20010逆练 {0}</color>）", gongFaBadFLevel);
                        //stringBuilder.Append("\n");
                        #endregion
                    }
                }
                #endregion

                __result = stringBuilder.ToString();

                return false;
            }

            return true;
        }
    }



    /// <summary>
    /// 显示心法逆练等级：书籍浮动信息窗口
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "ShowBookMassage")]
    public static class DisplayActorGongFaBadFLevelInShowBookMassage
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        /// <param name="itemId"></param>
        //[HarmonyPriority(Priority.First)]
        [HarmonyFinalizer]
        private static void ShowBookMassageFinalizer(WindowManage __instance, ref string __result, int itemId)
        //原方法签名
        //private string ShowBookMassage(int itemId)
        {
            #region MyRegion
            /*
            System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.MethodBase showBookMassage = typeof(WindowManage).GetMethod("ShowBookMassage", bindingFlags);
            HarmonyLib.Patches patch = HarmonyLib.Harmony.GetPatchInfo(showBookMassage);

            BetterFix.Main.Logger.LogDebug("WindowManage.ShowBookMassage 的所有前置补丁列表:");
            for (int i = 0; i < patch.Prefixes.Count; i++)
            {
                BetterFix.Main.Logger.LogDebug("补丁序号: " + patch.Prefixes[i].index);
                BetterFix.Main.Logger.LogDebug("所属HarmonyID: " + patch.Prefixes[i].owner);
                BetterFix.Main.Logger.LogDebug("优先级: " + patch.Prefixes[i].priority);
                BetterFix.Main.Logger.LogDebug("指定 before 个数: " + patch.Prefixes[i].before.Length);
                for (int j = 0; j < patch.Prefixes[i].before.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Prefixes[i].before[j]);
                }
                BetterFix.Main.Logger.LogDebug("指定 after 个数: " + patch.Prefixes[i].after.Length);
                for (int j = 0; j < patch.Prefixes[i].after.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Prefixes[i].after[j]);
                }
                BetterFix.Main.Logger.LogDebug("--------");
            }

            BetterFix.Main.Logger.LogDebug("WindowManage.ShowBookMassage 的所有后置补丁列表:");
            for (int i = 0; i < patch.Postfixes.Count; i++)
            {
                BetterFix.Main.Logger.LogDebug("补丁序号: " + patch.Postfixes[i].index);
                BetterFix.Main.Logger.LogDebug("所属HarmonyID: " + patch.Postfixes[i].owner);
                BetterFix.Main.Logger.LogDebug("优先级: " + patch.Postfixes[i].priority);
                BetterFix.Main.Logger.LogDebug("指定 before 个数: " + patch.Postfixes[i].before.Length);
                for (int j = 0; j < patch.Postfixes[i].before.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Postfixes[i].before[j]);
                }
                BetterFix.Main.Logger.LogDebug("指定 after 个数: " + patch.Postfixes[i].after.Length);
                for (int j = 0; j < patch.Postfixes[i].after.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Postfixes[i].after[j]);
                }
                BetterFix.Main.Logger.LogDebug("--------");
            }

            BetterFix.Main.Logger.LogDebug("WindowManage.ShowBookMassage 的所有最终补丁列表:");
            for (int i = 0; i < patch.Finalizers.Count; i++)
            {
                BetterFix.Main.Logger.LogDebug("补丁序号: " + patch.Finalizers[i].index);
                BetterFix.Main.Logger.LogDebug("所属HarmonyID: " + patch.Finalizers[i].owner);
                BetterFix.Main.Logger.LogDebug("优先级: " + patch.Finalizers[i].priority);
                BetterFix.Main.Logger.LogDebug("指定 before 个数: " + patch.Finalizers[i].before.Length);
                for (int j = 0; j < patch.Finalizers[i].before.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Finalizers[i].before[j]);
                }
                BetterFix.Main.Logger.LogDebug("指定 after 个数: " + patch.Finalizers[i].after.Length);
                for (int j = 0; j < patch.Finalizers[i].after.Length; j++)
                {
                    BetterFix.Main.Logger.LogDebug(patch.Finalizers[i].after[j]);
                }
                BetterFix.Main.Logger.LogDebug("--------");
            }
            */
            #endregion

            //QuickLogger.Log(LogLevel.Info, "书籍浮动信息 物品ID:({0}) 技能ID:({1}) 技能种类:({2})", itemId, DateFile.instance.GetItemDate(itemId, 32, true, -1), DateFile.instance.GetItemDate(itemId, 31, true, -1));

            if (Main.Setting.DisplayActorGongFaBadFLevel.Value && int.Parse(DateFile.instance.GetItemDate(itemId, 31, true, -1)) == 17)
            {
                int taiwuActorId = DateFile.instance.MianActorID();

                if (ShopSystem.Exists
                    || BookShopSystem.Exists
                    || Warehouse.instance.warehouseWindow.activeInHierarchy
                    || (ActorMenu.Exists && !ActorMenu.instance.isEnemy)
                    || DateFile.instance.actorItemsDate[taiwuActorId].ContainsKey(itemId)
                    || int.Parse(DateFile.instance.GetActorDate(taiwuActorId, 308, false)) == itemId
                    || int.Parse(DateFile.instance.GetActorDate(taiwuActorId, 309, false)) == itemId
                    || int.Parse(DateFile.instance.GetActorDate(taiwuActorId, 310, false)) == itemId
                    )
                {
                    int bookSkillid = int.Parse(DateFile.instance.GetItemDate(itemId, 32, true));
                    //功法心法等级（包含技艺补全）
                    int gongFaFLevel = DateFile.instance.GetGongFaFLevel(taiwuActorId, bookSkillid, false);
                    //人物对该功法的读书逆练等级
                    int gongFaReadingBadFLevel = DateFile.instance.GetGongFaLevel(taiwuActorId, bookSkillid, 2);
                    //技艺补全等级
                    int skillAddGongFaFLevel = DateFile.instance.GetGongFaFLevel(taiwuActorId, bookSkillid, true);

                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.AppendFormat("已读心法等级：{0}{1}</color> {2}(逆.{3})</color>",
                        //{0}心法读书总等级颜色前缀
                        DateFile.instance.massageDate[20009][0],
                        //{1}心法读书总等级数字
                        gongFaFLevel - skillAddGongFaFLevel,
                        //{2}心法读书逆练等级颜色前缀
                        DateFile.instance.massageDate[20010][0],
                        //{3}心法读书逆练等级数字
                        gongFaReadingBadFLevel
                        );

                    if (skillAddGongFaFLevel > 0)
                    {
                        //人物的处世立场
                        int taiwuGoodness = DateFile.instance.GetActorGoodness(taiwuActorId);
                        //技艺提供的心法补全等级颜色：冲解（黄色）
                        int skillAddLevelColoer = 20008;
                        //技艺提供的心法补全类型
                        int skillAddType = 0;
                        //根据处世立场决定补全颜色
                        switch (taiwuGoodness)
                        {
                            //中庸
                            //case 0:
                            //    break;
                            //仁善
                            case 1:
                            //刚正
                            case 2:
                                //正练（浅蓝）
                                skillAddLevelColoer = 20005;
                                skillAddType = 1;
                                break;
                            //叛逆
                            case 3:
                            //唯我
                            case 4:
                                //逆练（红色）
                                skillAddLevelColoer = 20010;
                                skillAddType = 2;
                                break;
                        }

                        stringBuilder.AppendFormat(" {0}(技艺补全+{1}{2})</color>",
                            //{0}技艺补全颜色前缀
                            DateFile.instance.massageDate[skillAddLevelColoer][0],
                            //{1}技艺补全等级数字
                            skillAddGongFaFLevel,
                            //{2}技艺补全类型文本
                            (skillAddType == 1) ? "正练" : ((skillAddType == 2) ? "逆练" : "冲解")
                            );
                        //stringBuilder.AppendFormat("C_{0}技艺补全：+{1} {2}</color>", skillAddLevelColoer, skillAddGongFaFLevel, (skillAddType == 1 ? "正练" : skillAddType == 2 ? "逆练" : "冲解"));
                    }

                    stringBuilder.Append("\n\n");
                    //原文本（加载原文本之前，因为加在后面会被其他MOD——功法显示增强MOD——给删掉）
                    stringBuilder.Append(__result);

                    __result = stringBuilder.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 显示心法逆练等级：建筑功法修习界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "UpdateStudySkillWindow")]
    public static class DisplayActorGongFaBadFLevelInBuildingStudySkill
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateStudySkillWindowPostfix(BuildingWindow __instance)
        //原方法签名
        //public void UpdateStudySkillWindow()
        {
            if (Main.Setting.DisplayActorGongFaBadFLevel.Value)
            {
                int studySkillId = Traverse.Create(__instance).Field<int>("studySkillId").Value;

                //QuickLogger.Log(LogLevel.Info, "建筑修习更新技能 技能ID:{0} 技能种类:{1}", studySkillId, __instance.studySkillTyp);

                if (studySkillId > 0 && __instance.studySkillTyp == 17)
                {
                    //设为允许文字水平方向超框显示（原本是超框后自动换行，现在设为不自动换行、允许超框）
                    __instance.gongFaFLevelText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    __instance.gongFaFLevelText.text = DisplaySupport.GetTaiwuAdvanceGongFaFLevelText(studySkillId);
                }
            }
        }
    }
    
    /// <summary>
    /// 显示心法逆练等级：建筑功法突破界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "UpdateLevelUPSkillWindow")]
    public static class DisplayActorGongFaBadFLevelInBuildingLevelUPSkill
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateLevelUPSkillWindowPostfix(BuildingWindow __instance)
        //原方法签名
        //public void UpdateLevelUPSkillWindow()
        {
            if (Main.Setting.DisplayActorGongFaBadFLevel.Value)
            {
                //QuickLogger.Log(LogLevel.Info, "建筑突破更新技能 技能ID:{0} 技能种类:{1}", __instance.levelUPSkillId, __instance.studySkillTyp);

                if (__instance.levelUPSkillId > 0 && __instance.studySkillTyp == 17)
                {
                    //设为允许文字水平方向超框显示（原本是超框后自动换行，现在设为不自动换行、允许超框）
                    __instance.levelUPFLevelText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    __instance.levelUPFLevelText.text = DisplaySupport.GetTaiwuAdvanceGongFaFLevelText(__instance.levelUPSkillId);
                }
            }
        }
    }

    /// <summary>
    /// 显示心法逆练等级：建筑书籍研读界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "UpdateReadBookWindow")]
    public static class DisplayActorGongFaBadFLevelInBuildingReadBook
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateReadBookWindowPostfix(BuildingWindow __instance)
        //原方法签名
        //public void UpdateReadBookWindow()
        {
            if (Main.Setting.DisplayActorGongFaBadFLevel.Value)
            {
                int bookSkillId = int.Parse(DateFile.instance.GetItemDate(__instance.readBookId, 32, true, -1));

                //QuickLogger.Log(LogLevel.Info, "建筑研读更新技能 技能ID:{0} 技能种类:{1}", bookSkillId, __instance.studySkillTyp);

                if (bookSkillId > 0 && __instance.studySkillTyp == 17)
                {
                    //设为允许文字水平方向超框显示（原本是超框后自动换行，现在设为不自动换行、允许超框）
                    __instance.readBookFLevelText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    __instance.readBookFLevelText.text = DisplaySupport.GetTaiwuAdvanceGongFaFLevelText(bookSkillId);
                }
            }
        }
    }
}
