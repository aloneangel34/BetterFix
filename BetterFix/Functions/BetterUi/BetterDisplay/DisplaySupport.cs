using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 显示信息的支援方法集
    /// </summary>
    public static class DisplaySupport
    {
        /// <summary>
        /// 字段：相枢被动能力列表
        /// </summary>
        private static List<int> _xxPassiveAbilityIds;

        /// <summary>
        /// 属性：相枢被动能力列表
        /// </summary>
        public static List<int> XXPassiveAbilityIds
        {
            get
            {
                //相枢被动能力列表无效时，尝试重新获取
                if (_xxPassiveAbilityIds == null || _xxPassiveAbilityIds.Count == 0)
                {
                    try
                    {
                        List<int> gongFaIds = new List<int>();
                        foreach (var gongFaKeyValuePair in DateFile.instance.gongFaDate)
                        {
                            if (int.Parse(gongFaKeyValuePair.Value[1]) == -1 && int.Parse(gongFaKeyValuePair.Value[3]) == 19)
                            {
                                gongFaIds.Add(gongFaKeyValuePair.Key);
                            }
                        }
                        _xxPassiveAbilityIds = gongFaIds;
                    }
                    catch (Exception ex)
                    {
                        QuickLogger.Log(BepInEx.Logging.LogLevel.Warning, "试图获取相枢被动能力列表时出错\n{0}", ex);
                        return new List<int>();
                    }
                }

                return _xxPassiveAbilityIds;
            }

            private set
            {
                _xxPassiveAbilityIds = value;
            }
        }

        /// <summary>
        /// 删除富文本中的所有颜色代码
        /// </summary>
        /// <param name="richText">需要处理的含有颜色代码的富文本</param>
        /// <returns>处理后不含颜色代码的文本（其他类型的富文本代码未被变更）</returns>
        public static string RemoveColorCodeInRichtext(string richText)
        {
            //比如替换 "<color=#EDA723FF>铜人腧穴图经</color>" 为 -> "铜人腧穴图经"

            // 声明一个 Regex类的变量来定义正则表达式
            Regex start = new Regex("<color=#[A-Za-z0-9]*>|</color>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return start.Replace(richText, string.Empty);
        }


        /// <summary>
        /// 获取人物所属势力名字的文本
        /// </summary>
        /// <param name="actorid">人物ID</param>
        /// <returns></returns>
        public static string GetActorGangText(int actorid)
        {
            return DateFile.instance.GetGangDate(int.Parse(DateFile.instance.GetActorDate(actorid, 19, false)), 0);
        }

        /// <summary>
        /// 获取人物身份称呼的文本（富文本，带有颜色代码——按人物的阶层上色）
        /// </summary>
        /// <param name="actorid">人物ID</param>
        /// <returns></returns>
        public static string GetGangLevelColorText(int actorid)
        {
            return DateFile.instance.SetColoer(20011 - Mathf.Abs(int.Parse(DateFile.instance.GetActorDate(actorid, 20, false))), GetGangLevelText(actorid), false);
        }

        /// <summary>
        /// 获取人物身份称呼的文本
        /// </summary>
        /// <param name="actorid">人物ID</param>
        /// <returns></returns>
        public static string GetGangLevelText(int actorid)
        {
            return DateFile.instance.presetGangGroupDateValue[GetGangValueId(actorid)][(int.Parse(DateFile.instance.GetActorDate(actorid, 20, false)) >= 0) ? 1001 : (1001 + int.Parse(DateFile.instance.GetActorDate(actorid, 14, false)))];
        }

        /// <summary>
        /// 获取人物所属商队的文本（不含是否是商人的判定）
        /// </summary>
        /// <param name="actorid">人物ID</param>
        /// <returns>（没有对应商队时返回"？"）</returns>
        public static string GetShopName(int actorid)
        {
            int shopId = int.Parse(DateFile.instance.GetGangDate(int.Parse(DateFile.instance.GetActorDate(actorid, 9, false)), 16));
            
            if (DateFile.instance.storyShopDate.ContainsKey(shopId))
            {
                return (DateFile.instance.storyShopDate[shopId][0] ?? "？");
            }
            else
            {
                return "？";
            }
        }

        /// <summary>
        /// 获取人物的GangValueId（可用于GangValueIdData.txt）
        /// </summary>
        /// <param name="actorid">人物ID</param>
        /// <returns></returns>
        public static int GetGangValueId(int actorid)
        {
            return DateFile.instance.GetGangValueId(int.Parse(DateFile.instance.GetActorDate(actorid, 19, false)), int.Parse(DateFile.instance.GetActorDate(actorid, 20, false)));
        }

        /// <summary>
        /// 判断人物是否为商人（是否具有“（浏览货物……）”互动选项）
        /// </summary>
        /// <param name="actorId">人物ID</param>
        /// <param name="excludeBaby">是否排除婴儿（无法实际对话）（排除后身份为商人的婴儿也返回否）</param>
        /// <returns>true 是商人 / false 不是</returns>
        public static bool IsActorMerchant(int actorId, bool excludeBaby = false)
        {
            bool result = false;

            //人物的所属势力
            int actorGang = int.Parse(DateFile.instance.GetActorDate(actorId, 19, false));
            //人物的身份品阶
            int actorLevelNumber = int.Parse(DateFile.instance.GetActorDate(actorId, 20, false));
            //人物的GangValueId
            int actorGangValueId = DateFile.instance.GetGangValueId(actorGang, actorLevelNumber);
            //人物的追加互动事件列表
            string[] additionalDialogEvents = DateFile.instance.presetGangGroupDateValue[actorGangValueId][812].Split('|');
            //若人物的对话追加互动事件中有“（浏览货物……）”选项，则人物为商人（若排除婴儿的话，则不可对话的婴儿不会包含在内）
            for (int i = 0; i < additionalDialogEvents.Length; i++)
            {
                int additionalDialogEventId = int.Parse(additionalDialogEvents[i]);

                if (additionalDialogEventId == 901300005 && (!excludeBaby || int.Parse(DateFile.instance.GetActorDate(actorId, 11, false)) > 3))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        //提供WindowManage里的私有方法Color6，由于很简短、又是工具类型的，所以直接复制了，不需要什么挖掘反射
        /// <summary>
        /// 根据数值所处阶段获取对应的颜色序号（用DateFile.instance.massageDate[颜色序号][0]获取颜色代码前缀）
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>蓝色(>最大值) 绿色(>默认值 & <最大值) 黄色(<默认值/2) 红色(<默认值/2)</returns>
        public static int Color6(int currentValue, int maxValue, int defaultValue = 100)
        //原方法签名
        //public static int Color6(int a, int max, int b = 100)
        {
            //黄色（当前值低于默认值，但高于默认值的50%）
            int result = 20008;

            //当前值高于最大值
            if (currentValue >= maxValue)
            {
                //蓝色
                result = 20005;
            }
            else
            {
                //当前值高于默认值，但低于最大值
                if (currentValue >= defaultValue)
                {
                    //绿色
                    result = 20004;
                }
                else
                {
                    //当前值低于默认值的50%
                    if (currentValue < defaultValue * 50 / 100)
                    {
                        //红色
                        result = 20010;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取更详尽的太吾心法等级文本（技艺补全等级\n已读总等级、已读逆练等级）（仅适用于建筑修习界面）
        /// </summary>
        /// <param name="gongFaId">功法ID</param>
        /// <returns>详细的心法等级文本</returns>
        public static string GetTaiwuAdvanceGongFaFLevelText(int gongFaId)
        {
            //太吾人物ID
            int taiwuActorId = DateFile.instance.MianActorID();
            //功法心法等级（包含技艺补全）
            int gongFaFLevel = DateFile.instance.GetGongFaFLevel(taiwuActorId, gongFaId, false);
            //人物对该功法的读书逆练等级
            int gongFaReadingBadFLevel = DateFile.instance.GetGongFaLevel(taiwuActorId, gongFaId, 2);
            //技艺提供的的心法等级
            int skillAddGongFaFLevel = DateFile.instance.GetGongFaFLevel(taiwuActorId, gongFaId, true);

            StringBuilder stringBuilder = new StringBuilder();
            //若技艺提供的了心法等级
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

                //造诣补全心法等级数字
                stringBuilder.AppendFormat("{0}(技艺+{1}{2})</color>\n",
                    //{0}技艺补全颜色前缀
                    DateFile.instance.massageDate[skillAddLevelColoer][0],
                    //{1}技艺补全等级数字
                    skillAddGongFaFLevel,
                    //{2}技艺补全类型文本
                    (skillAddType == 1) ? "正练" : ((skillAddType == 2) ? "逆练" : "冲解")
                    );
                //stringBuilder.AppendFormat("C_{0}技艺补全：+{1} {2}</color>\n", skillAddLevelColoer, skillAddGongFaFLevel, (skillAddType == 1 ? "正练" : skillAddType == 2 ? "逆练" : "冲解"));
            }

            stringBuilder.AppendFormat("{0}{1}{2}</color> {3}(逆.{4})</color>",
                //{0}“心.”
                DateFile.instance.massageDate[2003][0],
                //{1}心法读书总等级颜色前缀
                DateFile.instance.massageDate[20009][0],
                //{2}心法读书总等级数字
                gongFaFLevel - skillAddGongFaFLevel,
                //{3}心法读书逆练等级颜色前缀
                DateFile.instance.massageDate[20010][0],
                //{4}心法读书逆练等级数字
                gongFaReadingBadFLevel
                );

            return stringBuilder.ToString();
        }
    }
}
