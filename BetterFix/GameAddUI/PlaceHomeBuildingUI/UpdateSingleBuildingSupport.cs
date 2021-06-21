using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFix
{
    /// <summary>
    /// 用于辅助更新本MOD的地点建筑列表
    /// </summary>
    public class UpdateSingleBuildingSupport
    {
        public static void UpdateSingle(int partId, int placeId, int buildingIndex)
        {
            if (Main.Setting.UiAddPlaceBuildUI.Value
                && Main.InGameBuildingUI != null
                && Main.InGameBuildingUI._showBuildingType != 9
                && PlaceHomeBuildingUI._partId == partId
                && PlaceHomeBuildingUI._placeId == placeId
                && Main.InGameBuildingUI._showBuildingList != null
                && Main.InGameBuildingUI._showBuildingList.Contains(buildingIndex)
                )
            {
                foreach (var placeBuilding in Main.InGameBuildingUI.buildingInfiniteScrollView.dataList)
                {
                    string[] nameTexts = placeBuilding.GetComponent<SetPlaceIcon>().buildingButton.name.Split(',');
                    if (nameTexts.Length == 4 && int.TryParse(nameTexts[3], out int index) && index == buildingIndex)
                    {
                        //QuickLogger.Log(LogLevel.Info, "确认需要同步更新地点建筑 name:{0}", placeBuilding.name);
                        placeBuilding.GetComponent<SetPlaceIcon>().SetBuilding(partId, placeId, buildingIndex);
                    }
                }
            }
        }
    }
}
