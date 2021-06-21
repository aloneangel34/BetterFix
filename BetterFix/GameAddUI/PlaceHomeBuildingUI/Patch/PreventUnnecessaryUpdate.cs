using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{

#if false
    //弃用，这种方法和其他MOD的兼容性较差（比如太吾管家就对原方法有后置补丁：也是读取name来获取partId/placeId/buildingIndex）
    /// <summary>
    /// 防止非必要的产业地图建筑图标更新
    /// </summary>
    [HarmonyPatch(typeof(HomeBuilding), "UpdateBuilding")]
    public static class PreventUnnecessaryUpdate
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool UpdateBuildingPrefix(HomeBuilding __instance)
        //原方法签名
        //public void UpdateBuilding()
        {
            //产业建筑地图中，该项建筑图标UI的name
            string[] array = __instance.name.Split(',');

            //若name已被正确设置，继续执行原方法
            if (array.Length > 3 && int.TryParse(array[1], out int partId) && int.TryParse(array[2], out int placeId) &&  int.TryParse(array[3], out int buildingIndex))
            {
                return true;
            }

            //否则跳过原方法不再执行（一来是避免报错，二来这种情况肯定是处于“没有打开产业建筑地图时”、就不用更新图标可以减少运算负担）
            return false;
        }
    }

    //弃用，因为在HomeSystemWindow.Instance为null时调用会导致新建一个产业建筑界面，
    //但此时：一来、UI管理那边没注册，二来、没有MakeHomeMap过，就会显得一团糟
    
    /// <summary>
    /// 当处于产业地图外更新建筑图标时，修正UI的name属性
    /// </summary>
    [HarmonyPatch(typeof(HomeSystemWindow), "UpdateHomePlace")]
    public static class FixHomeBuldingNameWhenOutsideHomesystem
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static void UpdateHomePlacePrefix(HomeSystemWindow __instance, int partId, int placeId, int buildingIndex)
        //原方法签名
        //public void UpdateHomePlace(int partId, int placeId, int buildingIndex)
        {
            //满足原方法更新图标的要求时
            if (partId == HomeSystem.instance.homeMapPartId && placeId == HomeSystem.instance.homeMapPlaceId)
            {
                //强制更新图标UI的name属性（判断原name值是否符合格式反而更麻烦，不如不管是否符合都更新一次）
                __instance.allHomeBulding[buildingIndex].name = string.Format("HomeMapPlace,{0},{1},{2}", partId, placeId, buildingIndex);
            }
        }
    }
