using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Components;
using UnityUIKit.Core;
using YamlDotNet.Serialization;
using UnityUIKit.GameObjects;

namespace BetterFix
{
    /// <summary>
    /// 在限制宽度时，能实现单行竖排显示的Label（并未真正实现多列竖排显示）
    /// </summary>
    public class VerticalLabel : ManagedGameObject
    {
        [YamlIgnore]
        public VerticalTextControl VerticalTextControl
        {
            get
            {
                return base.Get<VerticalTextControl>();
            }
        }

        public string TextString
        {
            get
            {
                return this._Text.Content;
            }
            set
            {
                this._Text.Content = value;
                this.Apply();
            }
        }

        public override void Create(bool active)
        {
            base.Create(active);
            this.VerticalTextControl.Apply(this._Text);
            this.VerticalTextControl.Text.fontSize = 18;
        }

        public virtual void Apply()
        {
            if (base.Created)
            {
                this.VerticalTextControl.Apply(this._Text);
            }
        }

        public VerticalTextControl.ComponentAttributes _Text = new VerticalTextControl.ComponentAttributes();
    }
}

