using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.GameObjects;

namespace BetterFix
{
	/// <summary>
	/// 在限制宽度时，能实现单行竖排显示的Toggle（并未真正实现多列竖排显示）实现了真正有效的Interactable属性
	/// </summary>
	[YamlOnlySerializeSerializable]
	public class VerticalToggle : VerticalBaseTogleButton
	{
		private bool m_isOn;
		/// <summary>
		/// 预设大小
		/// </summary>
		[YamlSerializable]
		public List<float> PreferredSize = new List<float> { 0f, 50f };
		/// <summary>
		/// 私有的
		/// </summary>
		protected bool m_interactable = true;

		public virtual ColorBlock Res_Colors
		{
			get
			{
				return ColorBlock.defaultColorBlock;
			}
		}

		[YamlSerializable]
		public bool isOn
		{
			get
			{
				return this.m_isOn;
			}
			set
			{
				this.m_isOn = value;
				if (base.Created)
				{
					base.Get<UnityEngine.UI.Toggle>().isOn = this.m_isOn;
				}
			}
		}

		/// <summary>
		/// 可交互的
		/// </summary>
		[YamlSerializable]
		public bool Interactable
		{
			get
			{
				return this.m_interactable;
			}
			set
			{
				this.m_interactable = value;
				if (base.Created)
				{
					base.Get<UnityEngine.UI.Toggle>().interactable = this.m_interactable;
				}
			}
		}

		public override void Create(bool active)
		{
			if (this.Element.PreferredSize.Count == 0)
			{
				this.Element.PreferredSize = this.PreferredSize;
			}
			base.Create(active);
			UnityEngine.UI.Toggle toggle = base.Get<UnityEngine.UI.Toggle>();
			toggle.isOn = this.m_isOn;
			toggle.image = base.Get<Image>();
			toggle.onValueChanged.AddListener(new UnityAction<bool>(this.onValueChanged_Invoke));
			toggle.interactable = this.m_interactable;
		}

		private void onValueChanged_Invoke(bool isOn)
		{
			Action<bool, VerticalToggle> action = this.onValueChanged;
			if (action == null)
			{
				return;
			}
			action(isOn, this);
		}

		public Action<bool, VerticalToggle> onValueChanged = delegate (bool p0, VerticalToggle p1)
		{
		};
	}
}
