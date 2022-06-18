using System.Collections.Generic;

namespace ET
{
    [ChildType(typeof(RankInfo))]
    [ComponentOf(typeof(Scene))]
    public class RankComponent : Entity, IAwake, IDestroy
    {
        public List<RankInfo> RankInfos = new List<RankInfo>();
    }
}
