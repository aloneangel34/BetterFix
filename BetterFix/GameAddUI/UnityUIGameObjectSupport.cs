using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{
    public static class UnityUIGameObjectSupport
    {
        public static Text CreateTextGameObject(string name, Color color, bool isTitle = false)
        {
            //创建GameObject
            GameObject gameObject = new GameObject(name);
            gameObject.layer = 31;
            //加入Text组件（自带RectTransform和CanvasRenderer组件）
            gameObject.AddComponent<Text>();
            Text text = gameObject.GetComponent<Text>();
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.fontSize = 20;
            text.color = color;
            //加入描边组件
            gameObject.AddComponent<Outline>();
            gameObject.GetComponent<Outline>().effectColor = new Color32(0, 0, 0, 255);
            //加入太吾的设置字体SetFont组件
            gameObject.AddComponent<SetFont>();
            SetFont setFont = gameObject.GetComponent<SetFont>();
            setFont.isTitleFont = isTitle;

            return text;
        }

        public static Image CreateImageGameObject(string name, Color color, bool isBar = false)
        {
            //创建GameObject
            GameObject gameObject = new GameObject(name);
            gameObject.layer = 31;
            //加入Image组件（自带RectTransform和CanvasRenderer组件）
            gameObject.AddComponent<Image>();
            Image image = gameObject.GetComponent<Image>();
            if (isBar)
            {
                image.type = Image.Type.Filled;
                image.fillAmount = 0.25f;
                image.fillOrigin = 2;
            }
            image.raycastTarget = false;
            image.color = color;

            return image;
        }


        public static Button CreateButtonGameObject(string name, Image targetGraphic = null)
        {
            //创建GameObject
            GameObject gameObject = new GameObject(name);
            gameObject.layer = 31;
            //加入Image组件（自带RectTransform和CanvasRenderer组件）
            gameObject.AddComponent<Image>();
            Image image = gameObject.GetComponent<Image>();
            image.type = Image.Type.Sliced;
            image.raycastTarget = true;
            image.color = new Color32(255, 255, 255, 0);
            //加入Button组件（自动绑定同GameObject下的Image组件）
            gameObject.AddComponent<Button>();
            Button button = gameObject.GetComponent<Button>();
            if (targetGraphic != null)
            {
                button.targetGraphic = targetGraphic;
            }

            return button;
        }
    }
}
