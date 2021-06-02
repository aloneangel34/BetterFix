using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 将处于（按存档栏位区分）人物关注列表中的人物，置顶显示
    /// </summary>
    [HarmonyPatch(typeof(ui_PlaceActorWindow), "CompareActor")]
    public static class PlaceActorWindowResort
    {
        /// <summary>
        /// 修正比较结果
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">原方法的返回值</param>
        /// <param name="actorId1">一号要比较的人物ID</param>
        /// <param name="actorId2">二号要比较的人物ID</param>
        [HarmonyPostfix]
        private static void CompareActorPostfix(ui_PlaceActorWindow __instance, ref int __result, int actorId1, int actorId2)
        //原方法签名
        {
            if (Main.Setting.UiPinActors.Value)
            {
                List<int> shouldPinActorIds = new List<int>();
                switch (SaveDateFile.instance.dateId)
                {
                    case 1:
                        shouldPinActorIds = Main.Setting.RecordFirstSaveSlotPinActorIds.Value;
                        break;
                    case 2:
                        shouldPinActorIds = Main.Setting.RecordSecondSaveSlotPinActorIds.Value;
                        break;
                    case 3:
                        shouldPinActorIds = Main.Setting.RecordThirdSaveSlotPinActorIds.Value;
                        break;
                }

                if (shouldPinActorIds.Contains(actorId1))
                {
                    __result -= 10000;
                }

                if (shouldPinActorIds.Contains(actorId2))
                {
                    __result += 10000;
                }
            }
        }
    }
}
