using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterFix
{
    public class PinDisplayPointerEnter : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
		private void Start()
		{
			if (this.PinToggle == null)
			{
				this.PinToggle = base.transform.Find("BetterFix.PinToggle");
			}
		}

		public void OnPointerEnter(PointerEventData date)
		{
            if (this.PinToggle != null)
            {
				Toggle tg = this.PinToggle.GetComponent<Toggle>();
                if (!tg.isOn)
				{
					tg.graphic.color = Color.grey;
				}
			}
		}

		public void OnPointerExit(PointerEventData date)
		{
			if (this.PinToggle != null)
			{
				Toggle tg = this.PinToggle.GetComponent<Toggle>();
				if (!tg.isOn)
				{
					tg.graphic.color = Color.clear;
				}
			}
		}

		public Transform PinToggle;
	}
}
