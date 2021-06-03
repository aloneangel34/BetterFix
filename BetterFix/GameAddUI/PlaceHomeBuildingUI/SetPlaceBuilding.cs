using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

namespace BetterFix
{
#if true
    public class SetPlaceIcon : MonoBehaviour
    {
        //!--PlaceImageBack
        //  !--PlaceImage
        //  !--PlaceHpBar
        //  !--PlaceLevelBack
        //      !--PlaceLevelText
        //  !--TimeText
        //      !--TimeIcon
        //      !--BuildingText
        //  !--MakeIcon
        //      !--MakeTimeText
        //  !--ActorIcon
        //      !--PctIcon
        //          !--PctText
        //      !--NewMassageIcon
        //          !-NewMassageText
        //  !--PlaceNameText
        //  !--BuildingButton

        //bPlaceImageBack
        //    imagePlaceImage
        //    imagePlaceHpBar
        //    imagePlaceLevelBack
        //        textPlaceLevelText
        //    textTimeText
        //        imageTimeIcon
        //        textBuildingText
        //    imageMakeIcon
        //        textMakeTimeText
        //    imageActorIcon
        //        imagePctIcon
        //            textPctText
        //        imageNewMassageIcon
        //            textNewMassageText
        //    textPlaceNameText
        //    buttonBuildingButton

        internal static Sprite StoryIcon = null;

        //!--PlaceImageBack
        public Transform placeImageBack;
        //  !--PlaceImage
        public Image placeImage;
        //  !--PlaceHpBar
        public Image placeHpBar;
        //  !--PlaceLevelBack
        public GameObject placeLevelBack;
        //      !--PlaceLevelText
        public Text placeLevel;
        //  !--TimeText
        public Text placeTime;
        //      !--BuildingText
        public Text buildingText;
        //      !--TimeIcon
        //  !--MakeIcon
        public Image placeMakeIcon;
        //      !--MakeTimeText
        public Text makeTimeText;
        //  !--ActorIcon
        public GameObject actorIcon;
        //      !--PctIcon
        //          !--PctText
        public Text pctText;
        //      !--NewMassageIcon
        public GameObject newMassageIcon;
        //          !-NewMassageText
        public Text newMassageText;
        //  !--PlaceNameText
        public Text placeName;
        //  !--BuildingButton
        public GameObject buildingButton;

        public bool SetBuilding(int partId = 0, int placeId = 0, int buildingIndex = 0)
        {
            //QuickLogger.Log(LogLevel.Info, "地点建筑图标设置开始 地区ID:{0} 地格ID:{1} 建筑序号:{2}", partId, placeId, buildingIndex);

            if (!PlaceHomeBuildingUI.DoesHomeBuildingExsit(partId, placeId, buildingIndex))
            {
                //QuickLogger.Log(LogLevel.Info, "地点建筑图标设置中 建筑不存在 地区ID:{0} 地格ID:{1} 建筑序号:{2}", partId, placeId, buildingIndex);
                return false;
            }

            List<int> thisHomePlaceData = new List<int>();
            thisHomePlaceData.AddRange(DateFile.instance.homeBuildingsDate[partId][placeId][buildingIndex]);

            //设置建筑点击按钮GameObject的name属性（用于点击事件）
            this.buildingButton.name = string.Format("PlaceBuilding,{0},{1},{2}", partId, placeId, buildingIndex);
            //QuickLogger.Log(LogLevel.Info, "地点建筑图标设置中 按钮名称设置为:{0} 建筑名称:{1}", this.buildingButton.name, DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0]);
            if (partId != DateFile.instance.mianPartId || placeId != DateFile.instance.mianPlaceId)
            {
                this.buildingButton.GetComponent<Button>().interactable = false;
            }

            //重置建设状态文本（由于每次都是新生成的、不需要刷新，所以默认为空、不必重置）
            //this.buildingText.text = "";
            //设置建筑图标
            SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(this.placeImage, "buildingSprites", new int[] { int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][98]) });

