using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit;
using UnityUIKit;
using UnityUIKit.Core;
using YanLib.ModHelper;
using BepInEx.Logging;


namespace BetterFix
{
    /// <summary>
    /// 自用ModUI组件助手工具
    /// </summary>
    public class ModUIHelperAngel
    {
        /// <summary>
        /// 实例方法UIShowHide缓存下来的组件字典
        /// </summary>
        public Dictionary<Type, Dictionary<String, ManagedGameObject>> ManagedGameObjectCache = new Dictionary<Type, Dictionary<string, ManagedGameObject>>();
        ///// <summary>
        ///// 静态方法FindInAllChild缓存下来的组件字典
        ///// </summary>
        //public Dictionary<String, Dictionary<String, ManagedGameObject>> FindInAllChildCache = new Dictionary<String, Dictionary<String, ManagedGameObject>>();
        /// <summary>
        /// 字段：对应的YanLib.ModHelper实例
        /// </summary>
        private ModHelper _yanLibModHelper;
        /// <summary>
        /// 字段：对应的ModHelper实例的SettingUI（也是根ManagedGameObject）
        /// </summary>
        private ManagedGameObject _rootSettingUI;

        /// <summary>
        /// 属性：本ModUIShowHideHelper实例 对应的 YanLib的ModHelper实例
        /// </summary>
        public ModHelper YanLibModHelper
        {
            get { return _yanLibModHelper; }
        }

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="mod">对应的YanLib.ModHelper实例（请确保其已实例化）</param>
        public ModUIHelperAngel(ModHelper mod)
        {
            if (mod == null)
            {
                Main.Logger.LogFatal("传入构造函数的<ModHelper>mod参数为null(其未被实例化)");
            }
            _yanLibModHelper = mod;
            _rootSettingUI = mod.SettingUI;
        }

        /// <summary>
        /// 检测_rootSettingUI字段与_yanLibModHelper字段（不可修复）是否为null，并尝试修复
        /// </summary>
        /// <typeparam name="T">组件类型（必须继承自ManagedGameObject）</typeparam>
        /// <returns>是否出错（_yanLibModHelper为null时，返回false）</returns>
        private bool PassNullCheck<T>()
        {
            if (!ManagedGameObjectCache.ContainsKey(typeof(T)))
            {
                ManagedGameObjectCache[typeof(T)] = new Dictionary<String, ManagedGameObject>();
            }

            if (_rootSettingUI == null)
            {
                if (_yanLibModHelper == null)
                {
                    QuickLogger.Log(LogLevel.Fatal, "ModUIShowHideHelper 所需的 _yanLibModHelper 为 null，请联系作者修复");
                    return false;
                }
                _rootSettingUI = _yanLibModHelper.SettingUI;
            }
            return true;
        }

        /// <summary>
        /// 检测_rootSettingUI字段与_yanLibModHelper字段（不可修复）是否为null，并尝试修复
        /// </summary>
        /// <returns>是否出错（_yanLibModHelper为null时，返回false）</returns>
        private bool PassNullCheck()
        {
            return PassNullCheck<ManagedGameObject>();
        }

        /// <summary>
        /// 显示隐藏指定组件（带有缓存）
        /// </summary>
        /// <param name="name">组件的Name属性值</param>
        /// <param name="isActive">是否显示 true为显示 false为隐藏</param>
        public void UIShowHide(String name, bool isActive)
        {
            if (!PassNullCheck())
            {
                return;
            }

            //缓存字典中是否已有相应条目
            if (ManagedGameObjectCache[typeof(ManagedGameObject)].ContainsKey(name))
            {
                if (ManagedGameObjectCache[typeof(ManagedGameObject)][name].GameObject.activeSelf != isActive)
                {
                    ManagedGameObjectCache[typeof(ManagedGameObject)][name].SetActive(isActive);
                }
            }
            else
            {
                ManagedGameObject target = null;
                FindInAllChild(name, _rootSettingUI, out target);
                if (target.GameObject.activeSelf != isActive)
                {
                    target.SetActive(isActive);
                }
                //加入缓存字典
                ManagedGameObjectCache[typeof(ManagedGameObject)][name] = target;
            }
        }

