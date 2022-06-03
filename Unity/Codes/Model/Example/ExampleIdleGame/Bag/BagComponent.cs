using System.Collections.Generic;
#if SERVER
using MongoDB.Bson.Serialization.Attributes; 
#endif

namespace ET
{
    [ChildType(typeof(Item))]
#if SERVER
    [ComponentOf(typeof(Unit))]
    public class BagComponent : Entity, IAwake, IDestroy, IDeserialize, ITransfer, IUnitCache
#else
    [ComponentOf(typeof(Scene))]
    public class BagComponent : Entity, IAwake, IDestroy
#endif
    {
#if SERVER
        [BsonIgnore]
#endif
        public Dictionary<long, Item> ItemsDict = new Dictionary<long, Item>();

#if SERVER
        [BsonIgnore]
#endif
        public MultiMap<int, Item> ItemsMap = new MultiMap<int, Item>();

#if SERVER
        [BsonIgnore]
        public M2C_ItemUpdateOpInfo message = new M2C_ItemUpdateOpInfo() { ContainerType = (int)ItemContainerType.Bag };
#endif
    }
}
