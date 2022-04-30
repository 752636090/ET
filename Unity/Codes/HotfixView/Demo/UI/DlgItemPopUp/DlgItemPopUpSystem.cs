using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [FriendClass(typeof(AttributeEntry))]
    [FriendClass(typeof(EquipInfoComponent))]
    [FriendClass(typeof(DlgItemPopUp))]
    public static class DlgItemPopUpSystem
    {

        public static void RegisterUIEvent(this DlgItemPopUp self)
        {
            self.RegisterCloseEvent<DlgItemPopUp>(self.View.E_CloseButton);
            self.View.E_EntrysLoopVerticalScrollRect.AddItemRefreshListener(self.OnEntryLoopHandler);
            self.View.E_EquipButton.AddListenerAsync(self.OnEquipItemHandler);
            self.View.E_UnEquipButton.AddListenerAsync(self.OnUnloadEquipItemHandler);
            self.View.E_SellButton.AddListenerAsync(self.OnSellItemHandler);
        }

        public static void ShowWindow(this DlgItemPopUp self, Entity contextData = null)
        {
        }

        public static void HideWindow(this DlgItemPopUp self)
        {
            self.RemoveUIScrollItems(ref self.ScrollItemEntries);
        }

        public static void OnEntryLoopHandler(this DlgItemPopUp self, Transform transform, int index)
        {
            Scroll_Item_entry scrollItemEntry = self.ScrollItemEntries[index].BindTrans(transform);
            Item item = ItemHelper.GetItem(self.ZoneScene(), self.ItemId, self.ItemContainerType);
            AttributeEntry entry = item.GetComponent<EquipInfoComponent>().EntryList[index];
            scrollItemEntry.E_EntryNameText.text = PlayerNumericConfigCategory.Instance.Get(entry.Key).Name + ":";
            bool isPercent = PlayerNumericConfigCategory.Instance.Get(entry.Key).isPercent > 0;
            scrollItemEntry.E_EntryValueText.text = "+" + (isPercent ? $"{(entry.Value / 10000.0f).ToString("0.00")}%" : entry.Value.ToString());
        }

        public static void RefreshInfo(this DlgItemPopUp self, Item item, ItemContainerType itemContainerType)
        {
            self.ItemId = item.Id;
            self.ItemContainerType = itemContainerType;

            self.View.E_IconImage.overrideSprite = IconHelper.LoadIconSprite("Icons", item.Config.Icon);
            self.View.E_QualityImage.color = item.ItemQualityColor();
            self.View.E_NameText.text = item.Config.Name;
            self.View.E_DescText.text = item.Config.Desc;
            self.View.E_PriceText.text = item.Config.SellBasePrice.ToString();

            if (item.Config.Type == (int)ItemType.Prop)
            {
                self.View.E_EquipButton.SetVisible(false);
                self.View.E_UnEquipButton.SetVisible(false);
                self.View.E_SellButton.SetVisible(false);
                self.View.E_EntrysLoopVerticalScrollRect.SetVisible(true, 0);
                self.View.E_SellButton.SetVisible(itemContainerType == ItemContainerType.Bag);
            }
            else
            {
                self.View.E_ScoreText.text = item.GetComponent<EquipInfoComponent>().Score.ToString();
                int count = item.GetComponent<EquipInfoComponent>().EntryList.Count;
                self.AddUIScrollItems(ref self.ScrollItemEntries, count);
                self.View.E_EntrysLoopVerticalScrollRect.SetVisible(true, count);

                self.View.E_EquipButton.SetVisible(itemContainerType == ItemContainerType.Bag);
                self.View.E_UnEquipButton.SetVisible(itemContainerType == ItemContainerType.RoleInfo);
                self.View.E_SellButton.SetVisible(itemContainerType == ItemContainerType.Bag);
            }
        }

        public static async ETTask OnSellItemHandler(this DlgItemPopUp self)
        {
            try
            {
                int errorCode = await ItemApplyHelper.SellBagItem(self.ZoneScene(), self.ItemId);

                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }
                self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_ItemPopUp);
                self.ZoneScene().GetComponent<UIComponent>().GetDlgLogic<DlgBag>().Refresh();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