        /// <summary>
        /// 搜索指定组件（带有缓存）
        /// </summary>
        /// <typeparam name="T">所查找组件的类型（必须继承自ManagedGameObject）</typeparam>
        /// <param name="name">组件的Name属性值</param>
        /// <param name="result">搜索结果（要使用的话，请自行强制转换类型）</param>
        /// <returns>是否搜索成功</returns>
        public bool SearchUI<T>(String name, out T result) where T : ManagedGameObject
        {
            result = null;

            if (!PassNullCheck<T>())
            {
                return false;
            }

            //缓存字典中是否已有相应条目
            if (ManagedGameObjectCache[typeof(T)].ContainsKey(name) )
            {
                result = ManagedGameObjectCache[typeof(T)][name] as T;
                return true;
            }
            else
            {
                if (FindInAllChild<T>(name, _rootSettingUI, out result))
                {
                    //加入缓存字典
                    ManagedGameObjectCache[typeof(T)][name] = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 查找指定组件下的所有子组件中是否有符合名字的组件（递归）（不带缓存，如需使用请自行缓存）
        /// </summary>
        /// <typeparam name="T">所查找组件的类型（必须继承自ManagedGameObject）</typeparam>
        /// <param name="searchName">查找凭据：组件名字</param>
        /// <param name="parent">指定的根组件</param>
        /// <param name="findedChild">查找到的子组件（若未找到则为null）</param>
        /// <returns>是否查找成功</returns>
        ///// <param name="needCache">是否记录缓存（默认记录）</param>
        public bool FindInAllChild<T>(string searchName, ManagedGameObject parent, out T findedChild) where T : ManagedGameObject
        //public bool FindInAllChild(string searchName, ManagedGameObject parent, out ManagedGameObject findedChild, bool needCache = true)
        {
            bool isFindSucceed = false;
            findedChild = null;

            //if (needCache && FindInAllChildCache.ContainsKey(parent.Name) && FindInAllChildCache[parent.Name].ContainsKey(searchName))
            //{
            //    findedChild = FindInAllChildCache[parent.Name][searchName];
            //    return true; 
            //}

            foreach (ManagedGameObject item in parent.Children)
            {
                if (item.Name == searchName && item is T)
                {
                    findedChild = item as T;
                    isFindSucceed = true;
                    break;
                }

                if (FindInAllChild<T>(searchName, item, out findedChild))
                {
                    isFindSucceed = true;
                    break;
                }
            }

            //if (isFindSucceed && needCache)
            //{
            //    if (FindInAllChildCache.ContainsKey(parent.Name))
            //    {
            //        FindInAllChildCache[parent.Name][searchName] = findedChild;
            //    }
            //    else
            //    {
            //        FindInAllChildCache[parent.Name] = new Dictionary<string, ManagedGameObject>();
            //        FindInAllChildCache[parent.Name][searchName] = findedChild;
            //    }
            //}

            return isFindSucceed;
        }

        #region 弃用代码段，还是不方便，不如直接用泛型
#if false
        public enum SupportedUI
        {
            /// <summary>
            /// UnityUIKit.Core.ManagedGameObject
            /// </summary>
            ManagedGameObject,

            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxAutoSizeModelGameObject
            /// </summary>
            BoxAutoSizeModelGameObject,
            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxElementGameObject
            /// </summary>
            BoxElementGameObject,
            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxGridGameObject
            /// </summary>
            BoxGridGameObject,
            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxGroupGameObject
            /// </summary>
            BoxGroupGameObject,
            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxModelGameObject
            /// </summary>
            BoxModelGameObject,
            /// <summary>
            /// UnityUIKit.Core.GameObjects.BoxSizeFitterGameObject
            /// </summary>
            BoxSizeFitterGameObject,

            /// <summary>
            /// UnityUIKit.GameObjects.BaseTogleButton
            /// </summary>
            BaseTogleButton,
            /// <summary>
            /// UnityUIKit.GameObjects.Block
            /// </summary>
            Block,
            /// <summary>
            /// UnityUIKit.GameObjects.Button
            /// </summary>
            Button,
            /// <summary>
            /// UnityUIKit.GameObjects.Container
            /// </summary>
            Container,
            /// <summary>
            /// UnityUIKit.GameObjects.Container.CanvasContainer
            /// </summary>
            CanvasContainer,
            /// <summary>
            /// UnityUIKit.GameObjects.Container.FitterContainer
            /// </summary>
            FitterContainer,
            /// <summary>
            /// UnityUIKit.GameObjects.Container.GridContainer
            /// </summary>
            GridContainer,
            /// <summary>
            /// UnityUIKit.GameObjects.Container.ScrollContainer
            /// </summary>
            ScrollContainer,
            /// <summary>
            /// UnityUIKit.GameObjects.InputField
            /// </summary>
            InputField,
            /// <summary>
            /// UnityUIKit.GameObjects.Label
            /// </summary>
            Label,
            /// <summary>
            /// UnityUIKit.GameObjects.Slider
            /// </summary>
            Slider,
            /// <summary>
            /// UnityUIKit.GameObjects.Toggle
            /// </summary>
            Toggle,
            /// <summary>
            /// UnityUIKit.GameObjects.ToggleGroup
            /// </summary>
            ToggleGroup,

            /// <summary>
            /// TaiwuUIKit.GameObjects.BaseFrame
            /// </summary>
            BaseFrame,
            /// <summary>
            /// TaiwuUIKit.GameObjects.BaseScroll
            /// </summary>
            BaseScroll,
            /// <summary>
            /// TaiwuUIKit.GameObjects.BaseText
            /// </summary>
            BaseText,
            /// <summary>
            /// TaiwuUIKit.GameObjects.CloseButton
            /// </summary>
            CloseButton,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuActorFace
            /// </summary>
            TaiwuActorFace,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuButton
            /// </summary>
            TaiwuButton,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuInputField
            /// </summary>
            TaiwuInputField,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuLabel
            /// </summary>
            TaiwuLabel,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuSlider
            /// </summary>
            TaiwuSlider,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuTitle
            /// </summary>
            TaiwuTitle,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuToggle
            /// </summary>
            TaiwuToggle,
            /// <summary>
            /// TaiwuUIKit.GameObjects.TaiwuWindows
            /// </summary>
            TaiwuWindows,
        }

        Dictionary<SupportedUI, Type> SupportedType = new Dictionary<SupportedUI, Type>
        {
            { SupportedUI.ManagedGameObject, typeof(UnityUIKit.Core.ManagedGameObject) },

            { SupportedUI.BoxAutoSizeModelGameObject, typeof(UnityUIKit.Core.GameObjects.BoxAutoSizeModelGameObject) },
            { SupportedUI.BoxElementGameObject, typeof(UnityUIKit.Core.GameObjects.BoxElementGameObject) },
            { SupportedUI.BoxGridGameObject, typeof(UnityUIKit.Core.GameObjects.BoxGridGameObject) },
            { SupportedUI.BoxGroupGameObject, typeof(UnityUIKit.Core.GameObjects.BoxGroupGameObject) },
            { SupportedUI.BoxModelGameObject, typeof(UnityUIKit.Core.GameObjects.BoxModelGameObject) },
            { SupportedUI.BoxSizeFitterGameObject, typeof(UnityUIKit.Core.GameObjects.BoxSizeFitterGameObject) },

            { SupportedUI.BaseTogleButton, typeof(UnityUIKit.GameObjects.BaseTogleButton) },
            { SupportedUI.Block, typeof(UnityUIKit.GameObjects.Block) },
            { SupportedUI.Button, typeof(UnityUIKit.GameObjects.Button) },
            { SupportedUI.Container, typeof(UnityUIKit.GameObjects.Container) },
            { SupportedUI.CanvasContainer, typeof(UnityUIKit.GameObjects.Container.CanvasContainer) },
            { SupportedUI.FitterContainer, typeof(UnityUIKit.GameObjects.Container.FitterContainer) },
            { SupportedUI.GridContainer, typeof(UnityUIKit.GameObjects.Container.GridContainer) },
            { SupportedUI.ScrollContainer, typeof(UnityUIKit.GameObjects.Container.ScrollContainer) },
            { SupportedUI.InputField, typeof(UnityUIKit.GameObjects.InputField) },
            { SupportedUI.Label, typeof(UnityUIKit.GameObjects.Label) },
            { SupportedUI.Slider, typeof(UnityUIKit.GameObjects.Slider) },
            { SupportedUI.Toggle, typeof(UnityUIKit.GameObjects.Toggle) },
            { SupportedUI.ToggleGroup, typeof(UnityUIKit.GameObjects.ToggleGroup) },

            { SupportedUI.BaseFrame, typeof(TaiwuUIKit.GameObjects.BaseFrame) },
            { SupportedUI.BaseScroll, typeof(TaiwuUIKit.GameObjects.BaseScroll) },
            { SupportedUI.BaseText, typeof(TaiwuUIKit.GameObjects.BaseText) },
            { SupportedUI.CloseButton, typeof(TaiwuUIKit.GameObjects.CloseButton) },
            { SupportedUI.TaiwuActorFace, typeof(TaiwuUIKit.GameObjects.TaiwuActorFace) },
            { SupportedUI.TaiwuButton, typeof(TaiwuUIKit.GameObjects.TaiwuButton) },
            { SupportedUI.TaiwuInputField, typeof(TaiwuUIKit.GameObjects.TaiwuInputField) },
            { SupportedUI.TaiwuLabel, typeof(TaiwuUIKit.GameObjects.TaiwuLabel) },
            { SupportedUI.TaiwuSlider, typeof(TaiwuUIKit.GameObjects.TaiwuSlider) },
            { SupportedUI.TaiwuTitle, typeof(TaiwuUIKit.GameObjects.TaiwuTitle) },
            { SupportedUI.TaiwuToggle, typeof(TaiwuUIKit.GameObjects.TaiwuToggle) },
            { SupportedUI.TaiwuWindows, typeof(TaiwuUIKit.GameObjects.TaiwuWindows) }
        };
#endif
        #endregion
    }
}
