using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.Components;
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;
using BepInEx.Logging;

namespace BetterFix
{
#if true
    public partial class PlaceHomeBuildingUI
    {
        internal static CImage PlaceActor = null;
        internal static Sprite BaseCircleBack = null;
        internal static Sprite TimeIcon = null;
        internal static Sprite IsTeamActorIcon = null;

        public static Container CreatTemplateHomePlace(int index)
        {
            Transform oriHomeSystemWindow = Resources.Load<GameObject>("oldsceneprefabs/homesystemwindow").transform;

            //!----- 获取图片资源 -----!

            if (PlaceActor == null)
            {
                PlaceActor = Resources.Load<GameObject>("prefabs/ui/views/ui_PlaceActorWindow").transform.Find("ActorView/ActorViewport/PlaceActor/Actor,id").GetComponent<CImage>();
            }

            if (BaseCircleBack == null)
            {
                BaseCircleBack = oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/PlaceHpBar").GetComponent<Image>().sprite;
            }

            if (TimeIcon == null)
            {
                TimeIcon = oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/TimeText/TimeIcon").GetComponent<Image>().sprite;
            }

            if (IsTeamActorIcon == null)
            {
                IsTeamActorIcon = oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/ActorIcon").GetComponent<Image>().sprite;
            }

            //!----- 模板主容器 -----!

            Container placeBuildingHolder = new Container
            {
                Name = "PlaceBuildingHolder" + index.ToString(),
                //BackgroundImage = Resources.Load<GameObject>("prefabs/ui/views/ui_PlaceActorWindow").transform.Find("ActorView/ActorViewport/PlaceActor/Actor,id").GetComponent<CImage>(),
                BackgroundImage = PlaceActor,
            };
            RemoveLayout(placeBuildingHolder.GameObject);
            placeBuildingHolder.RectTransform.sizeDelta = IconCellSize;


            //!----- 子项创建开始 -----!

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

            //!--PlaceImageBack
            GameObject bPlaceImageBack = new GameObject();
            bPlaceImageBack.name = "PlaceImageBack";
            bPlaceImageBack.AddComponent<RectTransform>();
            RectTransform rectPlaceImageBack = bPlaceImageBack.GetComponent<RectTransform>();
            rectPlaceImageBack.SetParent(placeBuildingHolder.RectTransform);
            rectPlaceImageBack.anchorMin = new Vector2(0.5f, 0.5f);
            rectPlaceImageBack.anchorMax = new Vector2(0.5f, 0.5f);
            rectPlaceImageBack.anchoredPosition = new Vector2(0f, 0f);
            rectPlaceImageBack.sizeDelta = new Vector2(80f, 80f);
            rectPlaceImageBack.pivot = new Vector2(0.5f, 0.5f);

            //FFFFFFFF
            Image imagePlaceImage = UnityUIGameObjectSupport.CreateImageGameObject("PlaceImage", Color.white);
            //Sprite非固定
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/PlaceImage").GetComponent<Image>();
            imagePlaceImage.rectTransform.SetParent(rectPlaceImageBack);
            imagePlaceImage.rectTransform.anchorMin = new Vector2(0f, 0f);
            imagePlaceImage.rectTransform.anchorMax = new Vector2(1f, 1f);
            imagePlaceImage.rectTransform.anchoredPosition = new Vector2(0f, 15f);
            imagePlaceImage.rectTransform.sizeDelta = new Vector2(40f, 40f);
            imagePlaceImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            //6DB75FFF
            Image imagePlaceHpBar = UnityUIGameObjectSupport.CreateImageGameObject("PlaceHpBar", new Color32(101, 183, 95, 255), true);
            imagePlaceHpBar.sprite = BaseCircleBack;
            //ResLoader.Load<Sprite>("Graphics/BaseCircleBack", delegate (Sprite sp) { imagePlaceHpBar.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "PlaceHpBar 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/PlaceHpBar").GetComponent<Image>();
            imagePlaceHpBar.rectTransform.SetParent(rectPlaceImageBack);
            imagePlaceHpBar.rectTransform.anchorMin = new Vector2(1f, 0f);
            imagePlaceHpBar.rectTransform.anchorMax = new Vector2(1f, 0f);
            imagePlaceHpBar.rectTransform.anchoredPosition = new Vector2(20f, 100f);
            imagePlaceHpBar.rectTransform.sizeDelta = new Vector2(35f, 35f);
            imagePlaceHpBar.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            #region !--PlaceLevelBack
            //  !--PlaceLevelBack
            //      !--PlaceLevelText

            //191919FF
            Image imagePlaceLevelBack = UnityUIGameObjectSupport.CreateImageGameObject("PlaceLevelBack", new Color32(25, 25, 25, 255));
            imagePlaceLevelBack.sprite = BaseCircleBack;
            //ResLoader.Load<Sprite>("Graphics/BaseCircleBack", delegate (Sprite sp) { imagePlaceLevelBack.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "PlaceLevelBack 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/PlaceLevelBack").GetComponent<CImage>();
            imagePlaceLevelBack.rectTransform.SetParent(rectPlaceImageBack);
            imagePlaceLevelBack.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            imagePlaceLevelBack.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            imagePlaceLevelBack.rectTransform.anchoredPosition = new Vector2(60f, 60f);
            imagePlaceLevelBack.rectTransform.sizeDelta = new Vector2(30f, 30f);
            imagePlaceLevelBack.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            //E49A52FF
            Text textPlaceLevelText = UnityUIGameObjectSupport.CreateTextGameObject("PlaceLevelText", new Color32(228, 154, 82, 255), true);
            textPlaceLevelText.rectTransform.SetParent(imagePlaceLevelBack.rectTransform);
            textPlaceLevelText.rectTransform.anchorMin = new Vector2(0f, 0f);
            textPlaceLevelText.rectTransform.anchorMax = new Vector2(1f, 1f);
            textPlaceLevelText.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            textPlaceLevelText.rectTransform.sizeDelta = new Vector2(0f, 0f);
            textPlaceLevelText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textPlaceLevelText.text = "99";
            #endregion

            #region !--TimeText
            //  !--TimeText
            //      !--TimeIcon
            //      !--BuildingText

            //8FBAE7FF
            Text textTimeText = UnityUIGameObjectSupport.CreateTextGameObject("TimeText", new Color32(143, 186, 231, 255));
            textTimeText.rectTransform.SetParent(rectPlaceImageBack);
            textTimeText.rectTransform.anchorMin = new Vector2(1f, 0f);
            textTimeText.rectTransform.anchorMax = new Vector2(1f, 0f);
            textTimeText.rectTransform.anchoredPosition = new Vector2(17.5f, 7.5f);
            textTimeText.rectTransform.sizeDelta = new Vector2(40f, 25f);
            textTimeText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textTimeText.text = "999";

            //E4504DFF
            Image imageTimeIcon = UnityUIGameObjectSupport.CreateImageGameObject("TimeIcon", new Color32(228, 80, 77, 255));
            imageTimeIcon.sprite = TimeIcon;
            //ResLoader.Load<Sprite>("Graphics/ResourceIcon/TimeIcon", delegate (Sprite sp) { imageTimeIcon.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "TimeIcon 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/TimeText/TimeIcon").GetComponent<Image>();
            imageTimeIcon.rectTransform.SetParent(textTimeText.rectTransform);
            imageTimeIcon.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            imageTimeIcon.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            imageTimeIcon.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            imageTimeIcon.rectTransform.sizeDelta = new Vector2(25f, 25f);
            imageTimeIcon.rectTransform.pivot = new Vector2(1f, 0.5f);

            //E3C66DFF
            Text textBuildingText = UnityUIGameObjectSupport.CreateTextGameObject("BuildingText", new Color32(227, 198, 109, 255));
            textBuildingText.rectTransform.SetParent(textTimeText.rectTransform);
            textBuildingText.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            textBuildingText.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            textBuildingText.rectTransform.anchoredPosition = new Vector2(-65f, 0f);
            textBuildingText.rectTransform.sizeDelta = new Vector2(50f, 25f);
            textBuildingText.rectTransform.pivot = new Vector2(1f, 0.5f);
            #endregion

            #region !--MakeIcon
            //  !--MakeIcon
            //      !--MakeTimeText

            //FFFFFFFF
            Image imageMakeIcon = UnityUIGameObjectSupport.CreateImageGameObject("MakeIcon", Color.white);
            //Sprite非固定
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/MakeIcon").GetComponent<Image>();
            imageMakeIcon.rectTransform.SetParent(rectPlaceImageBack);
            imageMakeIcon.rectTransform.anchorMin = new Vector2(0f, 1f);
            imageMakeIcon.rectTransform.anchorMax = new Vector2(0f, 1f);
            imageMakeIcon.rectTransform.anchoredPosition = new Vector2(-12.5f, 12.5f);
            imageMakeIcon.rectTransform.sizeDelta = new Vector2(35f, 35f);
            imageMakeIcon.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            //E3C66DFF
            Text textMakeTimeText = UnityUIGameObjectSupport.CreateTextGameObject("MakeTimeText", new Color32(227, 198, 109, 255));
            textMakeTimeText.rectTransform.SetParent(imageMakeIcon.rectTransform);
            textMakeTimeText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMakeTimeText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMakeTimeText.rectTransform.anchoredPosition = new Vector2(0f, -15f);
            textMakeTimeText.rectTransform.sizeDelta = new Vector2(40f, 25f);
            textMakeTimeText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textMakeTimeText.text = "999";
            #endregion

            #region !--ActorIcon
            //  !--ActorIcon
            //      !--PctIcon
            //          !--PctText
            //      !--NewMassageIcon
            //          !-NewMassageText

            //E3C66DFF
            Image imageActorIcon = UnityUIGameObjectSupport.CreateImageGameObject("ActorIcon", new Color32(227, 198, 109, 255));
            imageActorIcon.sprite = IsTeamActorIcon;
            //ResLoader.Load<Sprite>("Graphics/BaseUI/IsTeamActorIcon", delegate (Sprite sp) { imageActorIcon.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "ActorIcon 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/ActorIcon").GetComponent<Image>();
            imageActorIcon.rectTransform.SetParent(rectPlaceImageBack);
            imageActorIcon.rectTransform.anchorMin = new Vector2(0f, 1f);
            imageActorIcon.rectTransform.anchorMax = new Vector2(0f, 1f);
            imageActorIcon.rectTransform.anchoredPosition = new Vector2(5f, -5f);
            imageActorIcon.rectTransform.sizeDelta = new Vector2(35f, 35f);
            imageActorIcon.rectTransform.pivot = new Vector2(1f, 0f);

            //191919FF
            Image imagePctIcon = UnityUIGameObjectSupport.CreateImageGameObject("PctIcon", new Color32(25, 25, 25, 0));
            imagePctIcon.sprite = BaseCircleBack;
            //ResLoader.Load<Sprite>("Graphics/BaseCircleBack", delegate (Sprite sp) { imagePctIcon.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "PctIcon 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/ActorIcon/PctIcon").GetComponent<Image>();
            imagePctIcon.rectTransform.SetParent(imageActorIcon.rectTransform);
            imagePctIcon.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            imagePctIcon.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            imagePctIcon.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            imagePctIcon.rectTransform.sizeDelta = new Vector2(40f, 15f);
            imagePctIcon.rectTransform.pivot = new Vector2(0.5f, 1f);

            //E3C66DFF
            Text textPctText = UnityUIGameObjectSupport.CreateTextGameObject("PctText", new Color32(227, 198, 109, 255));
            textPctText.rectTransform.SetParent(imagePctIcon.rectTransform);
            textPctText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textPctText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textPctText.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            textPctText.rectTransform.sizeDelta = new Vector2(40f, 25f);
            textPctText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textPctText.text = "100%";

            //191919FF
            Image imageNewMassageIcon = UnityUIGameObjectSupport.CreateImageGameObject("NewMassageIcon", new Color32(25, 25, 25, 0));
            imageNewMassageIcon.sprite = BaseCircleBack;
            //ResLoader.Load<Sprite>("Graphics/BaseCircleBack", delegate (Sprite sp) { imageNewMassageIcon.sprite = sp; }, false, delegate (string ex) { QuickLogger.Log(LogLevel.Warning, "NewMassageIcon 设置出错\n{0}", ex); });
            //oriHomeSystemWindow.Find("HomeView/HomeViewport/HomeMapHolder/HomeMapPlace (0)/PlaceImageBack/ActorIcon/NewMassageIcon").GetComponent<Image>();
            imageNewMassageIcon.rectTransform.SetParent(imageActorIcon.rectTransform);
            imageNewMassageIcon.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            imageNewMassageIcon.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            imageNewMassageIcon.rectTransform.anchoredPosition = new Vector2(0f, -15f);
            imageNewMassageIcon.rectTransform.sizeDelta = new Vector2(40f, 15f);
            imageNewMassageIcon.rectTransform.pivot = new Vector2(0.5f, 1f);

            //FFFFFFFF
            Text textNewMassageText = UnityUIGameObjectSupport.CreateTextGameObject("NewMassageText", Color.white);
            textNewMassageText.rectTransform.SetParent(imageNewMassageIcon.rectTransform);
            textNewMassageText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textNewMassageText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textNewMassageText.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            textNewMassageText.rectTransform.sizeDelta = new Vector2(20f, 25f);
            textNewMassageText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textNewMassageText.text = "99";
            #endregion

            //9B8773FF
            Text textPlaceNameText = UnityUIGameObjectSupport.CreateTextGameObject("PlaceNameText", new Color32(155, 135, 115, 255), true);
            textPlaceNameText.rectTransform.SetParent(rectPlaceImageBack);
            textPlaceNameText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textPlaceNameText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textPlaceNameText.rectTransform.anchoredPosition = new Vector2(0f, -60f);
            textPlaceNameText.rectTransform.sizeDelta = new Vector2(150f, 30f);
            textPlaceNameText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            textPlaceNameText.text = "Name";

            //FFFFFF00
            UnityEngine.UI.Button buttonBuildingButton = UnityUIGameObjectSupport.CreateButtonGameObject("PlaceBuilding,Part,Place,Index", imagePlaceImage);
            RectTransform rectTransformBuildingButton = buttonBuildingButton.GetComponent<RectTransform>();
            rectTransformBuildingButton.SetParent(rectPlaceImageBack);
            rectTransformBuildingButton.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransformBuildingButton.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransformBuildingButton.anchoredPosition = new Vector2(0f, 0f);
            rectTransformBuildingButton.sizeDelta = new Vector2(100f, 100f);
            rectTransformBuildingButton.pivot = new Vector2(0.5f, 0.5f);
            buttonBuildingButton.gameObject.AddComponent<PointerClick>();
            PointerClick pointerClickBuildingButton = buttonBuildingButton.GetComponent<PointerClick>();
            pointerClickBuildingButton.SEKey = "SE_BUTTONDEFAULT";
            buttonBuildingButton.onClick.AddListener(() => BuildingButtonOnClick(buttonBuildingButton));

            //!--------子项创建结束--------!

            #region 添加SetPlaceBuilding组件
            placeBuildingHolder.GameObject.AddComponent<SetPlaceIcon>();
            SetPlaceIcon bSetPlaceBuilding = placeBuildingHolder.GameObject.GetComponent<SetPlaceIcon>();

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

            bSetPlaceBuilding.placeImageBack = rectPlaceImageBack;
            bSetPlaceBuilding.placeImage = imagePlaceImage;
            bSetPlaceBuilding.placeHpBar = imagePlaceHpBar;
            bSetPlaceBuilding.placeLevelBack = imagePlaceLevelBack.gameObject;
            bSetPlaceBuilding.placeLevel = textPlaceLevelText;
            bSetPlaceBuilding.placeTime = textTimeText;
            bSetPlaceBuilding.buildingText = textBuildingText;
            bSetPlaceBuilding.placeMakeIcon = imageMakeIcon;
            bSetPlaceBuilding.makeTimeText = textMakeTimeText;
            bSetPlaceBuilding.actorIcon = imageActorIcon.gameObject;
            bSetPlaceBuilding.pctText = textPctText;
            bSetPlaceBuilding.newMassageIcon = imageNewMassageIcon.gameObject;
            bSetPlaceBuilding.newMassageText = textNewMassageText;
            bSetPlaceBuilding.placeName = textPlaceNameText;
            bSetPlaceBuilding.buildingButton = buttonBuildingButton.gameObject;
            #endregion

            return placeBuildingHolder;
        }

