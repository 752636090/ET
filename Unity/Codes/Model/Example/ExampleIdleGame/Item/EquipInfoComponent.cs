using System;
using System.Collections.Generic;
#if SERVER
using MongoDB.Bson.Serialization.Attributes;
#endif

namespace ET
{
    [ComponentOf(typeof(Item))]
    [ChildType(typeof(AttributeEntry))]
#if SERVER
    public class EquipInfoComponent : Entity, IAwake, IDestroy, ISerializeToEntity, IDeserialize
#else
    public class EquipInfoComponent : Entity, IAwake, IDestroy
#endif
    {
        public bool IsInited = false;

        public int Score = 0;

#if SERVER
        [BsonIgnore]
#endif
        public List<AttributeEntry> EntryList = new List<AttributeEntry>();
    }
}
