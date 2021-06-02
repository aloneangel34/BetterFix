using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;
using BepInEx.Logging;
using YamlDotNet.Serialization;

namespace BetterFix
{
    [YamlOnlySerializeSerializable]
    public class PinToggle : UnityUIKit.GameObjects.Toggle
	{
		private static readonly PointerEnter _pointerEnter;
		private static readonly PointerClick _pointerClick;
		private static readonly ColorBlock _colors;
		private static readonly Image _BackgroundImage;
		private List<string> TipParm = new List<string> { "", "" };
		private BaseText m_Label = new BaseText();

		public override Image Res_Image
		{
			get
			{
				return PinToggle._BackgroundImage;
			}
		}

		public virtual PointerEnter Res_PointerEnter
		{
			get
			{
				return PinToggle._pointerEnter;
			}
		}

		public virtual PointerClick Res_PointerClick
		{
			get
			{
				return PinToggle._pointerClick;
			}
		}

		public override ColorBlock Res_Colors
		{
			get
			{
				return PinToggle._colors;
			}
		}

		static PinToggle()
		{
			Transform noTakeIcon = Resources.Load<GameObject>("oldsceneprefabs/actormenu").transform.Find("ActorMenuBack/ActorItems/ItemsAllBack/ItemsAllView/ItemsAllViewport/ItemIconNoToggle/NotakeIcon");
			PinToggle._BackgroundImage = noTakeIcon.GetComponent<UnityEngine.UI.Image>();
			//物品锁（若要使用需要额外上色——设置image.color）
			//ResLoader.Load<Sprite>("Graphics/BaseUI/LockIcon", delegate (Sprite sp) { PinToggle._BackgroundImage.sprite = sp; }, false, null);
			//好感心型（若要使用需要额外上色——设置image.color）
			//ResLoader.Load<Sprite>("Graphics/BaseUI/PartGoodIcon", delegate (Sprite sp) { PinToggle._BackgroundImage.sprite = sp; }, false, null);
			Transform fullScreenToggle = Resources.Load<GameObject>("prefabs/ui/views/ui_systemsetting").transform.Find("SystemSetting/SetScreen/FullScreenToggle,702");
			PinToggle._pointerEnter = fullScreenToggle.GetComponent<PointerEnter>();
			PinToggle._pointerClick = fullScreenToggle.GetComponent<PointerClick>();
			PinToggle._colors = new ColorBlock
			{
                //normalColor = Color.grey,
                //highlightedColor = Color.grey,
                //pressedColor = new Color32(255, 100, 100, 255),
                //disabledColor = Color.clear,
                normalColor = new Color32(251, 251, 251, 255),
                highlightedColor = new Color32(245, 245, 245, 255),
                pressedColor = new Color32(142, 142, 142, 255),
                disabledColor = new Color32(75, 75, 75, 255),
                colorMultiplier = 1f,
				fadeDuration = 0.1f
			};
			//TaiwuToggle._colors = new ColorBlock
			//{
			//	normalColor = new Color32(251, 251, 251, byte.MaxValue),
			//	highlightedColor = new Color32(245, 245, 245, byte.MaxValue),
			//	pressedColor = new Color32(142, 142, 142, byte.MaxValue),
			//	disabledColor = new Color32(75, 75, 75, byte.MaxValue),
			//	colorMultiplier = 1f,
			//	fadeDuration = 0.1f
			//};
		}

		/// <summary>
		/// Tip 的标题
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
		public override Label Label
		{
			get
			{
				return this.m_Label;
			}
		}

		/// <summary>
		/// 使用粗体
		/// </summary>
		[YamlSerializable]
		public bool UseBoldFont
		{
			get
			{
				return (this.Label as BaseText).UseBoldFont;
			}
			set
			{
				(this.Label as BaseText).UseBoldFont = value;
			}
		}

		/// <summary>
		/// 使用 OutLine
		/// </summary>
		[YamlSerializable]
		public bool UseOutline
		{
			get
			{
				return (this.Label as BaseText).UseOutline;
			}
			set
			{
				(this.Label as BaseText).UseOutline = value;
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public PinToggle()
		{
			base.FontColor = Color.white;
		}

		/// <summary>
		/// 创建组件
		/// </summary>
		/// <param name="active"></param>
		// Token: 0x06000049 RID: 73 RVA: 0x00002FD8 File Offset: 0x000011D8
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
			this.Label.RectTransform.anchorMin = Vector2.zero;
			this.Label.RectTransform.anchorMax = Vector2.one;
			if (!string.IsNullOrEmpty(this.TipTitle) || !string.IsNullOrEmpty(this.TipContant))
			{
				base.Get<MouseTipDisplayer>().param = this.TipParm.ToArray();
			}
			UnityEngine.UI.Toggle toggle = base.Get<UnityEngine.UI.Toggle>();
			toggle.transition = Selectable.Transition.ColorTint;
			toggle.colors = this.Res_Colors;
            BoxModelGameObject labelChild = new BoxModelGameObject();
            labelChild.Name = "Label";
            labelChild.SetParent(this, false);
            Image labelChildImage = labelChild.Get<Image>();
            labelChildImage.type = this.Res_Image.type;
            labelChildImage.sprite = this.Res_Image.sprite;
			//labelChildImage.color = new Color(156f / 255f, 54f / 255f, 54f / 255f, 1f);
			labelChildImage.color = Color.red;
            labelChild.RectTransform.sizeDelta = Vector2.zero;
            labelChild.RectTransform.anchorMin = Vector2.zero;
            labelChild.RectTransform.anchorMax = Vector2.one;
            labelChild.RectTransform.SetAsFirstSibling();
            labelChild.Get<LayoutElement>().ignoreLayout = true;
            toggle.graphic = labelChildImage;
			//base.Get<Image>().color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
			base.Get<Image>().color = Color.grey;
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
			//新加入的，用于固定按钮大小
			this.RectTransform.sizeDelta = new Vector2(20f, 20f);
			this.RectTransform.anchorMin = new Vector2(0f, 1f);
			this.RectTransform.anchorMax = new Vector2(0f, 1f);
		}
	}
}

