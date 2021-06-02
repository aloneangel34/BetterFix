# Taiwu_mods_BIE
# 太吾绘卷游戏Mod BepInEx

for The Scroll Of Taiwu v0.2.8.4
对应游戏版本 v0.2.8.4

# BetterFix / 优化加强

# Functions / 功能：
## 游戏BUG修复：
1. 修复健康缺损BUG：修复“未成年者在年龄增长、揭示更多特性时，新揭示的特性若增加了人物健康上限、人物的当前健康值不会随之增加”的BUG
1. 修复功法图标出界：修复当装备的功法数量过多时，战斗中的功法图标会超出显示范围、导致无法使用的BUG
1. 修复石牢静坐BUG：修复当未开启元山地点的驿站时，太吾被石牢静坐会被传送至地图边缘的BUG
1. 修复无影令BUG：修复持有无影令的NPC被暗杀后，无影令不会消失、而会带入该NPC墓中(然后可能被其他NPC盗墓获得、导致持续死人)的BUG
1. 修复金刚宗灌顶BUG：修复武林大会金刚宗灌顶不会随机到定力的BUG

## 追加显示信息：
1. 显示地格ID：在地点信息中，额外显示地格对应的地格ID(同时在人物属性页面追加地点信息)以便精准定位人物所在位置
1. 显示入魔标记：在地点人物、人物名册界面的人物名字一栏，显示入魔标记(地点人物中处于远距离的入魔者也会显示标记)
1. 显示逆练等级：在功法/书籍的浮动信息窗口以及修习界面中，显示太吾该功法的心法已读逆练等级(不含技艺补全)与技艺补全等级，方便判断如何达成正逆练
1. 显示内息紊乱数值：在人物伤势界面的内息部分中，显示内息紊乱具体数值，方便选择最适宜的内息药
1. 显示商人所属商队：所有显示势力身份的地方，若为商人会追加显示商人所属商队(地点人物中处于远距离的商人也会显示其所属商队)
1. 显示对话人物身份：和人物对话时，在其名字下方显示其所属势力与身份信息(以免忘记找NPC对话原本是想要做什么事)
1. 显示相枢被动能力：在查看人物界面的功法一览(内功)下，显示相枢爪牙/剑冢人物的特殊被动。方便在战前调整配置(战前点击敌方人物来查看相应信息)
1. 显示地格特殊人数：在地图的地格上，额外显示地点中商人与入魔者的数量

## UI快捷操作：
1. ESC快捷键：在对话事件中，允许使用ESC键来选择“单选项”或者部分能够“结束对话”、“返回上一级”的选项。也允许使用ESC键关闭“产业建筑视图”、“战斗/较艺结束界面”、“事件记录”
1. 快速装备已装备的功法：装备功法界面中，允许装备已装备的功法(会先卸除、再装备，用于调整装备位置)
1. 快速切换修习技能：修习/突破/研读界面，按下移除技能/书籍按钮后，自动打开选择新技能/书籍的界面
1. 快速认输：在较艺、切磋战、接招战中，点击画面中的己方这边的白旗图标可以快速认输

## 记忆不显示：
1. 不显示版本资讯：记忆不显示开始游戏时的版本资讯弹窗
1. 不显示较艺提问方：记忆不显示较艺时的提问方提示弹窗
1. 不显示走火入魔：记忆不显示突破超出步数时的走火入魔提示

## 新增显示UI：
1. 增加地点奇遇/建筑UI：在屏幕右侧显示选择地点的奇遇与可互动建筑，若所选地点为当前地点可直接点击打开建筑
1. 增加人物关注按钮：在地点人物、人物名册界面中，加入人物关注按钮。已关注人物的将置顶显示(方便查找)(另外人物名册界面中可显示所有已关注人物——包含已故人物)
1. 增加商人提示按钮：在地点人物界面右上加入商人提示按钮，点击后地点人物切换为仅显示商人(仅在地点有商人时出现)
1. 人物名册信息扩展：在人物名册界面(游戏原生的人物搜索功能，按钮在右下)，追加更多信息(称号、好感、健康、所在地点)

