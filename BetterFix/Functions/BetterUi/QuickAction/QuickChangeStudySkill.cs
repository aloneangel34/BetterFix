using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 快速切换修习技能：修习界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "RemoveStudySkill")]
    public static class QuickChangeStudySkillForStudy
    {
        /// <summary>
        /// 按下“修习”界面的移除技能按钮后，追加调用“选择新技能”
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void RemoveStudySkillPostfix(BuildingWindow __instance)
        //原方法签名
        {
            //若“快速切换修习技能”选项开启
            if (Main.Setting.UiQuickChangeStudySkill.Value)
            {
                //调用选择修习技能窗口（修炼和突破共用）
                __instance.SetStudySkill();
            }
        }
    }

    /// <summary>
    /// 快速切换修习技能：突破界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "RemoveLevelUPSkill")]
    public static class QuickChangeStudySkillForLevelUP
    {
        /// <summary>
        /// 按下“突破”界面的移除技能按钮后，追加调用“选择新技能”
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void RemoveLevelUPSkillPostfix(BuildingWindow __instance)
        //原方法签名
        {
            //若“快速切换修习技能”选项开启
            if (Main.Setting.UiQuickChangeStudySkill.Value)
            {
                //调用选择修习技能窗口（修炼和突破共用）
                __instance.SetStudySkill();
            }
        }
    }

    /// <summary>
    /// 快速切换修习技能：研读界面
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "RemoveReadBook")]
    public static class QuickChangeStudySkillForReadBook
    {
        /// <summary>
        /// 按下“研读”界面的移除技能按钮后，追加调用“选择新技能”
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void RemoveReadBookPostfix(BuildingWindow __instance)
        //原方法签名
        {
            //若“快速切换修习技能”选项开启
            if (Main.Setting.UiQuickChangeStudySkill.Value)
            {
                //调用选择书籍窗口
                __instance.SetChooseBookWindow();
            }
        }
    }
}
