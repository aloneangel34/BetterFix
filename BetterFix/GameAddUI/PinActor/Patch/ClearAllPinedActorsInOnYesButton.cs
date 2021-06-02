using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace BetterFix
{
    /// <summary>
    /// 清空关注人物
    /// </summary>
    [HarmonyPatch(typeof(YesOrNoWindow), "OnYesButton")]
    public static class ClearAllPinedActorsInOnYesButton
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void OnYesButtonPostfix(YesOrNoWindow __instance)
        //原方法签名
        //public void OnYesButton()
        {
            if (Main.Setting.UiPinActors.Value)
            {
                if (OnClick.instance.ID == CreatePinToggleNpcSearchAwake.ClearAllPinedClickId)
                {
                    PinAcotrSupport.ClearThisSaveSortAllPinedActors();
                    OnClick.instance.ID = -1;
                    //刷新人物列表
                    GameObject ui_NpcSearch = GameObject.Find("UIRoot/Canvas/UIPopup/ui_NpcSearch");
                    if (ui_NpcSearch != null)
                    {
                        CToggle villageToggle = ui_NpcSearch.GetComponent<ui_NpcSearch>().CGet<CToggle>("VillageToggle");
                        villageToggle.group.SetAllTogglesOff();
                        villageToggle.isOn = true;
                    }
                }
            }
        }
    }
}
