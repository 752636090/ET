using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    // 没讲到
    public class EquipmentsComponentDeserializeSystem : DeserializeSystem<EquipmentsComponent>
    {
        public override void Deserialize(EquipmentsComponent self)
        {
            foreach (Entity entity in self.Children.Values)
            {
                Item item = entity as Item;
                self.EquipItems.Add(item.Config.EquipPosition, item);
            }
        }
    }

    [FriendClass(typeof(EquipmentsComponent))]
    public static class EquipmentsComponentSystem
    {
        // 没讲到
        public static bool IsEquipItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            return self.EquipItems.ContainsKey((int)equipPosition);
        }

        public static bool EquipItem(this EquipmentsComponent self, Item item)
        {
            if (!self.EquipItems.ContainsKey(item.Config.EquipPosition))
            {
                self.AddChild(item);
                self.EquipItems.Add(item.Config.EquipPosition, item);
                Game.EventSystem.Publish(new EventType.ChangeEquipItem() { Unit = self.GetParent<Unit>(), Item = item, EquipOp = EquipOp.Load });
                ItemUpdateNoticeHelper.SyncAddItem(self.GetParent<Unit>(), item, self.message);
                return true;
            }
            return false;
        }

        // 注意没把item Dispose掉
        public static Item UnloadEquipItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            if (self.EquipItems.TryGetValue((int)equipPosition, out Item item))
            {
                self.EquipItems.Remove((int)equipPosition);
                Game.EventSystem.Publish(new EventType.ChangeEquipItem() { Unit = self.GetParent<Unit>(), Item = item, EquipOp = EquipOp.Unload });
                ItemUpdateNoticeHelper.SyncRemoveItem(self.GetParent<Unit>(), item, self.message);
            }
            return item;
        }

        public static Item GetEquipItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            if (!self.EquipItems.TryGetValue((int)equipPosition, out Item item))
            {
                return null;
            }
            return item;
        }
    }
}
