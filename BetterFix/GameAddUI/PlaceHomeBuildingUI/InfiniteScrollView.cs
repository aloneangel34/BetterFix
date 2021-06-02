using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 无限滑动列表
    /// </summary>
    public class InfiniteScrollView : MonoBehaviour
    {
        private ScrollRect scrollRect;//滑动框组件
        private RectTransform content;//滑动框的Content
        private GridLayoutGroup layout;//布局组件

        [Header("滑动类型")]
        public ScrollType scrollType;
        [Header("固定的Item数量")]
        public int fixedCount;
        //[Header("Item的预制体")]
        //public GameObject itemPrefab;

        private int totalCount;//总的数据数量
        internal List<RectTransform> dataList = new List<RectTransform>();//数据实体列表
        private int headIndex;//头下标
        private int tailIndex;//尾下标
        private Vector2 firstItemAnchoredPos;//第一个Item的锚点坐标
        public Action<int, RectTransform> onItemRender;
        //private bool initSuccess = false;

        #region Init

        /// <summary>
        /// 实例化Item
        /// </summary>
        private void InitItem()
        {
            for (int i = 0; i < Mathf.Min(fixedCount, totalCount); i++)
            {
                //专用设置
                UnityUIKit.GameObjects.Container buildingIcon = PlaceHomeBuildingUI.CreatTemplateHomePlace(i);
                buildingIcon.RectTransform.SetParent(this.content, false);
                dataList.Add(buildingIcon.RectTransform);

                //QuickLogger.Log(LogLevel.Info, "初始化中{0}/{1} 固定数{2} 总数{3}", i + 1, Mathf.Min(fixedCount, totalCount), fixedCount, totalCount);

                SetShow(buildingIcon.RectTransform, i);

                //原本的通用化设置（使用预制体）
                //GameObject tempItem = Instantiate(itemPrefab, content);
                //dataList.Add(tempItem.GetComponent<RectTransform>());
                //SetShow(tempItem.GetComponent<RectTransform>(), i);
            }

        }

        /// <summary>
        /// 设置Content大小
        /// </summary>
        private void SetContentSize()
        {
            content.sizeDelta = new Vector2
                (
                    layout.padding.left + layout.padding.right + totalCount * (layout.cellSize.x + layout.spacing.x) - layout.spacing.x - content.rect.width,
                    layout.padding.top + layout.padding.bottom + totalCount * (layout.cellSize.y + layout.spacing.y) - layout.spacing.y
                    //(totalCount < fixedCount) ? 0 : (layout.padding.top + layout.padding.bottom + totalCount * (layout.cellSize.y + layout.spacing.y) - layout.spacing.y)
                );
        }

        /// <summary>
        /// 设置布局
        /// </summary>
        private void SetLayout()
        {
            //确保移动模式
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            //确保描点
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(0, 1);
            //----原本设定
            layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            layout.startAxis = GridLayoutGroup.Axis.Horizontal;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.constraintCount = 1;
            if (scrollType == ScrollType.Horizontal)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
                layout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            }
            else if (scrollType == ScrollType.Vertical)
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            }
        }

        /// <summary>
        /// 得到第一个数据的锚点位置
        /// </summary>
        private void GetFirstItemAnchoredPos()
        {
            firstItemAnchoredPos = new Vector2
                (
                    layout.padding.left + layout.cellSize.x / 2,
                    -layout.padding.top - layout.cellSize.y / 2
                );
        }

        #endregion

        #region Main

        /// <summary>
        /// 滑动中
        /// </summary>
        private void OnScroll(Vector2 v)
        {
            if (dataList.Count == 0)
            {
                return;
            }

            if (scrollType == ScrollType.Vertical)
            {
                //向上滑
                while (content.anchoredPosition.y >= layout.padding.top + (headIndex + 1) * (layout.cellSize.y + layout.spacing.y)
                && tailIndex < totalCount - 1)
                {
                    //将数据列表中的第一个元素移动到最后一个
                    RectTransform item = dataList[0];
                    dataList.Remove(item);
                    dataList.Add(item);

                    //QuickLogger.Log(LogLevel.Info, "首元素移动到最后 head{0} tail{1} fixedCount{2} totalCount{3}", headIndex, tailIndex, fixedCount, totalCount);

                    //设置位置
                    SetPos(item, tailIndex + 1);
                    //设置显示
                    SetShow(item, tailIndex + 1);

                    headIndex++;
                    tailIndex++;
                }
                //向下滑
                while (content.anchoredPosition.y <= layout.padding.top + (headIndex + 0) * (layout.cellSize.y + layout.spacing.y)
                    && headIndex > 0)
                {
                    //将数据列表中的最后一个元素移动到第一个
                    RectTransform item = dataList.Last();
                    dataList.Remove(item);
                    dataList.Insert(0, item);

                    //QuickLogger.Log(LogLevel.Info, "尾元素移动到最前 head{0} tail{1} fixedCount{2} totalCount{3}", headIndex, tailIndex, fixedCount, totalCount);

                    //设置位置
                    SetPos(item, headIndex - 1);
                    //设置显示
                    SetShow(item, headIndex - 1);

                    headIndex--;
                    tailIndex--;
                }
            }
            else if (scrollType == ScrollType.Horizontal)
            {
                //向左滑
                while (content.anchoredPosition.x <= -layout.padding.left - (headIndex + 1) * (layout.cellSize.x + layout.spacing.x)
                && tailIndex < totalCount - 1)
                {
                    //将数据列表中的第一个元素移动到最后一个
                    RectTransform item = dataList[0];
                    dataList.Remove(item);
                    dataList.Add(item);

                    //设置位置
                    SetPos(item, tailIndex + 1);
                    //设置显示
                    SetShow(item, tailIndex + 1);

                    headIndex++;
                    tailIndex++;
                }
                //向右滑
                while (content.anchoredPosition.x >= -layout.padding.left - (headIndex + 0) * (layout.cellSize.x + layout.spacing.x)
                && headIndex > 0)
                {
                    //将数据列表中的最后一个元素移动到第一个
                    RectTransform item = dataList.Last();
                    dataList.Remove(item);
                    dataList.Insert(0, item);

                    //设置位置
                    SetPos(item, headIndex - 1);
                    //设置显示
                    SetShow(item, headIndex - 1);

                    headIndex--;
                    tailIndex--;
                }
            }
        }

        #endregion

        #region Tool

        /// <summary>
        /// 设置位置
        /// </summary>
        private void SetPos(RectTransform trans, int index)
        {
            if (scrollType == ScrollType.Horizontal)
            {
                trans.anchoredPosition = new Vector2
                (
                    index == 0 ? layout.padding.left + firstItemAnchoredPos.x :
                    layout.padding.left + firstItemAnchoredPos.x + index * (layout.cellSize.x + layout.spacing.x),
                    firstItemAnchoredPos.y
                );
            }
            else if (scrollType == ScrollType.Vertical)
            {
                trans.anchoredPosition = new Vector2
                (
                    firstItemAnchoredPos.x,
                    index == 0 ? -layout.padding.top + firstItemAnchoredPos.y :
                    -layout.padding.top + firstItemAnchoredPos.y - index * (layout.cellSize.y + layout.spacing.y)
                );
            }
        }

        #endregion

        #region 外部调用

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            scrollRect = GetComponent<ScrollRect>();
            content = scrollRect.content;
            layout = content.GetComponent<GridLayoutGroup>();
            scrollRect.onValueChanged.AddListener((Vector2 v) => OnScroll(v));

            DestoryAll();

            //设置布局
            SetLayout();

            #region 挪位置
            /*
            //设置头下标和尾下标
            headIndex = 0;
            tailIndex = fixedCount - 1;

            //设置Content大小
            SetContentSize();

            //实例化Item
            InitItem();

            //得到第一个Item的锚点位置
            GetFirstItemAnchoredPos();
            */
            #endregion

            //this.initSuccess = true;
        }

        /// <summary>
        /// 设置显示
        /// </summary>
        public void SetShow(RectTransform trans, int index)
        {
            //QuickLogger.Log(LogLevel.Info, "设置滚动栏项目 序号{0} onItemRender不为空:{1}", index, (onItemRender != null));
            if (onItemRender != null)
            {
                onItemRender(index, trans);
            }

            //=====根据需求进行编写
            //trans.GetComponentInChildren<Text>().text = index.ToString();
            //trans.name = index.ToString();
        }

        /// <summary>
        /// 设置总的数据数量
        /// </summary>
        public void SetTotalCount(int count)
        {
            totalCount = Mathf.Max(count, 0);
            if (scrollRect == null)
            {
                this.Init();
            }

            //if (scrollType == ScrollType.Vertical)
            //{
            //    scrollRect.verticalNormalizedPosition = 0;
            //}
            //else if (scrollType == ScrollType.Horizontal)
            //{
            //    scrollRect.horizontalNormalizedPosition = 0;
            //}

            //设置头下标和尾下标
            headIndex = 0;
            tailIndex = Mathf.Min(fixedCount, totalCount) - 1;
            //tailIndex = Mathf.Max((Mathf.Min(fixedCount, totalCount) - 1), 0);

            //设置Content大小
            SetContentSize();

            DestoryAll();

            //实例化Item
            InitItem();

            //得到第一个Item的锚点位置
            GetFirstItemAnchoredPos();

        }

        //public void Refresh()
        //{
        //    this.OnScroll(Vector2.zero);
        //}

        /// <summary>
        /// 销毁所有的元素
        /// </summary>
        public void DestoryAll()
        {
            for (int i = dataList.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(dataList[i].gameObject);
            }
            dataList.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 滑动类型
    /// </summary>
    public enum ScrollType
    {
        Horizontal,//竖直滑动
        Vertical,//水平滑动
    }
}

//————————————————
//虽然游戏本身的代码中也有无限滚动，但那个需要预制体，预制体的refers怎么调都调不好……（克隆体上的SetPlaceBuilding全对应的是预制体的组件……我太菜了）
//修改自：
//原文链接：https://blog.csdn.net/LLLLL__/article/details/111033462