using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityUIKit.Core;
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using GameData;

#if true
namespace BetterFix
{
    /// <summary>
    /// 地点奇遇/建筑列表UI
    /// </summary>
    public partial class PlaceHomeBuildingUI
    {
        /// <summary></summary>
        public static bool Exists = false;
        /// <summary>本类的实例</summary>
        public static PlaceHomeBuildingUI Instance { get; private set; }

        /// <summary>Canvas容器</summary>
        public static Container.CanvasContainer _UICanvas = null;
        /// <summary>UI实际Container（整体UI）</summary>
        public static Container _ui_PlaceHomeBuildingContainer = null;
        /// <summary>地点奇遇/建筑列表窗口（不含开关组UI）</summary>
        public static TaiwuWindows _ui_PlaceHomeBuildingWindows = null;
        /// <summary>切换显示类型开关组</summary>
        public ToggleGroup buildingTypeToggleGroup;
        public UnityEngine.UI.ToggleGroup buildingTypeUnityToggleGroup;
        /// <summary>奇遇/建筑列表无限滑槽</summary>
        public InfiniteScrollView buildingInfiniteScrollView;
        /// <summary>奇遇/建筑滚动显示区域</summary>
        public BaseScroll buildingView;
        /// <summary>“奇遇”选项开关</summary>
        public TaiwuToggle _PlaceStoryToggle;
        public UnityEngine.UI.Toggle _PlaceStoryUnityToggle;
        /// <summary>“借用”选项开关</summary>
        public TaiwuToggle _OtherGangUseableToggle;
        public UnityEngine.UI.Toggle _OtherGangUseableUnityToggle;
        /// <summary>“自用”选项开关</summary>
        public TaiwuToggle _TaiwuUseableToggle;
        public UnityEngine.UI.Toggle _TaiwuUseableUnityToggle;
        /// <summary>“经营”选项开关</summary>
        public TaiwuToggle _TaiwuWorkPlaceToggle;
        public UnityEngine.UI.Toggle _TaiwuWorkPlaceUnityToggle;
        /// <summary>模板：单项奇遇/建筑图标</summary>
        public BaseFrame _TemplateHomePlace;

        /// <summary>显示的地区ID</summary>
        internal static int _partId = 0;
        /// <summary>显示的地格ID</summary>
        internal static int _placeId = 0;
        /// <summary>地点奇遇ID</summary>
        public int _PlaceStoryId = 0;
        /// <summary>“借用”建筑列表【其他势力：允许太吾使用的建筑】</summary>
        public List<int> _otherGangUseableBuildingList = new List<int>();
        /// <summary>“自用”建筑列表【太吾治下：特定可使用的建筑（修炼、制造、其他特殊功能）】</summary>
        public List<int> _taiwuUseableBuildingList = new List<int>();
        /// <summary>“经营”建筑列表【太吾治下：村民可工作地点建筑（采集、经营）】</summary>
        public List<int> _taiwuWorkPlaceBuildingList = new List<int>();
        /// <summary>当前显示的建筑列表</summary>
        public List<int> _showBuildingList = new List<int>();
        /// <summary>当前显示的建筑类型（9奇遇，0非太吾村可使用，1太吾村可使用, 2太吾村经营）</summary>
        public int _showBuildingType = -1;
        /// <summary>地点奇遇/建筑UI的无限滚动槽的固定图标数量（3个实际显示，1个用来缓冲）</summary>
        public const int IconFixCount = 4;
        /// <summary>地点奇遇/建筑UI的无限滚动槽单个图标的大小</summary>
        public static readonly Vector2 IconCellSize = new Vector2(160f, 160f);
        /// <summary>C_D10001</summary>
        public static Color32 TextColorDark = new Color32(75, 75, 75, 255);
        /// <summary>C_D20001</summary>
        public static Color32 TextColorLight = new Color32(225, 205, 170, 255);

        #region 弃用
        /// <summary>虽然太吾能使用，但所属权意外是2（别家不可使用）的建筑ID列表</summary>
        //public static readonly List<int> SpecialSelfTaiwuBuildings = new List<int>
        //{
        //    1001,   //太吾村
        //    40001,  //竹庐
        //    40002,  //竹庐
        //    40003   //竹庐
        //};
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PlaceHomeBuildingUI()
        {
            PlaceHomeBuildingUI.Instance = this;
            CreateUI();
            Exists = false;
        }

        /// <summary>
        /// 绑定界面UI至正确位置，激活并刷新
        /// </summary>
        /// <param name="isUnknowState">是否因用户开关MOD功能而激活（若为true 则会更新地点建筑列表）</param>
        public void AttachUI(bool isUnknowState = false)
        {
            if (_UICanvas != null)
            {
                //GameObject canvas = GameObject.Find("UIRoot/Canvas/UIPopup/BetterFix.Canvas");
                if (_UICanvas.RectTransform.parent == null || _UICanvas.RectTransform.parent.name != "UIPopup")
                {
                    _UICanvas.SetParent(GameObject.Find("UIRoot/Canvas/UIPopup"), false);
                    _UICanvas.RectTransform.sizeDelta = Vector2.zero;
                    _UICanvas.RectTransform.anchorMin = Vector2.zero;
                    _UICanvas.RectTransform.anchorMax = Vector2.one;
                    _UICanvas.RectTransform.anchoredPosition = Vector2.zero;
                    _UICanvas.GameObject.SetActive(true);
                }

                if (isUnknowState)
                {
                    this.UpdatePlaceBuilding();
                }
                #region 弃用
                //else
                //{
                //    int partId = DateFile.instance.mianPartId;
                //    int placeId = DateFile.instance.mianPlaceId;
                //    //PlaceHomeBuildingUI.SetPlace(partId, placeId);
                //    //this.SetUIActive(true, false);
                //}
                #endregion
            }
            else
            {
                QuickLogger.Log(LogLevel.Info, "地点建筑UI还未创建");
            }
        }

