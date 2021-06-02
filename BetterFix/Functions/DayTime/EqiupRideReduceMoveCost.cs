using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 装备代步时缩减移动所需行动力
    /// </summary>
    [HarmonyPatch(typeof(WorldMapSystem), "PlayerMoveDone")]
    public static class EqiupRideReduceMoveCost
    {
        //私有字段：总计移动专用时间
        private static int _totalTimeOnlyForMove = 0;

        /// <summary>
        /// 根据装备的代步品级，返还移动所消耗的行动力（不满整数无法返还的行动力、积累起来用于之后的返还）
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="showLoading">是否显示移动读条</param>
        /// <param name="noNeed">是否为无消耗</param>
        /// <param name="newMoveNeed">移动所需消耗列表（耗费时间、耗费银钱、耗费威望）</param>
        /// <param name="worldId">目标地点的地域</param>
        /// <param name="partId">目标地点的地区</param>
        /// <param name="placeId">目标地点的地格</param>
        /// <param name="fastMove">是否为快速移动</param>
        [HarmonyPostfix]
        private static void PlayerMoveDonePostfix(WorldMapSystem __instance, bool showLoading, bool noNeed, int[] newMoveNeed, int worldId, int partId, int placeId, bool fastMove)
        //原方法签名
        //private void PlayerMoveDone(bool showLoading, bool noNeed, int[] newMoveNeed, int worldId, int partId, int placeId, bool fastMove)
        {
            if (Main.Setting.DayTimeEqiupRideReduceMoveCost.Value)
            {
                //若不显示移动读条
                if (!showLoading && !noNeed)
                {
                    //太吾人物ID
                    int taiwuActorId = DateFile.instance.MianActorID();
                    //获取太吾装备栏中代步装备槽位所装备的物品ID
                    int transportationItemId = int.Parse(DateFile.instance.GetActorDate(taiwuActorId, 311, false));

                    //若 该装备物品的道具小类为18代步（绳子类也能装备在代步上，其道具小类为32，需要额外排除）
                    if (int.Parse(DateFile.instance.GetItemDate(transportationItemId, 5, false)) == 18)
                    {
                        //获取太吾装备的代步的物品品阶
                        int transportationItemLevel = int.Parse(DateFile.instance.GetItemDate(transportationItemId, 8, false));

                        //若代步品级为九品到一品（超出游戏范围的不作数）
                        if (transportationItemLevel >= 1 && transportationItemLevel <= 9)
                        {
                            #region 弃用（设定移动消耗时间减少百分比）
                            /*
                            //移动消耗时间减少百分比
                            int timeReductionPresent = 0;

                            //按照代步品阶设定移动消耗时间减少百分比
                            switch (transportationItemLevel)
                            {
                                //九阶代步
                                case 1:
                                //八阶代步
                                case 2:
                                    timeReductionPresent = 10;
                                    break;
                                //七阶代步
                                case 3:
                                //六阶代步
                                case 4:
                                    timeReductionPresent = 20;
                                    break;
                                //五阶代步
                                case 5:
                                //四阶代步
                                case 6:
                                    timeReductionPresent = 30;
                                    break;
                                //三阶代步
                                case 7:
                                //二阶代步
                                case 8:
                                    timeReductionPresent = 40;
                                    break;
                                //一阶代步
                                case 9:
                                    timeReductionPresent = 50;
                                    break;
                                default:
                                    break;
                            }
                            */
                            #endregion

                            //移动消耗时间减少百分比：（九品到一品的减少比率：10% ～ 50%，每一品阶多缩减5%）
                            int timeReductionPresent = 5 + transportationItemLevel * 5;

                            if (_totalTimeOnlyForMove < 0)
                            {
                                _totalTimeOnlyForMove = 0;
                            }

                            //将本次移动减少的时间累加至“总计移动专用时间”中
                            _totalTimeOnlyForMove += newMoveNeed[0] * timeReductionPresent;
                            //减少的时间量（取整）
                            int reducedTime = _totalTimeOnlyForMove / 100;
                            //增加当月剩余时间
                            UIDate.instance.ChangeTime(false, -reducedTime);
                            //“总计移动专用时间”减去“减少的时间量”的100倍（也可以看做是 TotalTimeOnlyForMove %= 100）
                            _totalTimeOnlyForMove -= reducedTime * 100;

                            //调试信息
                            if (Main.Setting.debugMode.Value)
                            {
                                QuickLogger.Log(LogLevel.Info, "减少移动耗时 代步名:{0} 品级:{1} 实际减少耗时:{2} 累计剩余可用耗时减少:{3}%天",
                                    //{0}代步名
                                    DateFile.instance.GetItemDate(transportationItemId, 0, false, -1),
                                    //{1}代步品级
                                    DateFile.instance.massageDate[8001][2].Split('|')[transportationItemLevel - 1],
                                    //{2}实际减少耗时
                                    reducedTime,
                                    //{3}累计剩余可用耗时减少
                                    _totalTimeOnlyForMove
                                    );
                            }
                        }
                        //else if (Main.Setting.debugMode.Value)
                        //{
                        //    QuickLogger.Log(LogLevel.Info, "未减少移动耗时 代步名:{0} 代步品级数值:{1}", DateFile.instance.GetItemDate(transportationItemId, 0, false, -1), transportationItemLevel);
                        //}
                    }
                    //else if (Main.Setting.debugMode.Value)
                    //{
                    //    QuickLogger.Log(LogLevel.Info, "未减少移动耗时 代步栏所装备的物品ID:{0} 物品名称:{1} 物品道具小类:{2}", transportationItemId, DateFile.instance.GetItemDate(transportationItemId, 0, false, -1), DateFile.instance.GetItemDate(transportationItemId, 5, false));
                    //}
                }
                //else if (Main.Setting.debugMode.Value)
                //{
                //    QuickLogger.Log(LogLevel.Info, "未减少移动耗时 showLoading:{0} noNeed:{1} ", showLoading, noNeed);
                //}
            }
        }
    }
}
