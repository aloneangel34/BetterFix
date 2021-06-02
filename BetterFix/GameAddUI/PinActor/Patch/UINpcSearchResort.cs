using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;

namespace BetterFix
{
    /// <summary>
    /// 在游戏原生NPC查找功能（人物名单）窗口中置顶显示人物：检测部分
    /// </summary>
    [HarmonyPatch(typeof(ui_NpcSearch), "UpdateNpcList")]
    public static class NpcSearchPinedActorCheck
    {
        internal static bool NeedFix = false;

        /// <summary>
        /// 原方法调用前设为需要修正
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="scrollToTop">是否滚动至滚动显示区域的最上方</param>
        [HarmonyPrefix]
        private static void UpdateNpcListPrefix(ui_NpcSearch __instance, bool scrollToTop)
        //原方法签名
        //public void UpdateNpcList(bool scrollToTop = true)
        {
            //设为需要修正
            NeedFix = true;
        }

#if false
        /// <summary>
        /// 原方法调用后重置为不需要修正
        /// </summary>
        [HarmonyPostfix]
        private static void UpdateNpcListPostfix()
        //原方法签名
        //public void UpdateNpcList(bool scrollToTop = true)
        {
            //重置为不需要修正
            NeedFix = false;
        }
#endif
    }

    /// <summary>
    /// 在游戏原生NPC查找功能（人物名单）窗口中置顶显示人物：执行重新排序部分
    /// </summary>
    [HarmonyPatch(typeof(InfinityScroll), "UpdateData")]
    public static class NpcSearchPinedActorSort
    {
        //internal static bool ShowOnlyPinedActor = false;

        /// <summary>
        /// 在原方法执行前，将要显示的人物ID列表重新排序（所有置顶人物前置）
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="newCount">无限滚动区域要显示的新内容个数</param>
        [HarmonyPrefix]
        private static void UpdateDataPrefix(InfinityScroll __instance, ref int newCount)
        //原方法签名
        //public void UpdateData(int newCount)
        {
            //若功能开启 且 需要修正
            if (Main.Setting.UiPinActors.Value && NpcSearchPinedActorCheck.NeedFix)
            {
                //获取游戏原生NPC查找功能（人物名单）窗口中，要显示的人物ID列表
                List<int> uiNpcSearchShowingActorIdList = Traverse.Create(CreatePinToggleNpcSearchAwake.RecordInstance).Field<List<int>>("showingActorIdList").Value;
                PinAcotrSupport.PinedSort(uiNpcSearchShowingActorIdList);

                #region 弃用（挪至别处实现）
                //if (ShowOnlyPinedActor)
                //{
                //    ShowOnlyPinedActor = false;
                //    uiNpcSearchShowingActorIdList.Clear();
                //    uiNpcSearchShowingActorIdList.AddRange(PinAcotrSupport.GetThisSaveSortValidPinedActors());
                //    uiNpcSearchShowingActorIdList.Sort(new Comparison<int>(PinAcotrSupport.NormalCompareActor));
                //    newCount = uiNpcSearchShowingActorIdList.Count;
                //}
                //else
                //{
                //    PinAcotrSupport.PinedSort(uiNpcSearchShowingActorIdList);
                //}
                #endregion
            }

            //不论如何重置
            NpcSearchPinedActorCheck.NeedFix = false;
        }
    }
}
