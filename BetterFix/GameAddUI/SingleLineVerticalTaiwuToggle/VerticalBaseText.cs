using System;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;

namespace BetterFix
{
	/// <summary>
	/// 在限制宽度时，能实现单行竖排显示的BaseText（并未真正实现多列竖排显示）
	/// </summary>
	public class VerticalBaseText : VerticalLabel
	{
		//public bool UseBoldFont;
		public bool UseOutline = true;
		public Color OutlineColor = new Color(0f, 0f, 0f, 1f);

		public TextAnchor Alignment
		{
			get
			{
				return this._Text.Alignment;
			}
			set
			{
				this._Text.Alignment = value;
			}
		}

		public override void Create(bool active)
		{
			//this._Text.Font = (this.UseBoldFont ? DateFile.instance.boldFont : DateFile.instance.font);
			base.Create(active);
			if (this.UseOutline)
			{
				base.Get<Outline>().effectColor = this.OutlineColor;
			}
			base.Get<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		public override void Apply()
		{
			base.Apply();
			if (base.Created)
			{
				//this._Text.Font = (this.UseBoldFont ? DateFile.instance.boldFont : DateFile.instance.font);
				if (this.UseOutline)
				{
					UnityEngine.Object.Destroy(base.Get<Outline>());
					return;
				}
				base.Get<Outline>().effectColor = this.OutlineColor;
			}
		}
	}
}