        public static void RemoveLayout(GameObject gameObject)
        {
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<UnityUIKit.Components.BoxElement>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<UnityEngine.UI.LayoutElement>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<UnityUIKit.Components.BoxGroup>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<UnityEngine.UI.VerticalLayoutGroup>());
        }

        /// <summary>
        /// 图标上的按钮点击时执行的动作
        /// </summary>
        /// <param name="button">对应的UnityEngine.UI按钮</param>
        public static void BuildingButtonOnClick(UnityEngine.UI.Button button)
        {
            string[] nameList = button.name.Split(',');
            int[] ids = new int[nameList.Length - 1];

            for (int i = 0; i < ids.Length; i++)
            {
                if (!int.TryParse(nameList[i + 1], out ids[i]))
                {
                    //调试信息
                    if (Main.Setting.debugMode.Value)
                    {
                        QuickLogger.Log(LogLevel.Warning, "地点奇遇/建筑按钮：尝试读取数据出错 name:{0}", button.name);
                    }
                    break;
                }
            }

            if (ids.Length == 3)
            {
                //建筑
                if (nameList[0] == "PlaceBuilding")
                {
                    if (DoesHomeBuildingExsit(ids[0], ids[1], ids[2]))
                    {
                        //调试信息
                        if (Main.Setting.debugMode.Value)
                        {
                            QuickLogger.Log(LogLevel.Info, "开启建筑页面 建筑Index:{0} 名称{1}", ids[2], DateFile.instance.homeBuildingsDate[ids[0]][ids[1]][ids[2]][0]);
                        }

                        if (GameObject.Find("UIRoot/Canvas/UIBackGround/HomeSystemWindow") == null)
                        {
                            UIManager.Instance.StackState();
                            //HomeSystemWindow.Instance.MakeHomeMap(ids[0], ids[1]);
                            HomeSystemWindow.Instance.allHomeBulding[ids[2]].name = string.Format("HomeMapPlace,{0},{1},{2}", ids[0], ids[1], ids[2]);
                            UIState.HomeSystem.Back();

                            //if (UIManager.Instance.curState == UIState.HomeSystem)
                            //{
                            //    UIState.HomeSystem.Back();
                            //}
                            //else
                            //{
                            //    UIManager.Instance.Back();
                            //}
                        }
                        else
                        {
                            //HomeSystemWindow.Instance.MakeHomeMap(ids[0], ids[1]);
                            HomeSystemWindow.Instance.allHomeBulding[ids[2]].name = string.Format("HomeMapPlace,{0},{1},{2}", ids[0], ids[1], ids[2]);
                        }

                        //HomeSystemWindow.Instance.gameObject.SetActive(false);
                        HomeSystem.instance.homeMapPartId = ids[0];
                        HomeSystem.instance.homeMapPlaceId = ids[1];

                        //打开按钮所对应的建筑
                        BuildingWindow.instance.ShowBuildingWindow(ids[0], ids[1], ids[2]);
                    }
                    else
                    {
                        QuickLogger.Log(LogLevel.Warning, "指定建筑不存在 PartId:{0} PlaceId:{1} BuildingIndex:{2}", ids[0], ids[1], ids[2]);

                        //System.Reflection.BindingFlags myBindingFlags = System.Reflection.BindingFlags.a | System.Reflection.BindingFlags.Instance;
                        //HarmonyLib.Patches patch = HarmonyLib.Harmony.GetPatchInfo(typeof(BuildingWindow).GetMethod("ShowBuildingWindow"));

                    }
                }
                //奇遇
                else if (nameList[0] == "PlaceStory")
                {
                    if (DateFile.instance.HaveStory(ids[0], ids[1]) && DateFile.instance.worldMapState[ids[0]][ids[1]][0] == ids[2])
                    {
                        //调试信息
                        if (Main.Setting.debugMode.Value)
                        {
                            QuickLogger.Log(LogLevel.Info, "打开进入奇遇窗口 奇遇Id:{0} 名称{1}", ids[2], DateFile.instance.baseStoryDate[ids[2]][0]);
                        }

                        //打开准备进入奇遇窗口
                        WorldMapSystem.instance.IconOpenToStory(ids[0], ids[1]);
                        //不使用屏幕正下的“选择地点界面”上的“进入奇遇按钮”，因为会与盗墓笔记MOD冲突
                        //WorldMapSystem.instance.OpenToStory();
                    }
                    else
                    {
                        QuickLogger.Log(LogLevel.Warning, "指定奇遇不存在 PartId:{0} PlaceId:{1} Story:{2}", ids[0], ids[1], ids[2]);
                    }
                }
            }
            else
            {
                QuickLogger.Log(LogLevel.Warning, "地点奇遇/建筑按钮：参数数量不符 name:{0}", button.name);
            }
        }
    }
#endif
}

