using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
#if true
    /// <summary>
    /// 较艺中的快速投降按钮
    /// </summary>
    [HarmonyPatch(typeof(SkillBattleSystem), "StartSkillBattle")]
    public static class QuickSkillBattleSurrender
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void StartSkillBattlePostfix(SkillBattleSystem __instance)
        //原方法签名
        //private IEnumerator StartSkillBattle(float waitTime, int chooseSkill)
        {
            GameObject surrenderIcon = GameObject.Find("UIRoot/Canvas/UIWindow/SkillBattleWindow/SkillBattleMain/BattleActorBack/ActorHpBack/ActorHp/ActorBattleLoseIcon,831");

            if (surrenderIcon != null)
            {
                UnityEngine.UI.Button button = surrenderIcon.GetComponent<UnityEngine.UI.Button>();

                if (Main.Setting.UiQuickSurrender.Value)
                {
                    if (button == null)
                    {
                        surrenderIcon.AddComponent<UnityEngine.UI.Button>();
                        button = surrenderIcon.GetComponent<UnityEngine.UI.Button>();
                    }

                    button.interactable = true;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate ()
                    {
                        YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.SkillBattleSurrenderClickId, "较艺认输", "要向对手认输吗？", false, true);
                    });
                }
                else if (button != null)
                {
                    button.interactable = false;
                }
            }

        }
    }

    /// <summary>
    /// 切磋战/接招战中的快速投降按钮
    /// </summary>
    [HarmonyPatch(typeof(BattleSystem), "ShowBattleWindow")]
    public static class QuickExerciseBattleSurrender
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="battleTyp">战斗种类</param>
        [HarmonyPostfix]
        private static void ShowBattleWindowPostfix(BattleSystem __instance, int battleTyp)
        //原方法签名
        //public void ShowBattleWindow(int battleTyp, int actorFirstTyp, int useFirstTyp, int battleLoseTyp, int actorUseItemId, int enemyUseItemId, int enemyTeamId, int changeEnemyObbs, int battleGetExp)
        {
            GameObject surrenderIcon = GameObject.Find("UIRoot/Canvas/UIBackGround/BattleSystem/ActorBack/BattleActorBack/ActorDp/ActorBattleLoseIcon,814");

            if (surrenderIcon != null)
            {
                UnityEngine.UI.Button button = surrenderIcon.GetComponent<UnityEngine.UI.Button>();

                if (Main.Setting.UiQuickSurrender.Value && (battleTyp == 1 || battleTyp == 2))
                {
                    if (button == null)
                    {
                        surrenderIcon.AddComponent<UnityEngine.UI.Button>();
                        button = surrenderIcon.GetComponent<UnityEngine.UI.Button>();
                    }

                    button.onClick.RemoveAllListeners();
                    button.interactable = true;
                    button.onClick.AddListener(delegate ()
                    {
                        YesOrNoWindow.instance.SetYesOrNoWindow(QuickSurrenderSupport.ExerciseSurrenderClickId, (battleTyp == 1) ? "切磋战认输" : "接招战认输", "要向对手认输吗？\n（算落败、不算逃脱）", false, true);
                    });
                }
                else if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.interactable = false;
                }
            }
        }
    }

    /// <summary>
    /// 执行快速认输
    /// </summary>
    [HarmonyPatch(typeof(YesOrNoWindow), "OnYesButton")]
    public static class QuickSurrenderExecution
    {
        //internal static bool DontShowFix = false;
        internal static string[] SkillQusetionMassage = null;

        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnYesButtonPostfix(YesOrNoWindow __instance)
        //原方法签名
        //public void OnYesButton()
        {
            if (OnClick.instance.ID == QuickSurrenderSupport.SkillBattleSurrenderClickId)
            {
                QuickSurrenderSupport.SkillBattleSurrender();
            }

            if (OnClick.instance.ID == QuickSurrenderSupport.ExerciseSurrenderClickId)
            {
                QuickSurrenderSupport.ExerciseBattleSurrender();
            }
        }
    }

    /// <summary>
    /// 快速认输支援方法
    /// </summary>
    public static class QuickSurrenderSupport
    {
        /// <summary>
        /// 较艺认输点击选项ID
        /// </summary>
        public const int SkillBattleSurrenderClickId = 10350;
        /// <summary>
        /// 切磋战/接招战认输点击选项ID
        /// </summary>
        public const int ExerciseSurrenderClickId = 10351;

        /// <summary>
        /// 较艺中快速认输
        /// </summary>
        public static void SkillBattleSurrender()
        {
            if (SkillBattleSystem.Exists && Traverse.Create(SkillBattleSystem.instance).Field<int>("actorSkillHp").Value > 0 && !Traverse.Create(SkillBattleSystem.instance).Field<bool>("battleEnd").Value)
            {
                Traverse.Create(SkillBattleSystem.instance).Field<int>("actorSkillHp").Value = 0;
                SkillBattleSystem.instance.LoseQusetion(true);
            }
        }

        //由于在通常战斗若快速投降可能会规避掉太多伤害，因此只允许在“切磋/接招”战中快速投降
        /// <summary>
        /// 切磋战/接招战中快速认输
        /// </summary>
        public static void ExerciseBattleSurrender()
        {
            if (BattleSystem.Exists && !BattleSystem.instance.battleEnd && (BattleSystem.instance.battleTyp == 1 || BattleSystem.instance.battleTyp == 2))
            {
                //玩家落败（非逃跑）
                BattleEndWindow.instance.BattleEnd(false, 0);
            }
        }
    }
#endif
}
