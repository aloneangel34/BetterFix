using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 用于创建UI的工具组（整合的专用于MouseTipActorInfoSimple追加信息UI 与 单项属性UI）
    /// </summary>
    public static class CreateUIForMouseTip
    {
        /// <summary>
        /// 创建（宽度固定固定、高度自适应的）整合的专用于MouseTipActorInfoSimple追加信息UI
        /// </summary>
        /// <returns>AdditionInfo组件（用于设定）（绑定父级请.transform）</returns>
        public static AdditionInfo CreateAdditionInfoUI()
        {
            //!----- 主UI容器 -----!
            GameObject additionInfoGO = new GameObject("BetterFix.AdditionInfoUI", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(AdditionInfo));
            RectTransform additionInfoRect = additionInfoGO.GetComponent<RectTransform>();
            additionInfoRect.sizeDelta = new Vector2(250, 195); 
            VerticalLayoutGroup additionInfoVLG = additionInfoGO.GetComponent<VerticalLayoutGroup>();
            additionInfoVLG.padding = new RectOffset(0, 0, -1, 5);
            additionInfoVLG.childAlignment = TextAnchor.UpperCenter;
            additionInfoVLG.childControlHeight = false;
            additionInfoVLG.childControlWidth = false;
            additionInfoVLG.childForceExpandHeight = false;
            additionInfoVLG.childForceExpandWidth = false;
            ContentSizeFitter additionInfoCSF = additionInfoGO.GetComponent<ContentSizeFitter>();
            additionInfoCSF.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            additionInfoCSF.verticalFit = ContentSizeFitter.FitMode.MinSize;

            //!----- 称号属性UI -----!
            AttrUI generationAttrUI = CreateNewAttrUI(AttrType.Generation);
            generationAttrUI.transform.SetParent(additionInfoRect, false);

            //!----- 好感属性UI -----!
            AttrUI favorAttrUI = CreateNewAttrUI(AttrType.Favor);
            favorAttrUI.transform.SetParent(additionInfoRect, false);

            //!----- 健康属性UI -----!
            AttrUI healthAttrUI = CreateNewAttrUI(AttrType.Health);
            healthAttrUI.transform.SetParent(additionInfoRect, false);

            //!----- 所处地点UI -----!
            RectTransform placeInfoUI = CreatePlaceInfoUI(out CText placeInfoCText);
            placeInfoUI.SetParent(additionInfoRect, false);

            //!----- 绑定AdditionInfo组件的所需设定项 -----!
            AdditionInfo additionInfoSet = additionInfoGO.GetComponent<AdditionInfo>();
            additionInfoSet.ActorGeneration = generationAttrUI;
            additionInfoSet.ActorFavor = favorAttrUI;
            additionInfoSet.ActorHealth = healthAttrUI;
            additionInfoSet.ActorPlace = placeInfoCText;

            return additionInfoSet;
        }

        /// <summary>
        /// 创建（固定宽高的）单项属性UI
        /// </summary>
        /// <returns>AttrUI组件（用于设定）（绑定父级请.transform）</returns>
        public static AttrUI CreateNewAttrUI(AttrType attrType)
        {
            //!----- 主UI容器 -----!
            GameObject attrUiGO = new GameObject("BetterFix.AttrUi", typeof(RectTransform), typeof(AttrUI));
            RectTransform attrUiRect = attrUiGO.GetComponent<RectTransform>();
            attrUiRect.sizeDelta = new Vector2(250, 35);

            //!----- 属性图标 -----!
            GameObject attrIconGO = new GameObject("Icon", typeof(RectTransform), typeof(CImage));
            RectTransform attrIconRect = attrIconGO.GetComponent<RectTransform>();
            attrIconRect.SetParent(attrUiRect, false);
            attrIconRect.anchorMin = new Vector2(0, 0.5f);
            attrIconRect.anchorMax = new Vector2(0, 0.5f);
            attrIconRect.anchoredPosition = new Vector2(0, 0);
            attrIconRect.sizeDelta = new Vector2(30, 30);
            attrIconRect.pivot = new Vector2(0, 0.5f);
            CImage attrIconCImage = attrIconGO.GetComponent<CImage>();
            attrIconCImage.raycastTarget = false;
            //C8B099FF
            attrIconCImage.color = new Color32(200, 176, 153, 255);

            //!----- 属性名称背景 -----!
            GameObject attrTipBackGO = new GameObject("TipBack", typeof(RectTransform), typeof(CImage));
            RectTransform attrTipBackRect = attrTipBackGO.GetComponent<RectTransform>();
            attrTipBackRect.SetParent(attrUiRect, false);
            attrTipBackRect.anchorMin = new Vector2(0, 0.5f);
            attrTipBackRect.anchorMax = new Vector2(0, 0.5f);
            attrTipBackRect.anchoredPosition = new Vector2(30, 0);
            attrTipBackRect.sizeDelta = new Vector2(50, 35);
            attrTipBackRect.pivot = new Vector2(0, 0.5f);
            CImage attrTipBackCImage = attrTipBackGO.GetComponent<CImage>();
            attrTipBackCImage.type = Image.Type.Sliced;
            attrTipBackCImage.raycastTarget = false;
            //1E1C1AFF
            attrTipBackCImage.color = new Color32(30, 28, 26, 255);
            attrTipBackCImage.SetGraphicsSprite("BaseUI/", "GUI_ValuBack", false);

            //!----- 属性名称文本 -----!
            GameObject attrTipTextGO = new GameObject("TipText", typeof(RectTransform), typeof(CText), typeof(SetFont), typeof(Outline));
            RectTransform attrTipTextRect = attrTipTextGO.GetComponent<RectTransform>();
            attrTipTextRect.SetParent(attrTipBackRect, false);
            attrTipTextRect.anchorMin = new Vector2(0, 0);
            attrTipTextRect.anchorMax = new Vector2(1, 1);
            attrTipTextRect.anchoredPosition = new Vector2(0, 0);
            attrTipTextRect.sizeDelta = new Vector2(0, 0);
            attrTipTextRect.pivot = new Vector2(0, 0.5f);
            SetFont attrTipTextSetFont = attrTipTextGO.GetComponent<SetFont>();
            attrTipTextSetFont.isTitleFont = true;
            CText attrTipTextCText = attrTipTextGO.GetComponent<CText>();
            attrTipTextCText.raycastTarget = false;
            attrTipTextCText.fontSize = 20;
            //504641FF
            attrTipTextCText.color = new Color32(80, 70, 65, 255);
            attrTipTextCText.alignment = TextAnchor.MiddleCenter;
            attrTipTextCText.horizontalOverflow = HorizontalWrapMode.Overflow;

            //!----- 属性信息背景 -----!
            GameObject attrAttrBackGO = new GameObject("AttrBack", typeof(RectTransform), typeof(CImage));
            RectTransform attrAttrBackRect = attrAttrBackGO.GetComponent<RectTransform>();
            attrAttrBackRect.SetParent(attrUiRect, false);
            attrAttrBackRect.anchorMin = new Vector2(0, 0.5f);
            attrAttrBackRect.anchorMax = new Vector2(0, 0.5f);
            attrAttrBackRect.anchoredPosition = new Vector2(80, 0);
            attrAttrBackRect.sizeDelta = new Vector2(170, 35);
            attrAttrBackRect.pivot = new Vector2(0, 0.5f);
            CImage attrAttrBackCImage = attrAttrBackGO.GetComponent<CImage>();
            attrAttrBackCImage.type = Image.Type.Sliced;
            attrAttrBackCImage.raycastTarget = false;
            //312B28FF
            attrAttrBackCImage.color = new Color32(49, 43, 40, 255);
            attrAttrBackCImage.SetGraphicsSprite("BaseUI/", "GUI_ValuBack", false);

            //!----- 属性信息文本 -----!
            GameObject attrAttrTextGO = new GameObject("AttrText", typeof(RectTransform), typeof(CText), typeof(SetFont));
            RectTransform attrAttrTextRect = attrAttrTextGO.GetComponent<RectTransform>();
            attrAttrTextRect.SetParent(attrAttrBackRect, false);
            attrAttrTextRect.anchorMin = new Vector2(0, 0);
            attrAttrTextRect.anchorMax = new Vector2(1, 1);
            attrAttrTextRect.anchoredPosition = new Vector2(0, 0);
            attrAttrTextRect.sizeDelta = new Vector2(0, 0);
            attrAttrTextRect.pivot = new Vector2(0, 0.5f);
            CText attrAttrTextCText = attrAttrTextGO.GetComponent<CText>();
            attrAttrTextCText.raycastTarget = false;
            attrAttrTextCText.fontSize = 20;
            //E1CDAAFF
            attrAttrTextCText.color = new Color32(225, 205, 170, 255);
            attrAttrTextCText.alignment = TextAnchor.MiddleCenter;
            attrAttrTextCText.horizontalOverflow = HorizontalWrapMode.Overflow;

            //!----- 绑定AttrUI组件的所需设定项 -----!
            //attrUiGO.AddComponent<AttrUI>();
            AttrUI attrUiSet = attrUiGO.GetComponent<AttrUI>();
            attrUiSet.AttrIcon = attrIconCImage;
            attrUiSet.TipText = attrTipTextCText;
            attrUiSet.AttrText = attrAttrTextCText;
            attrUiSet.SetAttrType(attrType);

            return attrUiSet;
        }

        /// <summary>
        /// 创建（宽度固定固定、高度自适应的）所处地点信息UI
        /// </summary>
        /// <param name="placeInfo">UI的CText组件（用于设定信息）</param>
        /// <returns>UI的RectTransform组件（用于绑定父级）</returns>
        public static RectTransform CreatePlaceInfoUI(out CText placeInfo)
        {
            //!----- 主UI容器 -----!
            GameObject placeInfoGO = new GameObject("BetterFix.PlaceInfoUI", typeof(RectTransform), typeof(ContentSizeFitter), typeof(CText), typeof(SetFont));
            RectTransform placeInfoRect = placeInfoGO.GetComponent<RectTransform>();
            placeInfoRect.sizeDelta = new Vector2(250, 85);
            ContentSizeFitter placeInfoContentSizeFitter = placeInfoGO.GetComponent<ContentSizeFitter>();
            placeInfoContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            placeInfoContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CText placeInfoCText = placeInfoGO.GetComponent<CText>();
            placeInfoCText.raycastTarget = false;
            placeInfoCText.fontSize = 20;
            //E1CDAAFF
            placeInfoCText.color = new Color32(225, 205, 170, 255);
            placeInfoCText.alignment = TextAnchor.UpperCenter;

            #region 弃用
            /*
            //!----- 文本 -----!
            GameObject placeInfoTextGO = new GameObject("AttrText", typeof(RectTransform), typeof(CText), typeof(SetFont));
            RectTransform placeInfoTextRect = placeInfoTextGO.GetComponent<RectTransform>();
            placeInfoTextRect.SetParent(placeInfoRect, false);
            placeInfoTextRect.anchorMin = new Vector2(0, 0);
            placeInfoTextRect.anchorMax = new Vector2(1, 1);
            placeInfoTextRect.anchoredPosition = new Vector2(0, 0);
            placeInfoTextRect.sizeDelta = new Vector2(0, 0);
            placeInfoTextRect.pivot = new Vector2(0, 0.5f);
            CText placeInfoTextCText = placeInfoTextGO.GetComponent<CText>();
            placeInfoTextCText.raycastTarget = false;
            placeInfoTextCText.fontSize = 18;
            //E1CDAAFF
            placeInfoTextCText.color = new Color32(225, 205, 170, 255);
            placeInfoTextCText.alignment = TextAnchor.UpperLeft;
            */
            #endregion

            placeInfo = placeInfoCText;

            return placeInfoRect;
        }
    }

}
