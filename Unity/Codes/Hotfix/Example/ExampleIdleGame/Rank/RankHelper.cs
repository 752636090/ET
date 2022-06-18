using System;

namespace ET
{
    public static class RankHelper
    {
        public static async ETTask<int> GetRankInfo(Scene ZoneScene)
        {
            Rank2C_GetRanksInfo rank2CGetRanksInfo = null;
            try
            {
                rank2CGetRanksInfo = (Rank2C_GetRanksInfo)await ZoneScene.GetComponent<SessionComponent>().Session.Call(new Rank2C_GetRanksInfo() { });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (rank2CGetRanksInfo.Error != ErrorCode.ERR_Success)
            {
                return rank2CGetRanksInfo.Error;
            }

            ZoneScene.GetComponent<RankComponent>().ClearAll();
            for (int i = 0; i < rank2CGetRanksInfo.RankInfoProtoList.Count; i++)
            {
                ZoneScene.GetComponent<RankComponent>().Add(rank2CGetRanksInfo.RankInfoProtoList[i]);
            }

            return rank2CGetRanksInfo.Error;
        }
    }
}
