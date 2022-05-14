using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    /// <summary>
    /// 全是猜的
    /// </summary>
    [FriendClass(typeof(EquipmentsComponent))]
    public static class EquipmentsComponentSystem
    {
        public static void Clear(this EquipmentsComponent self)
        {
            Log.Debug("调用了猜出来的方法");
            ForeachHelper.Foreach(self.EquipItems, (int position, Item item) =>
            {
                item?.Dispose();
            });
            self.EquipItems.Clear();
        }

        public static void AddEquipItem(this EquipmentsComponent self, Item item)
        {
            Log.Debug("调用了猜出来的方法");
            self.AddChild(item);
            self.EquipItems.Add(item.Config.EquipPosition, item);
        }

        public static Item GetItemById(this EquipmentsComponent self, long itemId)
        {
            Log.Debug("调用了猜出来的方法");
            return self.GetChild<Item>(itemId);
        }

        public static Item GetItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            Log.Debug("调用了猜出来的方法");
            if (self.EquipItems.TryGetValue((int)equipPosition, out Item item))
            {
                return item;
            }
            return null;
        }

        public static void UnloadEquipItem(this EquipmentsComponent self, Item item)
        {
            Log.Debug("调用了猜出来的方法");
            if (item == null)
            {
                Log.Error("bag item is null");
                return;
            }

            self.EquipItems.Remove(item.Config.EquipPosition);
            item?.Dispose();
        }
    }
}
