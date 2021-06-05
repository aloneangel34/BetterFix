using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameData;
using UnityEngine;
using HarmonyLib;
using Random = UnityEngine.Random;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// NPC可以在过月时获得威望
    /// </summary>
    [HarmonyPatch(typeof(PeopleLifeAI), "DoTrunAIChange")]
    public static class NpcGetPrestigeIsPossiable
    {
        /// <summary>
        /// 过月结束后额外判断
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="__result">人物死于天灾的几率（原方法的返回值）</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="mapId">地区ID</param>
        /// <param name="tileId">地格ID</param>
        /// <param name="mainActorId">太吾的人物ID</param>
        /// <param name="isTaiwuAtThisTile">人物是否和太吾处于同一格</param>
        /// <param name="worldId">地域ID（省份）</param>
        /// <param name="mainActorItems"></param>
        /// <param name="aliveChars">人物所在地格中，活着的人物列表</param>
        /// <param name="deadChars">人物所在地格中，死去的人物列表</param>
        /// <returns>是否执行原方法（本补丁为false，即不再执行）</returns>
        [HarmonyPostfix]
        private static void DoTrunAIChangePostfix(PeopleLifeAI __instance, ref int __result, int actorId, int mapId, int tileId, int mainActorId, bool isTaiwuAtThisTile, int worldId, Dictionary<int, List<int>> mainActorItems, int[] aliveChars, int[] deadChars)
        //原方法签名
        //private int DoTrunAIChange(int actorId, int mapId, int tileId, int mainActorId, bool isTaiwuAtThisTile, int worldId, Dictionary<int, List<int>> mainActorItems, int[] aliveChars, int[] deadChars)
        {
            //若功能开启
            if (Main.Setting.NpcGetPrestigeIsPossiable.Value && !__instance.allFamily.Contains(actorId))
            {
                //人物年龄
                int actorAge = int.Parse(DateFile.instance.GetActorDate(actorId, 11, false));

                //若 人物年龄大于6岁 且 人物不为太吾同道 且 模拟原方法中的本月获取资源类型为威望(75%几率的检定通过 且 随机0～6【食材～威望】的随机结果为6威望)
                if (actorAge > 6 && !__instance.allFamily.Contains(actorId) && ((Random.Range(0, 100) < 75) && Random.Range(0, 7) == 6))
                {
                    //人物七元赋性
                    int[] actorSevenElementAttr = DateFile.instance.GetActorResources(actorId);
                    //人物所属势力
                    int actorGang = int.Parse(DateFile.instance.GetActorDate(actorId, 19, false));
                    //人物阶层数值
                    int actorLevelValue = int.Parse(DateFile.instance.GetActorDate(actorId, 20, false));
                    //人物阶层（部分势力的首阶人物、其夫/妻的人物阶层数值为“-1”）
                    int actorLevel = Mathf.Abs(actorLevelValue);
                    //人物的处事立场
                    int actorGoodness = DateFile.instance.GetActorGoodness(actorId);
                    //威望的资源序号
                    int prestigeResourceIndex = 6;
                    //人物身份ID
                    int actorGangValueId = DateFile.instance.GetGangValueId(actorGang, actorLevelValue);
                    //人物身份所理应拥有的基础资源量
                    int actorShouldHaveResourceVaule = int.Parse(DateFile.instance.presetGangGroupDateValue[actorGangValueId][712]);

                    //人物未参加武林大会/促织大赛时、有75%的几率，【……尝试获取资源】
                    if (Random.Range(0, 100) < 75 && !WuLinGeneralAssembly.Instance.IsJoinWuLinGA(actorId) && !DateFile.instance.HaveLifeDate(actorId, 718))
                    {
                        //aiShopingDate 字典 的查询key：
                        //"0食材" "1木材" "2金石·金铁" "102金石·玉石" "3织物" "4药材·药物" "104药材·毒物" "5银钱" "6威望"
                        int aiShopingIndex = prestigeResourceIndex;
                        //技艺种类序号
                        //aiShopingDate key3行 4列的数值
                        //"0食材" 515厨艺 "1木材" 508制木 "2金石·金铁" 507锻造 "102金石·玉石" 512巧匠 "3织物" 织锦511 "4药材·药物" 509医术509 "104药材·毒物" 510毒术 "5银钱" 0无 "6威望" 0无
                        int skillIndex = int.Parse(DateFile.instance.aiShopingDate[aiShopingIndex][4]);
                        //人物对应技艺的收集能力
                        //若技艺种类序号不为0：能力设为 （对应的技艺数值（含加成）-30） x1.5
                        //若序号不为0：能力设为 5
                        int actorGatherAbility = (skillIndex != 0) ? ((int.Parse(DateFile.instance.GetActorDate(actorId, skillIndex, true)) - 30) * 150 / 100) : int.Parse(DateFile.instance.aiShopingDate[aiShopingIndex][5]);

                        //百分之（收集能力 + 七元：水性）的几率，【……采集地格资源补充自身】
                        if (Random.Range(0, 100) < actorGatherAbility + actorSevenElementAttr[2])
                        {
                            //理应拥有的资源量的修正值列表
                            string[] actorShouldHaveResourceModifyValues = DateFile.instance.presetGangGroupDateValue[actorGangValueId][711].Split(new char[] { '|' });
                            //资源增加量：
                            int actorResourceIncreaseValue = actorShouldHaveResourceVaule / 100 * (100 + int.Parse(actorShouldHaveResourceModifyValues[prestigeResourceIndex]) / 5) / 100;

                            //资源增加量不为0
                            if (actorResourceIncreaseValue > 0)
                            {
                                //调试信息
                                if (Main.Setting.debugMode.Value)
                                {
                                    QuickLogger.Log(LogLevel.Info, "NPC过月时获得威望 {0}({1}) 阶层:{2} 变更威望:原本{3}->现在{4} 增加量:{5}",
                                        //{0}人物姓名
                                        DateFile.instance.GetActorName(actorId),
                                        //{1}人物ID
                                        actorId,
                                        //{2}人物阶层
                                        actorLevel,
                                        //{3}原本威望
                                        DateFile.instance.GetActorResources(actorId)[prestigeResourceIndex],
                                        //{4}现在威望
                                        DateFile.instance.GetActorResources(actorId)[prestigeResourceIndex] + actorResourceIncreaseValue,
                                        //{5}威望增加量
                                        actorResourceIncreaseValue
                                        );
                                }

                                //人物随机增加威望
                                UIDate.instance.ChangeResource(actorId, prestigeResourceIndex, actorResourceIncreaseValue, true);

                                //若开启了“NPC获取威望会导致地格安定变动”，则有50%的几率进行安定变更判定
                                if (Main.Setting.NpcGetPrestigeWillReducePlaceStability.Value && Random.Range(0, 100) < 50)
                                {
                                    //当前地格的地格大类
                                    //（0一类资源点/1省会城市/2门派驻地/3村镇寨（包括太吾村）/4二类资源点/5三类资源点/6荒野、暗渊）
                                    int placeType = int.Parse(DateFile.instance.GetNewMapDate(mapId, tileId, 89));
                                    //地格是否可以变化安定值
                                    bool canChange = placeType == 1 || placeType == 2 || placeType == 3;

                                    //若不可以
                                    if (!canChange)
                                    {
                                        //结束本方法
                                        return;
                                    }

                                    //好事坏事值
                                    int GoodOrBad = 0;

                                    //!----- 按照处世立场来判定提升/降低地格安定 ----- !

                                    //治疗几率（处世立场的治疗友人几率）
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][11]))
                                    {
                                        //治疗成功率（人物医术/毒术 + 七元细腻）
                                        if (Random.Range(0, 100) < (Math.Max(int.Parse(DateFile.instance.GetActorDate(actorId, 509, true)), int.Parse(DateFile.instance.GetActorDate(actorId, 510, true))) - 20 + DateFile.instance.GetActorResources(actorId)[0]))
                                        {
                                            GoodOrBad += 1;
                                        }
                                    }

                                    //下毒几率（门派是否允许下毒 && 处世立场的下毒几率）
                                    if (int.Parse(DateFile.instance.GetGangDate(actorGang, 1)) != 0 && Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][22]))
                                    {
                                        //下毒成功率（人物毒术 + 七元聪颖）
                                        if (Random.Range(0, 100) < (Math.Max(int.Parse(DateFile.instance.GetActorDate(actorId, 509, true)), int.Parse(DateFile.instance.GetActorDate(actorId, 510, true))) - 20 + DateFile.instance.GetActorResources(actorId)[0]))
                                        {
                                            GoodOrBad -= 1;
                                        }
                                    }

                                    //抢劫几率
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][4]))
                                    {
                                        GoodOrBad += 1;
                                    }

                                    //送礼几率
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][2]))
                                    {
                                        GoodOrBad += 1;
                                    }

                                    //盗墓几率（处世立场的盗墓几率 && 25% + 七元水性）
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][21]) && Random.Range(0, 100) < 25 + actorSevenElementAttr[2])
                                    {
                                        GoodOrBad -= 1;
                                    }

                                    //报仇几率
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][7]))
                                    {
                                        GoodOrBad -= 1;
                                    }

                                    //结仇几率（处世立场的结仇几率 && - 七元冷静）
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][20]) - actorSevenElementAttr[5])
                                    {
                                        GoodOrBad -= 1;
                                    }

                                    //化解几率（处世立场的化解几率 && + 七元冷静）
                                    if (Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][19]) + actorSevenElementAttr[5])
                                    {
                                        GoodOrBad += 1;
                                    }

                                    //情难自已几率
                                    //Random.Range(0, 100) < int.Parse(DateFile.instance.goodnessDate[actorGoodness][25])

                                    //若好事坏事值不为0
                                    if (GoodOrBad != 0)
                                    {   
                                        //（调试信息记录用）地格的原安定
                                        int recordPlaceStability = 0;

                                        //调试信息
                                        if (Main.Setting.debugMode.Value)
                                        {
                                            //获取地格的原安定
                                            recordPlaceStability = DateFile.instance.GetPlaceResource(mapId, tileId)[prestigeResourceIndex];
                                        }

                                        if (GoodOrBad > 0 )
                                        {
                                            //当地安定上升1（其实是提升1%，安定值一般不过百，但最少至少+1）
                                            UIDate.instance.ChangePlaceResource(false, 1, mapId, tileId, prestigeResourceIndex, false);
                                        }
                                        else
                                        {
                                            //当地安定下降1（其实是下降1%，安定值一般不过百，但最少至少-1）
                                            UIDate.instance.ChangePlaceResource(false, -1, mapId, tileId, prestigeResourceIndex, false);
                                        }

                                        //调试信息
                                        if (Main.Setting.debugMode.Value)
                                        {
                                            QuickLogger.Log(LogLevel.Info, "NPC获取威望导致安定变更 地点:{0}{1} 安定值变化 原本:{2} -> 现在:{3} 变更:{4}",
                                                //{0}地点属地名称
                                                DateFile.instance.GetNewMapDate(mapId, tileId, 98),
                                                //{1}地点名称
                                                DateFile.instance.GetNewMapDate(mapId, tileId, 0),
                                                //{2}原本安定值
                                                recordPlaceStability,
                                                //{3}现在安定值
                                                DateFile.instance.GetPlaceResource(mapId, tileId)[prestigeResourceIndex],
                                                //{4}现在安定值
                                                DateFile.instance.GetPlaceResource(mapId, tileId)[prestigeResourceIndex] - recordPlaceStability
                                                );
                                        }
                                    }

                                    #region 弃用（安定仅会减少的设计）
                                    /*
                                    //地格资源的减少百分比上限：
                                    int placeResourceReduceSize = 0;

                                    //当前地格的地格大类
                                    //（0一类资源点/1省会城市/2门派驻地/3村镇寨（包括太吾村）/4二类资源点/5三类资源点/6荒野、暗渊）
                                    int placeType = int.Parse(DateFile.instance.GetNewMapDate(mapId, tileId, 89));

                                    //TODO: 减少百分比可以考虑不在这里全部设定完；而是还要看战力对比
                                    //【再以地格种类进一步设定地格安定的减少百分比上限】
                                    switch (placeType)
                                    {
                                        //村镇寨
                                        case 3:
                                            //随机较大幅减少
                                            placeResourceReduceSize = Convert.ToInt32(Random.Range((10 - actorLevel) * 1f / 2f, 10 - actorLevel) * 3f / 4f);
                                            break;
                                        //省会城市
                                        case 1:
                                            //随机中幅减少
                                            placeResourceReduceSize = Convert.ToInt32(Random.Range((10 - actorLevel) * 1f / 4f, (10 - actorLevel) * 3f / 4f));
                                            break;
                                        //门派驻地
                                        case 2:
                                            //随机较小幅减少
                                            placeResourceReduceSize = Convert.ToInt32(Random.Range((10 - actorLevel) * 1f / 4f, (10 - actorLevel) * 1f / 2f));
                                            break;
                                        //其他种类（安定值上限固定为0）
                                        default:
                                            //安定值不会减少
                                            placeResourceReduceSize = 0;
                                            break;
                                    }

                                    //若地格资源的减少百分比上限不为0，【……随机减少地格的安定值】
                                    //很合理，（闹事让他人害怕了来获取了威望）、（影响治安所以安定下降了）
                                    if (placeResourceReduceSize > 0)
                                    {

                                        //TODO: 降低地格安定前先按判定战力百分比对比来判定是否成功（人物ID 与 当前地点的最高级别人物的战力）
                                        //TODO: 查找势力首领的方法：public List<int> GetGangActor(int gangId, int level, bool getOther = true)
                                        //获取指定势力指定阶级的所有人物ID
                                        //gangId 势力ID、level 需要获取的阶级、getOther 是否获取所有阶级

                                        //if (true)
                                        //{
                                        //    return;
                                        //}

                                        //（调试信息记录用）地格的原安定
                                        int recordPlaceStability = 0;

                                        //调试信息
                                        if (Main.Setting.debugMode.Value)
                                        {
                                            //获取地格的原安定
                                            recordPlaceStability = DateFile.instance.GetPlaceResource(mapId, tileId)[prestigeResourceIndex];
                                        }

                                        //随机减少地块安定
                                        //（传入的-placeResourceReduceSize只是变动百分比上限，会在该方法内随机）
                                        UIDate.instance.ChangePlaceResource(true, -placeResourceReduceSize, mapId, tileId, prestigeResourceIndex, false);

                                        //调试信息
                                        if (Main.Setting.debugMode.Value)
                                        {
                                            QuickLogger.Log(LogLevel.Info, "NPC获取威望导致安定降低 地点:{0} 原安定:{1} 现安定:{2} 最大降幅:{3}%", (DateFile.instance.GetNewMapDate(mapId, tileId, 98) + DateFile.instance.GetNewMapDate(mapId, tileId, 0)), recordPlaceStability, DateFile.instance.GetPlaceResource(mapId, tileId)[prestigeResourceIndex], placeResourceReduceSize);
                                        }
                                    }
                                    */
                                    #endregion
                                }
                            }
                        }
                    }
                    //获取资源事件 未触发，【……凭空获得、丢失资源】
                    else
                    {
                        //通过50%的检定时，有百分之（5 + 七元：福源的十分之一）的几率
                        if (Random.Range(0, 100) < 50 && Random.Range(0, 100) < 5 + actorSevenElementAttr[6] / 10)
                        {
                            //调试信息
                            if (Main.Setting.debugMode.Value)
                            {
                                QuickLogger.Log(LogLevel.Info, "获得大量资源抽选列表 加入{0}({1})（资源类型：威望）",
                                    //{0}人物姓名
                                    DateFile.instance.GetActorName(actorId),
                                    //{1}人物ID
                                    actorId
                                    );
                            }

                            try
                            {
                                //用Traverse挖掘__instance实例中的getRActorId私有字段
                                List<int[]> getRActorIdList = Traverse.Create(__instance).Field("getRActorId").GetValue<List<int[]>>();
                                //当月随机一人获得大量资源列表加入人物获得威望条目数据
                                getRActorIdList.Add(new int[] { actorId, prestigeResourceIndex, mapId, tileId });
                            }
                            catch (Exception ex)
                            {
                                QuickLogger.Log(LogLevel.Error, "向获得大量资源抽选列表添加条目（威望）时出错：\n{0}", ex);
                            }
                        }
                        //否则则有百分之（5 - 七元：福源的十分之一）的几率
                        else if (Random.Range(0, 100) < 5 - actorSevenElementAttr[6] / 10)
                        {
                            //调试信息
                            if (Main.Setting.debugMode.Value)
                            {
                                QuickLogger.Log(LogLevel.Info, "丢失大量资源抽选列表 加入{0}({1})（资源类型：威望）",
                                    //{0}人物姓名
                                    DateFile.instance.GetActorName(actorId),
                                    //{1}人物ID
                                    actorId
                                    );
                            }

                            try
                            {
                                //用Traverse挖掘__instance实例中的loseRActorId私有字段
                                List<int[]> loseRActorIdList = Traverse.Create(__instance).Field("loseRActorId").GetValue<List<int[]>>();
                                //当月随机一人丢失大量资源列表加入人物丢失威望条目数据
                                loseRActorIdList.Add(new int[] { actorId, prestigeResourceIndex, mapId, tileId });
                            }
                            catch (Exception ex)
                            {
                                QuickLogger.Log(LogLevel.Error, "向丢失大量资源抽选列表添加条目（威望）时出错：\n{0}", ex);
                            }
                        }
                    }
                }
            }
        }
    }
}