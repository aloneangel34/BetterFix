using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 若装备功法数量过多，尝试换行显示
    /// </summary>
    [HarmonyPatch(typeof(BattleSystem), "GetActorGongFa")]
    //[HarmonyPatch(typeof(BattleSystem), "ShowBattleWindow")] //更换补丁目标
    public static class GongFaIconOverScreen
    {
        internal static bool IsMoveGongFaHolderAnchoredPositionModified = false;
        internal static bool IsDefGongFaHolderAnchoredPositionModified = false;

        internal static bool IsMoveGongFaHolderSpacingModified = false;
        internal static bool IsDefGongFaHolderSpacingModified = false;

        #region 更换补丁目标
        /*
        /// <summary>
        /// 若装备功法数量过多，在显示战斗窗口调用结束后，调整相关UI设置
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void ShowBattleWindowPostfix(BattleSystem __instance)
        //原方法签名
        //public void ShowBattleWindow(int battleTyp, int actorFirstTyp, int useFirstTyp, int battleLoseTyp, int actorUseItemId, int enemyUseItemId, int enemyTeamId, int changeEnemyObbs, int battleGetExp)
        */
        #endregion
        /// <summary>
        /// 若装备功法数量过多，在战斗开始时获取人物功法调用结束后，调整相关UI设置
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void GetActorGongFaPostfix(BattleSystem __instance)
        //原方法签名
        //private void GetActorGongFa()
        {
            if (Main.Setting.BugFixGongFaIconOverScreen.Value)
            {
                //以__instance势力创建Traverse实例
                Traverse t = Traverse.Create(__instance);
                //获取太吾装备的催破数量
                int actorAttackGongFaCount = (t.Field("actorAttackGongFas").GetValue<Dictionary<int, int[]>>()).Count;
                //获取太吾装备的身法数量
                int actorMoveGongFaCount = (t.Field("actorMoveGongFas").GetValue<Dictionary<int, int[]>>()).Count;
                //获取太吾装备的护体数量
                int actorDefGongFaCount = (t.Field("actorDefGongFas").GetValue<Dictionary<int, int[]>>()).Count;
                //获取太吾的被动数量
                int actorOtherEffectCount = (t.Field("actorOtherEffect").GetValue<Dictionary<int, int[]>>()).Count;

                //若太吾装备的催破数量大于1
                if (actorAttackGongFaCount > 1)
                {
                    //调整宽度（原本是125f基础宽度、每个催破多95，数量一多就太宽了）
                    __instance.otherGongFaRange.sizeDelta = new Vector2(125f + actorAttackGongFaCount * 85f + 10f, 160f);

                    //调试信息
                    if (Main.Setting.debugMode.Value)
                    {
                        QuickLogger.Log(LogLevel.Info, "调整otherGongFaRange的sizeDelta");
                    }
                }

                #region 身法UI调整
                //获取MoveGongFaHolder这个GameObject的Transform组件
                Transform moveGongFaHolderTransform = BattleSystem.instance.actorGongFaHolder[1];
                //获取MoveGongFaHolder这个GameObject的GridLayoutGroup组件
                GridLayoutGroup moveGongFaHolderGridLayout = moveGongFaHolderTransform.gameObject.GetComponent<GridLayoutGroup>();

                //若太吾装备的身法 + 催破的数量大于14
                if (actorAttackGongFaCount + actorMoveGongFaCount > 14)
                {
                    //单行最大图标个数（身法）
                    int singleLineMaxIcon = 14 - actorAttackGongFaCount;

                    //将moveGongFaRange的sizeDelta宽度依照单行最大图标个数调整
                    __instance.moveGongFaRange.sizeDelta = new Vector2((float)Mathf.Max(125 + singleLineMaxIcon * 95, 220), 160f);


                    //调整RectTransform的anchoredPosition位置（Y轴下移15f）
                    moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -15f);
                    //调整GridLayoutGroup的受约束的轴（游戏为X轴/水平方向）上应存在的单元格数
                    moveGongFaHolderGridLayout.constraintCount = singleLineMaxIcon;
                    //if (!IsMoveGongFaHolderAnchoredPositionModified)
                    //{
                    //    Vector2 anchoredPositionXY = moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition;
                    //    anchoredPositionXY.y -= 15f;
                    //    moveGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPositionXY;

                    //    IsMoveGongFaHolderAnchoredPositionModified = true;
                    //}


                    //调整GridLayoutGroup的间隔空间（Y轴间隔从0f变为15f）(无需调整回去)
                    moveGongFaHolderGridLayout.spacing = new Vector2(25f, 15f);
                    //if (!IsMoveGongFaHolderSpacingModified)
                    //{
                    //    //Vector2 spacingXY = moveGongFaHolderGridLayout.spacing;
                    //    //spacingXY.y += 15f;
                    //    //moveGongFaHolderGridLayout.spacing = spacingXY;

                    //    IsMoveGongFaHolderSpacingModified = true;
                    //}

                    //调试信息
                    if (Main.Setting.debugMode.Value)
                    {
                        QuickLogger.Log(LogLevel.Info, "调整身法UI 装备身法数:{0} UI附属图标数:{1}", actorMoveGongFaCount, moveGongFaHolderTransform.childCount);
                    }
                }
                else
                {
                    RecoverUiChange.RecoverMoveGongFaHolderUi();
                }
                #endregion

                #region 护体UI调整
                //获取DefGongFaHolder这个GameObject的Transform组件
                Transform defGongFaHolderTransform = BattleSystem.instance.actorGongFaHolder[2];
                //获取DefGongFaHolder这个GameObject的GridLayoutGroup组件
                GridLayoutGroup defGongFaHolderGridLayout = defGongFaHolderTransform.gameObject.GetComponent<GridLayoutGroup>();

                //若太吾装备的护体 + 催破的数量大于14
                if (actorAttackGongFaCount + actorDefGongFaCount > 14)
                {
                    //单行最大图标个数（护体）
                    int singleLineMaxIcon = 14 - actorAttackGongFaCount;

                    //将defGongFaRange的sizeDelta宽度依照单行最大图标个数调整
                    __instance.defGongFaRange.sizeDelta = new Vector2((float)Mathf.Max(125 + singleLineMaxIcon * 95, 220), 160f);



                    //调整RectTransform的anchoredPosition位置（Y轴下移15f）
                    defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -15f);
                    //调整GridLayoutGroup的受约束的轴（游戏为X轴/水平方向）上应存在的单元格数
                    defGongFaHolderGridLayout.constraintCount = singleLineMaxIcon;
                    //if (!IsDefGongFaHolderAnchoredPositionModified)
                    //{
                    //    Vector2 anchoredPositionXY = defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition;
                    //    anchoredPositionXY.y -= 15f;
                    //    defGongFaHolderTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPositionXY;

                    //    IsDefGongFaHolderAnchoredPositionModified = true;
                    //}

                    //调整GridLayoutGroup的间隔空间（Y轴间隔从0f变为15f）(无需调整回去)
                    defGongFaHolderGridLayout.spacing = new Vector2(25f, 15f);
                    //if (!IsDefGongFaHolderSpacingModified)
                    //{
                    //    Vector2 spacingXY = defGongFaHolderGridLayout.spacing;
                    //    spacingXY.y += 15f;
                    //    defGongFaHolderGridLayout.spacing = spacingXY;

                    //    IsDefGongFaHolderSpacingModified = true;
                    //}

                    //调试信息
                    if (Main.Setting.debugMode.Value)
                    {
                        QuickLogger.Log(LogLevel.Info, "调整护体UI 装备护体数:{0} UI附属图标数:{1}", actorDefGongFaCount, defGongFaHolderTransform.childCount);
                    }
                }
                else
                {
                    RecoverUiChange.RecoverDefGongFaHolderUi();
                }
                #endregion
            }
        }
    }
}
