using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class ItemApplyHelper
    {
        public static async ETTask<int> EquipItem(Scene ZoneScene, long itemId)
        {
            Item item = ItemHelper.GetItem(ZoneScene, itemId, ItemContainerType.Bag);

            if (item == null)
            {
                return ErrorCode.ERR_ItemNotExist;
            }

            M2C_EquipItem m2CEquipItem = null;

            try
            {
                m2CEquipItem = (M2C_EquipItem)await ZoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_EquipItem() { ItemUid = itemId });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            return m2CEquipItem.Error;
        }

        public static async ETTask<int> SellBagItem(Scene ZoneScene, long itemId)
        {
            Item item = ItemHelper.GetItem(ZoneScene, itemId, ItemContainerType.Bag);

            if (item == null)
            {
                return ErrorCode.ERR_ItemNotExist;
            }

            M2C_SellItem m2cSellItem = null;

            try
            {
                m2cSellItem = (M2C_SellItem)await ZoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_SellItem() { ItemUid = itemId });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }
            return m2cSellItem.Error;
        }

        public static async ETTask<int> UnloadEquipItem(Scene ZoneScene, long itemId)
        {
            Item item = ItemHelper.GetItem(ZoneScene, itemId, ItemContainerType.RoleInfo);

            if (item == null)
            {
                return ErrorCode.ERR_ItemNotExist;
            }

            M2C_UnloadEquipItem m2CUnloadEquipItem = null;

            try
            {
                m2CUnloadEquipItem = (M2C_UnloadEquipItem)await ZoneScene.GetComponent<SessionComponent>().Session.Call(
                    new C2M_UnloadEquipItem() { EquipPosition = item.Config.EquipPosition });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            return m2CUnloadEquipItem.Error;
        }
    }
}
