using System;
using System.Collections.Generic;
using System.Text;
using GameData;
using HarmonyLib;
using UnityEngine.UI;

namespace BetterFix
{
    /// <summary>
    /// 快速换装优化第一部分：允许在鼠标选中已装备功法时，装备功法浏览界面的“装备功法”按钮依然可以互动
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "UpdateGongFaInformation")]
    public static class EquipedGongFaCanInteractEquipButton
    {
        /// <summary>
        /// 调整装备功法浏览界面的“装备功法”按钮的可互动性
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPostfix]
        private static void UpdateGongFaInformationPostfix(ActorMenu __instance, int id)
        //原方法签名
        //public void UpdateGongFaInformation(int id)
        {
            //若本功能开启
            if (Main.Setting.UiQuickChangeEquipedGongFa.Value)
            {
                //若原方法调用结束后，“装备功法”按钮为不可互动
                if (__instance.equipGongFaViewButton.GetComponent<Button>().interactable == false)
                {
                    //是否可允许点击
                    bool canInteract = true;

                    //若装备槽位不为主运内功（检测功法占用格数与剩余格数来判断是否允许装备功法）
                    if (__instance.equipGongFaTyp != 0)
                    {
                        //技能装备所占用格子的数量
                        int combatSkillNeedGridCount = DateFile.GetCombatSkillDataInt(id, 7, __instance.actorId, true);

                        if (combatSkillNeedGridCount > 1)
                        {
                            //人物功法装备槽位的数据
                            int[] equipGirdData = null;
                            switch (__instance.GongFaBaseTyp)
                            {
                                case 100:
                                    equipGirdData = Traverse.Create(__instance).Field<int[]>("equipBase0").Value;
                                    break;
                                case 101:
                                    equipGirdData = Traverse.Create(__instance).Field<int[]>("equipBase1").Value;
                                    break;
                                case 102:
                                    equipGirdData = Traverse.Create(__instance).Field<int[]>("equipBase2").Value;
                                    break;
                                case 103:
                                    equipGirdData = Traverse.Create(__instance).Field<int[]>("equipBase3").Value;
                                    break;
                                case 104:
                                    equipGirdData = Traverse.Create(__instance).Field<int[]>("equipBase4").Value;
                                    break;
                                default:
                                    break;
                            }

                            //若（装备槽位序号  + 功法占用格数 - 1）超出了功法装备栏位的上限（内功3，其他功法9）
                            if (__instance.equipGongFaTyp + (combatSkillNeedGridCount - 1) > equipGirdData.Length)
                            {
                                canInteract = false;
                            }
                            else
                            {
                                for (int j = 1; j < combatSkillNeedGridCount; j++)
                                {
                                    //若功法将会占用的格子，处于闭锁状态（-999，因主运内功栏位不够、而处于不可使用）
                                    if (equipGirdData[__instance.equipGongFaTyp - 1 + j] == -999)
                                    {
                                        canInteract = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //若判定可允许点击
                    if (canInteract)
                    {
                        //“装备功法”按钮设置为可互动
                        __instance.equipGongFaViewButton.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 快速换装优化第二部分：在装备指定功法前先卸载指定功法
    /// </summary>
    [HarmonyPatch(typeof(ActorMenu), "EquipGongFaButton")]
    public static class EquipedGongFaMustUnequipFirst
    {

        /// <summary>
        /// 在装备指定功法前先卸载指定功法
        /// </summary>
        /// <param name="__instance">原方法所属的实例</param>
        [HarmonyPrefix]
        private static void EquipGongFaButtonPrefix(ActorMenu __instance)
        //原方法签名
        //public void EquipGongFaButton()
        {
            if (Main.Setting.UiQuickChangeEquipedGongFa.Value)
            {
                //太吾人物ID
                int taiwuActorId = DateFile.instance.MianActorID();
                //装备功法类型
                int equipType = __instance.gongFaViewTyp - 100;
                //装备槽位下标
                //（下标都是从0开始的，所以下面的循环中取不到-1，-1可用来表示“在UI界面上用鼠标选中的功法”未被装备）
                int equipIndex = -1;

                //遍历太吾对应功法类型所装备的所有功法
                for (int i = 0; i < DateFile.instance.GetActorEquipGongFa(taiwuActorId)[equipType].Length; i++)
                {
                    //若“在UI界面上用鼠标选中的功法”已被装备，则记录其所装备的槽位下标
                    if (DateFile.instance.GetActorEquipGongFa(taiwuActorId)[equipType][i] == __instance.equipGongFaId)
                    {
                        equipIndex = i;
                        break;
                    }
                }

                //若“在UI界面上用鼠标选中的功法”已被装备
                if (equipIndex != -1)
                {
                    //调用卸除战斗用功法
                    DateFile.UnequipCombatSkill(taiwuActorId, equipType, equipIndex);
                }
            }
        }
    }
}
