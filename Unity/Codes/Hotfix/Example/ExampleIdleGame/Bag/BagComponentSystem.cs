using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClass(typeof(BagComponent))]
    public static class BagComponentSystem
    {
        public static void Clear(this BagComponent self)
        {
            ForeachHelper.Foreach(self.ItemsDict, (long id, Item item) =>
            {
                item?.Dispose();
            });
            self.ItemsDict.Clear();
            self.ItemsMap.Clear();
        }

        // 里面内容都是猜的
        public static int GetItemCountByItemType(this BagComponent self, ItemType itemType)
        {
            Log.Debug("调用了猜出来的方法");
            return self.ItemsMap[(int)itemType].Count;
        }

        public static void AddItem(this BagComponent self, Item item)
        {
            self.AddChild(item);
            self.ItemsDict.Add(item.Id, item);
            self.ItemsMap.Add(item.Config.Type, item);
        }

        public static void RemoveItem(this BagComponent self, Item item)
        {
            if (item == null)
            {
                Log.Error("bag item is null");
                return;
            }

            self.ItemsDict.Remove(item.Id);
            self.ItemsMap.Remove(item.Config.Type, item);
            item?.Dispose();
        }

        public static Item GetItemById(this BagComponent self, long itemId)
        {
            if (self.ItemsDict.TryGetValue(itemId, out Item item))
            {
                return item;
            }
            return null;
        }
    }
}
