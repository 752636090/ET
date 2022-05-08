using UnityEngine;

namespace ET
{
    public static class ES_EquipItemSystem
    {
        public static void RegisterEventHandler(this ES_EquipItem self, EquipPosition equipPosition)
        {
            self.E_SelectButton.AddListenerWithId(self.OnSelectedHandler, (int)equipPosition);
        }

        public static void OnSelectedHandler(this ES_EquipItem self, int equipPosition)
        {
            Item item = self.ZoneScene().GetComponent<EquipmentsComponent>().GetItemByPosition((EquipPosition)equipPosition);
            if (null == item)
            {
                return;
            }
            self.ZoneScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_ItemPopUp);
            self.ZoneScene().GetComponent<UIComponent>().GetDlgLogic<DlgItemPopUp>().RefreshInfo(item, ItemContainerType.RoleInfo);
        }

        public static void RefreshShowItem(this ES_EquipItem self, EquipPosition equipPosition)
        {
            Item item = self.ZoneScene().GetComponent<EquipmentsComponent>().GetItemByPosition((EquipPosition)equipPosition);
            if (null != item)
            {
                self.E_IconImage.overrideSprite = IconHelper.LoadIconSprite("Icons", item.Config.Icon);
                self.E_QualityImage.color = item.ItemQualityColor();
            }
            else
            {
                self.E_IconImage.sprite = null;
                self.E_IconImage.overrideSprite = null;
                self.E_QualityImage.color = Color.gray;
            }
        }
    }
}
