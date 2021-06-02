using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;
using UnityUIKit.Components;
using UnityUIKit.Core.GameObjects;
using BepInEx.Logging;
using GameData;

namespace BetterFix
{
    /// <summary>
    /// 人物关注锁的支援方法集
    /// </summary>
    public static class PinAcotrSupport
    {
        /// <summary>
        /// 用于“显示全部已关注人物”的排序方法
        /// </summary>
        /// <param name="actorId1">人物ID一</param>
        /// <param name="actorId2">人物ID二</param>
        /// <returns>比较结果（排序依据）</returns>
        public static int NormalCompareActor(int actorId1, int actorId2)
        {
            int taiwuActorId = DateFile.instance.MianActorID();

            //人物一是否为紫竹化身
            bool isActor1stZiZhuChild = int.Parse(DateFile.instance.GetActorDate(actorId1, 8, true)) == 4;
            //人物二是否为紫竹化身
            bool isActor2ndZiZhuChild = int.Parse(DateFile.instance.GetActorDate(actorId2, 8, true)) == 4;
            //人物一身份阶级
            int actor1stLevel = Mathf.Abs(int.Parse(DateFile.instance.GetActorDate(actorId1, 20, false)));
            //人物二身份阶级
            int actor2ndLevel = Mathf.Abs(int.Parse(DateFile.instance.GetActorDate(actorId2, 20, false)));
            //人物一好感
            int actor1stFavor = DateFile.instance.GetActorFavor(false, taiwuActorId, actorId1, false, false);
            //人物二好感
            int actor2ndFavor = DateFile.instance.GetActorFavor(false, taiwuActorId, actorId2, false, false);
            //人物一年龄
            int actor1stAge = int.Parse(DateFile.instance.GetActorDate(actorId1, 11, false));
            //人物二年龄
            int actor2ndAge = int.Parse(DateFile.instance.GetActorDate(actorId2, 11, false));

            //顺位差
            int result;

            //若其中一人为紫竹化身、另一个不为紫竹
            if (isActor1stZiZhuChild != isActor2ndZiZhuChild)
            {
                //顺位差：人物一若为紫竹则顺位差为-1（否则为1）
                result = (isActor1stZiZhuChild ? -1 : 1);
            }
            //若两人身份阶级不同
            else if (actor1stLevel != actor2ndLevel)
            {
                //顺位差：人物一的身份阶级 - 人物二的身份阶级（身份阶级顺序 九品至一品 9～1）
                result = actor1stLevel - actor2ndLevel;
            }
            //若两人身份好感度不同
            else if (actor1stFavor != actor2ndFavor)
            {
                //顺位差：人物二的好感度 - 人物一的好感度（好感度倒序）
                result = actor2ndFavor - actor1stFavor;
            }
            //最后按身份排序
            else
            {
                //顺位差：人物二的年龄 - 人物一的年龄（年龄倒序）
                result = actor2ndAge - actor1stAge;
            }

            //返回结果
            return result;
        }

        /// <summary>
        /// 依照关注与否将人物列表再次排序（被关注人物置顶显示）
        /// </summary>
        /// <param name="actorIdList">人物ID一</param>
        /// <param name="actorId2">人物ID二</param>
        /// <returns>比较差值</returns>
        public static void PinedSort(List<int> actorIdList)
        {
            int n = 0;
            for (int i = actorIdList.Count - 1; i >= n; i--)
            {
                int actorId = actorIdList[i];
                if (IsActorAlreadyPined(actorId))
                {
                    actorIdList.RemoveAt(i);
                    actorIdList.Insert(0, actorId);
                    n++;
                    i++;
                }
            }
        }