!---- 会影响平衡的功能(默认关闭，如有需要请自行开启) ----!

## 行动力优化：
1. 触发童颜功法时的行动力优化：成年太吾(年龄>14)完整触发童颜的功法(太阴一明珏/洗髓经)的正逆练特效时，每月的最大行动力按18岁的行动力计算
1. 代步提供移动耗时削减：依照太吾装备的代步品级提供小地图移动耗时削减。九品到一品耗时削减幅度：10%～50%，每一品阶多5%

## 讨要事件优化：
1. 减弱拒绝惩罚：拒绝NPC的讨要时，减少好感度降为原先的5%(1/20)、结仇几率降为原先的20%(1/5)。以便太吾为非仁善性格时，也能有较优的体验
1. 不再发生NPC讨要事件：不会发生讨要事件，避免过月后被打扰

## NPC过月获取威望：
1. 开启NPC过月获取威望：模拟NPC在过月时、(像获取其他资源那样)能够尝试自行获取威望。以便解决NPC普遍威望较少的问题
1. 获取威望是否会导致地格安定变动：开启后NPC在获取威望时可能会导致地格安定变动(根据NPC处事立场进行一系列判定)这样安定负相关的建筑也有用武之地了

# Release Site / 发布地址:
https://bbs.nga.cn/read.php?tid=27022629

# Dependency Mod / 前置MOD:
* YanCore 1.5.1.1

# UpdataLog / 更新日志:

## v1.0.0
* 最初发布版本

# MOD冲突 / 兼容性
* 尽量避免将BIE mod和UMM mod(UMMLoader)混用，以避免性能损失和可能的冲突。
(若发现本MOD失效，请参看本帖开头的“若发现本MOD失效的自助解决步骤：”)

1. “键盘快捷键MOD”大概率与本MOD的“ESC快捷键”功能冲突，若已使用“键盘快捷键MOD”，请关闭本MOD的“ESC键对话快捷键”功能
1. “太吾修改器MOD”的“行动力不减”功能不受本MOD的“触发童颜功法时的行动力优化”功能的影响(涉及线程问题无法修复——具体来说硬是要兼容也是可以的，但太繁琐了)

# MOD已知问题

* Q1:在点击地点建筑的图标打开建筑窗口时有卡顿现象
* A:由于建筑窗口内的操作会对产业地图进行更新，为避免报错、打开建筑前会先开启产业地图系统、因此打开建筑会有卡顿现象
个人希望官方在正式版中修改一下代码，允许在产业建筑地图以外的地方打开建筑窗口(此时不更新产业建筑系统内的图标)，这样就能减少资源消耗、避免卡顿

* Q2:已关注人物对不上(比如“为啥我新开了一个档，还有已关注人物啊？”)
* A:本MOD的“已关注人物列表”仅对应存档栏位(读取界面的 左、中、右 三个栏位)，而不对应具体的存档
“已关注人物列表”的数据保存在MOD的配置文件里，而不写入游戏存档(我个人技术不足，向游戏存档额外写入数据我怕会产生怀挡)
所以请期待官方在正式版中加入原生的关注人物功能

* Q3:把"关注人物按钮"功能关闭或者关闭整个MOD之后，关注按钮还残留在画面上是怎么回事？
* A:推荐在关闭"关注人物按钮"功能或关闭本MOD后，重启一次游戏、以解决UI残留问题
(毕竟这不是官方原生的功能，是补丁额外加入的，要处理起来很麻烦。所以还是那句话：请期待官方在正式版中加入原生的关注人物功能)

* Q4:为什么地点奇遇/建筑UI上显示的剑冢难度与实际不符？
* A:本MOD显示的剑冢难度“剑X”是按太吾村地图上剩余的剑冢奇遇数量来判定的，
若使用MOD创建了剑冢奇遇、或先前偷取了太多精纯却没把剑冢打掉，则显示难度会与实际难度不符