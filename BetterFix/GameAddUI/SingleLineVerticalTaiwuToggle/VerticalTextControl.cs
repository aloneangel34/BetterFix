using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Components;

namespace BetterFix
{
    /// <summary>
    /// 在限制宽度时，能实现单行竖排显示的TextControl（并未真正实现多列竖排显示）
    /// </summary>
    public class VerticalTextControl : ManagedComponent
    {
        public Text Text
        {
            get
            {
                return base.Get<Text>();
            }
        }

        public override void Apply(ManagedComponent.ComponentAttributes componentAttributes)
        {
            VerticalTextControl.ComponentAttributes applyAttributes = componentAttributes as VerticalTextControl.ComponentAttributes;
            if (!applyAttributes)
            {
                return;
            }
            //this.Text.fontStyle = applyAttributes.FontStyle;
            //this.Text.font = applyAttributes.Font;
            //this.Text.fontSize = applyAttributes.FontSize;
            //this.Text.color = applyAttributes.Color;
            this.Text.alignment = applyAttributes.Alignment;
            this.Text.text = applyAttributes.Content;
            this.Text.horizontalOverflow = HorizontalWrapMode.Wrap;
            this.Text.verticalOverflow = VerticalWrapMode.Overflow;
        }

        public new class ComponentAttributes : ManagedComponent.ComponentAttributes
        {
            //public Font Font;
            //public int FontSize = 18;
            //public Color Color = Color.white;
            public TextAnchor Alignment = TextAnchor.MiddleCenter;
            public string Content;
            //public FontStyle FontStyle;
        }
    }

}
