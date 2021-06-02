using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 属性类型
    /// </summary>
    public enum AttrType
    {
        /// <summary>未设置</summary>
        NotSet,
        /// <summary>称号</summary>
        Generation,
        /// <summary>姓名</summary>
        Name,
        /// <summary>性别</summary>
        Gender,
        /// <summary>魅力</summary>
        Charm,
        /// <summary>从属</summary>
        Gang,
        /// <summary>身份</summary>
        GangLevel,
        /// <summary>立场</summary>
        Goodness,
        /// <summary>名誉</summary>
        Fame,
        /// <summary>心情</summary>
        Mood,
        /// <summary>好感</summary>
        Favor,
        /// <summary>轮回</summary>
        Samsara,
        /// <summary>年龄</summary>
        Age,
        /// <summary>健康</summary>
        Health
    }

    /// <summary>
    /// 用于设定单项属性UI的组件
    /// </summary>
    public class AttrUI : MonoBehaviour
    {
        public AttrType AttrUiType = AttrType.NotSet;
        public CImage AttrIcon;
        public Text TipText;
        public Text AttrText;

        /// <summary>
        /// 设定单项属性UI的类型（用于确定图标和属性名称及信息更新方式）（在更新数据前请务必设定一次）
        /// </summary>
        /// <param name="attrType">属性类型（请勿输入AttrType.NoSet）</param>
        public void SetAttrType(AttrType attrType)
        {
            AttrUiType = attrType;

            switch (attrType)
            {
                case AttrType.Generation:
                    TipText.text = "称号";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 0);
                    break;
                case AttrType.Name:
                    TipText.text = "姓名";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 4);
                    break;
                case AttrType.Gender:
                    TipText.text = "性别";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 5);
                    break;
                case AttrType.Charm:
                    TipText.text = "魅力";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 7);
                    break;
                case AttrType.Gang:
                    TipText.text = "从属";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 8);
                    break;
                case AttrType.GangLevel:
                    TipText.text = "身份";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 9);
                    break;
                case AttrType.Goodness:
                    TipText.text = "立场";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 11);
                    break;
                case AttrType.Fame:
                    TipText.text = "名誉";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 10);
                    break;
                case AttrType.Mood:
                    TipText.text = "心情";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 1);
                    break;
                case AttrType.Favor:
                    TipText.text = "好感";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 2);
                    break;
                case AttrType.Samsara:
                    TipText.text = "轮回";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 3);
                    break;
                case AttrType.Age:
                    TipText.text = "年龄";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 35);
                    break;
                case AttrType.Health:
                    TipText.text = "健康";
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(AttrIcon, "attSprites", 35);
                    break;
            }
        }

        /// <summary>
        /// 更新单项属性UI的信息（请确保已设定单项属性UI的类型）
        /// </summary>
        /// <param name="actorId">人物ID</param>
        public void SetAttrInfo(int actorId)
        {
            try
            {
                switch (AttrUiType)
                {
                    //未设置
                    case AttrType.NotSet:
                        AttrText.text = string.Empty;
                        break;
                    //称号
                    case AttrType.Generation:
                        AttrText.text = DateFile.instance.GetActorGeneration(actorId, 999, false);
                        break;
                    //姓名
                    case AttrType.Name:
                        AttrText.text = DateFile.instance.GetActorName(actorId, true, false);
                        break;
                    //性别
                    case AttrType.Gender:
                        AttrText.text = DateFile.instance.massageDate[7][0].Split('|')[int.Parse(DateFile.instance.GetActorDate(actorId, 14, false))];
                        break;
                    //魅力
                    case AttrType.Charm:
                        //未成年
                        if (int.Parse(DateFile.instance.GetActorDate(actorId, 11, false)) <= 14)
                        {
                            //C_20002年幼C_D
                            AttrText.text = DateFile.instance.massageDate[25][5].Split('|')[0];
                        }
                        //实际人物且未着衣
                        else if (int.Parse(DateFile.instance.GetActorDate(actorId, 8, false)) == 1 && int.Parse(DateFile.instance.GetActorDate(actorId, 305, false)) == 0)
                        {
                            //C_20002衣不蔽体C_D
                            AttrText.text = DateFile.instance.massageDate[25][5].Split('|')[1];
                        }
                        else
                        {
                            //魅力文本
                            AttrText.text = DateFile.instance.massageDate[25][int.Parse(DateFile.instance.GetActorDate(actorId, 14, false)) - 1].Split('|')[Mathf.Clamp(int.Parse(DateFile.instance.GetActorDate(actorId, 15, true)) / 100, 0, 9)];
                        };
                        break;
                    //从属
                    case AttrType.Gang:
                        AttrText.text = DateFile.instance.GetGangDate(int.Parse(DateFile.instance.GetActorDate(actorId, 19, false)), 0); ;
                        break;
                    //身份
                    case AttrType.GangLevel:
                        int actorGang = int.Parse(DateFile.instance.GetActorDate(actorId, 19, false));
                        int actorLevelNum = int.Parse(DateFile.instance.GetActorDate(actorId, 20, false));
                        int actorLevelNameIndex = (actorLevelNum < 0) ? (1001 + int.Parse(DateFile.instance.GetActorDate(actorId, 14, false))) : 1001;
                        int gangValueId = DateFile.instance.GetGangValueId(actorGang, actorLevelNum);
                        AttrText.text = DateFile.instance.SetColoer((gangValueId == 0) ? 20002 : (20011 - Mathf.Abs(actorLevelNum)), DateFile.instance.presetGangGroupDateValue[gangValueId][actorLevelNameIndex], false);
                        break;
                    //立场
                    case AttrType.Goodness:
                        AttrText.text = DateFile.instance.massageDate[9][0].Split('|')[DateFile.instance.GetActorGoodness(actorId)]; ;
                        break;
                    //名誉
                    case AttrType.Fame:
                        AttrText.text = DateFile.instance.GetActorFameText(actorId);
                        break;
                    //心情
                    case AttrType.Mood:
                        TipText.text = DateFile.instance.Color2(DateFile.instance.GetActorDate(actorId, 4, false));
                        break;
                    //好感
                    case AttrType.Favor:
                        int taiwuActorId = DateFile.instance.MianActorID();
                        int actorFavor = DateFile.instance.GetActorFavor(false, taiwuActorId, actorId, false, false);
                        AttrText.text = ((actorId == taiwuActorId || actorFavor == -1) ? DateFile.instance.SetColoer(20002, DateFile.instance.massageDate[303][2], false) : DateFile.instance.Color5(actorFavor, true, -1)); ;
                        break;
                    //轮回
                    case AttrType.Samsara:
                        AttrText.text = DateFile.instance.GetLifeDateList(actorId, 801, false).Count.ToString();
                        break;
                    //年龄
                    case AttrType.Age:
                        AttrText.text = DateFile.instance.ShowActorAge(actorId);
                        break;
                    //健康
                    case AttrType.Health:
                        int actorHealth = DateFile.instance.Health(actorId);
                        int actorMaxHealth = DateFile.instance.MaxHealth(actorId);

                        StringBuilder stringBuilder = new StringBuilder();

                        if (int.Parse(DateFile.instance.GetActorDate(actorId, 26, false)) != 0)
                        {
                            stringBuilder.Append("死前：");
                        }

                        if (int.Parse(DateFile.instance.GetActorDate(actorId, 8, false)) != 1)
                        {
                            stringBuilder.AppendFormat("{0}{1}</color> / ???", DateFile.instance.Color3(actorHealth, actorHealth), actorHealth);
                        }
                        else
                        {
                            stringBuilder.AppendFormat("{0}{1}</color> / {2}", DateFile.instance.Color3(actorHealth, actorMaxHealth), actorHealth, actorMaxHealth);
                        }

                        AttrText.text = stringBuilder.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Log(LogLevel.Error, "更新AttrUi文本时出错 name:{0}\n{1}", base.gameObject.name, ex);
                AttrText.text = string.Empty;
            }
        }
    }
}