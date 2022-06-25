using System.Collections.Generic;

namespace ET
{
    public class RankInfosComponentDestroySystem : DestroySystem<RankInfosComponent>
    {
        public override void Destroy(RankInfosComponent self)
        {
            foreach (RankInfo rankInfo in self.RankInfosDictionary.Values)
            {
                rankInfo?.Dispose();
            }
            self.RankInfosDictionary.Clear();
            self.SortedRankInfoList.Clear();
        }
    }

    [FriendClass(typeof(RankInfo))]
    [FriendClass(typeof(RankInfosComponent))]
    public static class RankInfosComponentSystem
    {
        public static async ETTask LoadRankInfo(this RankInfosComponent self)
        {
            List<RankInfo> rankInfoList = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<RankInfo>(d => true, collection: "RankInfosComponent");

            foreach (RankInfo rankInfo in rankInfoList)
            {
                self.AddChild(rankInfo);
                self.RankInfosDictionary.Add(rankInfo.UnitId, rankInfo);
                self.SortedRankInfoList.Add(rankInfo, rankInfo.UnitId);
            }
        }

        public static void AddOrUpdate(this RankInfosComponent self, RankInfo newRankInfo)
        {
            if (self.RankInfosDictionary.ContainsKey(newRankInfo.UnitId))
            {
                RankInfo oldRankInfo = self.RankInfosDictionary[newRankInfo.UnitId];
                if (oldRankInfo.Count == newRankInfo.Count)
                {
                    return;
                }

                DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Remove<RankInfo>(oldRankInfo.UnitId, oldRankInfo.Id, "RankInfosComponent").Coroutine();
                self.RankInfosDictionary.Remove(oldRankInfo.UnitId);
                self.SortedRankInfoList.Remove(oldRankInfo);
                oldRankInfo?.Dispose();
            }

            self.AddChild(newRankInfo);
            self.RankInfosDictionary.Add(newRankInfo.UnitId, newRankInfo);
            self.SortedRankInfoList.Add(newRankInfo, newRankInfo.UnitId);
            DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save(newRankInfo.UnitId, newRankInfo, "RankInfosComponent").Coroutine();
        }
    }
}
