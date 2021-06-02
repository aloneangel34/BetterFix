using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 修正未开启元山地区驿站，石牢静坐传送至错误地点：判断部分
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "MoveToPrisonPlace")]
    public static class IncorrectPlaceNeedFixCheck
    {
        internal static bool NeedFix = false;

        /// <summary>
        /// 在执行原方法前判断是否是需要修正，若需要则开启修正
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="actorId"></param>
        [HarmonyPrefix]
        private static void MoveToPrisonPlacePrefix(DateFile __instance, int actorId)
        //原方法签名
        {
            if (Main.Setting.BugFixMoveToPrisonIncorrectPlace.Value && actorId == __instance.mianActorId)
            {
                NeedFix = true;
            }
        }

        /// <summary>
        /// 在执行原方法后统一关闭修正
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void MoveToPrisonPlacePostfix(DateFile __instance, int actorId)
        //原方法签名
        {
            NeedFix = false;
        }
    }

    /// <summary>
    /// 修正未开启元山地区驿站，石牢静坐传送至错误地点：执行部分
    /// </summary>
    [HarmonyPatch(typeof(WorldMapSystem), "GetMoveTime",new Type[] { typeof(int), typeof(int) })]
    public static class IncorrectPlaceFixExecute
    {
        /// <summary>
        /// 若需要修正元山石牢静坐的传送地点，在执行原方法后修正所需威望为0
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值（所需时间、所需银钱、所需威望）</param>
        /// <param name="worldId">前往地域（省份）</param>
        /// <param name="partId">前往地区</param>
        [HarmonyPostfix]
        private static void GetMoveTimePostfix(DateFile __instance, ref int[] __result, int worldId, int partId)
        //原方法签名
        //public int[] GetMoveTime(int worldId, int partId)
        {
            if (IncorrectPlaceNeedFixCheck.NeedFix && __result[2] != 0)
            {
                //调试信息
                if (Main.Setting.debugMode.Value)
                {
                    QuickLogger.Log(LogLevel.Warning, "检测到元山地区驿站未开启，修正本次太吾石牢静坐传送点");
                }

                __result[2] = 0;
            }
        }
    }
}
