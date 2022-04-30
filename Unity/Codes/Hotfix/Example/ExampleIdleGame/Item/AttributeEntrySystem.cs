using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class AttributeEntryDestroySystem : DestroySystem<AttributeEntry>
    {
        public override void Destroy(AttributeEntry self)
        {
            self.Key = 0;
            self.Value = 0;
            self.Type = EntryType.Common;
        }
    }

    [FriendClass(typeof(AttributeEntry))]
    public static class AttributeEntrySystem
    {
        public static void FromMessage(this AttributeEntry self, AttributeEntryProto attributeEntryProto)
        {
            self.Id = attributeEntryProto.Id;
            self.Key = attributeEntryProto.Key;
            self.Value = attributeEntryProto.Value;
            self.Type = (EntryType)attributeEntryProto.EntryType;
        }
    }
}
