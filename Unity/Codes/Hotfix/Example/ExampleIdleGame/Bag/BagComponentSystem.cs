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
        


        public static void AddItem(this BagComponent self, Item item)
        {
            self.AddChild(item);
            self.ItemsDict.Add(item.Id, item);
            self.ItemsMap.Add(item.Config.Type, item);
        }
    }
}
