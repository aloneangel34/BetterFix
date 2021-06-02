using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 当人物已死亡时，点击人物头像不再打开人物查看菜单
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "OnClick")]
    public static class DontOpenActorMenuOnDeadActor
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="button">C按钮</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool OnClickPrefix(ui_NpcSearch __instance, CButton button)
        //原方法签名
        //public override void OnClick(CButton button)
        {
            //保持开启，无需关闭

            if (button.name == "NpcFace")
            {
                //人物ID
                int actorId = int.Parse(button.transform.parent.name.Split(',')[1]);

                //如果人物已死
                if (int.Parse(DateFile.instance.GetActorDate(actorId, 26, false)) != 0)
                {
                    //跳过原方法（即不打开人物菜单）
                    return false;
                }
            }

            return true;
        }
    }
}
