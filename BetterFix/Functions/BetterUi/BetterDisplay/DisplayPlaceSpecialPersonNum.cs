using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 商人提示按钮的附属功能：直接在地格上显示商人数量
    /// </summary>
    [HarmonyPatch(typeof(WorldMapPlace), "UpdatePlaceActors")]
    public static class DisplayPlaceSpecialPersonNum
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdatePlaceActorsPostfix(WorldMapPlace __instance)
        //原方法签名
        //public void UpdatePlaceActors()
        {
            if (Main.Setting.DisplayPlaceSpecialPersonNum.Value)
            {
                //若地点人数图标在显示中（即原本已有人数显示）
                if (__instance.actorIcon.gameObject.activeSelf)
                {
                    //地点ID
                    int placeId = Traverse.Create(__instance).Field<int>("placeId").Value;
                    //获取该地点的可交谈人物
                    List<int> placeActorIds = DateFile.instance.HaveActor(DateFile.instance.mianPartId, placeId, true, false, false, true, true);

                    int merchantCount = 0;
                    int xxMadCount = 0;

                    //统计可交谈人物中的商人人数
                    foreach (int actorId in placeActorIds)
                    {
                        if (DisplaySupport.IsActorMerchant(actorId))
                        {
                            merchantCount++;
                        }
                    }

                    //获取该地点的敌对人物
                    List<int> placeEnemyIds = DateFile.instance.HaveActor(DateFile.instance.mianPartId, placeId, false, false, true, true, false);

                    //统计敌对人物中的入魔人人数
                    foreach (int actorId in placeEnemyIds)
                    {
                        if (int.Parse(DateFile.instance.GetActorDate(actorId, 6, false)) == 1)
                        {
                            xxMadCount++;
                        }
                    }

                    //若商人/入魔人的人数大于0
                    if (merchantCount > 0 || xxMadCount> 0)
                    {
                        //允许水平超框显示
                        __instance.actorSizeText.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
                        //允许垂直超框显示
                        __instance.actorSizeText.verticalOverflow = UnityEngine.VerticalWrapMode.Overflow;
                        //追加显示商人/入魔人的人数
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(__instance.actorSizeText.text);
                        stringBuilder.Append("\n");

                        if (xxMadCount > 0)
                        {
                            stringBuilder.AppendFormat("{0}魔</color>{1}", DateFile.instance.massageDate[20010][0], xxMadCount);
                        }

                        if (merchantCount > 0)
                        {
                            stringBuilder.AppendFormat("{0}商</color>{1}", DateFile.instance.massageDate[20005][0], merchantCount);
                        }

                        __instance.actorSizeText.text = stringBuilder.ToString();
                    }
                }
            }
        }
    }
}
