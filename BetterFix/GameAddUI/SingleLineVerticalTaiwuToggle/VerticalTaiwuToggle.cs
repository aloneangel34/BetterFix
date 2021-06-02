using System.Collections.Generic;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;

namespace BetterFix
{
    /// <summary>
    /// 在限制宽度时，能实现单行竖排显示的TaiwuToggle（并未真正实现多列竖排显示）
    /// </summary>
    [YamlOnlySerializeSerializable]
    public class VerticalTaiwuToggle : VerticalToggle
    {
        private static readonly PointerEnter _pointerEnter;
        private static readonly PointerClick _pointerClick;
        private static readonly ColorBlock _colors;
        private static readonly Image _BackgroundImage;
        private List<string> TipParm = new List<string> { "", "" };
        private VerticalBaseText m_Label = new VerticalBaseText();
        private bool _isTitel = false;

        public override Image Res_Image
        {
            get
            {
                return VerticalTaiwuToggle._BackgroundImage;
            }
        }

        public virtual PointerEnter Res_PointerEnter
        {
            get
            {
                return VerticalTaiwuToggle._pointerEnter;
            }
        }

        public virtual PointerClick Res_PointerClick
        {
            get
            {
                return VerticalTaiwuToggle._pointerClick;
            }
        }

        public override ColorBlock Res_Colors
        {
            get
            {
                return VerticalTaiwuToggle._colors;
            }
        }