        /// <summary>
        /// 判断人物ID是否处于关注列表中（自动判断存档栏位）
        /// </summary>
        /// <param name="actorId">要判断的人物ID</param>
        /// <returns>true:（本存档栏位）已关注该人物 false:（本存档栏位）未关注</returns>
        public static bool IsActorAlreadyPined(int actorId)
        {
            bool result = false;

            switch (SaveDateFile.instance.dateId)
            {
                case 1:
                    result = Main.Setting.RecordFirstSaveSlotPinActorIds.Value.Contains(actorId);
                    break;
                case 2:
                    result = Main.Setting.RecordSecondSaveSlotPinActorIds.Value.Contains(actorId);
                    break;
                case 3:
                    result = Main.Setting.RecordThirdSaveSlotPinActorIds.Value.Contains(actorId);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 将人物ID录入已关注人物列表（自动判断存档栏位）
        /// </summary>
        /// <param name="actorId">要录入的人物ID</param>
        public static void AddActorToPinedActorIdList(int actorId)
        {
            #region 暂时用不到
#if false
            QuickLogger.Log(BepInEx.Logging.LogLevel.Info, "设置关注 存档栏位:{0} 关注人物{1}({2})",
                //{0}存档栏位
                SaveDateFile.instance.dateId,
                //{1}人物姓名
                DateFile.instance.GetActorName(actorId),
                //{2}人物ID
                actorId
                );
#endif
            #endregion

            switch (SaveDateFile.instance.dateId)
            {
                case 1:
                    if (!Main.Setting.RecordFirstSaveSlotPinActorIds.Value.Contains(actorId))
                    {
                        Main.Setting.RecordFirstSaveSlotPinActorIds.Value.Add(actorId);
                        //更新（必须用新建列表的方式来变更引用地址，否则BepInEx不会触发Config的保存动作）
                        //由于List<int>是引用类型，上述的操作并不会触发ConfigEntry<T>中的OnSettingChanged方法，所以需要重新设定来触发更新
                        Main.Setting.RecordFirstSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordFirstSaveSlotPinActorIds.Value);
                    }
                    break;
                case 2:
                    if (!Main.Setting.RecordSecondSaveSlotPinActorIds.Value.Contains(actorId))
                    {
                        Main.Setting.RecordSecondSaveSlotPinActorIds.Value.Add(actorId);
                        //更新
                        Main.Setting.RecordSecondSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordSecondSaveSlotPinActorIds.Value);
                    }
                    break;
                case 3:
                    if (!Main.Setting.RecordThirdSaveSlotPinActorIds.Value.Contains(actorId))
                    {
                        Main.Setting.RecordThirdSaveSlotPinActorIds.Value.Add(actorId);
                        //更新
                        Main.Setting.RecordThirdSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordThirdSaveSlotPinActorIds.Value);
                    }
                    break;
            }
        }

