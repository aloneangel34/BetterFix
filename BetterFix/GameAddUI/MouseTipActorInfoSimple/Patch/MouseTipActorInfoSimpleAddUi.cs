using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 人物名册浮动信息：加入额外信息的UI
    /// </summary>
    [HarmonyPatch(typeof(MouseTipActorInfoSimple), "UpdateContentAndSize")]
    public static class MouseTipActorInfoSimpleAddUi
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="args">鼠标滑过传入的参数组</param>
        [HarmonyPostfix]
        private static void UpdateContentAndSizePostfix(MouseTipActorInfoSimple __instance, params object[] args)
        //原方法签名
        //protected override void UpdateContentAndSize(params object[] args)
        {
            if (Main.Setting.UiMouseActorSimpleAdditionInfo.Value)
            {
                //显示对象的人物ID
                int actorId = (int)args[0];

                //加入窗体大小自动适应组件
                ContentSizeFitter tipSizeFitter = __instance.GetComponent<ContentSizeFitter>();
                if (tipSizeFitter == null)
                {
                    __instance.gameObject.AddComponent<ContentSizeFitter>();
                    tipSizeFitter = __instance.GetComponent<ContentSizeFitter>();
                    tipSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    tipSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                }

                //若还未创建则创建UI
                Transform additionInfoTran = __instance.transform.Find("BetterFix.AdditionInfoUI");
                if (additionInfoTran == null)
                {
                    additionInfoTran = CreateUIForMouseTip.CreateAdditionInfoUI().transform;
                    additionInfoTran.SetParent(__instance.transform, false);
                }

                //若处于隐藏则显示
                if (!additionInfoTran.gameObject.activeSelf)
                {
                    additionInfoTran.gameObject.SetActive(true);
                }

                //更新信息
                AdditionInfo additionInfoSet = additionInfoTran.GetComponent<AdditionInfo>();
                additionInfoSet.SetAdditionInfo(actorId);

            }
        }
    }
}
