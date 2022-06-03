using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ET
{
    [ChildType(typeof(Production))]
#if SERVER
    [ComponentOf(typeof(Unit))]
    public class ForgeComponent : Entity, IAwake, IDestroy, IDeserialize, ITransfer, IUnitCache
#else
    [ComponentOf(typeof(Scene))]
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
