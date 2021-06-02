using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 记忆不显示较艺提问弹窗
    /// </summary>
    [HarmonyPatch(typeof(YesOrNoWindow), "SetYesOrNoWindow")]
    public static class RememberDontShowSkillQusetionSideFix
    {
        //internal static bool DontShowFix = false;
        internal static string[] SkillQusetionMassage = null;

        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="clickId">确认选项ID</param>
        /// <param name="title">标题</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool SetYesOrNoWindowPrefix(YesOrNoWindow __instance, int clickId, string title)
        //原方法签名
        //public void SetYesOrNoWindow(int clickId, string title, string massage, bool backMask = false, bool canClose = true)
        {
            if (Main.Setting.RememberDontShowSkillQusetionSide.Value)
            {
                if (SkillQusetionMassage == null || SkillQusetionMassage.Length < 4)
                {
                    SkillQusetionMassage = DateFile.instance.massageDate[908][1].Split('|');
                }

                if (SkillQusetionMassage.Length > 3 && (title == SkillQusetionMassage[0] || title == SkillQusetionMassage[2]))
                {
                    //当为对方提问时
                    if (clickId == 801)
                    {
                        //执行原本应该在点击Yes按钮后执行的项目
                        SkillBattleSystem.instance.StartCoroutine(SkillBattleSystem.instance.EnemySetQusetion(1f));
                    }
                    return false;
                }
            }

            return true;
        }
    }
}