        /// <summary>
        /// 将人物ID移出关注列表（自动判断存档栏位）
        /// </summary>
        /// <param name="actorId">要移出的人物ID</param>
        public static void RemoveActorFromPinedActorIdList(int actorId)
        {
            #region 暂时用不到
#if false
            QuickLogger.Log(BepInEx.Logging.LogLevel.Info, "取消关注 存档栏位:{0} 取消人物{1}({2})",
                //{0}存档栏位
                SaveDateFile.instance.dateId,
                //{1}人物姓名
                DateFile.instance.GetActorName(actorId),
                //{2}人物ID
                actorId
                );
#endif
            #endregion

            switch (SaveDateFile.instance.dateId)
            {
                case 1:
                    Main.Setting.RecordFirstSaveSlotPinActorIds.Value.RemoveAll(n => n == actorId);
                    //更新（必须用新建列表的方式来变更引用地址，否则BepInEx不会触发Config的保存动作）
                    Main.Setting.RecordFirstSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordFirstSaveSlotPinActorIds.Value);
                    break;
                case 2:
                    Main.Setting.RecordSecondSaveSlotPinActorIds.Value.RemoveAll(n => n == actorId);
                    //更新
                    Main.Setting.RecordSecondSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordSecondSaveSlotPinActorIds.Value);
                    break;
                case 3:
                    Main.Setting.RecordThirdSaveSlotPinActorIds.Value.RemoveAll(n => n == actorId);
                    //更新
                    Main.Setting.RecordThirdSaveSlotPinActorIds.Value = new List<int>(Main.Setting.RecordThirdSaveSlotPinActorIds.Value);
                    break;
            }
        }

        /// <summary>
        /// 新建一个关注图标按钮
        /// </summary>
        /// <param name="xOffset">按钮RectTransform的anchoredPosition.x</param>
        /// <param name="yOffset">按钮RectTransform的anchoredPosition.y</param>
        /// <returns>关注图标按钮的RectTransform</returns>
        public static RectTransform NewPinToggle(int xOffset = 0, int yOffset = 0)
        {
            PinToggle pinToggle = new PinToggle
            {
                Name = "BetterFix.PinToggle",
                TipTitle = "人物关注锁",
                TipContant = "关注人物后，该人物将在地点人物和人物名册界面（游戏原生的人物搜索功能，按钮在右下）的列表中置顶显示",
            };
            pinToggle.RectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            return pinToggle.RectTransform;
        }

        /// <summary>
        /// 尝试从组件名字获取中获取人物ID
        /// </summary>
        /// <param name="name">组件名字</param>
        /// <param name="actorId">返回的人物ID（获取失败时返回0）</param>
        /// <returns>是否获取成功</returns>
        public static bool TryGetActorIdFromName(string name, out int actorId)
        {
            actorId = 0;
            string[] nameList = name.Split(',');
            if (nameList.Length > 1 && int.TryParse(nameList[1], out actorId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 人物关注锁图标（开关）在被点击时的事件
        /// </summary>
        /// <param name="isOn">点击后的开关状态</param>
        /// <param name="tg">UnityEngine.UI的Toggle组件</param>
        public static void PinOnValueChanged_Invoke(bool isOn, UnityEngine.UI.Toggle tg)
        {
            if (Main.Setting.ModEnable.Value && Main.Setting.UiPinActors.Value)
            {
                if (PinAcotrSupport.TryGetActorIdFromName(tg.transform.parent.name, out int actorId))
                {
                    if (isOn)
                    {
                        //开启时添加关注
                        PinAcotrSupport.AddActorToPinedActorIdList(actorId);
                    }
                    else
                    {
                        //关闭时移除关注
                        PinAcotrSupport.RemoveActorFromPinedActorIdList(actorId);
                    }
                }
                else
                {
                    QuickLogger.Log(LogLevel.Warning, "关注人物锁 无法获取人物ID name:{0}", tg.transform.parent.name);
                }
            }
        }

        /// <summary>
        /// 获取对应当前存档栏位的有效已关注人物（不含当前数据中不存在的人物）
        /// </summary>
        /// <returns>当前存档栏位的有效已关注人物ID列表</returns>
        public static List<int> GetThisSaveSortValidPinedActors()
        {
            List<int> result = new List<int>();
            switch (SaveDateFile.instance.dateId)
            {
                case 1:
                    result = GetOnlyExsitActorList(Main.Setting.RecordFirstSaveSlotPinActorIds.Value);
                    break;
                case 2:
                    result = GetOnlyExsitActorList(Main.Setting.RecordSecondSaveSlotPinActorIds.Value);
                    break;
                case 3:
                    result = GetOnlyExsitActorList(Main.Setting.RecordThirdSaveSlotPinActorIds.Value);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 从给定列表中获取有效的人物ID列表（剔除当前数据中不存在的人物ID）
        /// </summary>
        /// <param name="actorIdList">需处理的人物ID列表</param>
        /// <returns>有效的人物ID列表</returns>
        public static List<int> GetOnlyExsitActorList(List<int> actorIdList)
        {
            List<int> result = new List<int>();

            if (actorIdList == null)
            {
                return result;
            }

            for (int i = 0; i < actorIdList.Count; i++)
            {
                if (Characters.HasChar(actorIdList[i]))
                {
                    result.Add(actorIdList[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// 清空当前存档栏位所对应的所有已关注人物（会保存最后一次清空的数据，以便手动还原）
        /// </summary>
        internal static void ClearThisSaveSortAllPinedActors()
        {
            switch (SaveDateFile.instance.dateId)
            {
                case 1:
                    //更新（必须用新建列表的方式来变更引用地址，否则BepInEx不会触发Config的保存动作）
                    Main.Setting.LastClearedPinActorIdsBackup.Value = new List<int>(Main.Setting.RecordFirstSaveSlotPinActorIds.Value);
                    Main.Setting.RecordFirstSaveSlotPinActorIds.Value = new List<int>();
                    break;
                case 2:
                    //更新
                    Main.Setting.LastClearedPinActorIdsBackup.Value = new List<int>(Main.Setting.RecordSecondSaveSlotPinActorIds.Value);
                    Main.Setting.RecordSecondSaveSlotPinActorIds.Value = new List<int>();
                    break;
                case 3:
                    //更新
                    Main.Setting.LastClearedPinActorIdsBackup.Value = new List<int>(Main.Setting.RecordThirdSaveSlotPinActorIds.Value);
                    Main.Setting.RecordThirdSaveSlotPinActorIds.Value = new List<int>();
                    break;
            }

            //QuickLogger.Log(BepInEx.Logging.LogLevel.Info, "清空关注 存档栏位:{0} 备份被清空的关注人物ID数量:{1}", SaveDateFile.instance.dateId, Main.Setting.LastClearedPinActorIdsBackup.Value.Count);
        }
    }
}
