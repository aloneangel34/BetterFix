using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 修复抓捕人物的父系母系血统与人物ID不匹配的BUG
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "MakeCatchActor")]
    public static class CatchActorBloodInfoMissmatch
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        [HarmonyPostfix]
        private static void MakeCatchActorPostfix(DateFile __instance, int __result, int oldActorId)
        //原方法签名
        {
            if (Main.Setting.BugFixCatchActorBloodInfoMissmatch.Value)
            {
                if (__instance.HaveLifeDate(__result, 601) && __instance.actorLife[__result][601][0] == oldActorId)
                {
                    __instance.actorLife[__result][601][0] = __result;
                }

                if (__instance.HaveLifeDate(__result, 602) && __instance.actorLife[__result][602][0] == oldActorId)
                {
                    __instance.actorLife[__result][602][0] = __result;
                }
            }
        }
    }
}