        /// <summary>
        /// 解绑界面UI、隐藏
        /// </summary>
        public static void DetachUI()
        {
            if (_UICanvas != null)
            {
                _UICanvas.RectTransform.SetParent(null);
                _UICanvas.GameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 创建界面UI
        /// </summary>
        public void CreateUI()
        {
            //若UI已创建则先销毁并清空旧UI
            if (_UICanvas != null)
            {
                _ui_PlaceHomeBuildingContainer = null;
                _ui_PlaceHomeBuildingWindows = null;
                GameObject.Destroy(_UICanvas.GameObject);
                _UICanvas = null;
            }

            //若还未创建
            if (_UICanvas == null)
            {
                string lastModName = Main.GUID.Split('.').Last();

                #region !----- 创建滚动显示界面 -----!
                //滚动显示界面
                this.buildingView = new BaseScroll
                {
                    Name = lastModName + ".BuildingView",
                    Element =
                    {
                        PreferredSize = { 220, 490 },
                    },
                    Group =
                    {
                        Direction = Direction.Vertical,
                        Spacing = 10,
                    },
                };
                //移除Image组件
                UnityEngine.Object.DestroyImmediate(this.buildingView.GameObject.GetComponent<UnityEngine.UI.Image>());
                //移除ContentSizeFitter组件
                UnityEngine.Object.DestroyImmediate(this.buildingView.ScrollRect.content.gameObject.GetComponent<UnityEngine.UI.ContentSizeFitter>());
                //移除BoxGroup组件
                UnityEngine.Object.DestroyImmediate(this.buildingView.ScrollRect.content.gameObject.GetComponent<UnityUIKit.Components.BoxGroup>());
                //移除LayoutGroup组件
                UnityEngine.Object.DestroyImmediate(this.buildingView.ScrollRect.content.gameObject.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>());
                UnityEngine.Object.DestroyImmediate(this.buildingView.ScrollRect.content.gameObject.GetComponent<UnityEngine.UI.VerticalLayoutGroup>());
                //加入GridLayoutGroup组件
                this.buildingView.ScrollRect.content.gameObject.AddComponent<UnityEngine.UI.GridLayoutGroup>();
                UnityEngine.UI.GridLayoutGroup buildingViewGridLayoutGroup = this.buildingView.ScrollRect.content.gameObject.GetComponent<UnityEngine.UI.GridLayoutGroup>();
                buildingViewGridLayoutGroup.cellSize = IconCellSize;
                buildingViewGridLayoutGroup.padding = new RectOffset(7, 7, 0, 0);
                buildingViewGridLayoutGroup.spacing = new Vector2(5, 5);

                buildingView.GameObject.AddComponent<InfiniteScrollView>();
                this.buildingInfiniteScrollView = buildingView.GameObject.GetComponent<InfiniteScrollView>();
                this.buildingInfiniteScrollView.fixedCount = IconFixCount;
                this.buildingInfiniteScrollView.scrollType = ScrollType.Vertical;
                this.buildingInfiniteScrollView.onItemRender = new Action<int, RectTransform>(this.OnRenderBuilding);
                #endregion

                #region !----- 创建选项开关组UI -----!
                this._PlaceStoryToggle = new TaiwuToggle()
                {
                    Name = lastModName + ".PlaceStory",
                    Text = "奇遇",
                    //Text = "<color=#E1CDAAFF>奇遇</color>",
                    TipTitle = "说明",
                    TipContant = "该地点的奇遇",
                    Element =
                    {
                        PreferredSize = { 0, 30 },
                    },
                    onValueChanged = (bool value, Toggle b) =>
                    {
                        if (value && this._showBuildingType != 9)
                        {
                            this._showBuildingType = 9;
                            this.UpdateBuildingScroll(true);
                        }
                    },
                };

                this._OtherGangUseableToggle = new TaiwuToggle()
                {
                    Name = lastModName + ".OtherGangUseable",
                    Text = "借用",
                    //Text = "<color=#E1CDAAFF>借用</color>",
                    TipTitle = "说明",
                    TipContant = "该地点属于“其他势力”的“可使用”建筑列表",
                    Element =
                    {
                        PreferredSize = { 0, 30 },
                    },
                    onValueChanged = (bool value, Toggle b) =>
                    {
                        if (value && this._showBuildingType != 0)
                        {
                            this._showBuildingType = 0;
                            this.UpdateBuildingScroll(true);
                        }
                    },
                };

                this._TaiwuUseableToggle = new TaiwuToggle()
                {
                    Name = lastModName + ".TaiwuUseable",
                    Text = "自用",
                    //Text = "<color=#E1CDAAFF>自用</color>",
                    TipTitle = "说明",
                    TipContant = "该地点属于太吾治下的“可使用”建筑列表",
                    Element =
                    {
                        PreferredSize = { 0, 30 },
                    },
                    onValueChanged = (bool value, Toggle b) =>
                    {
                        if (value && this._showBuildingType != 1)
                        {
                            this._showBuildingType = 1;
                            this.UpdateBuildingScroll(true);
                        }
                    },
                };

                this._TaiwuWorkPlaceToggle = new TaiwuToggle()
                {
                    Name = lastModName + ".TaiwuWorkPlace",
                    Text = "经营",
                    //Text = "<color=#E1CDAAFF>经营</color>",
                    TipTitle = "说明",
                    TipContant = "该地点属于太吾治下的“可经营”建筑列表",
                    Element =
                    {
                        PreferredSize = { 0, 30 },
                    },
                    onValueChanged = (bool value, Toggle b) =>
                    {
                        if (value && this._showBuildingType != 2)
                        {
                            this._showBuildingType = 2;
                            this.UpdateBuildingScroll(true);
                        }
                    },
                };

                this.buildingTypeToggleGroup = new ToggleGroup
                {
                    Name = lastModName + ".ToggleGroup",
                    Element =
                    {
                        PreferredSize = { 220, 40 },
                    },
                    Group =
                    {
                        Spacing = 0,
                        Direction = Direction.Horizontal,
                    },
                    Children =
                    {
                        this._PlaceStoryToggle,
                        this._OtherGangUseableToggle,
                        this._TaiwuUseableToggle,
                        this._TaiwuWorkPlaceToggle,
                    }
                };

                //UnityEngine.UI.Toggle
                this._PlaceStoryUnityToggle = this._PlaceStoryToggle.Get<UnityEngine.UI.Toggle>();
                this._OtherGangUseableUnityToggle = this._OtherGangUseableToggle.Get<UnityEngine.UI.Toggle>();
                this._TaiwuUseableUnityToggle = this._TaiwuUseableToggle.Get<UnityEngine.UI.Toggle>();
                this._TaiwuWorkPlaceUnityToggle = this._TaiwuWorkPlaceToggle.Get<UnityEngine.UI.Toggle>();

                //UnityEngine.UI.ToggleGroup
                this.buildingTypeUnityToggleGroup = this.buildingTypeToggleGroup.Get<UnityEngine.UI.ToggleGroup>();

                //为UnityEngine.UI.Toggle设定UnityEngine.UI.ToggleGroup
                this._PlaceStoryUnityToggle.group = this.buildingTypeUnityToggleGroup;
                this._OtherGangUseableUnityToggle.group = this.buildingTypeUnityToggleGroup;
                this._TaiwuUseableUnityToggle.group = this.buildingTypeUnityToggleGroup;
                this._TaiwuWorkPlaceUnityToggle.group = this.buildingTypeUnityToggleGroup;

                //将单独的开关UI设为开关组UI的子级
                this._PlaceStoryToggle.SetParent(this.buildingTypeToggleGroup, false);
                this._OtherGangUseableToggle.SetParent(this.buildingTypeToggleGroup, false);
                this._TaiwuUseableToggle.SetParent(this.buildingTypeToggleGroup, false);
                this._TaiwuWorkPlaceToggle.SetParent(this.buildingTypeToggleGroup, false);
                #endregion

                #region !----- 创建地点奇遇/建筑界面窗口 -----!
                //地点奇遇/建筑界面窗口
                _ui_PlaceHomeBuildingWindows = new TaiwuWindows
                {
                    Name = lastModName + ".Windows",
                    Title = "地点奇遇/建筑",
                    Direction = Direction.Vertical,
                    Spacing = 10f,
                    DefaultActive = Main.Setting.UiAddPlaceBuildUI.Value,
                    Group =
                    {
                        ChildrenAlignment = TextAnchor.UpperCenter,
                    },
                    Element =
                    {
                        PreferredSize = { 220f, 580f },
                    },
                    Children =
                    {
                        this.buildingView,
                    },
                };
                this.buildingView.SetParent(_ui_PlaceHomeBuildingWindows, false);

                _ui_PlaceHomeBuildingWindows.CloseButton.SetActive(false);
                #endregion

                #region !----- 创建实际UI容器 (Container) -----!
                //实际UI容器
                _ui_PlaceHomeBuildingContainer = new Container()
                {
                    Name = lastModName + ".Container",
                    DefaultActive = false,
                    Group =
                    {
                        Direction = Direction.Vertical,
                        Padding = { 0, 10, 0, 0 },
                    },
                    Element =
                    {
                        PreferredSize = { 220, 620 },
                    },
                    Children =
                    {
                        this.buildingTypeToggleGroup,
                        _ui_PlaceHomeBuildingWindows,
                    },
                };
                this.buildingTypeToggleGroup.SetParent(_ui_PlaceHomeBuildingContainer, false);
                _ui_PlaceHomeBuildingWindows.SetParent(_ui_PlaceHomeBuildingContainer, false);

                this.buildingTypeToggleGroup.RectTransform.anchoredPosition = new Vector2(-20, -5);
                #endregion

                #region !----- 创建界面UI的Canvas (CanvasContainer) -----!
                //界面UI的Canvas
                _UICanvas = new Container.CanvasContainer
                {
                    Name = lastModName + ".Canvas",
                    DefaultActive = false,
                    Children =
                    {
                        _ui_PlaceHomeBuildingContainer,
                    },
                };
                UnityEngine.Object.DestroyImmediate(_UICanvas.GameObject.GetComponent<UnityUIKit.Components.BoxElement>());
                UnityEngine.Object.DestroyImmediate(_UICanvas.GameObject.GetComponent<UnityEngine.UI.LayoutElement>());
                UnityEngine.Object.DestroyImmediate(_UICanvas.GameObject.GetComponent<UnityUIKit.Components.BoxGroup>());
                UnityEngine.Object.DestroyImmediate(_UICanvas.GameObject.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>());
                UnityEngine.Object.DestroyImmediate(_UICanvas.GameObject.GetComponent<UnityEngine.UI.VerticalLayoutGroup>());
                _ui_PlaceHomeBuildingContainer.RectTransform.anchorMax = Vector2.zero;
                _ui_PlaceHomeBuildingContainer.RectTransform.anchorMin = Vector2.one;
                _UICanvas.RectTransform.sizeDelta = Vector2.zero;

                _UICanvas.GameObject.layer = 5;

                _ui_PlaceHomeBuildingContainer.SetParent(_UICanvas, false);

                _ui_PlaceHomeBuildingContainer.RectTransform.anchorMax = new Vector2(1f, 0.5f);
                _ui_PlaceHomeBuildingContainer.RectTransform.anchorMin = new Vector2(1f, 0.5f);
                _ui_PlaceHomeBuildingContainer.RectTransform.pivot = new Vector2(1f, 0.5f);
                _ui_PlaceHomeBuildingContainer.RectTransform.anchoredPosition = new Vector2(210f, 30f);
                _ui_PlaceHomeBuildingContainer.RectTransform.sizeDelta = new Vector2(220f, 620f);
                #endregion
            }
        }

        /// <summary>
        /// 设定地点建筑所使用的地点
        /// </summary>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        /// <param name="notChangeShowType">是否不重置所显示类型</param>
        public static void SetPlace(int partId, int placeId, bool notChangeShowType = false)
        {
            PlaceHomeBuildingUI._partId = partId;
            PlaceHomeBuildingUI._placeId = placeId;

            //若实例已被创建，则更新地点建筑列表
            if (Instance != null)
            {
                Instance.UpdatePlaceBuilding(notChangeShowType);
            }
        }

        /// <summary>
        /// 更新地点建筑列表（若功能已开启并决定是否显示界面
        /// </summary>
        /// <param name="notChangeShowType">是否不重置所显示类型</param>
        public void UpdatePlaceBuilding(bool notChangeShowType = false)
        {
            //更新该地点的奇遇ID
            this._PlaceStoryId = DateFile.instance.HaveStory(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId) ? DateFile.instance.worldMapState[PlaceHomeBuildingUI._partId][PlaceHomeBuildingUI._placeId][0] : 0;
            //更新该地点的“借用”建筑列表
            this._otherGangUseableBuildingList = PlaceHaveBuilding(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, BuildingType.OnlyOtherGangUseableBuildingIndex);
            //更新该地点的“自用”建筑列表
            this._taiwuUseableBuildingList = PlaceHaveBuilding(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, BuildingType.OnlyTaiwuUseableBuildingIndex, true);
            //更新该地点的“经营”建筑列表
            this._taiwuWorkPlaceBuildingList = PlaceHaveBuilding(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, BuildingType.OnlyTaiwuWorkPlaceBuildingIndex);

            //!----- 建筑列表重新排序 -----!
            this._otherGangUseableBuildingList.Sort(new Comparison<int>(CompareBuildingIndex));
            this._taiwuUseableBuildingList.Sort(new Comparison<int>(CompareBuildingIndex));
            this._taiwuWorkPlaceBuildingList.Sort(new Comparison<int>(CompareBuildingIndex));

            //QuickLogger.Log(LogLevel.Info, "获取地点建筑 奇遇Id:{0} 借用数量:{1} 自用数量:{2} 经营数量:{3}", this._PlaceStoryId, this._otherGangUseableBuildingList.Count, this._taiwuUseableBuildingList.Count, this._taiwuWorkPlaceBuildingList);

            //若UI已创建 且 处于激活状态（处于AttachUI中）
            if (_UICanvas != null && _UICanvas.GameObject.activeSelf)
            {
                //!----- 可用状态判定 -----!
                bool isPlaceStoryAvailable = this._PlaceStoryId != 0;
                bool isOtherGangUseableAvailable = this._otherGangUseableBuildingList.Count > 0;
                bool isTaiwuUseableAvailable = this._taiwuUseableBuildingList.Count > 0;
                bool isTaiwuWorkPlaceAvailable = this._taiwuWorkPlaceBuildingList.Count > 0;

                //若地点有奇遇 或 有符合要求的建筑
                if (isPlaceStoryAvailable || isOtherGangUseableAvailable || isTaiwuUseableAvailable || isTaiwuWorkPlaceAvailable)
                {
                    //!----- 根据可用状态更新开关的可互动性与开关上的文本颜色 -----!

                    if (this._PlaceStoryUnityToggle.interactable != isPlaceStoryAvailable)
                    {
                        this._PlaceStoryUnityToggle.interactable = isPlaceStoryAvailable;
                        //this._PlaceStoryToggle.Text = isPlaceStoryAvailable ? "<color=#E1CDAAFF>奇遇</color>" : "<color=#4B4B4BFF>奇遇</color>";
                        this._PlaceStoryToggle.FontColor = isPlaceStoryAvailable ? TextColorLight : TextColorDark;
                        this._PlaceStoryToggle.Label.TextControl.Text.color = isPlaceStoryAvailable ? TextColorLight : TextColorDark;
                    }

                    if (this._OtherGangUseableUnityToggle.interactable != isOtherGangUseableAvailable)
                    {
                        this._OtherGangUseableUnityToggle.interactable = isOtherGangUseableAvailable;
                        //this._OtherGangUseableToggle.Text = isOtherGangUseableAvailable ? "<color=#E1CDAAFF>借用</color>" : "<color=#4B4B4BFF>借用</color>";
                        this._OtherGangUseableToggle.FontColor = isOtherGangUseableAvailable ? TextColorLight : TextColorDark;
                        this._OtherGangUseableToggle.Label.TextControl.Text.color = isOtherGangUseableAvailable ? TextColorLight : TextColorDark;
                    }

                    if (this._TaiwuUseableUnityToggle.interactable != isTaiwuUseableAvailable)
                    {
                        this._TaiwuUseableUnityToggle.interactable = isTaiwuUseableAvailable;
                        //this._TaiwuUseableToggle.Text = isTaiwuUseableAvailable ? "<color=#E1CDAAFF>自用</color>" : "<color=#4B4B4BFF>自用</color>";
                        this._TaiwuUseableToggle.FontColor = isTaiwuUseableAvailable ? TextColorLight : TextColorDark;
                        this._TaiwuUseableToggle.Label.TextControl.Text.color = isTaiwuUseableAvailable ? TextColorLight : TextColorDark;
                    }

                    if (this._TaiwuWorkPlaceUnityToggle.interactable != isTaiwuWorkPlaceAvailable)
                    {
                        this._TaiwuWorkPlaceUnityToggle.interactable = isTaiwuWorkPlaceAvailable;
                        //this._TaiwuWorkPlaceToggle.Text = isTaiwuWorkPlaceAvailable ? "<color=#E1CDAAFF>经营</color>" : "<color=#4B4B4BFF>经营</color>";
                        this._TaiwuWorkPlaceToggle.FontColor = isTaiwuWorkPlaceAvailable ? TextColorLight : TextColorDark;
                        this._TaiwuWorkPlaceToggle.Label.TextControl.Text.color = isTaiwuWorkPlaceAvailable ? TextColorLight : TextColorDark;
                    }

                    #region 判定显示类型并刷新对应的列表内容
                    //若不重置所显示类型
                    if (notChangeShowType)
                    {
                        //当前显示类型是否依旧有效
                        bool vaild = false;
                        //根据当前显示类型判定是否依旧有效
                        switch (this._showBuildingType)
                        {
                            case 9:
                                vaild = isPlaceStoryAvailable;
                                break;
                            case 0:
                                vaild = isOtherGangUseableAvailable;
                                break;
                            case 1:
                                vaild = isTaiwuUseableAvailable;
                                break;
                            case 2:
                                vaild = isTaiwuWorkPlaceAvailable;
                                break;
                        }

                        //若依旧有效，则仅刷新（不然继续往下判定）
                        if (vaild)
                        {
                            this.UpdateBuildingScroll(false);
                            PlaceHomeBuildingUI.Instance.ShowWindow();
                            return;
                        }
                    }

                    //!----- 以“奇遇”、“借用”、“自用”、“精英”的顺序选取靠前且有效的类型显示 -----!
                    if (isPlaceStoryAvailable)
                    {
                        if (this._showBuildingType == 9)
                        {
                            this.UpdateBuildingScroll(false);
                        }
                        else
                        {
                            this.buildingTypeUnityToggleGroup.SetAllTogglesOff();
                            _PlaceStoryToggle.isOn = true;
                        }
                    }
                    else if (isOtherGangUseableAvailable)
                    {
                        if (this._showBuildingType == 0)
                        {
                            this.UpdateBuildingScroll(false);
                        }
                        else
                        {
                            this.buildingTypeUnityToggleGroup.SetAllTogglesOff();
                            _OtherGangUseableToggle.isOn = true;
                        }
                    }
                    else if (isTaiwuUseableAvailable)
                    {
                        if (this._showBuildingType == 1)
                        {
                            this.UpdateBuildingScroll(false);
                        }
                        else
                        {
                            this.buildingTypeUnityToggleGroup.SetAllTogglesOff();
                            this._TaiwuUseableToggle.isOn = true;
                        }
                    }
                    else if (isTaiwuWorkPlaceAvailable)
                    {
                        if (this._showBuildingType == 2)
                        {
                            this.UpdateBuildingScroll(false);
                        }
                        else
                        {
                            this.buildingTypeUnityToggleGroup.SetAllTogglesOff();
                            this._TaiwuWorkPlaceToggle.isOn = true;
                        }
                    }
                    #endregion

                    PlaceHomeBuildingUI.Instance.ShowWindow();
                }
                //地点没有奇遇、也没有符合要求的建筑
                else
                {
                    #region !----- 更新开关的可互动性与开关上的文本颜色 -----!
                    
                    if (this._PlaceStoryUnityToggle.interactable != false)
                    {
                        this._PlaceStoryUnityToggle.interactable = false;
                        //this._PlaceStoryToggle.Text = "<color=#4B4B4BFF>奇遇</color>";
                        this._PlaceStoryToggle.FontColor = TextColorDark;
                        this._PlaceStoryToggle.Label.TextControl.Text.color = TextColorDark;
                    }

                    if (this._OtherGangUseableUnityToggle.interactable != false)
                    {
                        this._OtherGangUseableUnityToggle.interactable = false;
                        //this._OtherGangUseableToggle.Text = "<color=#4B4B4BFF>借用</color>";
                        this._OtherGangUseableToggle.FontColor = TextColorDark;
                        this._OtherGangUseableToggle.Label.TextControl.Text.color = TextColorDark;
                    }

                    if (this._TaiwuUseableUnityToggle.interactable != false)
                    {
                        this._TaiwuUseableUnityToggle.interactable = false;
                        //this._TaiwuUseableToggle.Text = "<color=#4B4B4BFF>自用</color>";
                        this._TaiwuUseableToggle.FontColor = TextColorDark;
                        this._TaiwuUseableToggle.Label.TextControl.Text.color = TextColorDark;
                    }

                    if (this._TaiwuWorkPlaceUnityToggle.interactable != false)
                    {
                        this._TaiwuWorkPlaceUnityToggle.interactable = false;
                        //this._TaiwuWorkPlaceToggle.Text = "<color=#4B4B4BFF>经营</color>";
                        this._TaiwuWorkPlaceToggle.FontColor = TextColorDark;
                        this._TaiwuWorkPlaceToggle.Label.TextControl.Text.color = TextColorDark;
                    }
                    #endregion

                    this.buildingTypeUnityToggleGroup.SetAllTogglesOff();
                    this._showBuildingType = -1;
                    PlaceHomeBuildingUI.Instance.HideWindow();
                }
            }
        }

        /// <summary>
        /// 获取指定地格的建筑Index列表
        /// </summary>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        /// <param name="buildingType">建筑类型（总共4种）</param>
        /// <param name="isOnlyHighestLevel">是否仅返回同类建筑中最高等级的建筑</param>
        /// <returns>指定地格符合要求的建筑Index列表</returns>
        public static List<int> PlaceHaveBuilding(int partId, int placeId, BuildingType buildingType, bool isOnlyHighestLevel = false)
        {
            //若该地点没有产业地图，则返回空列表
            if (!DoesPlaceHaveHomeBuilding(partId, placeId))
            {
                return new List<int>();
            }

            //获取该地格的所有产业建筑数据
            Dictionary<int, int[]> placeBuildingsDate = DateFile.instance.homeBuildingsDate[partId][placeId];
            //此地是否处于太吾治下/允许建设（原游戏中只有一般太吾村和竹庐为1，）
            bool isPlaceManagedByTaiwu = DateFile.instance.baseHomeDate[partId][placeId] != 0;

            //每种homePlaceId的建筑等级最高记录
            Dictionary<int, List<int>> highestLevelRecord = new Dictionary<int, List<int>>();
            //返回结果列表
            List<int> result = new List<int>();

            //遍历指定地点的建筑
            foreach (int buildingIndex in placeBuildingsDate.Keys)
            {
                //遍历到的条目所对应的建筑类型ID
                int homePlaceId = placeBuildingsDate[buildingIndex][0];
                //遍历到的条目所对应的建筑等级
                int buildingLevel = placeBuildingsDate[buildingIndex][1];
                //遍历到的条目所对应的建筑所属类型（不准确描述：1所属太吾村 2所属为其他势力不可使用 3所属为其他势力可以借用）
                int buildingBelongType = placeBuildingsDate[buildingIndex][2];

                //若该建筑的ID为0（空地），则此项跳过
                if (homePlaceId == 0)
                {
                    continue;
                }

                //判断
                switch (buildingType)
                {
                    //除了空地以外的所有建筑
                    case BuildingType.AllBuildingIndex:
                        if (isOnlyHighestLevel)
                        {
                            if (highestLevelRecord.ContainsKey(homePlaceId))
                            {
                                if (buildingLevel > highestLevelRecord[homePlaceId][1])
                                {
                                    result.Remove(highestLevelRecord[homePlaceId][0]);
                                    result.Add(buildingIndex);
                                    highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                }
                            }
                            else
                            {
                                result.Add(buildingIndex);
                                highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                            }
                        }
                        else
                        {
                            result.Add(buildingIndex);
                        }
                        break;
                    //“借用”建筑
                    case BuildingType.OnlyOtherGangUseableBuildingIndex:
                        //若该建筑的所属值为3（其他势力建筑：但允许太吾使用）
                        if (buildingBelongType == 3)
                        {
                            if (isOnlyHighestLevel)
                            {
                                if (highestLevelRecord.ContainsKey(homePlaceId))
                                {
                                    if (buildingLevel > highestLevelRecord[homePlaceId][1])
                                    {
                                        result.Remove(highestLevelRecord[homePlaceId][0]);
                                        result.Add(buildingIndex);
                                        highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                    }
                                }
                                else
                                {
                                    result.Add(buildingIndex);
                                    highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                }
                            }
                            else
                            {
                                result.Add(buildingIndex);
                            }
                        }
                        break;
                    //“自用”建筑
                    case BuildingType.OnlyTaiwuUseableBuildingIndex:
                        ////若为可使用建筑 且 （可以修习 或 可以制造 或 可以促织展示 或 可以灌顶 或 可以存取物品 或 轮回台功能）
                        //if (buildingBelongType == 1 &&
                        //若为此地为太吾治下 且 建筑所属类型不为“借用” 且 （可以修习 或 可以制造 或 可以促织展示 或 可以灌顶 或 可以存取物品 或 轮回台功能）
                        if (isPlaceManagedByTaiwu && buildingBelongType != 3 &&
                            //可以修习
                            (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][65]) > 0
                            //可以制造
                            || int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][72]) > 0
                            //可以促织展示
                            || int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][44]) == 1
                            //可以灌顶（祠堂）
                            || int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][64]) > 0
                            //可以存取物品
                            || int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][63]) != 0
                            //轮回台功能
                            || int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][83]) == 1
                            )
                           )
                        {
                            if (isOnlyHighestLevel)
                            {
                                if (highestLevelRecord.ContainsKey(homePlaceId))
                                {
                                    if (buildingLevel > highestLevelRecord[homePlaceId][1])
                                    {
                                        result.Remove(highestLevelRecord[homePlaceId][0]);
                                        result.Add(buildingIndex);
                                        highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                    }
                                }
                                else
                                {
                                    result.Add(buildingIndex);
                                    highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                }
                            }
                            else
                            {
                                result.Add(buildingIndex);
                            }
                        }

                        #region 弃用
                        ////特定建筑额外加入（太吾村、竹庐。其的所属类型意外的为“2所属为其他势力不可使用”）
                        //if (buildingBelongType == 2  && SpecialSelfTaiwuBuildings.Contains(homePlaceId))
                        //{
                        //    result.Add(buildingIndex);
                        //}
                        #endregion

                        break;
                    //“经营”建筑
                    case BuildingType.OnlyTaiwuWorkPlaceBuildingIndex:
                        //若建筑可以分配村民
                        //if (buildingBelongType == 1 && int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][3]) == 1)
                        //若为此地为太吾治下 且 建筑所属类型不为“借用” 且 建筑可以分配村民
                        if (isPlaceManagedByTaiwu && buildingBelongType != 3 && int.Parse(DateFile.instance.basehomePlaceDate[homePlaceId][3]) == 1)
                        {
                            if (isOnlyHighestLevel)
                            {
                                if (highestLevelRecord.ContainsKey(homePlaceId))
                                {
                                    if (buildingLevel > highestLevelRecord[homePlaceId][1])
                                    {
                                        result.Remove(highestLevelRecord[homePlaceId][0]);
                                        result.Add(buildingIndex);
                                        highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                    }
                                }
                                else
                                {
                                    result.Add(buildingIndex);
                                    highestLevelRecord[homePlaceId] = new List<int> { buildingIndex, buildingLevel };
                                }
                            }
                            else
                            {
                                result.Add(buildingIndex);
                            }
                        }
                        break;
                }
            }

            //返回结果
            return result;
        }

        /// <summary>
        /// 更新地点建筑滑动框
        /// </summary>
        /// <param name="scrollToTop">是否滑动至最上层（本设定暂无实际效果）</param>
        private void UpdateBuildingScroll(bool scrollToTop = false)
        {
            //显示奇遇
            if (_showBuildingType == 9)
            {
                this._showBuildingList = new List<int> { this._PlaceStoryId };
                this.buildingInfiniteScrollView.SetTotalCount(1);
            }
            //显示建筑
            else
            {
                this._showBuildingList = (this._showBuildingType == 0) ? this._otherGangUseableBuildingList : ((this._showBuildingType == 1) ? this._taiwuUseableBuildingList : ((this._showBuildingType == 2) ? this._taiwuWorkPlaceBuildingList : new List<int>()));
                //QuickLogger.Log(LogLevel.Info, "刷新地点建筑列表前 显示建筑数量:{0} 类型:{1}", this._showBuildingList.Count, this._showBuildingType);
                this.buildingInfiniteScrollView.SetTotalCount(this._showBuildingList.Count);
            }
        }

        /// <summary>
        /// 对无限滚动区域的单项奇遇/建筑进行重新渲染
        /// </summary>
        /// <param name="index"></param>
        /// <param name="refer"></param>
        private void OnRenderBuilding(int index, RectTransform trans)
        {
            //奇遇
            if (this._showBuildingType == 9)
            {
                int storyId = this._showBuildingList[index];

                //QuickLogger.Log(LogLevel.Info, "渲染地点奇遇图标 奇遇ID:{0} 地区ID:{1} 地格ID:{2} 记录奇遇ID:{3}", storyId, PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, this._PlaceStoryId);

                trans.name = "placeStory" + index.ToString();
                trans.GetComponent<SetPlaceIcon>().SetStory(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, storyId);
            }
            //建筑
            else
            {
                int buildingIndex = this._showBuildingList[index];

                //QuickLogger.Log(LogLevel.Info, "渲染地点建筑图标 序号:{0}/{1} 地区ID:{2} 地格ID:{3} 建筑序号:{4}", (index + 1), this._showBuildingList.Count, PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, buildingIndex);

                trans.name = "placeBuilding" + index.ToString();
                trans.GetComponent<SetPlaceIcon>().SetBuilding(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, buildingIndex);
            }
        }

        /// <summary>
        /// 设定地点建筑UI的显示状态
        /// </summary>
        /// <param name="isActive">是否显示</param>
        /// <param name="isActive">是否需要更新地点建筑列表（仅在isActive 设为 true 时有意义）</param>
        /// <param name="isActive">是否快速隐藏（仅在isActive 设为 false 时有意义）</param>
        public void SetUIActive(bool isActive, bool needUpdate = true, bool quickHide = false)
        {
            if (isActive)
            {
                if (needUpdate)
                {
                    this.UpdatePlaceBuilding();
                }
                else if (this._PlaceStoryId != 0 || this._otherGangUseableBuildingList.Count > 0 || this._taiwuUseableBuildingList.Count > 0 || this._taiwuWorkPlaceBuildingList.Count > 0)
                {
                    this.ShowWindow();
                }
            }
            else
            {
                this.HideWindow(quickHide);
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private void ShowWindow()
        {
            if (!Exists && UIState.MainWorld == UIManager.Instance.curState)
            {
                Exists = true;
                _ui_PlaceHomeBuildingContainer.RectTransform.DOKill(false);
                _ui_PlaceHomeBuildingContainer.GameObject.SetActive(true);
                _ui_PlaceHomeBuildingContainer.RectTransform.DOAnchorPosX(10f, 0.3f, false).SetEase(Ease.OutBack);
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="quickHide">是否快速隐藏（用来解决“查看人物界面”突兀的隐藏动画）</param>
        private void HideWindow(bool quickHide = false)
        {
            if (Exists)
            {
                Exists = false;
                _ui_PlaceHomeBuildingContainer.RectTransform.DOKill(false);
                if (quickHide)
                {
                    _ui_PlaceHomeBuildingContainer.GameObject.SetActive(false);
                    _ui_PlaceHomeBuildingContainer.RectTransform.DOAnchorPosX(230f, 0.3f, false).SetEase(Ease.Linear);
                }
                else
                {
                    _ui_PlaceHomeBuildingContainer.RectTransform.DOAnchorPosX(230f, 0.3f, false).SetEase(Ease.Linear).OnComplete(delegate
                    {
                        _ui_PlaceHomeBuildingContainer.GameObject.SetActive(false);
                    });
                }
            }
        }

        /// <summary>
        /// 以建筑序号来比较记录地点（_currentPartId, _currentPlaceId）的建筑显示顺位（更重要的更靠前）
        /// </summary>
        /// <param name="buildingIndex1">要比较的记录地点的一号建筑的序号</param>
        /// <param name="buildingIndex2">要比较的记录地点的二号建筑的序号</param>
        /// <returns>比较结果（结果为负则第一项在前，为正则第二项在前）</returns>
        public int CompareBuildingIndex(int buildingIndex1, int buildingIndex2)
        {
            if (!DoesHomeBuildingExsit(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, buildingIndex1) || !DoesHomeBuildingExsit(PlaceHomeBuildingUI._partId, PlaceHomeBuildingUI._placeId, buildingIndex2))
            {
                return 0;
            }

            int homePlaceId1st = DateFile.instance.homeBuildingsDate[_partId][PlaceHomeBuildingUI._placeId][buildingIndex1][0];
            int homePlaceId2nd = DateFile.instance.homeBuildingsDate[_partId][PlaceHomeBuildingUI._placeId][buildingIndex2][0];

            int result = 0;

            //若两者的建筑ID不同
            if (homePlaceId1st != homePlaceId2nd)
            {
                int[] homePlaceIds = { homePlaceId1st, homePlaceId2nd };
                int[] buildingCalcNums = new int[2];

                //分别为两者判断
                for (int i = 0; i < 2; i++)
                {
                    //建筑的修炼类型
                    int buildingStudyType = int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][65]);
                    //建筑的制造类型
                    int buildingProduceType = int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][72]);

                    //可以存放促织罐（如太吾村）
                    if (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][44]) == 1)
                    {
                        buildingCalcNums[i] = 240;
                    }

                    //每年提供威望（太吾村祠堂）1005
                    //if (homePlaceIds[i] == 1005)
                    if (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][64]) > 0)
                    {
                        buildingCalcNums[i] = 260;
                    }

                    //提供资源上限不为0（仓库，-1为其他势力的驿站远程仓库）
                    if (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][63]) != 0)
                    {
                        buildingCalcNums[i] = 300;
                    }

                    //是否具有轮回台特效
                    if (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][83]) == 1)
                    {
                        buildingCalcNums[i] = 70;
                    }

                    //若建筑可以修炼功法
                    if (buildingStudyType == 17)
                    { buildingCalcNums[i] = 280; }

                    //若建筑可以制造物品
                    if (buildingProduceType > 0)
                    {
                        //判断建筑制造类型
                        switch (buildingProduceType)
                        {
                            //食物
                            case 15:
                                buildingCalcNums[i] = 200;
                                break;
                            //药物
                            case 9:
                                buildingCalcNums[i] = 180;
                                break;
                            //毒物
                            case 10:
                                buildingCalcNums[i] = 160;
                                break;
                            //锻造
                            case 7:
                                buildingCalcNums[i] = 140;
                                break;
                            //木工
                            case 8:
                                buildingCalcNums[i] = 120;
                                break;
                            //织锦
                            case 11:
                                buildingCalcNums[i] = 100;
                                break;
                            //制石
                            case 12:
                                buildingCalcNums[i] = 80;
                                break;
                        }

                        //若建筑即可制造、也可修炼
                        if (buildingStudyType > 0)
                        { buildingCalcNums[i] += 10; }
                    }
                    //若建筑不可制造、但可以修炼技艺（排除修炼功法——上方已指定）
                    else if (buildingStudyType > 0 && buildingStudyType != 17)
                    { buildingCalcNums[i] = 40; }

                    //若建筑可以派驻人手
                    if (int.Parse(DateFile.instance.basehomePlaceDate[homePlaceIds[i]][3]) == 1)
                    { buildingCalcNums[i] += 5; }
                }

                //计算判断结果（结果为负则第一项在前，为正则第二项在前）
                //（非0的最小差值为500）
                result = buildingCalcNums[1] - buildingCalcNums[0];
            }

            //若两者的建筑ID相同或者比对结果相同则按照双方的建筑Index修正
            if (homePlaceId1st == homePlaceId2nd || result == 0)
            {
                result += (buildingIndex1 < buildingIndex2) ? -1 : 1;
            }

            //返回结果（结果为负则第一项在前，为正则第二项在前）
            return result;
        }

        /// <summary>
        /// 判断指定地点是否存在产业地图
        /// </summary>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        /// <returns>产业地图是否存在</returns>
        public static bool DoesPlaceHaveHomeBuilding(int partId, int placeId)
        {
            return (DateFile.instance.homeBuildingsDate.ContainsKey(partId) && DateFile.instance.homeBuildingsDate[partId].ContainsKey(placeId));
        }

        /// <summary>
        /// 判断指定建筑是否存在
        /// </summary>
        /// <param name="partId">地区ID</param>
        /// <param name="placeId">地格ID</param>
        /// <param name="buildingIndex">建筑序号</param>
        /// <returns>建筑是否存在</returns>
        public static bool DoesHomeBuildingExsit(int partId, int placeId, int buildingIndex)
        {
            return (DateFile.instance.homeBuildingsDate.ContainsKey(partId) && DateFile.instance.homeBuildingsDate[partId].ContainsKey(placeId) && DateFile.instance.homeBuildingsDate[partId][placeId].ContainsKey(buildingIndex));
        }

        /// <summary>
        /// 4种指定建筑类型
        /// </summary>
        public enum BuildingType
        {
            /// <summary>除了空地以外的所有建筑（buildingIndex）</summary>
            AllBuildingIndex = -1,
            /// <summary>“借用”其他势力：允许太吾使用的建筑</summary>
            OnlyOtherGangUseableBuildingIndex = 0,
            /// <summary>“自用”太吾治下：特定可使用的建筑（修炼、制造、其他特殊功能）</summary>
            OnlyTaiwuUseableBuildingIndex = 1,
            /// <summary>“经营”太吾治下：村民可工作地点建筑（采集、经营）</summary>
            OnlyTaiwuWorkPlaceBuildingIndex = 2,
        }
    }
}
#endif