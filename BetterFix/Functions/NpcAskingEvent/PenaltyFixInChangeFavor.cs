using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace BetterFix
{
	/// <summary>
	/// 讨要事件拒绝惩罚修正（好感度变动）（可能会跳过原方法）
	/// </summary>
	[HarmonyPatch(typeof(DateFile), "ChangeFavor")]
	public static class PenaltyFixInChangeFavor
	{
		/// <summary>
		/// 讨要事件拒绝惩罚修正（好感度变动）
		/// </summary>
		/// <param name="__instance">原方法所属的实例</param>
		/// <param name="actorId">人物ID</param>
		/// <param name="value">好感变动值</param>
		/// <param name="updateActor">是否更新人物？</param>
		/// <param name="showMassage">是否显示信息？</param>
		/// <returns>是否继续执行原方法</returns>
		[HarmonyPrefix]
		private static bool ChangeFavorPrefix(DateFile __instance, int actorId, ref int value, bool updateActor, bool showMassage)
		//原方法签名
		//public void ChangeFavor(int actorId, int value, bool updateActor = true, bool showMassage = true)
		{
			//若是开启了讨要事件拒绝惩罚降低 且 好感变动值为负，拒绝降低的好感值降至原先的1/10
			if (ReduceRefusePenaltyCheckInEndEvent.AllAskingNeedFix && value < 0)
			{
				//调试信息
				if (Main.Setting.debugMode.Value)
				{
					QuickLogger.Log(LogLevel.Info, "拒绝讨要事件惩罚降低 {0}({1}) 好感变动值 原本:{2} -> 修正:{3}",
						//{0}讨要人姓名
						DateFile.instance.GetActorName(actorId),
						//{1}讨要人ID
						actorId,
						//{2}原本好感变动值
						value,
						//{3}修正好感变动值
						value / 20
						);
				}

				//好感变动值降至原先的1/20
				value /= 20;
			}

			//继续执行原方法
			return true;
		}
	}
}
