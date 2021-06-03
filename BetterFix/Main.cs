using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using GameData;
using HarmonyLib;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using YanLib.ModHelper;

namespace BetterFix
{
    /// <summary>
    ///  Mod 入口
    /// </summary>
    [BepInPlugin(GUID, ModDisplayName, Version)]            //GUID, 在BepInEx中的显示名称, 版本
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]    //限制插件所能加载的程序
    [BepInDependency("0.0Yan.Lib")]                         //插件的硬性前置依赖MOD/插件
    public class Main : BaseUnityPlugin
    {
        /// <summary>插件版本</summary>
        public const string Version = "1.0.2";
        /// <summary>插件名字</summary>
        public const string ModDisplayName = "BetterFix/优化加强";
        /// <summary>插件ID</summary>
        public const string GUID = "TaiwuMOD.BetterFix";
        /// <summary>日志</summary> 
        public static new ManualLogSource Logger;   //声明一个ManualLogSource实例类的的静态字段（因为有基类继承，所以用new隐藏）
        /// <summary>MOD设置界面</summary>
        public static ModHelper Mod;                //声明一个ModHelper实例类的的静态字段
        /// <summary>设置</summary>
        public static Settings Setting;             //声明一个Settings实例类的静态字段
        /// <summary>用于显示隐藏组件</summary>
        public static ModUIHelperAngel UIAngel;
        /// <summary>地点建筑UI</summary>
        public static PlaceHomeBuildingUI InGameBuildingUI;

        /// <summary>
        /// Unity组件脚本加载时调用
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this);    //本脚本在读取时不会被销毁
            Logger = base.Logger;       //将Logger字段赋值为基类（BaseUnityPlugin）的Logger实例
            Setting = new Settings();   //创建Settings实例类的对象、并赋值给Setting字段
            Setting.Init(Config);       //读取本插件的配置文件（继承下来的Config），调用Init方法来进行设置选项的初始化

            //以本MOD的GUID创建Harmony实例
            Harmony harmony = new Harmony(GUID);

            //若MOD启用，则尝试加载所有Harmony补丁【不然则等用户开启MOD时再尝试加载】
            if (Setting.ModEnable.Value)
            {
                try
                {
                    harmony.PatchAll();
                }
                catch (Exception ex)
                {
                    Logger.LogFatal("尝试加载Harmony补丁时出现异常");
                    Logger.LogFatal(ex);
                }
            }

            //MOD设置界面主体
            Mod = new ModHelper(GUID, ModDisplayName + Version);
            //组件显示隐藏助手（ShowHide的创建实例的顺序一定要在Mod创建实例之后，Mod为空会有问题）
            UIAngel = new ModUIHelperAngel(Mod);
            //设置MOD设置界面UI【自适应垂直UI组：MOD设置UI界面】
            Mod.SettingUI = new BoxAutoSizeModelGameObject()
            {
                //该组件内的子组件排布设定
                Group =
                {
                    //子组件垂直排布
                    Direction = UnityUIKit.Core.Direction.Vertical,
                    //子组件排布间隔
                    Spacing = 10,
                    //组件边缘填充
                    Padding = { 10, 0, 0, 0 },
                },
                //大小自适应设置
                SizeFitter =
                {
                    //垂直高度自适应
                    VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                },
                Children = 
                //该组件内的子组件
                {
                    //【MOD总开关】
                    new TaiwuToggle()
                    {
                        //组件的Name ID（是字符串，但并非该组件在游戏中的显示名称）
                        Name = "MOD总开关",
                        //组件的显示文本（会显示在UI上）
                        Text = Setting.ModEnable.Value ? "MOD 已开启" : "MOD 已关闭",
                        //开关组件的开关状态
                        isOn = Setting.ModEnable.Value,
                        //当数值改变时的操作
                        onValueChanged = (bool value, Toggle tg) =>
                        {
                            //将变动后的数值 赋值给 Setting.enabled.Value
                            //（bool型数值）“value”，是由这个动作 onValueChanged = (bool 「value」, Toggle tg) 传出来的
                            Setting.ModEnable.Value = value;

                            //开关MOD时直接将会应用/卸载 Patch
                            //（该方式不需要YanCore作为支持，直接绿皮全开全关）
                            if (Setting.ModEnable.Value)
                            {
                                try
                                {
                                    harmony.PatchAll();
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogFatal("尝试加载Harmony补丁时出现异常");
                                    Logger.LogFatal(ex);
                                }
                            }
                            else
                            {
                                try
                                {
                                    harmony.UnpatchAll();
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogFatal("尝试卸载Harmony补丁时出现异常");
                                    Logger.LogFatal(ex);
                                }
                            }

                            RecoverUiChange.RecoverAll(Setting.ModEnable.Value);

                            tg.Text = Setting.ModEnable.Value ? "MOD 已开启" : "MOD 已关闭";

                            UIAngel.UIShowHide("全体显示关闭", Setting.ModEnable.Value);

                            ////遍历本组件（tg）的父组件（Parent）中的所有子组件（Children）【即可以理解为同级组件】
                            //foreach (UnityUIKit.Core.ManagedGameObject managedGameObject in tg.Parent.Children)
                            //{
                            //    //改变特定组件的显示、隐藏
                            //    if (managedGameObject.Name == "全体显示关闭")
                            //    {
                            //        //切换组件的显示/隐藏“SetActive(value)”
                            //        managedGameObject.SetActive(Setting.ModEnable.Value);
                            //        break;
                            //    }
                            //}
                        },
                        //设定该元素的首选宽度、首选高度。
                        //边框留白大约12.5，上下合计 或 左右合计 留白大约25。（游戏中MOD设置UI中）单个文字长宽约25。
                        //例比如1个文字，建议设为{ 50, 50 }；6个文字，建议设为{ 175, 50 } (宽度为左右留白25 + 文字宽度25 x 文字字数6)
                        //如果想要 宽度, 高度 自适应，则将对应项设为0即可。除了TaiwuToggle()类，不建议两项皆设为0（可能会导致外边框不显示？我不确定）
                        Element = { PreferredSize = { 0, 50 } },
                        //粗体
                        UseBoldFont = true,
                    },
                    //【自适应垂直UI组：设定全体（装进来，方便开启和关闭显示）】
                    new BoxAutoSizeModelGameObject()
                    {
                        Name = "全体显示关闭",
                        //该组件内的子组件排布设定
                        Group =
                        {
                            //子组件垂直排布
                            Direction = UnityUIKit.Core.Direction.Vertical,
                            //子组件排布间隔
                            Spacing = 10,
                        },
                        //默认按照MOD总开关的值来确定是显示还是关闭
                        //（不加的话：若先将MOD设为关、然后退出重进，能看到虽然MOD启用按钮是灰色的，但下面的菜单却显示出来了）
                        DefaultActive = Setting.ModEnable.Value,
                        //大小自适应设置
                        SizeFitter =
                        {
                            //垂直高度自适应
                            VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                        },
                        Children =
                        {
                            //【标签：选项设置 分隔用】
                            new Container()
                            {
                                //不建议用设定 Name 的方式来辅助记忆，如果不需要调用操作的话，那组件就不用设定 Name 属性
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：选项设置 分隔用】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>选项设置</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 0, 50 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：Debug模式开关】
                                    new TaiwuToggle()
                                    {
                                        Text = "Debug模式开关",
                                        isOn = Setting.debugMode.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "用于输出Debug信息，一般无需开启",
                                        Element = { PreferredSize = { 200 } },
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.debugMode.Value = value;
                                        }
                                    },

                                }
                            },
                            //【水平UI组：Bug修复】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：Bug修复】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>Bug修复</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：新显性人物特性的健康修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "修复健康缺损BUG",
                                        isOn = Setting.BugFixNewHealthyFeatureCauseHealthNotFull.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修复“未成年者在年龄增长、揭示更多特性时，新揭示的特性若增加了人物健康上限、人物的当前健康值不会随之增加”的BUG",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.BugFixNewHealthyFeatureCauseHealthNotFull.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：战斗中防止功法按钮超出屏幕修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "修复功法图标出界",
                                        isOn = Setting.BugFixGongFaIconOverScreen.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修复当装备的功法数量过多时，战斗中的功法图标会超出显示范围、导致无法使用的BUG",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.BugFixGongFaIconOverScreen.Value = value;

                                            if (!Setting.BugFixGongFaIconOverScreen.Value)
                                            {
                                                RecoverUiChange.RecoverBattleGongFaUi();
                                            }
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：修复石牢静坐传送BUG】
                                    new TaiwuToggle()
                                    {
                                        Text = "修复石牢静坐BUG",
                                        isOn = Setting.BugFixMoveToPrisonIncorrectPlace.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修复当未开启元山地点的驿站时，太吾被石牢静坐会被传送至地图边缘的BUG",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.BugFixMoveToPrisonIncorrectPlace.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：修复无影令会带入被暗杀者墓中的BUG】
                                    new TaiwuToggle()
                                    {
                                        Text = "修复无影令BUG",
                                        isOn = Setting.BugFixWuYinLingNotDisappear.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修复持有无影令的NPC被暗杀后，无影令不会消失、而会带入该NPC墓中（然后可能被其他NPC盗墓获得、导致持续死人）的BUG",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.BugFixWuYinLingNotDisappear.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：修复武林大会金刚宗灌顶不会随机到定力的BUG】
                                    new TaiwuToggle()
                                    {
                                        Text = "修复金刚宗灌顶BUG",
                                        isOn = Setting.BugFixEvent20979WrongRandowRange.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修复武林大会事件的金刚宗灌顶不会随机到定力的BUG",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.BugFixEvent20979WrongRandowRange.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：追加显示信息】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：追加显示信息】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>追加显示信息</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：显示地格ID】
                                    new TaiwuToggle()
                                    {
                                        Text = "地格ID",
                                        isOn = Setting.DisplayPlaceId.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在地点信息中，额外显示地格对应的地格ID\n（同时人物属性页面追加地点信息）\n以便精准定位人物所在位置\n\n注：游戏中采用地区ID（partId）、地格ID（placeId）来记录地点。\n其中地格ID（placeId）由于没有显示具体数值、且对应的地格名称常常是重复的、导致极其难以确认其所对应的具体地格。\n\n部分MOD利用地图边长来计算出地格ID对应的地图横纵坐标，但使用起来需要换算也很麻烦（对于MOD制作者来说）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayPlaceId.Value = value;
                                        },
                                        Element = { PreferredSize = { 75, 50 } }
                                    },
                                    //【开关：显示入魔标记】
                                    new TaiwuToggle()
                                    {
                                        Text = "入魔标记",
                                        isOn = Setting.DisplayXxMadOnName.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在地点人物、人物名册界面（游戏原生的人物搜索功能，按钮在右下）的人物名字一栏，显示入魔标记\n（地点人物中处于远距离的入魔者也会显示标记）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayXxMadOnName.Value = value;
                                        },
                                        Element = { PreferredSize = { 75, 50 } }
                                    },
                                    //【开关：显示逆练等级】
                                    new TaiwuToggle()
                                    {
                                        Text = "逆练等级",
                                        isOn = Setting.DisplayActorGongFaBadFLevel.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在功法/书籍的浮动信息窗口以及修习界面中，显示太吾该功法的心法已读逆练等级（不含技艺补全）与技艺补全等级，方便判断如何达成正逆练",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayActorGongFaBadFLevel.Value = value;
                                        },
                                        Element = { PreferredSize = { 75, 50 } }
                                    },
                                    //【开关：显示内息紊乱具体数值修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "内息紊乱数值",
                                        isOn = Setting.DisplayActorMianQiNumber.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在人物伤势界面的内息部分中，显示内息紊乱具体数值，方便选择最适宜的内息药",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayActorMianQiNumber.Value = value;

                                            if (!Setting.DisplayActorMianQiNumber.Value)
                                            {
                                                RecoverUiChange.RecoverMianQiText();
                                            }
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：显示商人所属商队】
                                    new TaiwuToggle()
                                    {
                                        Text = "商人所属商队",
                                        isOn = Setting.DisplayMerchantType.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "所有显示势力身份的地方，若为商人会追加显示商人所属商队\n（地点人物中处于远距离的商人也会显示其所属商队）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayMerchantType.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：显示对话人物势力身份】
                                    new TaiwuToggle()
                                    {
                                        Text = "对话人物身份",
                                        isOn = Setting.DisplayDialogActorIdentity.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "和人物对话时，在其名字下方显示其所属势力与身份信息（以免忘记找NPC对话原本是想要做什么事）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayDialogActorIdentity.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：显示相枢被动能力】
                                    new TaiwuToggle()
                                    {
                                        Text = "相枢被动能力",
                                        isOn = Setting.DisplayXXPassiveInActorMenu.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在查看人物界面的功法一览（内功）下，显示相枢爪牙/剑冢人物的特殊被动。\n方便在战前调整配置（战前点击敌方人物来查看相应信息）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayXXPassiveInActorMenu.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：显示地格特殊人物数量】
                                    new TaiwuToggle()
                                    {
                                        Text = "地格特殊人数",
                                        isOn = Setting.DisplayPlaceSpecialPersonNum.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在地图的地格上，额外显示地点中商人与入魔者的数量",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DisplayPlaceSpecialPersonNum.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：UI快捷操作】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：UI快捷操作】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>UI快捷操作</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：ESC对话快捷键】
                                    new TaiwuToggle()
                                    {
                                        Text = "ESC快捷键",
                                        isOn = Setting.UiEscQuickAction.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "大概率与“键盘快捷键MOD”冲突，若已使用“键盘快捷键MOD”，请关闭本项！\n\n在对话事件中，允许使用ESC键来选择“单选项”或者部分能够“结束对话”、“返回上一级”的选项\n\n也允许使用ESC键关闭“产业建筑视图”、“战斗/较艺结束界面”、“事件记录”",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiEscQuickAction.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：快速装备已装备的功法】
                                    new TaiwuToggle()
                                    {
                                        Text = "快速装备已装备的功法",
                                        isOn = Setting.UiQuickChangeEquipedGongFa.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "装备功法界面中，允许装备已装备的功法\n（会先卸除、再装备，用于调整装备位置）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiQuickChangeEquipedGongFa.Value = value;

                                            RecoverUiChange.RefreshEquipedGongFaButtonInteractable();
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：快速切换修习技能】
                                    new TaiwuToggle()
                                    {
                                        Text = "快速切换修习技能",
                                        isOn = Setting.UiQuickChangeStudySkill.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "修习/突破/研读界面，按下移除技能/书籍按钮后，自动打开选择新技能/书籍的界面",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiQuickChangeStudySkill.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：快速切换修习技能】
                                    new TaiwuToggle()
                                    {
                                        Text = "快速认输",
                                        isOn = Setting.UiQuickSurrender.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在较艺、切磋战、接招战中，点击画面中的己方这边的白旗图标可以快速认输",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiQuickSurrender.Value = value;
                                            RecoverUiChange.SetQuickSurrenderButton(Setting.UiQuickSurrender.Value);
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：记忆不再显示】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：记忆不再显示】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>记忆不再显示</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：不显示版本资讯】
                                    new TaiwuToggle()
                                    {
                                        Text = "不显示版本资讯",
                                        isOn = Setting.RememberDontShowWelcomeDialog.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "记忆不显示开始游戏时的版本资讯弹窗",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.RememberDontShowWelcomeDialog.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：不显示较艺提问方】
                                    new TaiwuToggle()
                                    {
                                        Text = "不显示较艺提问方",
                                        isOn = Setting.RememberDontShowSkillQusetionSide.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "记忆不显示较艺时的提问方提示弹窗",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.RememberDontShowSkillQusetionSide.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：不显示走火入魔】
                                    new TaiwuToggle()
                                    {
                                        Text = "不显示走火入魔",
                                        isOn = Setting.RememberDontShowStudyMad.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "记忆不显示突破超出步数时的走火入魔提示",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.RememberDontShowStudyMad.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：新增显示UI】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：新增显示UI】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>新增显示UI</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：增加地点建筑UI】
                                    new TaiwuToggle()
                                    {
                                        Text = "增加地点奇遇/建筑UI",
                                        isOn = Setting.UiAddPlaceBuildUI.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在屏幕右侧显示选择地点的奇遇与可互动建筑，若所选地点为当前地点可直接点击打开建筑\n\n（注：由于建筑窗口会对产业地图进行更新，为避免报错、打开建筑前会先开启产业地图系统、因此打开建筑会有卡顿现象）",
                                        Element = { PreferredSize = { 0, 50 } },
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiAddPlaceBuildUI.Value = value;
                                            if (Setting.UiAddPlaceBuildUI.Value)
                                            {
                                                if (Main.IsInGame())
                                                {
                                                    if (InGameBuildingUI == null)
                                                    {
                                                        InGameBuildingUI = new PlaceHomeBuildingUI();
                                                    }
                                                    Main.InGameBuildingUI.AttachUI(true);
                                                }
                                            }
                                            else
                                            {
                                                PlaceHomeBuildingUI.DetachUI();
                                            }
                                            //RecoverUiChange.SetPlaceBuildingUiActive(Setting.UiAddPlaceBuildUI.Value);
                                        },
                                    },
                                    //【开关：增加人物关注按钮】
                                    new TaiwuToggle()
                                    {
                                        Text = "增加人物关注按钮",
                                        isOn = Setting.UiPinActors.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在地点人物、人物名册界面（游戏原生的人物搜索功能，按钮在右下）中，加入人物关注按钮\n已关注人物的将置顶显示（方便查找）（另外人物名册界面中可显示所有已关注人物——包含已故人物）\n\n<color=#E4504DFF>重要！！</color>本功能关闭或本MOD关闭后，请重启游戏、不然会有按钮UI残留\n\n关注人物对应存档栏位、而不对应具体存档，数据记录在MOD配置文件中（不会向存档中额外写入数据，以免坏档）",
                                        Element = { PreferredSize = { 0, 50 } },
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiPinActors.Value = value;
                                            RecoverUiChange.SetPinActorToggleActive(Setting.UiPinActors.Value);
                                        },
                                    },
                                    //【开关：增加商人提示按钮】
                                    new TaiwuToggle()
                                    {
                                        Text = "增加商人提示按钮",
                                        isOn = Setting.UiOnlyMerchantToggle.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在地点人物界面右上加入商人提示按钮，点击后地点人物切换为仅显示商人\n（仅在地点有商人时出现）",
                                        Element = { PreferredSize = { 0, 50 } },
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiOnlyMerchantToggle.Value = value;
                                            RecoverUiChange.SetPlaceMerchantButtonActive(Setting.UiOnlyMerchantToggle.Value);
                                        },
                                    },
                                    //【开关：简略人物信息扩展】
                                    new TaiwuToggle()
                                    {
                                        Text = "人物名册信息扩展",
                                        isOn = Setting.UiMouseActorSimpleAdditionInfo.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "在人物名册界面（游戏原生的人物搜索功能，按钮在右下）的浮动信息窗口中，追加更多信息（称号、好感、健康、所在地点）",
                                        Element = { PreferredSize = { 0, 50 } },
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.UiMouseActorSimpleAdditionInfo.Value = value;
                                            if (!Setting.UiMouseActorSimpleAdditionInfo.Value)
                                            {
                                                RecoverUiChange.RecoverMouseActorSimpleAdditionInfo();
                                            }
                                        },
                                    },
                                }
                            },
                            //【标签：会影响平衡性的功能 分隔用】
                            new Container()
                            {
                                //不建议用设定 Name 的方式来辅助记忆，如果不需要调用操作的话，那组件就不用设定 Name 属性
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：选项设置 分隔用】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>会影响平衡性的功能</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 0, 50 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                }
                            },
                            //【水平UI组：行动力优化】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：行动力优化】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>行动力优化</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：触发童颜功法时的行动力修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "触发童颜功法时的行动力优化",
                                        isOn = Setting.DayTimeKeepYouthGongFaGetNewMaximum.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "成年太吾（年龄>14）完整触发童颜的功法（太阴一明珏/洗髓经）的正逆练特效时，每月的最大行动力按18岁的行动力计算\n\n已知冲突：“太吾修改器MOD”的“行动力不减”无法享受本功能的影响（涉及线程问题无法修复——具体来说硬是要兼容也是可以的，但太繁琐了）",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DayTimeKeepYouthGongFaGetNewMaximum.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：代步提供移动耗时削减修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "代步提供移动耗时削减优化",
                                        isOn = Setting.DayTimeEqiupRideReduceMoveCost.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "依照太吾装备的代步品级提供小地图移动耗时削减\n\n九品到一品耗时削减幅度：10% ～ 50% 每一品阶多5%",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.DayTimeEqiupRideReduceMoveCost.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：讨要事件优化】
                            new Container()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Horizontal,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                Element =
                                {
                                    //设定首选高度为50。
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    //【标签：讨要事件优化】
                                    new TaiwuLabel()
                                    {
                                        //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                        Text = "<color=#E1CDAA>讨要事件优化</color>",
                                        //设定首选高度为50。
                                        Element = { PreferredSize = { 150 } },
                                        //粗体
                                        UseBoldFont = true,
                                        UseOutline = true,
                                    },
                                    //【开关：拒绝NPC讨要时的惩罚修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "减弱拒绝惩罚",
                                        isOn = Setting.NpcAskingEventReduceRefusePenalty.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "拒绝NPC的讨要时，\n减少好感度降为原先的5%（1/20）、\n结仇几率降为原先的20%（1/5）\n以便太吾为非仁善性格时，也能有较优的体验",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.NpcAskingEventReduceRefusePenalty.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                    //【开关：不再发生NPC讨要事件修正】
                                    new TaiwuToggle()
                                    {
                                        Text = "不再发生NPC讨要事件",
                                        isOn = Setting.NpcAskingEventWillNotHappen.Value,
                                        TipTitle = "功能说明",
                                        TipContant = "不会发生讨要事件\n避免过月后被打扰",
                                        //当数值改变时（开关按钮）
                                        onValueChanged = (bool value, Toggle tg) =>
                                        {
                                            Setting.NpcAskingEventWillNotHappen.Value = value;
                                        },
                                        Element = { PreferredSize = { 0, 50 } }
                                    },
                                }
                            },
                            //【水平UI组：NPC过月获取威望】
                            new BoxAutoSizeModelGameObject()
                            {
                                Group =
                                {
                                    //子组件垂直排布
                                    Direction = UnityUIKit.Core.Direction.Vertical,
                                    //子组件排布间隔
                                    Spacing = 10,
                                },
                                //大小自适应设置
                                SizeFitter =
                                {
                                    //垂直高度自适应
                                    VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                                },
                                Children =
                                {
                                    //【水平UI组：NPC过月获取威望 开关】
                                    new Container()
                                        {
                                            Group =
                                            {
                                                //子组件垂直排布
                                                Direction = UnityUIKit.Core.Direction.Horizontal,
                                                //子组件排布间隔
                                                Spacing = 10,
                                            },
                                            Element =
                                            {
                                                //设定首选高度为50。
                                                PreferredSize = { 0 , 50 }
                                            },
                                            Children =
                                            {
                                                //【标签：NPC过月获取威望】
                                                new TaiwuLabel()
                                                {
                                                    //标签文本，采用<color=#E1CDAA>较亮</color><color=#998877>柔和</color>两种文本颜色。默认的颜色太暗了
                                                    Text = "<color=#E1CDAA>NPC获取威望</color>",
                                                    //设定首选高度为50。
                                                    Element = { PreferredSize = { 150 } },
                                                    //粗体
                                                    UseBoldFont = true,
                                                    UseOutline = true,
                                                },
                                                //【开关：NPC过月行动获取威望的修正】
                                                new TaiwuToggle()
                                                {
                                                    Text = "开启NPC过月获取威望",
                                                    isOn = Setting.NpcGetPrestigeIsPossiable.Value,
                                                    TipTitle = "功能说明",
                                                    TipContant = "模拟NPC在过月时、（像获取其他资源那样）能够尝试自行获取威望\n以便解决NPC普遍威望较少的问题",
                                                    //当数值改变时（开关按钮）
                                                    onValueChanged = (bool value, Toggle tg) =>
                                                    {
                                                        Setting.NpcGetPrestigeIsPossiable.Value = value;

                                                        UIAngel.UIShowHide("NPC获取威望详细设置", Setting.NpcGetPrestigeIsPossiable.Value);

                                                        ////遍历本组件（tg）的父组件（Parent）的父组件（Parent）中的所有子组件（Children）
                                                        //foreach (UnityUIKit.Core.ManagedGameObject managedGameObject in tg.Parent.Parent.Children)
                                                        //{
                                                        //    //改变特定组件的显示、隐藏
                                                        //    if (managedGameObject.Name == "NPC获取威望详细设置")
                                                        //    {
                                                        //        //切换组件的显示/隐藏“SetActive(value)”
                                                        //        managedGameObject.SetActive(Setting.enableNpcGetPrestigeFix.Value);
                                                        //        break;
                                                        //    }
                                                        //}
                                                    },
                                                    Element = { PreferredSize = { 0, 50 } }
                                                },
                                            }
                                        },
                                    //【水平UI组：NPC过月获取威望 进一步设置】
                                    new Container()
                                        {
                                            Name = "NPC获取威望详细设置",
                                            Group =
                                            {
                                                //子组件垂直排布
                                                Direction = UnityUIKit.Core.Direction.Horizontal,
                                                //子组件排布间隔
                                                Spacing = 10,
                                            },
                                            Element =
                                            {
                                                //设定首选高度为50。
                                                PreferredSize = { 0 , 50 }
                                            },
                                            //依照NPC过月获取威望开关的值来设定默认显示状态
                                            DefaultActive = Setting.NpcGetPrestigeIsPossiable.Value,
                                            Children =
                                            {
                                                //【开关：NPC尝试获取威望时是否会导致地格安定下降】
                                                new TaiwuToggle()
                                                {
                                                    Text = "获取威望是否会导致地格安定变动",
                                                    isOn = Setting.NpcGetPrestigeWillReducePlaceStability.Value,
                                                    TipTitle = "功能说明",
                                                    TipContant = "开启后NPC在获取威望时可能会导致地格安定变动（根据NPC处事立场进行一系列判定）\n\n这样安定负相关的建筑也有用武之地了",
                                                    //当数值改变时（开关按钮）
                                                    onValueChanged = (bool value, Toggle tg) =>
                                                    {
                                                        Setting.NpcGetPrestigeWillReducePlaceStability.Value = value;
                                                    },
                                                    Element = { PreferredSize = { 0, 50 } }
                                                },
                                            }
                                        },
                                    }
                            },
                        }
                    },
                }
            };
        }

        /// <summary>
        /// 判断是否在游戏中
        /// </summary>
        /// <returns></returns>
        public static bool IsInGame()
        {
            bool result = false;
            if (DateFile.instance != null && Characters.HasChar(DateFile.instance.MianActorID()))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 游戏实时更新（可用来检测按键输入等，Unity组件的功能）
        /// </summary>
        private void Update()
        {
            //检测ESC按键
            if (Input.GetKeyDown(KeyCode.Escape) && Main.Setting.UiEscQuickAction.Value)
            {
                //产业建筑界面
                if (UIManager.Instance.curState == UIState.HomeSystem && HomeSystemWindow.Instance.gameObject.activeInHierarchy)
                {
                    //ESC关闭产业建筑界面中的技艺查阅界面
                    if (HomeSystemWindow.Instance.skillView.activeInHierarchy)
                    {
                        HomeSystemWindow.Instance.ShowSkillView();
                    }
                    //ESC关闭产业建筑界面
                    else
                    {
                        GameObject BuildingWindowGO = GameObject.Find("UIRoot/Canvas/UIPopup/BuildingWindow");
                        GameObject TipsWindowGO = GameObject.Find("UIRoot/Canvas/UIPopup/TipsWindow");

                        GameObject ui_ManPowerManageGO = GameObject.Find("UIRoot/Canvas/UIPopup/ui_ManPowerManage");
                        GameObject ui_TurnChangeGO = GameObject.Find("UIRoot/Canvas/UIPopup/ui_TurnChange");
                        GameObject ui_TaiwuLegacyGO = GameObject.Find("UIRoot/Canvas/UIPopup/ui_TaiwuLegacy");
                        GameObject ui_InscriptionGO = GameObject.Find("UIRoot/Canvas/UIWindow/ui_Inscription");
                        GameObject ui_SystemSettingGO = GameObject.Find("UIRoot/Canvas/UIPopup/ui_SystemSetting");

                        bool isOtherWindowExist = (BuildingWindowGO != null && BuildingWindowGO.activeInHierarchy)
                            || (TipsWindowGO != null && TipsWindow.instance.tipsReadWindow.activeInHierarchy)
                            || (ui_ManPowerManageGO != null && ui_ManPowerManageGO.activeInHierarchy)
                            || (ui_TurnChangeGO != null && ui_TurnChangeGO.activeInHierarchy)
                            || (ui_TaiwuLegacy.Exists && ui_TaiwuLegacyGO.activeInHierarchy)
                            || (ui_InscriptionGO != null && ui_InscriptionGO.activeInHierarchy)
                            || (ui_SystemSettingGO != null && ui_SystemSettingGO.activeInHierarchy);

                        if (!isOtherWindowExist)
                        {
                            UIState.HomeSystem.Back();
                        }
                    }
                }

                //ESC关闭战斗结算界面
                //if (BattleEndWindow.Exists && BattleEndWindow.instance.closeBattleEndWindowButton.gameObject.activeInHierarchy)
                if (UIManager.Instance.curState == UIState.BattleSystem && BattleEndWindow.instance.closeBattleEndWindowButton.gameObject.activeInHierarchy)
                {
                    BattleEndWindow.instance.closeBattleEndWindowButton.onClick.Invoke();
                }

                //ESC关闭较艺结算界面
                //if (SkillBattleSystem.Exists && SkillBattleSystem.instance.closeBattleButton.activeInHierarchy)
                if (UIManager.Instance.curState == UIState.SkillBattle && SkillBattleSystem.instance.closeBattleButton.activeInHierarchy)
                {
                    SkillBattleSystem.instance.closeBattleButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                }
            }
        }
    }
}
