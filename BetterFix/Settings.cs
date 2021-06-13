using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace BetterFix
{
    /// <summary>MOD设置选项</summary>
    public class Settings
    {
        //通用设置项
        /// <summary>总开关：MOD启用与否</summary>
        public ConfigEntry<bool> ModEnable;
        /// <summary>开关：Debug模式开关</summary>
        public ConfigEntry<bool> debugMode;
        /// <summary>字符串：许可声明</summary>
        public ConfigEntry<string> modClaimInfo;

        //MOD个性化设置项
        //【BUG修复】
        /// <summary>开关：新揭示人物特性的健康修正</summary>   CLEAR!
        public ConfigEntry<bool> BugFixNewHealthyFeatureCauseHealthNotFull;
        /// <summary>开关：战斗中防止功法按钮超出屏幕修正</summary>   CLEAR!
        public ConfigEntry<bool> BugFixGongFaIconOverScreen;
        /// <summary>开关：修复石牢静坐传送BUG</summary>   CLEAR!
        public ConfigEntry<bool> BugFixMoveToPrisonIncorrectPlace;
        /// <summary>开关：修复无影令会带入被暗杀者墓中的BUG</summary>   CLEAR!
        public ConfigEntry<bool> BugFixWuYinLingNotDisappear;
        /// <summary>开关：修复武林大会金刚宗灌顶不会随机到定力的BUG</summary>
        public ConfigEntry<bool> BugFixEvent20979WrongRandowRange;
        /// <summary>开关：修复抓捕人物的父系母系血统不匹配的BUG</summary>
        public ConfigEntry<bool> BugFixCatchActorBloodInfoMissmatch;

        //【追加显示信息】
        /// <summary>开关：显示内息紊乱具体数值</summary>   CLEAR!
        public ConfigEntry<bool> DisplayActorMianQiNumber;
        /// <summary>开关：显示心法逆练等级</summary>   CLEAR!
        public ConfigEntry<bool> DisplayActorGongFaBadFLevel;
        /// <summary>开关：显示商人所属商队</summary>   CLEAR!
        public ConfigEntry<bool> DisplayMerchantType;
        /// <summary>开关：显示入魔标记</summary>   CLEAR!
        public ConfigEntry<bool> DisplayXxMadOnName;
        /// <summary>开关：显示对话对象的身份</summary>   CLEAR!
        public ConfigEntry<bool> DisplayDialogActorIdentity;
        /// <summary>开关：显示相枢被动能力</summary>   CLEAR!
        public ConfigEntry<bool> DisplayXXPassiveInActorMenu;
        /// <summary>开关：显示地格特殊人物数量</summary>   CLEAR!
        public ConfigEntry<bool> DisplayPlaceSpecialPersonNum;
        /// <summary>开关：显示地格ID</summary>   CLEAR!
        public ConfigEntry<bool> DisplayPlaceId;

        //TODO: （鸽了）功法无法被闪避（不判定）的部分，修正显示文本为不判定，而非闪避

        //【UI快捷操作】
        /// <summary>开关：允许使用ESC键选择部分对话选项</summary>   CLEAR!
        public ConfigEntry<bool> UiEscQuickAction;
        /// <summary>开关：快速切换已装备功法的位置</summary>   CLEAR!
        public ConfigEntry<bool> UiQuickChangeEquipedGongFa;
        /// <summary>开关：快速切换修习/突破/研读的技能</summary>   CLEAR!
        public ConfigEntry<bool> UiQuickChangeStudySkill;
        /// <summary>开关：快速认输（较艺、切磋战、接招战）</summary>   CLEAR!
        public ConfigEntry<bool> UiQuickSurrender;

        //【记忆不显示】
        /// <summary>开关：记忆不显示开始时的版本信息弹窗</summary>   CLEAR!
        public ConfigEntry<bool> RememberDontShowWelcomeDialog;
        /// <summary>开关：记忆不显示较艺开始提示</summary>   CLEAR!
        public ConfigEntry<bool> RememberDontShowSkillQusetionSide;
        /// <summary>开关：记忆不显示走火入魔提示</summary>   CLEAR!
        public ConfigEntry<bool> RememberDontShowStudyMad;
        /// <summary>开关：记忆不显示</summary>
        //public ConfigEntry<bool> RememberDontShow;


        //【新增显示UI】
        /// <summary>开关：屏幕右侧显示地格建筑UI</summary>   CLEAR!
        public ConfigEntry<bool> UiAddPlaceBuildUI;

        /// <summary>开关：增加人物关注按钮</summary>   CLEAR!
        public ConfigEntry<bool> UiPinActors;
        /// <summary>列表：（存档栏位一）关注人物ID列表</summary>   CLEAR!
        public ConfigEntry<List<int>> RecordFirstSaveSlotPinActorIds;
        /// <summary>列表：（存档栏位二）关注人物ID列表</summary>   CLEAR!
        public ConfigEntry<List<int>> RecordSecondSaveSlotPinActorIds;
        /// <summary>列表：（存档栏位三）关注人物ID列表</summary>   CLEAR!
        public ConfigEntry<List<int>> RecordThirdSaveSlotPinActorIds;
        /// <summary>列表：最后一次使用清空功能时备份下的关注人物ID列表</summary>   CLEAR!
        public ConfigEntry<List<int>> LastClearedPinActorIdsBackup;

        /// <summary>开关：显示地点商人功能</summary>   CLEAR!（远程选择地点时，出现商人提示按钮，但不显示具体商人）
        public ConfigEntry<bool> UiOnlyMerchantToggle;

        #region 鸽了
        //TODO: 鸽了 未实现的功能（势力所属商人的本地店铺功能）
        /// <summary>开关：显示地点商人功能启用本地店铺功能（设计思路为：实质远程买卖，但只允许买卖、不允许其他交流）</summary>
        //public ConfigEntry<bool> UiSwitchOnlyMerchantIncludeRemoteShop;
        #endregion

        /// <summary>开关：人物名册信息扩展</summary>   CLEAR!（人物名册界面：游戏原生的人物搜索功能，按钮在右下）
        public ConfigEntry<bool> UiMouseActorSimpleAdditionInfo;

        //【行动力优化】
        /// <summary>开关：触发童颜功法时的行动力修正</summary>   CLEAR!
        public ConfigEntry<bool> DayTimeKeepYouthGongFaGetNewMaximum;
        /// <summary>开关：代步提供移动耗时削减修正</summary>   CLEAR!
        public ConfigEntry<bool> DayTimeEqiupRideReduceMoveCost;

        //【讨要事件优化】
        /// <summary>开关：拒绝NPC讨要时的惩罚修正</summary>   CLEAR!
        public ConfigEntry<bool> NpcAskingEventReduceRefusePenalty;
        /// <summary>开关：不再发生NPC讨要事件修正</summary>   CLEAR!
        public ConfigEntry<bool> NpcAskingEventWillNotHappen;

        //【NPC过月获取威望】
        /// <summary>开关：NPC过月行动获取威望的修正</summary>   CLEAR!
        public ConfigEntry<bool> NpcGetPrestigeIsPossiable;
        /// <summary>开关：NPC尝试获取威望时是否会导致地格安定下降</summary>
        public ConfigEntry<bool> NpcGetPrestigeWillReducePlaceStability;

        /// <summary>Config的属性（公开可读，私有可写）</summary>
        public ConfigFile Config { get; private set; }

        /// <summary>设置选项初始化</summary>
        public void Init(ConfigFile Config)
        {
            //将选项参数接口与配置文件绑定，并填入缺省值
            ModEnable = Config.Bind("BetterFix/优化加强", "enable", true, "【MOD开关】");
            //将选项参数接口与配置文件绑定，并填入缺省值
            debugMode = Config.Bind("BetterFix/优化加强", "debugMode", false, "【Debug模式开关】\n一般无需开启（开启后会拖慢性能）\n若有遭遇BUG，请尽量“先开启此项、再重试一遍遭遇BUG的场景，然后把输出的信息日志LOG提交给作者”");
            //许可协议声明
            modClaimInfo = Config.Bind("BetterFix/优化加强", "许可协议声明", "", "作者：aloneangel34\n许可协议声明：MIT许可\nMOD适配游戏版本：v0.2.8.4");

            //将选项参数接口与配置文件绑定，并填入缺省值
            //因为BepInEx的Mod没有Info.json，所以在这里记录（如果因MOD更新而改动了“描述/第四项参数”的话，“描述”的文字段会自动更新。包括“描述”被用户手动改掉、初始化后也会按这里的来更新。）

            //将选项参数接口与配置文件绑定，并填入缺省值（如果因MOD更新而改动了相应的描述的话，描述类的文字段这些会自动更新）
            //（小括号内的四个参数分别是【选项参数在配置文件里所属标签分类】，【选项参数在配置文件里的Key名称】，【选项参数的缺省值】，【选项参数的描述（如下所示，可以用\n等转义符）】）
            BugFixNewHealthyFeatureCauseHealthNotFull = Config.Bind("BUG修复", nameof(BugFixNewHealthyFeatureCauseHealthNotFull), true, "【修复健康缺损BUG】\n修复“未成年者在年龄增长、揭示更多特性时，新揭示的特性若增加了人物健康上限、人物的当前健康值不会随之增加”的BUG");
            BugFixGongFaIconOverScreen = Config.Bind("BUG修复", nameof(BugFixGongFaIconOverScreen), true, "【修复功法图标出界】\n修复当装备的功法数量过多时，战斗中的功法图标会超出显示范围、导致无法使用的BUG");
            BugFixMoveToPrisonIncorrectPlace = Config.Bind("BUG修复", nameof(BugFixMoveToPrisonIncorrectPlace), true, "【修复石牢静坐BUG】\n修复当未开启元山地点的驿站时，太吾被石牢静坐会被传送至地图边缘的BUG");
            BugFixWuYinLingNotDisappear = Config.Bind("BUG修复", nameof(BugFixWuYinLingNotDisappear), true, "【修复无影令BUG】\n修复持有无影令的NPC被暗杀后，无影令不会消失、而会带入该NPC墓中（然后可能被其他NPC盗墓获得、导致持续死人）的BUG");
            BugFixEvent20979WrongRandowRange = Config.Bind("BUG修复", nameof(BugFixEvent20979WrongRandowRange), true, "【修复金刚宗灌顶BUG】\n修复武林大会金刚宗灌顶不会随机到定力的BUG");
            BugFixCatchActorBloodInfoMissmatch = Config.Bind("BUG修复", nameof(BugFixCatchActorBloodInfoMissmatch), true, "【修复抓捕血统BUG】\n修复抓捕人物的父系母系血统与人物ID不匹配的BUG");

            DisplayActorMianQiNumber = Config.Bind("追加显示信息", nameof(DisplayActorMianQiNumber), true, "【显示内息紊乱数值】\n在人物伤势界面的内息部分中，显示内息紊乱具体数值，方便选择最适宜的内息药");
            DisplayActorGongFaBadFLevel = Config.Bind("追加显示信息", nameof(DisplayActorGongFaBadFLevel), true, "【显示逆练等级】\n在功法/书籍的浮动信息窗口以及修习界面中，显示太吾该功法的心法已读逆练等级（不含技艺补全）与技艺补全等级，方便判断如何达成正逆练");
            DisplayMerchantType = Config.Bind("追加显示信息", nameof(DisplayMerchantType), true, "【显示商人所属商队】\n所有显示势力身份的地方，若为商人会追加显示商人所属商队\n（地点人物中处于远距离的商人也会显示其所属商队）");
            DisplayXxMadOnName = Config.Bind("追加显示信息", nameof(DisplayXxMadOnName), true, "【显示入魔标记】\n在地点人物、人物名册界面（游戏原生的人物搜索功能，按钮在右下）的人物名字一栏，显示入魔标记\n（地点人物中处于远距离的入魔者也会显示标记）");
            DisplayDialogActorIdentity = Config.Bind("追加显示信息", nameof(DisplayDialogActorIdentity), true, "【显示对话人物身份】\n和人物对话时，在其名字下方显示其所属势力与身份信息（以免忘记找NPC对话原本是想要做什么事）");
            DisplayXXPassiveInActorMenu = Config.Bind("追加显示信息", nameof(DisplayXXPassiveInActorMenu), true, "【显示相枢被动能力】\n在查看人物界面的功法一览（内功）下，显示相枢爪牙/剑冢人物的特殊被动。\n方便在战前调整配置（战前点击敌方人物来查看相应信息）");
            DisplayPlaceSpecialPersonNum = Config.Bind("追加显示信息", nameof(DisplayPlaceSpecialPersonNum), true, "【显示地格特殊人数】\n在地图的地格上，额外显示地点中商人与入魔者的数量");
            DisplayPlaceId = Config.Bind("追加显示信息", nameof(DisplayPlaceId), true, "【显示地格ID】\n在地点信息中，额外显示地格对应的地格ID\n（同时人物属性页面追加地点信息）\n以便精准定位人物所在位置\n\n注：游戏中采用地区ID（partId）、地格ID（placeId）来记录地点。\n其中地格ID（placeId）由于没有显示具体数值、且对应的地格名称常常是重复的、导致极其难以确认其所对应的具体地格。\n\n部分MOD利用地图边长来计算出地格ID对应的地图横纵坐标，但使用起来需要换算也很麻烦（对于MOD制作者来说）");

            UiEscQuickAction = Config.Bind("UI快捷操作", nameof(UiEscQuickAction), true, "【ESC快捷键】\n大概率与“键盘快捷键MOD”冲突，若已使用“键盘快捷键MOD”，请关闭本项！\n\n在对话事件中，允许使用ESC键来选择“单选项”或者部分能够“结束对话”、“返回上一级”的选项\n\n也允许使用ESC键关闭“产业建筑视图”、“战斗/较艺结束界面”、“事件记录”");
            UiQuickChangeEquipedGongFa = Config.Bind("UI快捷操作", nameof(UiQuickChangeEquipedGongFa), true, "【快速装备已装备的功法】\n装备功法界面中，允许装备已装备的功法\n（会先卸除、再装备，用于调整装备位置）");
            UiQuickChangeStudySkill = Config.Bind("UI快捷操作", nameof(UiQuickChangeStudySkill), true, "【快速切换修习技能】\n修习/突破/研读界面，按下移除技能/书籍按钮后，自动打开选择新技能/书籍的界面");
            UiQuickSurrender = Config.Bind("UI快捷操作", nameof(UiQuickSurrender), true, "【快速认输】\n在较艺、切磋战、接招战中，点击画面中的己方这边的白旗图标可以快速认输");

            RememberDontShowWelcomeDialog = Config.Bind("记忆不显示", nameof(RememberDontShowWelcomeDialog), true, "【不显示版本资讯】\n记忆不显示开始游戏时的版本资讯弹窗");
            RememberDontShowSkillQusetionSide = Config.Bind("记忆不显示", nameof(RememberDontShowSkillQusetionSide), true, "【不显示较艺提问方】\n记忆不显示较艺时的提问方提示弹窗");
            RememberDontShowStudyMad = Config.Bind("记忆不显示", nameof(RememberDontShowStudyMad), true, "【不显示走火入魔】\n记忆不显示突破超出步数时的走火入魔提示");

            UiAddPlaceBuildUI = Config.Bind("新增显示UI", nameof(UiAddPlaceBuildUI), true, "【增加地点奇遇/建筑UI】\n在屏幕右侧显示选择地点的奇遇与可互动建筑，若所选地点为当前地点可直接点击打开建筑\n\n（注：由于建筑窗口会对产业地图进行更新，为避免报错、打开建筑前会先开启产业地图系统、因此打开建筑会有卡顿现象）");

            UiPinActors = Config.Bind("新增显示UI", nameof(UiPinActors), true, "【增加人物关注按钮】\n在地点人物、人物名册界面（游戏原生的人物搜索功能，按钮在右下）中，加入人物关注按钮\n已关注人物的将置顶显示（人物名册界面中可显示所有已关注人物——包含已故人物）\n\n重要！！本功能关闭或本MOD关闭后，请重启游戏、不然会有按钮UI残留\n\n关注人物对应存档栏位、而不对应具体存档，数据记录在MOD配置文件中（不会向存档中额外写入数据，以免坏档）");
            RecordFirstSaveSlotPinActorIds = Config.Bind("新增显示UI", nameof(RecordFirstSaveSlotPinActorIds), new List<int> { }, "【存档栏位一：关注人物ID列表】\n第一个存档栏位对应的关注人物ID列表");
            RecordSecondSaveSlotPinActorIds = Config.Bind("新增显示UI", nameof(RecordSecondSaveSlotPinActorIds), new List<int> { }, "【存档栏位二：关注人物ID列表】\n第二个存档栏位对应的关注人物ID列表");
            RecordThirdSaveSlotPinActorIds = Config.Bind("新增显示UI", nameof(RecordThirdSaveSlotPinActorIds), new List<int> { }, "【存档栏位三：关注人物ID列表】\n第三个存档栏位对应的关注人物ID列表");
            LastClearedPinActorIdsBackup = Config.Bind("新增显示UI", nameof(LastClearedPinActorIdsBackup), new List<int> { }, "【最后一次使用清空功能时备份下的关注人物ID列表】\n有需要的话可以手动还原");

            UiOnlyMerchantToggle = Config.Bind("新增显示UI", nameof(UiOnlyMerchantToggle), true, "【增加商人提示按钮】\n在地点人物界面右上加入商人提示按钮，点击后地点人物切换为仅显示商人\n（仅在地点有商人时出现）");
            //UiSwitchOnlyMerchantIncludeRemoteShop = Config.Bind("新增显示UI", nameof(UiSwitchOnlyMerchantIncludeRemoteShop), true, "【启用本地店铺功能】\n若该地点为势力所在地，将会查找所有属于该势力但不在该地的商人。允许浏览其物品（但不允许交流，可以理解为商人开的本地店铺）");

            UiMouseActorSimpleAdditionInfo = Config.Bind("新增显示UI", nameof(UiMouseActorSimpleAdditionInfo), true, "【人物名册信息扩展】\n在人物名册界面（游戏原生的人物搜索功能，按钮在右下），追加更多信息（称号、好感、健康、所在地点）");

            DayTimeKeepYouthGongFaGetNewMaximum = Config.Bind("行动力优化", nameof(DayTimeKeepYouthGongFaGetNewMaximum), false, "【触发童颜功法时的行动力优化】\n成年太吾（年龄>14）完整触发童颜的功法（太阴一明珏/洗髓经）的正逆练特效时，每月的最大行动力按18岁的行动力计算\n\n已知冲突：“太吾修改器MOD”的“行动力不减”无法享受本功能的影响（涉及线程问题无法修复——具体来说硬是要兼容也是可以的，但太繁琐了）");
            DayTimeEqiupRideReduceMoveCost = Config.Bind("行动力优化", nameof(DayTimeEqiupRideReduceMoveCost), false, "【代步提供移动耗时削减】\n依照太吾装备的代步品级提供小地图移动耗时削减\n\n九品到一品耗时削减幅度：10% ～ 50% 每一品阶多5%");

            NpcAskingEventReduceRefusePenalty = Config.Bind("讨要事件优化", nameof(NpcAskingEventReduceRefusePenalty), false, "【减弱拒绝惩罚】\n拒绝NPC的讨要时，减少好感度降为原先的5%（1/20）、结仇几率降为原先的20%（1/5）\n以便太吾为非仁善性格时，也能有较优的体验");
            NpcAskingEventWillNotHappen = Config.Bind("讨要事件优化", nameof(NpcAskingEventWillNotHappen), false, "【不再发生NPC讨要事件】\n不会发生讨要事件\n避免过月后被打扰");

            NpcGetPrestigeIsPossiable = Config.Bind("NPC过月获取威望", nameof(NpcGetPrestigeIsPossiable), false, "【开启NPC过月获取威望】\n模拟NPC在过月时、（像获取其他资源那样）能够尝试自行获取威望\n以便解决NPC普遍威望较少的问题");
            NpcGetPrestigeWillReducePlaceStability = Config.Bind("NPC过月获取威望", nameof(NpcGetPrestigeWillReducePlaceStability), false, "【获取威望是否会导致地格安定变动】\n开启后NPC在获取威望时可能会导致地格安定变动（根据NPC处事立场进行一系列判定）\n\n这样安定负相关的建筑也有用武之地了");
            
            //防止读取到非法数值（理论上应该由属性去控制的……但Config好像是一整个，不知道怎么下手去单独为某一项设限）
        }
    }
}
