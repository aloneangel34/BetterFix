using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 不再发生NPC讨要事件
    /// </summary>
    [HarmonyPatch(typeof(MessageEventManager), "DoEvent")]
    public static class NpcAskingEventWillNotHappen
    {
        /// <summary>
        /// 原方法执行前，若判断为讨要事件、则移除事件不再执行
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="eventIndex">事件在eventId数据中的序号</param>
        /// <returns>是否继续执行原方法</returns>
        [HarmonyPrefix]
        private static bool DoEventPrefix(MessageEventManager __instance, int eventIndex)
        //原方法签名
        //private void DoEvent(int eventIndex)
        {
            //若开启了“不再发生NPC讨要事件”
            if (Main.Setting.NpcAskingEventWillNotHappen.Value)
            {
                //若事件下标超限
                if (DateFile.instance.eventId.Count - 1 < eventIndex)
                {
                    QuickLogger.Log(LogLevel.Info, "请求的事件下标({0})超出事件数据列表的元素个数({1})", eventIndex, DateFile.instance.eventId.Count);
                }
                //事件下标未超限
                else
                {
                    //该事件的事件信息列表（建立空数组）
                    int[] aiEventDate = new int[DateFile.instance.eventId[eventIndex].Length];
                    //为该事件的事件信息列表实际赋值（避免潜复制）
                    DateFile.instance.eventId[eventIndex].CopyTo(aiEventDate, 0);

                    //该事件的事件类型ID
                    int eventTypeId = 0;
                    //若列表长度至少为3
                    if (aiEventDate.Length >= 3)
                    {
                        //更新事件类型ID
                        eventTypeId = aiEventDate[2];
                    }

                    //若该事件为讨要事件
                    if (NpcAskingEventIds.ContainsKey(eventTypeId))
                    {
                        //调试信息
                        if (Main.Setting.debugMode.Value)
                        {
                            QuickLogger.Log(LogLevel.Info, "取消讨要事件 事件发起人 {0}({1}) 事件名:{2}",
                                //{0}事件发起人姓名
                                DateFile.instance.GetActorName(aiEventDate[1]),
                                //{1}事件发起人ID
                                aiEventDate[1],
                                //{2}事件名
                                NpcAskingEventIds[eventTypeId]
                                );
                        }

                        //从eventId数据中移除该事件
                        DateFile.instance.eventId.RemoveAt(eventIndex);
                        //跳过原方法不再执行
                        return false;
                    }
                }
            }

            //继续执行原方法
            return true;
        }

        /// <summary>
        /// 讨要事件的事件类型ID（键） 与 名称（值）【对应 Event_Date.txt 中的数据】
        /// </summary>
        public static readonly Dictionary<int, string> NpcAskingEventIds = new Dictionary<int, string>
        {
            { 207, "遭遇讨要食物"},
            { 208, "遭遇讨要补品"},
            { 209, "遭遇讨要资源"},
            { 218, "遭遇讨要外伤药"},
            { 219, "遭遇讨要内伤药"},
            { 220, "遭遇讨要解毒药"},
            { 221, "遭遇讨要毒药"},
            { 222, "遭遇讨要秘籍"},
            { 223, "遭遇讨要道具"},
            { 224, "遭遇讨教功法"},
            { 230, "遭遇讨要内息药"},
        };
    }
}
