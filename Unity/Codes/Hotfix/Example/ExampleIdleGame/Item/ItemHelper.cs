using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class ItemHelper
    {
        public static void Clear(Scene ZoneScene, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene?.GetComponent<BagComponent>()?.Clear();
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                ZoneScene?.GetComponent<EquipmentsComponent>()?.Clear();
            }
        }



        public static void AddItem(Scene ZoneScene, Item item, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene.GetComponent<BagComponent>().AddItem(item);
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                ZoneScene.GetComponent<EquipmentsComponent>().AddEquipItem(item);
            }
        }
    }
}
