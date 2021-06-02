using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 用于设定MouseTipActorInfoSimple追加信息UI的组件
    /// </summary>
    public class AdditionInfo : MonoBehaviour
    {
        public AttrUI ActorGeneration;
        public AttrUI ActorFavor;
        public AttrUI ActorHealth;
        public CText ActorPlace;

        /// <summary>
        /// 更新指定人物的追加信息
        /// </summary>
        /// <param name="actorId">人物ID</param>
        public void SetAdditionInfo(int actorId)
        {
            ActorGeneration.SetAttrInfo(actorId);
            ActorFavor.SetAttrInfo(actorId);
            ActorHealth.SetAttrInfo(actorId);
            ActorPlace.text = ShowActorPlaceInfo(actorId, PlaceInfoType.MouseTipActorInfoSimple);
        }

        /// <summary>
        /// 输出指定人物的所处地点信息文本
        /// </summary>
        /// <param name="actorId">人物ID</param>
        /// <param name="shortText">是否采用短消息（长消息用于在MouseTipActorInfoSimple中使用——开头空一行以变向作为UI调整）（短消息用于人物信息窗口）</param>
        /// <returns>所处地点信息文本</returns>
        public static string ShowActorPlaceInfo(int actorId, PlaceInfoType placeInfoType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //若人物为实在人物
            if (int.Parse(DateFile.instance.GetActorDate(actorId, 8, false)) == 1)
            {
                //若人物是否存活
                bool isActorAlive = int.Parse(DateFile.instance.GetActorDate(actorId, 26, false)) == 0;
                //人物所在地点
                List<int> actorAtPlace = DateFile.instance.GetActorAtPlace(actorId);

                //所在地点不为空
                if (actorAtPlace != null)
                {
                    switch (placeInfoType)
                    {
                        case PlaceInfoType.MouseTipActorInfoSimple:
                            stringBuilder.AppendFormat("<size=10>\n</size>{0}{1}\n<color=#B97D4BFF>{2}</color><color=#9B8773FF>「{3}」</color>-\n{4}{5}</color>\n(地格.{6})",
                                //{0}地点颜色前缀
                                DateFile.instance.massageDate[20005][0],
                                //{1}“正在：”/“安葬于：”
                                DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 0 : 2],
                                //{2}地域名称
                                DateFile.instance.allWorldDate[int.Parse(DateFile.instance.partWorldMapDate[actorAtPlace[0]][95])][0],
                                //{3}地区名称
                                DateFile.instance.partWorldMapDate[actorAtPlace[0]][0],
                                //{4}地格所属聚落名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 98),
                                //{5}地格名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 0),
                                //{6}地格ID
                                actorAtPlace[1]
                                );
                            break;
                        case PlaceInfoType.ActorMeumPeoplePlace:
                            stringBuilder.AppendFormat("{0}<color=#B97D4BFF>{1}</color><color=#9B8773FF>「{2}」</color>-\n{3}{4}\n<color=#E1CDAAFF>(地格.{5})</color>",
                                //{0}“正在：”/“安葬于：”
                                DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 0 : 2],
                                //{1}地域名称
                                DateFile.instance.allWorldDate[int.Parse(DateFile.instance.partWorldMapDate[actorAtPlace[0]][95])][0],
                                //{2}地区名称
                                DateFile.instance.partWorldMapDate[actorAtPlace[0]][0],
                                //{3}地格所属聚落名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 98),
                                //{4}地格名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 0),
                                //{5}地格ID
                                actorAtPlace[1]
                                );
                            break;
                        case PlaceInfoType.ActorMeumMainAttr:
                            stringBuilder.AppendFormat("{0}{1}<color=#B97D4BFF>{2}</color><color=#9B8773FF>「{3}」</color>-\n{4}{5}</color>(地格.{6})",
                                //{0}地点颜色前缀
                                DateFile.instance.massageDate[20005][0],
                                //{1}“正在：”/“安葬于：”
                                DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 0 : 2],
                                //{2}地域名称
                                DateFile.instance.allWorldDate[int.Parse(DateFile.instance.partWorldMapDate[actorAtPlace[0]][95])][0],
                                //{3}地区名称
                                DateFile.instance.partWorldMapDate[actorAtPlace[0]][0],
                                //{4}地格所属聚落名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 98),
                                //{5}地格名称
                                DateFile.instance.GetNewMapDate(actorAtPlace[0], actorAtPlace[1], 0),
                                //{6}地格ID
                                actorAtPlace[1]
                                );
                            break;
                    }
                }
                //所在地点为空
                else
                {
                    //!----- 人物存活：“身处未知之地…” / 人物死亡：“墓地寻而不得……” -----!
                    switch (placeInfoType)
                    {
                        case PlaceInfoType.MouseTipActorInfoSimple:
                            stringBuilder.AppendFormat("<size=10>\n</size>{0}{1}</color>",
                                //{0}地点颜色前缀
                                DateFile.instance.massageDate[20005][0],
                                DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 1 : 3]
                                );
                            break;
                        case PlaceInfoType.ActorMeumPeoplePlace:
                            stringBuilder.Append(DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 1 : 3]);
                            break;
                        case PlaceInfoType.ActorMeumMainAttr:
                            stringBuilder.AppendFormat("{0}{1}</color>",
                                //{0}地点颜色前缀
                                DateFile.instance.massageDate[20005][0],
                                DateFile.instance.massageDate[8010][3].Split('|')[isActorAlive ? 1 : 3]
                                );
                            break;
                    }
                }
            }
            //若人物为非实在人物
            else
            {
                //!----- “身处未知之地…” -----!
                switch (placeInfoType)
                {
                    case PlaceInfoType.MouseTipActorInfoSimple:
                        stringBuilder.AppendFormat("<size=10>\n</size>{0}{1}</color>",
                            //{0}地点颜色前缀
                            DateFile.instance.massageDate[20005][0],
                            DateFile.instance.massageDate[8010][3].Split('|')[1]
                            );
                        break;
                    case PlaceInfoType.ActorMeumPeoplePlace:
                        stringBuilder.Append(DateFile.instance.massageDate[8010][3].Split('|')[1]);
                        break;
                    case PlaceInfoType.ActorMeumMainAttr:
                        stringBuilder.AppendFormat("{0}{1}</color>",
                            //{0}地点颜色前缀
                            DateFile.instance.massageDate[20005][0],
                            DateFile.instance.massageDate[8010][3].Split('|')[1]
                            );
                        break;
                }
            }

            return stringBuilder.ToString();
        }

        public enum PlaceInfoType
        {
            /// <summary>鼠标浮动信息：人物简略信息</summary>
            MouseTipActorInfoSimple,
            /// <summary>人物信息界面：人物关系页面</summary>
            ActorMeumPeoplePlace,
            /// <summary>人物信息界面：人物属性页面</summary>
            ActorMeumMainAttr
        }
    }
}