            //若建筑损失血量为正
            if (thisHomePlaceData[14] > 0)
            {
                //建筑血条的填充度（100% - 损失血量/总血量）
                this.placeHpBar.fillAmount = 1f - (float)thisHomePlaceData[14] * 100f / (float)DateFile.instance.maxBuildingHp / 100f;
                //建筑血条的颜色（50%以下红色，50%以上橙色）
                this.placeHpBar.color = ((this.placeHpBar.fillAmount <= 0.5f) ? new Color32(255, 0, 0, 255) : new Color32(255, 200, 0, 255));
            }

            //若建筑对应的homePlaceId为正（0为空地）
            if (thisHomePlaceData[0] > 0)
            {
                //若建筑等级为正
                if (thisHomePlaceData[1] > 0)
                {
                    //设置建筑等级文本
                    this.placeLevel.text = thisHomePlaceData[1].ToString();
                    this.placeLevel.alignment = TextAnchor.MiddleCenter;
                }

                //若“建造”剩余时间为正
                if (thisHomePlaceData[3] > 0)
                {
                    //设置建设时间文本
                    this.placeTime.text = thisHomePlaceData[3].ToString();
                    //设置建设状态文本（“建造中”）
                    this.buildingText.text = DateFile.instance.massageDate[157][0].Split('|')[0];
                }

                //若“制造”剩余时间为正
                if (thisHomePlaceData[4] > 0)
                {
                    //设置物品图标（物品ID：thisHomePlaceData[4]）
                    SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(this.placeMakeIcon, "itemSprites", int.Parse(DateFile.instance.makeItemDate[thisHomePlaceData[9]][98]));
                    //设置建设状态文本（“剩余时间 - 1”，若剩余时间为1则显示 “-”）
                    this.makeTimeText.text = ((thisHomePlaceData[4] > 1) ? (thisHomePlaceData[4] - 1).ToString() : DateFile.instance.massageDate[303][2]);
                }

                //若“扩建”剩余时间为正
                if (thisHomePlaceData[5] > 0)
                {
                    //设置建设时间文本
                    this.placeTime.text = thisHomePlaceData[5].ToString();
                    //设置建设状态文本（“扩建中”）
                    this.buildingText.text = DateFile.instance.massageDate[157][0].Split('|')[1];
                }

                //若“撤除/采集”剩余时间为正
                if (thisHomePlaceData[6] > 0)
                {
                    //设置建设时间文本
                    this.placeTime.text = thisHomePlaceData[6].ToString();
                    //设置建设状态文本（“撤除/采集中”）
                    this.buildingText.text = DateFile.instance.massageDate[157][0].Split('|')[2 + thisHomePlaceData[7]];
                }

                //建筑类型
                //2绿色资源 3紫色资源 1太吾村建筑
                int homePlaceType = int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][2]);

                //若建筑所属是 2（非太吾村建筑，不可互动）
                if (thisHomePlaceData[2] == 2)
                {
                    //
                    this.placeName.text = DateFile.instance.SetColoer(20009, DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0], false);
                }
                else
                {
                    //若建筑类型是资源（2绿色资源 3紫色资源）
                    if (homePlaceType >= 2)
                    {
                        //设置建筑名字（绿色资源 颜色20004；紫色资源 颜色20007）
                        this.placeName.text = DateFile.instance.SetColoer((homePlaceType == 2) ? 20004 : 20007, DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0], false);
                    }
                    else
                    {
                        //若建筑类型是非太吾村建筑（0其他建筑）
                        if (homePlaceType == 0)
                        {
                            //设置建筑名字（颜色20002）
                            this.placeName.text = DateFile.instance.SetColoer(20002, DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0], false);
                        }
                        else
                        {
                            //若建筑所属是 1（太吾村建筑，可互动）
                            if (thisHomePlaceData[2] == 1)
                            {
                                //设置建筑名字（颜色20008）
                                this.placeName.text = DateFile.instance.SetColoer(20008, DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0], false);
                            }
                            else
                            {
                                //设置建筑名字（无色）
                                this.placeName.text = DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][0];
                            }
                        }
                    }
                }
            }

            //设置建筑血条显示/隐藏
            bool showHpBar = thisHomePlaceData[14] > 0;
            if (this.placeHpBar.gameObject.activeSelf != showHpBar)
            {
                this.placeHpBar.gameObject.SetActive(showHpBar);
            }
            //设置建筑制造图标的显示/隐藏（若“制造”剩余时间为正则显示）
            bool showMakeIcon = thisHomePlaceData[4] > 0;
            if (this.placeMakeIcon.gameObject.activeSelf != showMakeIcon)
            {
                this.placeMakeIcon.gameObject.SetActive(showMakeIcon);
            }
            //设置建筑名字的显示/隐藏（若“homePlaceId”为正则显示）
            bool showName = thisHomePlaceData[0] > 0;
            if (this.placeName.gameObject.activeSelf != showName)
            {
                this.placeName.gameObject.SetActive(showName);
            }
            //设置建筑建设时间的显示/隐藏（若“homePlaceId”为正，且“建造/扩建/采集/撤除”剩余时间为正则显示）
            bool showTime = thisHomePlaceData[0] > 0 && (thisHomePlaceData[3] > 0 || thisHomePlaceData[5] > 0 || thisHomePlaceData[6] > 0);
            if (this.placeTime.gameObject.activeSelf != showTime)
            {
                this.placeTime.gameObject.SetActive(showTime);
            }
            //设置建筑等级背景的显示/隐藏（若“homePlaceId”为正，且建筑等级为正则显示）
            bool showLevelBack = thisHomePlaceData[0] > 0 && thisHomePlaceData[1] > 0;
            if (this.placeLevelBack.gameObject.activeSelf != showLevelBack)
            {
                this.placeLevelBack.SetActive(showLevelBack);
            }

            //是否显示人物图标（默认不显示）
            bool showActorIcon = false;
            //是否显示新消息图标（默认不显示）
            bool showNewMassageIcon = false;

            //若“homePlaceId”为正，且建筑可以派驻村民，且该建筑的“建设”剩余时间不为正（已建设完成）
            if (thisHomePlaceData[0] > 0 && int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][3]) == 1 && thisHomePlaceData[3] <= 0)
            {
                //若建筑的新消息个数为正
                if (thisHomePlaceData[12] > 0)
                {
                    //显示新消息图标
                    //this.newMassageIcon.SetActive(true);
                    showNewMassageIcon = true;
                    //设置新消息文本的水平溢出设置
                    this.newMassageText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    //设置新消息文本（新消息个数）
                    this.newMassageText.text = thisHomePlaceData[12].ToString();
                }
                //else
                //{
                //    //隐藏新消息图标
                //    this.newMassageIcon.SetActive(false);
                //}

                //（当前建筑已配置工作村民）若村民工作数据中，包含partId，且对应的[partId]中包含placeId，且对应的[partId][placeId]中包含buildingIndex
                if (DateFile.instance.actorsWorkingDate.ContainsKey(partId) && DateFile.instance.actorsWorkingDate[partId].ContainsKey(placeId) && DateFile.instance.actorsWorkingDate[partId][placeId].ContainsKey(buildingIndex))
                {
                    //显示建筑的工作人物图标
                    //this.actorIcon.gameObject.SetActive(true);
                    showActorIcon = true;

                    //是否显示工作进度文本（默认不显示）
                    bool showPctText = false;

                    //若建筑不具有轮回台特效
                    if (int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][83]) == 0)
                    {
                        //工作村民的人物ID
                        int buildingWorkingActorId = DateFile.instance.actorsWorkingDate[partId][placeId][buildingIndex];
                        //人物对太吾的好感
                        int actorFavor = DateFile.instance.GetActorFavor(false, DateFile.instance.MianActorID(), buildingWorkingActorId, false, false);
                        //设置工作进度文本的文本（若积累总量为正：thisHomePlaceData[11]/积累总量的百分比；积累总量不为正：亲密度类型）
                        this.pctText.text = ((int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][91]) > 0) ? (thisHomePlaceData[11] * 100 / int.Parse(DateFile.instance.basehomePlaceDate[thisHomePlaceData[0]][91]) + "%") : ((actorFavor != -1) ? DateFile.instance.Color5(actorFavor, true, -1) : DateFile.instance.SetColoer(20002, DateFile.instance.massageDate[303][2], false)));
                        //显示工作进度文本
                        //this.pctText.transform.parent.gameObject.SetActive(true);
                        showPctText = true;
                    }
                    //建筑具有轮回台特效
                    else
                    {
                        //隐藏工作进度文本
                        //this.pctText.transform.parent.gameObject.SetActive(false);
                        //隐藏新消息图标
                        //this.newMassageIcon.SetActive(false);
                        showNewMassageIcon = false;
                    }

                    //设置工作进度文本的显示/隐藏
                    if (this.pctText.transform.parent.gameObject.activeSelf != showPctText)
                    {
                        this.pctText.transform.parent.gameObject.SetActive(showPctText);
                    }
                }
                else
                {
                    ////隐藏建筑的工作人物图标
                    //this.actorIcon.gameObject.SetActive(false);
                    //隐藏新消息图标
                    //this.newMassageIcon.SetActive(false);
                    showNewMassageIcon = false;
                }
            }
            //else
            //{
            //    //隐藏建筑的工作人物图标
            //    this.actorIcon.gameObject.SetActive(false);
            //}

            //设置人物图标的显示 / 隐藏
            if (this.actorIcon.gameObject.activeSelf != showActorIcon)
            {
                this.actorIcon.gameObject.SetActive(showActorIcon);
            }
            //设置新消息图标的显示 / 隐藏
            if (this.newMassageIcon.activeSelf != showNewMassageIcon)
            {
                this.newMassageIcon.SetActive(showNewMassageIcon);
            }

            return true;
        }

        public bool SetStory(int partId = 0, int placeId = 0, int storyId = 0)
        {
            if (!(DateFile.instance.HaveStory(partId, placeId) && DateFile.instance.worldMapState[partId][placeId][0] == storyId))
            {
                //QuickLogger.Log(LogLevel.Info, "地点奇遇图标设置中 奇遇不存在 地区ID:{0} 地格ID:{1} 奇遇ID:{2}", partId, placeId, storyId);
                return false;
            }

            int[] placeWorldMapState = DateFile.instance.worldMapState[partId][placeId];

            #region 弃用
            /*
            if (StoryIcon == null)
            {
                try
                {
                    //GameObject StoryIconGO = GameObject.Find("UIRoot/Canvas/UIBackGround/WorldMapSystem/WorldMapView/WorldMapViewport/WorldMapMapHolder/WorldMapPlace,0,0,0/StoryIcon");
                    GameObject worldMapSystemGO = GameObject.Find("UIRoot/Canvas/UIBackGround/WorldMapSystem");

                    if (worldMapSystemGO != null)
                    {
                        StoryIcon = worldMapSystemGO.GetComponent<WorldMapSystem>().placePrefab.storyImage;
                    }
                    else
                    {
                        QuickLogger.Log(LogLevel.Info, "WorldMapSystem未创建");
                    }

                    //StoryIcon = Resources.Load<GameObject>("WorldMapPlace").transform.Find("StoryIcon").GetComponent<Image>();
                }
                catch (Exception ex)
                {
                    QuickLogger.Log(LogLevel.Info, "尝试获取StoryIcon时发生异常:{0}", ex);
                }
            }

            if (StoryIcon != null)
            {
                //设置奇遇图标
                this.placeImage.sprite = StoryIcon.sprite;
                this.placeImage.color = StoryIcon.color;
            }
            else
            {
                QuickLogger.Log(LogLevel.Warning, "设置奇遇图标StoryIcon失败");
            }
            */
            #endregion

            if (StoryIcon == null)
            {
                //第一次获取奇遇图标
                try
                {
                    Transform openToStoryButton = Resources.Load<GameObject>("oldsceneprefabs/chooseplacewindow").transform.Find("MoveRoot/OpenToStoryButton,619");
                    StoryIcon = openToStoryButton.GetComponent<Image>().sprite;
                }
                catch (Exception ex1)
                {
                    QuickLogger.Log(LogLevel.Warning, "第一次获取奇遇图标Sprite失败\n{0}", ex1);

                    //第二次获取奇遇图标
                    try
                    {
                        ResLoader.Load<Sprite>("Graphics/Buttons/OpenToStoryButton", delegate (Sprite sp) { StoryIcon = sp; }, false, null);
                    }
                    catch (Exception ex2)
                    {
                        QuickLogger.Log(LogLevel.Warning, "第二次获取奇遇图标Sprite失败\n{0}", ex2);
                    }
                }
            }

            if (StoryIcon != null)
            {
                //设置奇遇图标
                this.placeImage.sprite = StoryIcon;
            }

            //进入地点奇遇图标
            //ResLoader.Load<Sprite>("Graphics/Buttons/OpenToStoryButton", delegate (Sprite sp) { this.placeImage.sprite = sp; }, false, null);
            //地点奇遇图标（若要使用需要额外上色——设置image.color）
            //ResLoader.Load<Sprite>("Graphics/BaseUI/PlaceEventIcon", delegate (Sprite sp) { this.placeImage.sprite = sp; }, false, null);

            StringBuilder stringBuilder = new StringBuilder();

            //设定按钮GameObject的name属性——用于按钮点击事件
            stringBuilder.AppendFormat("PlaceStory,{0},{1},{2}", partId, placeId, storyId);
            this.buildingButton.name = stringBuilder.ToString();
            //this.buildingButton.name = string.Format("PlaceStory,{0},{1},{2}", partId, placeId, storyId);

            //QuickLogger.Log(LogLevel.Info, "地点奇遇图标设置中 按钮名称设置为:{0} 奇遇名称:{1}", this.buildingButton.name, DateFile.instance.baseStoryDate[storyId][0]);

            //奇遇难度
            int storyDiffcultyNum = 0;
            int.TryParse(DateFile.instance.baseStoryDate[storyId][3], out storyDiffcultyNum);
            //字体颜色ID
            int colorId = (storyDiffcultyNum > 0) ? (20001 + Mathf.Clamp(storyDiffcultyNum, 1, 9)) : ((storyDiffcultyNum == -1) ? 20011 : 10006);

            #region !----- 设置建筑（奇遇）等级文本 -----!

            stringBuilder.Clear();
            //若难度为-1 且 剑冢人物列表中包含对应的奇遇人物（即奇遇为剑冢）
            if (storyDiffcultyNum == -1 && xxBossActorIdList.Contains(storyId - 18000))
            {
                int xxRemainNum = 0;
                foreach (var partState in DateFile.instance.worldMapState.Values)
                {
                    foreach (var placeState in partState.Values)
                    {
                        if (xxBossActorIdList.Contains(placeState[0] - 18000))
                        {
                            xxRemainNum++;
                        }
                    }
                }

                //剑冢难度等级（1～7）
                int xxDiffcult  = Mathf.Min(DateFile.instance.GetWorldXXLevel(false), 6) + 1;

                //例如<color=#8FBAE7FF>(剩999剑冢) 剑七</color>
                stringBuilder.AppendFormat("{0}(剩{1}剑冢) 剑{2}</color>",
                    //{0}颜色前缀
                    DateFile.instance.massageDate[20010][0],
                    //{1}全部地图内剑冢奇遇的总数量
                    xxRemainNum,
                    //{2}剑冢难度等级对应的中文数字
                    NumChineseName[xxDiffcult]
                    );

                this.placeLevel.text = stringBuilder.ToString();
                //this.placeLevel.text = DateFile.instance.SetColoer(colorId, ("剑" + NumChineseName[7 - xxRemainNum + 1]));

            }
            else
            {
                if (storyDiffcultyNum < 0)
                {
                    stringBuilder.AppendFormat("{0}未知</color>",
                        //{0}颜色前缀
                        DateFile.instance.massageDate[colorId][0]
                        );
                }
                else
                {
                    stringBuilder.AppendFormat("{0}难{1}</color>",
                        //{0}颜色前缀
                        DateFile.instance.massageDate[colorId][0],
                        //{1}奇遇难度等级文本
                        (storyDiffcultyNum > 10) ? storyDiffcultyNum.ToString() : NumChineseName[storyDiffcultyNum]
                        );
                }

                this.placeLevel.text = stringBuilder.ToString();
                //this.placeLevel.text = (storyDiffcultyNum > 10) ? DateFile.instance.SetColoer(colorId, "难" + storyDiffculty) : ((storyDiffcultyNum > 0) ? DateFile.instance.SetColoer(colorId, "难" + NumChineseName[storyDiffcultyNum]) : DateFile.instance.SetColoer(colorId, "未知"));
            }

            this.placeLevel.alignment = TextAnchor.MiddleRight;

            //设置建筑（奇遇）名称文本
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0}{1}</color>",
                //{0}颜色前缀
                DateFile.instance.massageDate[colorId][0],
                //{1}奇遇名称
                DateFile.instance.baseStoryDate[storyId][0]
                );

            this.placeName.text = stringBuilder.ToString();
            //this.placeName.text = DateFile.instance.SetColoer(colorId, DateFile.instance.baseStoryDate[storyId][0]);
            #endregion

            #region !----- 设置建设时间文本（奇遇进入所需耗时） -----!

            stringBuilder.Clear();
            stringBuilder.AppendFormat("-{0}",
                //{0}奇遇进入所需耗时
                DateFile.instance.baseStoryDate[storyId][11]
                );

            this.placeTime.text = stringBuilder.ToString();
            //this.placeTime.text = "-" + DateFile.instance.baseStoryDate[storyId][11];
            #endregion

            #region !----- 设置建设状态文本（奇遇剩余持续时间） -----!

            //奇遇剩余持续时间
            int storyDuration = placeWorldMapState[1];
            //若奇遇剩余持续时间不为负
            if (storyDuration >= 0)
            {
                //设置建设状态文本（奇遇剩余持续时间）
                stringBuilder.Clear();
                stringBuilder.AppendFormat("剩余:{0}",
                    //{0}奇遇剩余持续时间
                    storyDuration
                    );

                this.buildingText.text = stringBuilder.ToString();
                //this.buildingText.text = "剩余:" + storyDuration.ToString();
            }
            else
            {
                this.buildingText.text = string.Empty;
            }
            #endregion

            //隐藏建筑血条
            if (this.placeHpBar.gameObject.activeSelf)
            {
                this.placeHpBar.gameObject.SetActive(false);
            }

            //隐藏制造图标
            if (this.placeMakeIcon.gameObject.activeSelf)
            {
                this.placeMakeIcon.gameObject.SetActive(false);
            }

            //隐藏建筑的工作人物图标
            if (this.actorIcon.gameObject.activeSelf)
            {
                this.actorIcon.gameObject.SetActive(false);
            }

            return true;
        }

        /// <summary>
        /// 0～10 int对应的中文字符
        /// </summary>
        public static readonly Dictionary<int, string> NumChineseName = new Dictionary<int, string>
        {
            { 0,"零" },
            { 1,"一" },
            { 2,"二" },
            { 3,"三" },
            { 4,"四" },
            { 5,"五" },
            { 6,"六" },
            { 7,"七" },
            { 8,"八" },
            { 9,"九" },
            { 10,"十" },
        };

        public static readonly List<int> xxBossActorIdList = new List<int>
        {
            2001,
            2002,
            2003,
            2004,
            2005,
            2006,
            2007,
            2008,
            2009
        };
    }
#endif
}


