using MongoDB.Bson.Serialization.Attributes;
using System.Collections;
using System.Collections.Generic;

namespace ET
{
    [ChildType(typeof(RankInfo))]
    [ComponentOf(typeof(Scene))]
    public class RankInfosComponent : Entity, IAwake, IDestroy
    {
        [BsonIgnore]
        public SortedList<RankInfo, long> SortedRankInfoList = new SortedList<RankInfo, long>(new RankInfoCompare());

        [BsonIgnore]
        public Dictionary<long, RankInfo> RankInfosDictionary = new Dictionary<long, RankInfo>();
    }

    [FriendClassAttribute(typeof(ET.RankInfo))]
    public class RankInfoCompare : IComparer<RankInfo>
    {
        public int Compare(RankInfo a, RankInfo b)
        {
            int result = b.Count - a.Count;

            if (result != 0)
            {
                return result;
            }

            if (a.Id < b.Id)
            {
                return 1;
            }

            if (a.Id > b.Id)
            {
                return -1;
            }
            return 0;
        }
    }
}
