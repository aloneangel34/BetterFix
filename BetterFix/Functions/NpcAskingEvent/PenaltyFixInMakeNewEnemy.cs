using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using Random = UnityEngine.Random;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 讨要事件拒绝惩罚修正（按设定不成为敌对）（可能会跳过原方法）
    /// </summary>
    [HarmonyPatch(typeof(MessageEventManager), "MakeNewEnemy")]
    public static class PenaltyFixInMakeNewEnemy
    {
        /// <summary>
        /// 讨要事件拒绝惩罚修正（按设定不成为敌对）
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="enemyId">敌对方ID</param>
		/// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool MakeNewEnemyPrefix(MessageEventManager __instance, int actorId, int enemyId)
        //原方法签名
        //private void MakeNewEnemy(int actorId, int enemyId)
        {
            //若是开启了讨要事件拒绝惩罚降低，拒绝后本来产生敌对的场合、75%的几率不会导致敌对
            if (ReduceRefusePenaltyCheckInEndEvent.AllAskingNeedFix)
            {
                bool isThisNotMakeEnemy = Random.Range(0, 100) < 80;

                //调试信息
                if (Main.Setting.debugMode.Value)
                {
                    QuickLogger.Log(LogLevel.Info, "拒绝惩罚降低（检测到将产生敌对）  {0}({1}) 本次是否不会敌对（80%）:{2}",
                        //{0}讨要人姓名
                        DateFile.instance.GetActorName(actorId),
                        //{1}讨要人ID
                        actorId,
                        //{2}不会敌对是否判定成功
                        isThisNotMakeEnemy
                        );
                }

                if (isThisNotMakeEnemy)
                {
                    //跳过原方法不再执行（不敌对）
                    return false;
                }
            }

            //继续执行原方法
            return true;
        }
    }
}
