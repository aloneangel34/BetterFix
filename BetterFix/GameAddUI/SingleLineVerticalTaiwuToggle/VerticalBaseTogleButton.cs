using System;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core.GameObjects;
using YamlDotNet.Serialization;
using UnityUIKit.GameObjects;

namespace BetterFix
{
	/// <summary>
	/// 调整后的BaseTogleButton，去除了无效的Interactable属性
	/// </summary>
	public class VerticalBaseTogleButton : BoxModelGameObject
	{
		private VerticalLabel m_Label = new VerticalLabel();
		public Color ImageColor = Color.clear;

		[YamlIgnore]
		public virtual Image Res_Image
		{
			get
			{
				return null;
			}
		}

		[YamlIgnore]
		public virtual VerticalLabel Label
		{
			get
			{
				return this.m_Label;
			}
		}

		public VerticalBaseTogleButton()
		{
			//this.Label._Text.Color = Color.gray;
		}

		/// <summary>
		/// 字体颜色
		/// </summary>
		//public Color FontColor
		//{
		//	get
		//	{
		//		return this.Label._Text.Color;
		//	}
		//	set
		//	{
		//		this.Label._Text.Color = value;
		//	}
		//}

		/// <summary>
		/// 内容
		/// </summary>
		public string TextString
		{
			get
			{
				return this.Label.TextString;
			}
			set
			{
				this.Label.TextString = value;
			}
		}

		/// <summary>
		/// 创建对像
		/// </summary>
		/// <param name="active"></param>
		public override void Create(bool active)
		{
			base.Create(active);
			base.LayoutGroup.childForceExpandWidth = true;
			Image image = base.Get<Image>();
			image.type = this.Res_Image.type;
			image.sprite = this.Res_Image.sprite;
			image.color = this.ImageColor;
			this.Label.Name = base.Name + ":Text";
			this.Label.SetParent(this, false);
		}
	}
}

