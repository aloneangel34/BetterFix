using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 在查看人物界面的功法一览下，显示相枢被动：加入图标
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "SetGongFa")]
    public static class DisplayXXPassiveAddIcon
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void SetGongFaPostfix(ActorMenu __instance, int typ)
        //原方法签名
        //public void SetGongFa(int typ)
        {
            if (Main.Setting.DisplayXXPassiveInActorMenu.Value && typ == 0 && __instance.actorId != DateFile.instance.MianActorID())
            {
                //人物功法ID列表
                List<int> _gongFaIds = new List<int>(DateFile.instance.actorGongFas[__instance.actorId].Keys);
                //人物拥有的相枢被动能力的功法ID列表
                List<int> actorXXPassiveAbilityIds = new List<int>();

                //遍历功法列表更新相枢被动能力列表
                foreach (int gongFaId in _gongFaIds)
                {
                    if (DisplaySupport.XXPassiveAbilityIds.Contains(gongFaId))
                    {
                        actorXXPassiveAbilityIds.Add(gongFaId);
                    }
                }

                //若人物有相枢被动
                if (actorXXPassiveAbilityIds.Count > 0)
                {
                    //从小到大排序
                    actorXXPassiveAbilityIds.Sort();
                    //功法显示框的格状布局组件暂时禁用
                    __instance.gongFaHolder.GetComponent<GridLayoutGroup>().enabled = false;

                    //设置功法图标
                    AtlasInfo.Instance.LoadAtlas("GongFaIcon", delegate (Atlas at, bool b)
                    {
                        //循环设置所有相枢被动
                        for (int i = 0; i < actorXXPassiveAbilityIds.Count; i++)
                        {
                            //相枢被动的功法ID
                            int thisGongFaId = actorXXPassiveAbilityIds[i];

                            //预制体克隆出来的功法图标gameObject（不可点击）
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.gongFaNoToggle, Vector3.zero, Quaternion.identity);
                            //设置gameObject的name（用于后续的SetGongFa方法）
                            gameObject.name = "GongFa," + thisGongFaId;
                            //将图标设为功法查看框的子级
                            gameObject.transform.SetParent(__instance.gongFaHolder, false);
                            //将图标的描绘顺序设为第一位
                            gameObject.transform.SetAsFirstSibling();
                            //获取图标的SetGongFaIcon组件
                            SetGongFaIcon component = gameObject.GetComponent<SetGongFaIcon>();
                            //设置图标
                            component.SetGongFa(thisGongFaId, __instance.actorId);
                            //隐藏功法的装备标记
                            component.SetEquipIcon(false);
                        }

                        //功法显示框的格状布局组件恢复启用
                        __instance.gongFaHolder.GetComponent<GridLayoutGroup>().enabled = true;
                    });
                }
            }
        }
    }

    /// <summary>
    /// 在查看人物界面的功法一览下，显示相枢被动：具体设置修正
    /// </summary>
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(SetGongFaIcon), "SetGongFa")]
    public static class DisplayXXPassiveDetailFixInSetIcon
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyFinalizer]
        private static void SetGongFaFinalizer(SetGongFaIcon __instance, int gongFaId, int actorId)
        //原方法签名
        //public void SetGongFa(int gongFaId, int actorId)
        {
            if(DisplaySupport.XXPassiveAbilityIds.Contains(gongFaId))
            {
                #region 弃用
                ////名字上色为红色（先去除原本的颜色代码）
                //__instance.gongFaNameText.text = DateFile.instance.SetColoer(20010, DisplaySupport.RemoveColorCodeInRichtext(DateFile.instance.gongFaDate[gongFaId][0]));
                #endregion

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}{1}</color>",
                    //{0}颜色代码前缀
                    DateFile.instance.massageDate[20010][0],
                    //{1}（去除原本字体代码后的）功法名称
                    DisplaySupport.RemoveColorCodeInRichtext(DateFile.instance.gongFaDate[gongFaId][0])
                    );

                __instance.gongFaNameText.text = stringBuilder.ToString();
                stringBuilder.Clear();

                //显示为相枢
                //若功法的格子占用数大于0
                if (DateFile.GetCombatSkillDataInt(gongFaId, 7, actorId, true) > 0)
                {
                    #region 弃用
                    //__instance.gongFaSizeText.text = "相枢绝技\n" + DateFile.instance.SetColoer(int.Parse(DateFile.instance.massageDate[2003][2].Split('|')[int.Parse(DateFile.instance.gongFaDate[gongFaId][6])]), DateFile.instance.massageDate[2003][1].Split('|')[Mathf.Clamp(DateFile.GetCombatSkillDataInt(gongFaId, 7, actorId, true) - 1, 0, 2)], false);
                    #endregion

                    stringBuilder.AppendFormat("相枢绝技\n{0}{1}</color>",
                        //{0}功法占用格菱形标志的颜色代码前缀
                        DateFile.instance.massageDate[int.Parse(DateFile.instance.massageDate[2003][2].Split('|')[int.Parse(DateFile.instance.gongFaDate[gongFaId][6])])][0],
                        //{1}功法占用格菱形标志文本
                        DateFile.instance.massageDate[2003][1].Split('|')[Mathf.Clamp(DateFile.GetCombatSkillDataInt(gongFaId, 7, actorId, true) - 1, 0, 2)]
                        );

                    __instance.gongFaSizeText.text = stringBuilder.ToString();
                }
                else
                {
                    __instance.gongFaSizeText.text = "相枢绝技\n";
                }
            }
        }
    }

    /// <summary>
    /// 修正相枢被动能力：功法浮动信息窗口（功法详情）
    /// </summary>
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(WindowManage), "SetGongFaTypText")]
    public static class DisplayXXPassiveDetailFixInSetGongFaTypText
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
            if (DisplaySupport.XXPassiveAbilityIds.Contains(gongFaId))
            {
                //这个功法的数据
                Dictionary<int, string> thisGongFaData = DateFile.instance.gongFaDate[gongFaId];
                //功法类型（心法、身法、绝技、以及所有催破的详细分类）
                //int gongFaType = Mathf.Max(int.Parse(thisGongFaData[1]), 0);
                //功法类型（设为“绝技”）
                int gongFaType = 2;
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
                    //{7}传承名称
                    //DateFile.instance.GetGangDate(int.Parse(thisGongFaData[3])
                    //{7}传承名称（相枢）
                    "相枢"
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

                    //功法威力加成（若功法威力加成不为0）
                    if (gongFaBasePower != 0)
                    {
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
    /// 修正相枢被动能力：功法浮动信息窗口（标题）
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "ShowGongFaMassage")]
    public static class DisplayXXPassiveDetailFixInShowGongFaMassage
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillTyp">技能种类</param>
        /// <param name="levelTyp">正逆练冲解</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="toggle">技能图标对应的Toggle组件</param>
        /// <returns></returns>
        [HarmonyPostfix]
        private static void ShowGongFaMassagePostfix(WindowManage __instance, int skillId, int skillTyp, int levelTyp, int actorId, Toggle toggle)
        //原方法签名
        //private void ShowGongFaMassage(int skillId, int skillTyp, int levelTyp = -1, int actorId = -1, Toggle toggle = null)
        {
            if (DisplaySupport.XXPassiveAbilityIds.Contains(skillId))
            {
                #region 用于对照的原文本
                //int gongFaQualityLevel = int.Parse(DateFile.instance.gongFaDate[skillId][2]);
                //__instance.informationName.text = string.Format("{0}\n{1}", DateFile.instance.gongFaDate[skillId][0], DateFile.instance.SetColoer(20001 + gongFaQualityLevel, DateFile.instance.massageDate[8][3].Split('|')[gongFaQualityLevel] + DateFile.instance.massageDate[8][1], false));
                #endregion

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}{1}\n未知·无法定阶</color>",
                    //{0}颜色代码前缀
                    DateFile.instance.massageDate[20010][0],
                    //{1}（去除原本字体代码后的）功法名称
                    DisplaySupport.RemoveColorCodeInRichtext(DateFile.instance.gongFaDate[skillId][0])
                    );

                __instance.informationName.text = stringBuilder.ToString();
            }
        }
    }


    /// <summary>
    /// 修正相枢被动能力：功法浮动信息窗口（ALT键额外信息）
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "GongFaUseNeed")]
    public static class DisplayXXPassiveDetailFixInGongFaUseNeed
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        /// <param name="gongFaId">功法ID</param>
        /// <param name="gongFaLevel">功法熟练度</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="levelTyp">正逆练冲解</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool GongFaUseNeedPrefix(WindowManage __instance, ref string __result, int gongFaId, int gongFaLevel, int actorId, int levelTyp)
        //原方法签名
        //private string GongFaUseNeed(int gongFaId, int gongFaLevel, int actorId, int levelTyp = -1)
        {
            if (DisplaySupport.XXPassiveAbilityIds.Contains(gongFaId))
            {
                __result = "";
                return false;
            }
            return true;

            //原显示文本（由于相枢被动的功法数据不全、导致"按下Alt键显示更多信息"、生成功法需求信息时会读取到无效值而报错，所以要在上方将更多信息的文本替换为空）
            //this.informationMassage.text = string.Format("{0}{1}", this.baseGongFaMassage, this.GongFaUseNeed(this.showGongFaId, this.showGongFaLevel, this.showGongFaActorId, this.showGongFaLevelTyp));
        }
    }
    
}