#endif

    //要点在于：当不处于产业地图中时，避免 HomeSystemWindow.Instance被调用
    //不然导致游戏新建一个产业建筑界面，
    //但此时：
    //一来、UI管理那边没注册（无法正常关闭、即便尝试按产业地图按钮正常进入、再尝试退出也不行），
    //二来、没有掉用过MakeHomeMap方法更新地图（所有建筑图标上所有文本都是默认值，建筑图标也是空白图片）

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(MakeSystem), "CloseMakeWindow")]
    public static class PreventInCloseMakeWindow
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static void CloseMakeWindowPrefix(MakeSystem __instance)
        //原方法签名
        //public void CloseMakeWindow()
        {
            //不处于产业地图中时
            if (!HomeSystemWindow.Exists)
            {
                //强行将 MakeSystem.__instance.itemWindowFix 私有字段的值设为true(这样就不满足原方法中更新图标的条件)
                Traverse.Create(__instance).Field<bool>("itemWindowFix").Value = true;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "ChanageWorkingAcotr")]
    public static class PreventInChanageWorkingAcotr
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool ChanageWorkingAcotrPrefix(BuildingWindow __instance)
        //原方法签名
        //public void ChanageWorkingAcotr()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段

                //挖掘私有字段 BuildingWindow.instance.workingActorId
                int workingActorId = Traverse.Create(__instance).Field<int>("workingActorId").Value;

                int homeMapPartId = HomeSystem.instance.homeMapPartId;
                int homeMapPlaceId = HomeSystem.instance.homeMapPlaceId;
                int homeMapbuildingIndex = HomeSystem.instance.homeMapbuildingIndex;
                int[] array = DateFile.instance.ActorIsWorking(workingActorId);

                if (array != null)
                {
                    DateFile.instance.RemoveWorkingActor(array[0], array[1], array[2], false);

                    //不执行UpdateHomePlace方法（主要是为了避免调用HomeSystemWindow.Instance）
                    //HomeSystemWindow.Instance.UpdateHomePlace(array[0], array[1], array[2]);
                    //补上本MOD地点建筑的图标更新
                    UpdateSingleBuildingSupport.UpdateSingle(array[0], array[1], array[2]);
                }

                if (DateFile.instance.actorsWorkingDate.ContainsKey(homeMapPartId))
                {
                    if (DateFile.instance.actorsWorkingDate[homeMapPartId].ContainsKey(homeMapPlaceId))
                    {
                        if (DateFile.instance.actorsWorkingDate[homeMapPartId][homeMapPlaceId].ContainsKey(homeMapbuildingIndex))
                        {
                            DateFile.instance.actorsWorkingDate[homeMapPartId][homeMapPlaceId][homeMapbuildingIndex] = workingActorId;
                        }
                        else
                        {
                            DateFile.instance.actorsWorkingDate[homeMapPartId][homeMapPlaceId].Add(homeMapbuildingIndex, workingActorId);
                        }
                    }
                    else
                    {
                        Dictionary<int, int> value = new Dictionary<int, int>
                        {
                            {
                                homeMapbuildingIndex,
                                workingActorId
                            }
                        };

                        DateFile.instance.actorsWorkingDate[homeMapPartId].Add(homeMapPlaceId, value);
                    }
                }
                else
                {
                    Dictionary<int, int> value2 = new Dictionary<int, int>
                    {
                        {
                            homeMapbuildingIndex,
                            workingActorId
                        }
                    };

                    Dictionary<int, Dictionary<int, int>> value3 = new Dictionary<int, Dictionary<int, int>>
                    {
                        {
                            homeMapPlaceId,
                            value2
                        }
                    };

                    DateFile.instance.actorsWorkingDate.Add(homeMapPartId, value3);
                }
                __instance.UpdateWorkingActor(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);
                __instance.CloseActorWindow();
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "DoStopRemoveBuilding")]
    public static class PreventInDoStopRemoveBuilding
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool DoStopRemoveBuildingPrefix(BuildingWindow __instance)
        //原方法签名
        //public void DoStopRemoveBuilding()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段

                int homeMapPartId = HomeSystem.instance.homeMapPartId;
                int homeMapPlaceId = HomeSystem.instance.homeMapPlaceId;
                int homeMapbuildingIndex = HomeSystem.instance.homeMapbuildingIndex;
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 6, 0);
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 7, 0);
                UIDate.instance.RemoveManPowerUse(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 2);

                TipsWindow.instance.SetTips(25 + __instance.removeTyp, new string[]
                {
                    DateFile.instance.GetBuildingTipName(homeMapPartId, homeMapPlaceId),
                    DateFile.instance.basehomePlaceDate[DateFile.instance.homeBuildingsDate[homeMapPartId][homeMapPlaceId][homeMapbuildingIndex][0]][0]
                }, 180, 225f, 153f, 450, 100);

                __instance.buildingTyp[3].GetComponent<Toggle>().interactable = true;

                //不执行UpdateHomePlace方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);
                //补上本MOD地点建筑的图标更新
                UpdateSingleBuildingSupport.UpdateSingle(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);

                __instance.GetBuildingMassage();
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "MakeBuildingRemove")]
    public static class PreventInMakeBuildingRemove
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool MakeBuildingRemovePrefix(BuildingWindow __instance)
        //原方法签名
        //public void MakeBuildingRemove()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段


                Traverse traverse = Traverse.Create(__instance);
                //挖掘私有字段 BuildingWindow.instance.useMenpower3
                int useMenpower3 = traverse.Field<int>("useMenpower3").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingId
                int buildingId = traverse.Field<int>("buildingId").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingTime3
                int buildingTime3 = traverse.Field<int>("buildingTime3").Value;

                int homeMapPartId = HomeSystem.instance.homeMapPartId;
                int homeMapPlaceId = HomeSystem.instance.homeMapPlaceId;
                int homeMapbuildingIndex = HomeSystem.instance.homeMapbuildingIndex;
                int[] array = DateFile.instance.homeBuildingsDate[homeMapPartId][homeMapPlaceId][homeMapbuildingIndex];
                __instance.RemoveWorkingActor(true);

                if (array[4] > 0)
                {
                    DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 4, 0);
                    DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 9, 0);
                    DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 10, 0);
                }

                if (DateFile.instance.manpowerHomeRemoveList.ContainsKey(homeMapPartId))
                {
                    if (DateFile.instance.manpowerHomeRemoveList[homeMapPartId].ContainsKey(homeMapPlaceId))
                    {
                        DateFile.instance.manpowerHomeRemoveList[homeMapPartId][homeMapPlaceId].Add(homeMapbuildingIndex, useMenpower3);
                    }
                    else
                    {
                        Dictionary<int, int> value = new Dictionary<int, int>
                        {
                            {
                                homeMapbuildingIndex,
                                useMenpower3
                            }
                        };

                        DateFile.instance.manpowerHomeRemoveList[homeMapPartId].Add(homeMapPlaceId, value);
                    }
                }
                else
                {
                    Dictionary<int, int> value2 = new Dictionary<int, int>
                    {
                        {
                            homeMapbuildingIndex,
                            useMenpower3
                        }
                    };

                    Dictionary<int, Dictionary<int, int>> value3 = new Dictionary<int, Dictionary<int, int>>
                    {
                        {
                            homeMapPlaceId,
                            value2
                        }
                    };

                    DateFile.instance.manpowerHomeRemoveList.Add(homeMapPartId, value3);
                }

                TipsWindow.instance.SetTips(12 + __instance.removeTyp, new string[]
                {
                    DateFile.instance.GetBuildingTipName(homeMapPartId, homeMapPlaceId),
                    DateFile.instance.basehomePlaceDate[buildingId][0]
                }, 180, 225f, 153f, 450, 100);

                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 6, buildingTime3);
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 7, __instance.removeTyp);

                //不执行UpdateHomePlace/UpdatePlaceSize方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);
                //HomeSystemWindow.Instance.UpdatePlaceSize(homeMapPartId, homeMapPlaceId);
                //补上本MOD地点建筑的图标更新
                UpdateSingleBuildingSupport.UpdateSingle(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);

                __instance.buildingTyp[2].GetComponent<Toggle>().interactable = false;
                __instance.buildingTyp[3].GetComponent<Toggle>().interactable = false;
                __instance.buildingTyp[0].GetComponent<Toggle>().isOn = true;
                __instance.GetBuildingMassage();
                __instance.CloseBuildingWindow();
                GEvent.OnEvent(eEvents.ManpowerChange, Array.Empty<object>());
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "MakeBuildingUp")]
    public static class PreventInMakeBuildingUp
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool MakeBuildingUpPrefix(BuildingWindow __instance)
        //原方法签名
        //public void MakeBuildingUp()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段

                Traverse traverse = Traverse.Create(__instance);

                //挖掘私有字段 BuildingWindow.instance._needResource2
                Dictionary<int, int> _needResource2 = traverse.Field<Dictionary<int, int>>("_needResource2").Value;
                //挖掘私有字段 BuildingWindow.instance.useMenpower2
                int useMenpower2 = traverse.Field<int>("useMenpower2").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingId
                int buildingId = traverse.Field<int>("buildingId").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingTime2
                int buildingTime2 = traverse.Field<int>("buildingTime2").Value;

                int homeMapPartId = HomeSystem.instance.homeMapPartId;
                int homeMapPlaceId = HomeSystem.instance.homeMapPlaceId;
                int homeMapbuildingIndex = HomeSystem.instance.homeMapbuildingIndex;
                List<int> list = new List<int>(_needResource2.Keys);

                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        int val = -_needResource2[list[i]];
                        UIDate.instance.ChangeResource(DateFile.instance.MianActorID(), list[i], val, false);
                    }
                }

                if (DateFile.instance.manpowerHomeUpList.ContainsKey(homeMapPartId))
                {

                    if (DateFile.instance.manpowerHomeUpList[homeMapPartId].ContainsKey(homeMapPlaceId))
                    {
                        DateFile.instance.manpowerHomeUpList[homeMapPartId][homeMapPlaceId].Add(homeMapbuildingIndex, useMenpower2);
                    }
                    else
                    {
                        Dictionary<int, int> value = new Dictionary<int, int>
                        {
                            {
                                homeMapbuildingIndex,
                                useMenpower2
                            }
                        };

                        DateFile.instance.manpowerHomeUpList[homeMapPartId].Add(homeMapPlaceId, value);
                    }
                }
                else
                {
                    Dictionary<int, int> value2 = new Dictionary<int, int>
                    {
                        {
                            homeMapbuildingIndex,
                            useMenpower2
                        }
                    };

                    Dictionary<int, Dictionary<int, int>> value3 = new Dictionary<int, Dictionary<int, int>>
                    {
                        {
                            homeMapPlaceId,
                            value2
                        }
                    };

                    DateFile.instance.manpowerHomeUpList.Add(homeMapPartId, value3);
                }

                TipsWindow.instance.SetTips(11, new string[]
                {
                    DateFile.instance.GetBuildingTipName(homeMapPartId, homeMapPlaceId),
                    DateFile.instance.basehomePlaceDate[buildingId][0]
                }, 180, 225f, 153f, 450, 100);

                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 5, buildingTime2);

                //不执行UpdateHomePlace/UpdatePlaceSize方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);
                //HomeSystemWindow.Instance.UpdatePlaceSize(homeMapPartId, homeMapPlaceId);
                //补上本MOD地点建筑的图标更新
                UpdateSingleBuildingSupport.UpdateSingle(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);

                __instance.buildingTyp[2].GetComponent<Toggle>().interactable = false;
                __instance.buildingTyp[3].GetComponent<Toggle>().interactable = false;
                __instance.buildingTyp[0].GetComponent<Toggle>().isOn = true;
                __instance.GetBuildingMassage();
                GEvent.OnEvent(eEvents.ManpowerChange, Array.Empty<object>());
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "MakeNewBuilding")]
    public static class PreventInMakeNewBuilding
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool MakeNewBuildingPrefix(BuildingWindow __instance)
        //原方法签名
        //public void MakeNewBuilding()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段

                Traverse traverse = Traverse.Create(__instance);

                //挖掘私有字段 BuildingWindow.instance._needResource1
                Dictionary<int, int> _needResource1 = traverse.Field<Dictionary<int, int>>("_needResource1").Value;
                //挖掘私有字段 BuildingWindow.instance.useItemId
                int useItemId = traverse.Field<int>("useItemId").Value;
                //挖掘私有字段 BuildingWindow.instance.useMenpower1
                int useMenpower1 = traverse.Field<int>("useMenpower1").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingId
                int buildingId = traverse.Field<int>("buildingId").Value;
                //挖掘私有字段 BuildingWindow.instance.buildingTime1
                int buildingTime1 = traverse.Field<int>("buildingTime1").Value;


                int homeMapPartId = HomeSystem.instance.homeMapPartId;
                int homeMapPlaceId = HomeSystem.instance.homeMapPlaceId;
                int homeMapbuildingIndex = HomeSystem.instance.homeMapbuildingIndex;
                int num = DateFile.instance.MianActorID();
                DateFile.instance.SetHomeMapShow(homeMapPartId, homeMapPlaceId, true);
                List<int> list = new List<int>(_needResource1.Keys);

                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        int val = -_needResource1[list[i]];
                        UIDate.instance.ChangeResource(num, list[i], val, false);
                    }
                }

                if (useItemId > 0)
                {
                    if (DateFile.instance.GetActorItems(num, 0, false).ContainsKey(useItemId))
                    {
                        DateFile.instance.LoseItem(num, useItemId, 1, true, true, 1);
                    }
                    else
                    {
                        if (DateFile.instance.GetActorItems(-999, 0, false).ContainsKey(useItemId))
                        {
                            DateFile.instance.LoseItem(-999, useItemId, 1, true, true, 1);
                        }
                    }
                }

                if (DateFile.instance.manpowerHomeUseList.ContainsKey(homeMapPartId))
                {
                    if (DateFile.instance.manpowerHomeUseList[homeMapPartId].ContainsKey(homeMapPlaceId))
                    {
                        DateFile.instance.manpowerHomeUseList[homeMapPartId][homeMapPlaceId].Add(homeMapbuildingIndex, useMenpower1);
                    }
                    else
                    {
                        Dictionary<int, int> value = new Dictionary<int, int>
                        {
                            {
                                homeMapbuildingIndex,
                                useMenpower1
                            }
                        };

                        DateFile.instance.manpowerHomeUseList[homeMapPartId].Add(homeMapPlaceId, value);
                    }
                }
                else
                {
                    Dictionary<int, int> value2 = new Dictionary<int, int>
                    {
                        {
                            homeMapbuildingIndex,
                            useMenpower1
                        }
                    };

                    Dictionary<int, Dictionary<int, int>> value3 = new Dictionary<int, Dictionary<int, int>>
                    {
                        {
                            homeMapPlaceId,
                            value2
                        }
                    };

                    DateFile.instance.manpowerHomeUseList.Add(homeMapPartId, value3);
                }

                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 0, buildingId);
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 1, 1);
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 2, 1);
                DateFile.instance.SetHomeBuildingValue(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex, 3, buildingTime1);

                TipsWindow.instance.SetTips(10, new string[]
                {
                    DateFile.instance.GetBuildingTipName(homeMapPartId, homeMapPlaceId),
                    DateFile.instance.basehomePlaceDate[buildingId][0]
                }, 180, 225f, 153f, 450, 100);

                //不执行UpdateHomePlace/UpdatePlaceSize方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);
                //HomeSystemWindow.Instance.UpdatePlaceSize(homeMapPartId, homeMapPlaceId);
                //补上本MOD地点建筑的图标更新
                UpdateSingleBuildingSupport.UpdateSingle(homeMapPartId, homeMapPlaceId, homeMapbuildingIndex);

                HomeSystem.instance.UpdateHomeSize(homeMapPartId, homeMapPlaceId);
                __instance.buildingTyp[1].GetComponent<Toggle>().interactable = false;
                __instance.buildingTyp[0].GetComponent<Toggle>().isOn = true;
                __instance.GetBuildingMassage();
                GEvent.OnEvent(eEvents.ManpowerChange, Array.Empty<object>());
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "Study")]
    public static class PreventInStudy
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool StudyPrefix(BuildingWindow __instance)
        //原方法签名
        //public void Study()
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                //若不满足原代码中“将会刷新产业地图建筑图标”的条件
                if (__instance.studySkillTyp == 0 || Traverse.Create(__instance).Field<int>("studyChooseTyp").Value == 1)
                {
                    //继续执行原方法
                    return true;
                }

                #region 原方法代码段(精简)

                Traverse traverse = Traverse.Create(__instance);

                //挖掘私有字段 BuildingWindow.instance.useItemId
                int useItemId = traverse.Field<int>("useItemId").Value;

                traverse.Field<int>("studySkillId").Value = traverse.Field<int>("chooseStudyId").Value;
                __instance.UpdateStudySkillWindow();
                //不执行UpdateHomePlace方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(HomeSystem.instance.homeMapPartId, HomeSystem.instance.homeMapPlaceId, HomeSystem.instance.homeMapbuildingIndex);
                //补上本MOD地点建筑的图标更新（似乎没有必要）
                //UpdateSingleBuildingSupport.UpdateSingle(HomeSystem.instance.homeMapPartId, HomeSystem.instance.homeMapPlaceId, HomeSystem.instance.homeMapbuildingIndex);

                __instance.UpdateButtonText();

                __instance.CloseSetStudyWindow();
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }

    /// <summary>
    /// 当不处于产业地图中、则不再更新产业地图的建筑图标
    /// </summary>
    [HarmonyPatch(typeof(BuildingWindow), "UpdateWorkingActor")]
    public static class PreventInUpdateWorkingActor
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPriority(Priority.First)]
        [HarmonyPrefix]
        private static bool UpdateWorkingActorPrefix(BuildingWindow __instance, int partId, int placeId, int buildingIndex)
        //原方法签名
        //public void UpdateWorkingActor(int partId, int placeId, int buildingIndex)
        {
            //处于产业地图中时
            if (HomeSystemWindow.Exists)
            {
                //继续执行原方法
                return true;
            }
            else
            {
                #region 原方法代码段

                int[] array = DateFile.instance.homeBuildingsDate[partId][placeId][buildingIndex];
                bool flag = int.Parse(DateFile.instance.basehomePlaceDate[array[0]][83]) > 0;
                __instance.noActorBack.SetActive(true);
                __instance.actorFaceBack.SetActive(false);

                if (DateFile.instance.actorsWorkingDate.ContainsKey(partId) && DateFile.instance.actorsWorkingDate[partId].ContainsKey(placeId) && DateFile.instance.actorsWorkingDate[partId][placeId].ContainsKey(buildingIndex))
                {
                    __instance.noActorBack.SetActive(false);
                    __instance.actorFaceBack.SetActive(true);
                }

                __instance.samsaraChangeText.gameObject.SetActive(flag);
                int num = 0;
                int num2 = 0;
                int num3 = int.Parse(DateFile.instance.basehomePlaceDate[array[0]][62]);
                __instance.buildingMassageText.text = DateFile.instance.massageDate[7019][4];

                if (__instance.actorFaceBack.activeSelf)
                {
                    int num4 = DateFile.instance.actorsWorkingDate[partId][placeId][buildingIndex];
                    __instance.homeActorFace.SetActorFace(num4, false);
                    string text = DateFile.instance.GetActorName(num4, false, false);
                    num = DateFile.instance.GetActorFavor(false, DateFile.instance.MianActorID(), num4, false, num3 > 0);
                    __instance.actorName.text = text;
                    __instance.actorGoodness.text = DateFile.instance.massageDate[9][0].Split('|')[DateFile.instance.GetActorGoodness(num4)];
                    __instance.actorMood.text = DateFile.instance.Color2(DateFile.instance.GetActorDate(num4, 4, false));
                    __instance.actorFavor.text = ((num != -1) ? DateFile.instance.Color5(num, true, -1) : DateFile.instance.SetColoer(20002, DateFile.instance.massageDate[303][2], false));
                    __instance.actorSamsara.text = DateFile.instance.GetLifeDateList(num4, 801, false).Count.ToString();
                    __instance.actorAge.text = DateFile.instance.ShowActorAge(num4);
                    __instance.actorFame.text = DateFile.instance.Color7(DateFile.instance.GetActorFame(num4));
                    __instance.actorFameIcon.name = "FameIcon,19," + num4;
                    __instance.actorFaovrIcon.name = "FavorIcon,21," + num4;
                    int num5 = DateFile.instance.Health(num4);
                    int num6 = DateFile.instance.MaxHealth(num4);
                    __instance.actorHealth.text = string.Format("{0}{1}</color> / {2}", DateFile.instance.Color3(num5, num6), num5, num6);
                    __instance.actorHealthBar.fillAmount = Mathf.Clamp((float)num5 / (float)num6, 0f, 1f);

                    if (num3 > 0)
                    {
                        __instance.favorBack.SetActive(false);
                        __instance.samsaraBack.SetActive(true);
                        num2 = DateFile.instance.GetFavorChange(num4, array[0], array[1]);
                        int num7 = DateFile.instance.GetActorGoodness(DateFile.instance.MianActorID());
                        int num8 = DateFile.instance.GetActorGoodness(num4);
                        int num9 = 1;

                        if (num7 == num8)
                        {
                            num9 = 0;
                        }
                        else
                        {
                            if (((num8 == 1 || num8 == 2) && (num7 == 3 || num7 == 4)) || ((num7 == 1 || num7 == 2) && (num8 == 3 || num8 == 4)))
                            {
                                num9 = 2;
                            }
                        }

                        __instance.buildingMassageText.text = string.Format("{0}{1}\n{2}{3}", new object[]
                        {
                            DateFile.instance.SetColoer(10002, text, false),
                            DateFile.instance.massageDate[7019][1].Split('|')[num9],
                            DateFile.instance.SetColoer(10002, text, false),
                            DateFile.instance.massageDate[7019][0].Split('|')[DateFile.instance.GetActorMoodLevel(num4)]
                        });
                    }
                    else
                    {
                        __instance.favorBack.SetActive(true);
                        __instance.samsaraBack.SetActive(false);
                    }
                }

                if (num3 > 0)
                {
                    __instance.skillTypBack.SetActive(false);
                    __instance.favorChangeBack.SetActive(true);
                    __instance.buildingPctNameText.text = DateFile.instance.massageDate[7017][2].Split('|')[1];
                    __instance.favorBar1.fillAmount = (float)num / 30000f;
                    __instance.favorBar2.fillAmount = (float)(num - 30000) / 30000f;
                    __instance.favorChangeText.text = string.Format("{0} (+{1} / {2})", DateFile.instance.Color5(num, false, -1), num2, DateFile.instance.massageDate[7006][1]);
                }
                else
                {
                    if (flag)
                    {
                        __instance.skillTypBack.SetActive(false);
                        __instance.favorChangeBack.SetActive(false);
                        int num10 = 0;

                        if (__instance.actorFaceBack.activeSelf)
                        {
                            int actorId = DateFile.instance.actorsWorkingDate[partId][placeId][buildingIndex];
                            num10 = Mathf.Min(DateFile.instance.GetLifeDateList(actorId, 801, false).Count + 1, 12);
                        }

                        string text2;

                        if (num10 > 0)
                        {
                            text2 = StringCenter.GetFormat("UI_BUILDING_SAMSARA_UP_VALUE", 1 + "~" + num10);
                        }
                        else
                        {
                            text2 = DateFile.instance.SetColoer(10001, StringCenter.GetFormat("UI_BUILDING_SAMSARA_UP_VALUE", 0), false);
                        }

                        __instance.buildingPctNameText.text = StringCenter.Get("UI_BUILDING_SAMSARA_TIP");
                        __instance.buildingMassageText.text = StringCenter.GetFormat("UI_SAMSARA_PLATFORM_BUILDING_MSG", DateFile.instance.SetColoer(10002, DateFile.instance.GetNewMapDate(partId, placeId, 0), false));
                        __instance.samsaraChangeText.text = text2;
                    }
                    else
                    {
                        __instance.favorChangeBack.SetActive(false);
                        __instance.buildingPctNameText.text = DateFile.instance.massageDate[7017][2].Split('|')[0];

                        __instance.buildingMassageText.text = string.Concat(new string[]
                        {
                            DateFile.instance.SetColoer(10002, DateFile.instance.GetNewMapDate(partId, placeId, 0), false),
                            DateFile.instance.SetColoer(10003, DateFile.instance.basehomePlaceDate[array[0]][0], false),
                            DateFile.instance.massageDate[3005][1].Split('|')[0],
                            DateFile.instance.SetColoer(10002, DateFile.instance.GetNewMapDate(partId, placeId, 98), false),
                            DateFile.instance.massageDate[7019][3].Split('|')[DateFile.instance.GetHomeShopLevel(partId, placeId, buildingIndex)]
                        });

                        int num11 = int.Parse(DateFile.instance.basehomePlaceDate[array[0]][33]);

                        if (num11 > 0)
                        {
                            __instance.skillTypBack.SetActive(true);
                            __instance.buildingSkillIcon.name = "ActorAttr," + num11;
                            int num12 = int.Parse(DateFile.instance.basehomePlaceDate[array[0]][91]);

                            if (num12 > 0)
                            {
                                __instance.buildingPctBar.fillAmount = (float)(array[11] / num12);
                                int num13 = 0;

                                if (__instance.actorFaceBack.activeSelf)
                                {
                                    num13 = DateFile.instance.GetBuildingLevelPct(partId, placeId, buildingIndex);
                                    string text3 = DateFile.instance.GetActorName(DateFile.instance.actorsWorkingDate[partId][placeId][buildingIndex], false, false);
                                    Text text4 = __instance.buildingMassageText;
                                    text4.text += string.Format("\n{0}{1}", DateFile.instance.SetColoer(10002, text3, false), DateFile.instance.massageDate[7019][2].Split('|')[(num13 >= 100) ? 0 : 1]);
                                }
                                __instance.skillLevelText.text = string.Format("{0}% (+{1}% / {2})", array[11] * 100 / num12, num13 * 100 / num12, DateFile.instance.massageDate[7006][1]);
                            }
                        }
                        else
                        {
                            __instance.skillTypBack.SetActive(false);
                        }
                    }
                }

                //不执行UpdateHomePlace方法（主要是为了避免调用HomeSystemWindow.Instance）
                //HomeSystemWindow.Instance.UpdateHomePlace(partId, placeId, buildingIndex);
                //补上本MOD地点建筑的图标更新
                UpdateSingleBuildingSupport.UpdateSingle(partId, placeId, buildingIndex);
                #endregion

                //最后跳过原方法不再执行
                return false;
            }
        }
    }
}