        static VerticalTaiwuToggle()
        {
            Transform transform = Resources.Load<GameObject>("prefabs/ui/views/ui_systemsetting").transform.Find("SystemSetting/SetScreen/FullScreenToggle,702");
            VerticalTaiwuToggle._pointerEnter = transform.GetComponent<PointerEnter>();
            VerticalTaiwuToggle._pointerClick = transform.GetComponent<PointerClick>();
            VerticalTaiwuToggle._BackgroundImage = transform.GetComponent<CToggle>().image;
            VerticalTaiwuToggle._colors = new ColorBlock
            {
                normalColor = new Color32(251, 251, 251, byte.MaxValue),
                highlightedColor = new Color32(245, 245, 245, byte.MaxValue),
                pressedColor = new Color32(142, 142, 142, byte.MaxValue),
                disabledColor = new Color32(75, 75, 75, byte.MaxValue),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }

        /// <summary>
        /// 是否为标题样式（仅用于创建前快速设定 Label 的字体、字体大小、字体颜色。创建后该项属性不再有任何意义）
        /// </summary>
        [YamlSerializable]
        public bool IsTitel
        {
            get
            {
                return this._isTitel;
            }
            set
            {
                this._isTitel = value;
            }
        }

        /// <summary>
        /// Tip 的 标题
        /// </summary>
        [YamlSerializable]
        public string TipTitle
        {
            get
            {
                return this.TipParm[0];
            }
            set
            {
                this.TipParm[0] = value;
                if (base.Created)
                {
                    base.Get<MouseTipDisplayer>().param = this.TipParm.ToArray();
                }
            }
        }

        /// <summary>
        /// Tip 的 内容
        /// </summary>
        [YamlSerializable]
        public string TipContant
        {
            get
            {
                return this.TipParm[1];
            }
            set
            {
                this.TipParm[1] = value;
                if (base.Created)
                {
                    base.Get<MouseTipDisplayer>().param = this.TipParm.ToArray();
                }
            }
        }

        /// <summary>
        /// 文本表情（实际上是 BaseText 类型的）
        /// </summary>
        public override VerticalLabel Label
        {
            get
            {
                return this.m_Label;
            }
        }

        /// <summary>
        /// 使用粗体
        /// </summary>
        //[YamlSerializable]
        //public bool UseBoldFont
        //{
        //    get
        //    {
        //        return (this.Label as VerticalBaseText).UseBoldFont;
        //    }
        //    set
        //    {
        //        (this.Label as VerticalBaseText).UseBoldFont = value;
        //    }
        //}

        /// <summary>
        /// 使用 OutLine
        /// </summary>
        [YamlSerializable]
        public bool UseOutline
        {
            get
            {
                return (this.Label as VerticalBaseText).UseOutline;
            }
            set
            {
                (this.Label as VerticalBaseText).UseOutline = value;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public VerticalTaiwuToggle()
        {
            //base.FontColor = Color.white;
        }

        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="active"></param>
        public override void Create(bool active = true)
        {
            if (this.Element.PreferredSize.Count == 0)
            {
                this.Element.PreferredSize = this.PreferredSize;
            }
            base.Create(active);
     
            UnityEngine.Object.Destroy(this.Label.Get<ContentSizeFitter>());
            this.Label.Get<LayoutElement>().ignoreLayout = true;
            this.Label.RectTransform.sizeDelta = Vector2.zero;
            this.Label.RectTransform.anchoredPosition = Vector2.zero;
            this.Label.RectTransform.anchorMin = new Vector2(0.5f, 0);
            this.Label.RectTransform.anchorMax = new Vector2(0.5f, 1);
            if (this._isTitel)
            {
                this.Label.Get<SetFont>().isTitleFont = true;
                //this.UseBoldFont = true;
                //this.Label.VerticalTextControl.Text.fontSize += 2;
                this.Label.VerticalTextControl.Text.color = new Color32(225, 220, 170, 255);
            }
            if (!string.IsNullOrEmpty(this.TipTitle) || !string.IsNullOrEmpty(this.TipContant))
            {
                base.Get<MouseTipDisplayer>().param = this.TipParm.ToArray();
            }
            UnityEngine.UI.Toggle toggle = base.Get<UnityEngine.UI.Toggle>();
            toggle.transition = Selectable.Transition.ColorTint;
            toggle.colors = this.Res_Colors;
            BoxModelGameObject boxModelGameObject = new BoxModelGameObject();
            boxModelGameObject.Name = "Label";
            boxModelGameObject.SetParent(this, false);
            Image image = boxModelGameObject.Get<Image>();
            image.type = this.Res_Image.type;
            image.sprite = this.Res_Image.sprite;
            image.color = new Color(0.6117647f, 0.21176471f, 0.21176471f, 1f);
            boxModelGameObject.RectTransform.sizeDelta = Vector2.zero;
            boxModelGameObject.RectTransform.anchorMin = Vector2.zero;
            boxModelGameObject.RectTransform.anchorMax = Vector2.one;
            boxModelGameObject.RectTransform.SetAsFirstSibling();
            boxModelGameObject.Get<LayoutElement>().ignoreLayout = true;
            toggle.graphic = image;
            base.Get<Image>().color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
            if (this.Res_PointerClick != null)
            {
                PointerClick pointerClick = base.Get<PointerClick>();
                pointerClick.playSE = this.Res_PointerClick.playSE;
                pointerClick.SEKey = this.Res_PointerClick.SEKey;
            }
            if (this.Res_PointerEnter != null)
            {
                PointerEnter pointerEnter = base.Get<PointerEnter>();
                pointerEnter.changeSize = this.Res_PointerEnter.changeSize;
                pointerEnter.restSize = this.Res_PointerEnter.restSize;
                pointerEnter.xMirror = this.Res_PointerEnter.xMirror;
                pointerEnter.yMirror = this.Res_PointerEnter.yMirror;
                pointerEnter.move = this.Res_PointerEnter.move;
                pointerEnter.moveX = this.Res_PointerEnter.moveX;
                pointerEnter.moveSize = this.Res_PointerEnter.moveSize;
                pointerEnter.restMoveSize = this.Res_PointerEnter.restMoveSize;
                pointerEnter.SEKey = this.Res_PointerEnter.SEKey;
                pointerEnter.changeTarget = this.Res_PointerEnter.changeTarget;
            }
        }
    }
}

