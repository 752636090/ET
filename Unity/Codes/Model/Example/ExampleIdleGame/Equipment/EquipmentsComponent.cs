using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ChildType(typeof(Item))]
#if SERVER
    [ComponentOf(typeof(Unit))]
    public class EquipmentsComponent : Entity, IAwake, IDestroy, ITransfer, IDeserialize, IUnitCache
#else
    [ComponentOf(typeof(Scene))]
    public class EquipmentsComponent : Entity, IAwake, IDestroy
#endif
    {
        /// <summary>
        /// key: 装备位置
        /// </summary>
#if SERVER
        [BsonIgnore]
#endif
        public Dictionary<int, Item> EquipItems = new Dictionary<int, Item>();

#if SERVER
        [BsonIgnore]
        public M2C_ItemUpdateOpInfo message = new M2C_ItemUpdateOpInfo() { ContainerType = (int)ItemContainerType.RoleInfo };
#endif
    }
}
