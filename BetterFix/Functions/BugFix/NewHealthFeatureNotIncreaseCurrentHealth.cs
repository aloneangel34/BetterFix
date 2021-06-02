using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameData;
using HarmonyLib;
using AI;
using BepInEx.Logging;
using UnityEngine;
using System.Collections;

namespace BetterFix
{
    /// <summary>
    /// 在人物年龄增长时，记录变更数据
    /// </summary>
    [HarmonyPatch(typeof(CharacterAge), "IncreaseAge")]
    public static class NewHealthFeatureActorDataRecord
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__result">原方法的返回的人物新的年龄</param>
        /// <param name="actorId">人物ID</param>
        /// <param name="age">人物年龄</param>
        /// <param name="mainActorId">太吾人物ID</param>
        /// <param name="modifiedData">AgeChangeModifiedData实例</param>
        [HarmonyPostfix]
        private static void IncreaseAgePostfix(int actorId, int age)
        //原方法签名
        //private static int IncreaseAge(int actorId, int age, int mainActorId, AgeChangeModifiedData modifiedData)
        {
            //所有年龄增长人物都需要记录（用来清空对应的人物特性缓存）
            HealthFeatureSupport.AgeChangedActorIds.Add(actorId);

            //若 人物原本年龄未满15岁 且 人物为实际人物 且 功能开关开启
            if (age <= 14 && int.Parse(DateFile.instance.GetActorDate(actorId, 8, false)) == 1 && Main.Setting.BugFixNewHealthyFeatureCauseHealthNotFull.Value)
            {
                //获取人物的特性ID列表（不触发特性缓存）
                List<int> actorFeatures = HealthFeatureSupport.GetFeatureListFromString(DateFile.instance.GetActorDate(actorId, 101, false));
                //人物的可揭示特性ID列表
                List<int> canHideAndShowWithAgeFeatures = new List<int>();

                //更新人物的可揭示特性ID列表
                foreach (int featureId in actorFeatures)
                {
                    if (int.Parse(DateFile.instance.actorFeaturesDate[featureId][8]) > 0)
                    {
                        canHideAndShowWithAgeFeatures.Add(featureId);
                    }
                }

                //总可揭示特性数量
                int displayCountAll = canHideAndShowWithAgeFeatures.Count;

                //年龄增长前的可揭示特性数量
                int displayCountBeforeGrowUp = Mathf.Clamp(age * 1000 / (14000 / Mathf.Max(displayCountAll, 1)), 0, displayCountAll);
                //年龄增长后的可揭示特性数量
                int displayCountAfterGrowUp = (age == 14) ? displayCountAll : Mathf.Clamp((age + 1) * 1000 / (14000 / Mathf.Max(displayCountAll, 1)), 0, displayCountAll);

                #region 暂时不需要
                //调试信息
                //if (Main.Setting.debugMode.Value)
                //{
                //    FeatureSupport.ActorFeaturesCacheResetInAllThread(actorId, true);
                //    QuickLogger.Log(LogLevel.Info, "计算人物健康修正 {0}({1}) 可揭示特性数量（原本{3}->现在{4}/总{2}） 年龄{5}", DateFile.instance.GetActorName(actorId), actorId, displayCountAll, displayCountBeforeGrowUp, displayCountAfterGrowUp, age);
                //}
                #endregion

                //人物因揭示特性而变动的健康上限变动总值
                int totalMaxHealthChangeAmount = 0;

                //根据新揭示特性来计算健康上限变动总值
                for (int i = displayCountBeforeGrowUp; i < displayCountAfterGrowUp; i++)
                {
                    //该特性对健康上限的影响值
                    int featureMaxHealthChangeAmount;
                    if (int.TryParse(DateFile.instance.actorFeaturesDate[canHideAndShowWithAgeFeatures[i]][50013], out featureMaxHealthChangeAmount))
                    {
                        if (featureMaxHealthChangeAmount != 0)
                        {
                            #region 暂时不需要
                            //调试信息
                            //if (Main.Setting.debugMode.Value)
                            //{
                            //    QuickLogger.Log(LogLevel.Info, "计算人物健康修正 {0}({1}) 揭示特性{2}({3}) 上限变动:{4}", DateFile.instance.GetActorName(actorId), actorId, DateFile.instance.actorFeaturesDate[canHideAndShowWithAgeFeatures[i]][0], canHideAndShowWithAgeFeatures[i], featureMaxHealthChangeAmount);
                            //}
                            #endregion

                            //更新“人物因揭示特性而变动的健康上限变动总值”
                            totalMaxHealthChangeAmount += featureMaxHealthChangeAmount;
                        }
                    }
                }

                //若 人物因揭示特性而变动的健康上限变动总值为正
                if (totalMaxHealthChangeAmount > 0)
                {
                    //调试信息
                    if (Main.Setting.debugMode.Value)
                    {
                        QuickLogger.Log(LogLevel.Info, "检测到需要修正健康的人物 {0}({1}) 原年龄:{2} 揭示特性健康上限增加:{3}",
                            //{0}人物姓名
                            DateFile.instance.GetActorName(actorId),
                            //{1}人物ID
                            actorId,
                            //{2}原年龄
                            age,
                            //{3}因新揭示特性健康上限增加
                            totalMaxHealthChangeAmount
                            );
                    }

                    //记录需要重设健康值的条目数据（Key:人物ID，Value:健康上限上调值）
                    //（不能在此处直接上调健康值）
                    HealthFeatureSupport.MaxHealthyChangedActorIds[actorId] = totalMaxHealthChangeAmount;
                }
            }
        }
    }

    /// <summary>
    /// 在执行人物年龄变更计算前，清空变更数据
    /// </summary>
    [HarmonyPatch(typeof(CharacterAge), "CollectModifications")]
    public static class NewHealthFeatureActorDataClear
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solarTerms">时节</param>
        /// <param name="mainActorId">太吾人物ID</param>
        [HarmonyPrefix]
        private static void CollectModificationsPrefix()
        //原方法签名
        //private static AgeChangeModifiedData[] CollectModifications(int solarTerms, int mainActorId)
        {
            HealthFeatureSupport.AgeChangedActorIds.Clear();
            HealthFeatureSupport.MaxHealthyChangedActorIds.Clear();
        }
    }

    /// <summary>
    /// 在过月后应用更改变更数据
    /// </summary>
    [HarmonyPatch(typeof(UIDate), "DoChangeTrun")]
    public static class NewHealthFeatureActorDataApply
    {
        /// <summary>
        /// 
        /// </summary>
        [HarmonyPrefix]
        private static void DoChangeTrunPrefix()
        //原方法签名
        //private IEnumerator DoChangeTrun(float waitTime)
        {
            if (Main.Setting.debugMode.Value)
            {
                QuickLogger.Log(LogLevel.Info, "StartYield: HealthFeatureSupport.ApplyChange()");
            }

            //调用应用更改方法（其会等待DoChangeTrun实际执行结束后、再应用变更）
            SingletonObject.getInstance<YieldHelper>().StartYield(HealthFeatureSupport.ApplyChange());
        }
    }
    
    /// <summary>
    /// 用于健康修正的支援方法集
    /// </summary>
    public static class HealthFeatureSupport
    {
        internal static List<int> AgeChangedActorIds = new List<int>();
        internal static Dictionary<int, int> MaxHealthyChangedActorIds = new Dictionary<int, int>();

        /// <summary>
        /// 在UIDate.instance.DoChangeTrun()结束后，重置“年龄增长了的人物”的人物特性缓存，并补偿“因新现实特性导致健康上限增加了的人物”的健康值
        /// </summary>
        /// <returns></returns>
        public static IEnumerator ApplyChange()
        {
            if (Main.Setting.debugMode.Value)
            {
                QuickLogger.Log(LogLevel.Info, "等待DoChangeTrun结束");
            }

            while (UIDate.instance.trunChangeing)
            {
                yield return null;
            }

            if (Main.Setting.debugMode.Value)
            {
                HealthFeatureSupport.ShowThreadForActorFeaturesCache(true);
                QuickLogger.Log(LogLevel.Info, "DoChangeTrun已结束 年龄增长人数:{0} / 需健康修正人数:{1}", AgeChangedActorIds.Count, MaxHealthyChangedActorIds.Keys.Count);
            }

            foreach (int actorId in AgeChangedActorIds)
            {
                DateFile.instance.ActorFeaturesCacheReset(actorId);
                //QuickLogger.Log(LogLevel.Info, "清除特性缓存 actorId:{0}", actorId);
            }

            foreach (var actorHealthData in MaxHealthyChangedActorIds)
            {
                //人物的新健康值：（过月后的原健康值 + 健康上限变动值）
                int actorNewHealth = DateFile.instance.Health(actorHealthData.Key) + actorHealthData.Value;
                //重设人物健康值
                DateFile.instance.ChangeActorHealth(actorHealthData.Key, actorNewHealth, false);

                if (Main.Setting.debugMode.Value)
                {
                    QuickLogger.Log(LogLevel.Info, "重设健康值 {0}({1}) 健康值变动:原本{2} -> 现在{3} (上调{4})",
                        //{0}人物姓名
                        DateFile.instance.GetActorName(actorHealthData.Key),
                        //{1}人物ID
                        actorHealthData.Key,
                        //{2}原本健康值
                        (actorNewHealth - actorHealthData.Value),
                        //{3}现在健康值
                        actorNewHealth,
                        //{4}变更值
                        actorHealthData.Value
                        );
                }
            }

            AgeChangedActorIds.Clear();
            MaxHealthyChangedActorIds.Clear();

            yield break;
        }

        /// <summary>
        /// 从字符串中获取有效的人物特性ID列表
        /// </summary>
        /// <param name="featureIdsString">将特性ID以“|”分隔的字符串（可使用DateFile.instance.GetActorDate(人物ID, 101, false)）</param>
        /// <returns>有效的人物特性ID列表</returns>
        public static List<int> GetFeatureListFromString(string featureIdsString)
        {
            List<int> result = new List<int>();

            if (!featureIdsString.IsNullOrEmpty())
            {
                string[] featureIdStrList = featureIdsString.Split('|');

                foreach (string s in featureIdStrList)
                {
                    if (int.TryParse(s, out int featureId) && featureId != 0 && DateFile.instance.actorFeaturesDate.ContainsKey(featureId))
                    {
                        result.Add(featureId);
                    }
                }
            }

            return result;
        }

        public static void ShowThreadForActorFeaturesCache(bool onlyShowCurrentThreadId, int actorId = 0)
        {
            if (onlyShowCurrentThreadId)
            {
                QuickLogger.Log(LogLevel.Info, "CurrentThread.ManagedThreadId:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            else
            {
                Dictionary<int, Dictionary<int, List<int>>> _actorsFeaturesCache = Traverse.Create(DateFile.instance).Field<Dictionary<int, Dictionary<int, List<int>>>>("_actorsFeaturesCache").Value;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("CurrentThread.ManagedThreadId:{1}\nManagedThreadId List:\nactorId:{0}", actorId, System.Threading.Thread.CurrentThread.ManagedThreadId);

                foreach (int threadId in _actorsFeaturesCache.Keys)
                {
                    stringBuilder.AppendFormat("\n| id:{0} have:{1}", threadId, _actorsFeaturesCache[threadId].ContainsKey(actorId));
                }

                QuickLogger.Log(LogLevel.Info, stringBuilder.ToString());
            }
        }
    }
}
