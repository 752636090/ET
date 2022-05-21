using System.Collections.Generic;

namespace ET
{
    [ChildType(typeof(Production))]
#if SERVER
    public class ForgeComponent : Entity, IAwake, IDestroy, IDeserialize, ITransfer, IUnitCache
#else
    public class ForgeComponent : Entity, IAwake, IDestroy
#endif
    {
#if SERVER
        [BsonIgnore]
#endif
        public Dictionary<long, long> ProductionTimeDict = new Dictionary<long, long>();

#if SERVER
        [BsonIgnore]
#endif
        // 生产队列
        public List<Production> ProductionList = new List<Production>();
    }
}
