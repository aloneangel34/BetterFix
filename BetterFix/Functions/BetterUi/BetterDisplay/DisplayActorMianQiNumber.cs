using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 显示人物内息紊乱具体数值：人物信息窗口——伤势界面的内息紊乱程度名称（UI标签）
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "UpdateMianQi")]
    public static class DisplayActorMianQiNumberInActorMenu
    {
        //internal static RectTransform ActorMenuActorMianQiBack = null;
        //internal static Transform ActorMenumianQiArrowLeft = null;
        //internal static Transform ActorMenumianQiArrowRight = null;
        //internal static bool IsModified = false;

        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="key">人物ID</param>
        /// <param name="cut"></param>
        [HarmonyPostfix]
        private static void UpdateMianQiPostfix(ActorMenu __instance, int key, int cut)
        //原方法签名
        //public void UpdateMianQi(int key, int cut)
        {
            if (Main.Setting.DisplayActorMianQiNumber.Value)
            {
                //UI中再加三位数的数字，刚好能勉强挤下
                //但是要显示精准数值的话，需要扩大UI
                __instance.actorMianQiBack.sizeDelta = new Vector2(100f, 30f);
                __instance.mianQiArrowLeft.transform.localPosition = new Vector3(-50f, 0f, 0f);
                __instance.mianQiArrowRight.transform.localPosition = new Vector3(50f, 0f, 0f);

                #region 弃用（修正UI大小，以便显示更多文本）
                /*
                if (!IsModified)
                {
                    ActorMenuActorMianQiBack = __instance.actorMianQiBack;
                    //扩大RectTransform组件sizeDelta的宽度（80f -> 100f）
                    ActorMenuActorMianQiBack.sizeDelta = new Vector2(100f, 30f);
                    //Vector2 mianQiBackXY = ActorMenuActorMianQiBack.sizeDelta;
                    //mianQiBackXY.x += 20f;
                    //ActorMenuActorMianQiBack.sizeDelta = mianQiBackXY;

                    ActorMenumianQiArrowLeft = __instance.mianQiArrowLeft.transform;
                    //调整Transform组件localPosition的横坐标（-40f -> -50f）
                    ActorMenumianQiArrowLeft.localPosition = new Vector3(-50f, 0f, 0f);
                    //Vector3 arrowLeftXYZ = ActorMenumianQiArrowLeft.localPosition;
                    //arrowLeftXYZ.x -= 10f;
                    //ActorMenumianQiArrowLeft.localPosition = arrowLeftXYZ;

                    ActorMenumianQiArrowRight = __instance.mianQiArrowRight.transform;
                    //调整Transform组件localPosition的横坐标（40f -> 50f）
                    ActorMenumianQiArrowRight.localPosition = new Vector3(50f, 0f, 0f);
                    //Vector3 arrowRightXYZ = ActorMenumianQiArrowRight.localPosition;
                    //arrowRightXYZ.x += 10f;
                    //ActorMenumianQiArrowRight.localPosition = arrowRightXYZ;
                }
                */
                #endregion

                //人物内息紊乱值（游戏里内息药显示的治疗数值是0～800，但紊乱值的实际数值为0～8000）
                int actorMianQi = DateFile.instance.GetActorMianQi(key);
                //人物内息状态
                int mianQiType = actorMianQi / 2000;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(__instance.actorMianQiText.text.Replace("</color>", string.Empty));
                stringBuilder.Append(" ");
                stringBuilder.Append(actorMianQi / 10);
                //若实际内息紊乱值能被10整除
                if (actorMianQi % 10 != 0)
                {
                    //显示小数点后一位（除余结果）
                    stringBuilder.AppendFormat(".{0}", (actorMianQi % 10));
                }
                stringBuilder.Append("</color>");

                //重设文本（补上了内息紊乱值）
                __instance.actorMianQiText.text = stringBuilder.ToString();
            }
        }
    }

    /// <summary>
    /// 显示人物内息紊乱具体数值：人物信息窗口——伤势界面的内息紊乱程度名称（鼠标移上去时显示的浮动窗口）
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "WindowSwitch")]
    public static class DisplayActorMianQiNumberInTipsWindow
    {
        /// <summary>
        /// 作用
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        /// <param name="on"></param>
        /// <param name="tips"></param>
        [HarmonyPostfix]
        private static void WindowSwitchPostfix(WindowManage __instance, bool on, GameObject tips)
        //原方法签名
        //public void WindowSwitch(bool on, GameObject tips = null)
        {
            if (Main.Setting.DisplayActorMianQiNumber.Value)
            {
                if (tips != null && on)
                {
                    //组建的tag
                    string tag = tips.tag;
                    //第一次检测
                    if (tag == "SystemIcon")
                    {
                        string[] array = tips.name.Split(',');
                        int massageIndex = (array.Length > 1) ? int.Parse(array[1]) : 0;

                        //若要显示的浮动窗口为人物信息窗口伤势界面的内息（第二次检测）
                        if (massageIndex == 758)
                        {
                            int actorMianQi = DateFile.instance.GetActorMianQi(ActorMenu.instance.actorId);
                            int mianQiType = actorMianQi / 2000;

                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendFormat("{0}人物当前内息紊乱值：{1}{2}.{3}</color>\n", __instance.informationMassage.text, DateFile.instance.massageDate[int.Parse(DateFile.instance.ageDate[mianQiType][303])][0], (actorMianQi / 10), (actorMianQi % 10));

                            __instance.informationMassage.text = stringBuilder.ToString();

                            #region 用于查看的源代码（稍微处理过）
                            /*
                            int actorMianQiIndex = actorMianQi / 2000;
                            int actorAllAttFactor = int.Parse(DateFile.instance.ageDate[actorMianQiIndex][302]);

                            //内息能力对主属性影响
                            //__instance.informationMassage.text = string.Format("{0}{1}{2}{3}{4}", new object[]
                            //{
                            string MianQitxt0 = DateFile.instance.massageDate[massageIndex][1].Split('|')[0];
                            string MianQitxt1 = DateFile.instance.SetColoer(int.Parse(DateFile.instance.ageDate[actorMianQiIndex][303]), DateFile.instance.ageDate[actorMianQiIndex][301], false);
                            string MianQitxt2 = DateFile.instance.massageDate[massageIndex][1].Split('|')[1];
                            string MianQitxt3 = DateFile.instance.SetColoer((actorAllAttFactor > 0) ? 20004 : 20010, ((actorAllAttFactor > 0) ? "+" : "-") + Mathf.Abs(actorAllAttFactor) + "%", false);
                            string MianQitxt4 = DateFile.instance.massageDate[massageIndex][1].Split('|')[2];
                            //});

                            int actorMianQiRecoverAbility = Mathf.Clamp(int.Parse(DateFile.instance.GetActorDate(ActorMenu.instance.actorId, 40, true)), 0, 8000);

                            Text text2;
                            text2 = __instance.informationMassage;

                            //负面特性导致每时节内息紊乱
                            if (actorMianQiRecoverAbility > 0)
                            {
                                //text2.text += string.Format("{0}{1}{2}{3}{4}", new object[]
                                //{
                                string Recovertxt0 = DateFile.instance.massageDate[758][4].Split('|')[0];
                                int Recovertxt1 = actorMianQiRecoverAbility / 50;
                                string Recovertxt2 = DateFile.instance.massageDate[758][4].Split('|')[1];
                                int Recovertxt3 = actorMianQiRecoverAbility / 10;
                                string Recovertxt4 = DateFile.instance.massageDate[758][4].Split('|')[2];
                                //});
                            }

                            int InBattleTypeId = DateFile.instance.ActorIsInBattle(ActorMenu.instance.actorId);

                            bool isActor3 = InBattleTypeId == 0 || InBattleTypeId == 1;

                            if (ActorMenu.Exists)
                            {
                                isActor3 = !ActorMenu.instance.isEnemy;
                            }

                            int num51 = BattleVaule.instance.GetDeferDefuse(isActor3, ActorMenu.instance.actorId, InBattleTypeId != 0, 2, 0);

                            if (actorMianQi > num51)
                            {
                                num51 /= 2;
                            }

                            //内息紊乱回复
                            if (num51 > 0)
                            {
                                text2.text += string.Format("{0}{1}{2}", DateFile.instance.massageDate[758][3].Split('|')[0], num51 / 10, DateFile.instance.massageDate[758][3].Split('|')[1]);
                            }

                            //内息断绝
                            if (actorMianQiIndex >= 4)
                            {
                                text2.text += DateFile.instance.massageDate[758][2];
                            }

                            text2.text += "\n";
                            */
                            #endregion
                        }
                    }
                }
            }
        }
    }
}
