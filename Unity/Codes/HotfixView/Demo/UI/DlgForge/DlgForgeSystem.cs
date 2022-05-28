using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [Timer(TimerType.MakeQueueUI)]
    public class MakeQueueUITimer : ATimer<DlgForge>
    {
        public override void Run(DlgForge t)
        {
            t?.RefreshMakeQueue();
        }
    }

    [FriendClass(typeof(DlgForge))]
    public static class DlgForgeSystem
    {
        public static void RegisterUIEvent(this DlgForge self)
        {
            self.RegisterCloseEvent<DlgForge>(self.View.E_CloseButton);
            self.View.E_ProductionLoopVerticalScrollRect.AddItemRefreshListener(self.OnProductionRefreshHandler);
        }

        public static void ShowWindow(this DlgForge self, Entity contextData = null)
        {
            self.Refresh();
        }

        private static void Refresh(this DlgForge self)
        {
            self.RefreshMakeQueue();
            self.RefreshProduction();
            self.RefreshMaterialCount();
        }

        public static void HideWindow(this DlgForge self)
        {
            TimerComponent.Instance.Remove(ref self.MakeQueueTimer);
            self.RemoveUIScrollItems(ref self.ScrollItemProductions);
        }

        public static void RefreshMaterialCount(this DlgForge self)
        {
            NumericComponent numericComponent = UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene());
            self.View.E_IronStoneCountText.SetText(numericComponent.GetAsInt(NumericType.IronStone).ToString());
            self.View.E_FurCountText.SetText(numericComponent.GetAsInt(NumericType.Fur).ToString());
        }

        public static void RefreshMakeQueue(this DlgForge self)
        {
            Production production = self.ZoneScene().GetComponent<ForgeComponent>().GetProductionByIndex(0);
            self.View.ES_MakeQueueOne.Refresh(production);

            production = self.ZoneScene().GetComponent<ForgeComponent>().GetProductionByIndex(1);
            self.View.ES_MakeQueueTwo.Refresh(production);

            TimerComponent.Instance.Remove(ref self.MakeQueueTimer);

            int count = self.ZoneScene().GetComponent<ForgeComponent>().GetMakingProductionQueueCount();
            if (count > 0)
            {
                TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 1000, TimerType.MakeQueueUI, self);
            }
        }

        public static void RefreshProduction(this DlgForge self)
        {
            int unitLevel = UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene()).GetAsInt(NumericType.Level);
            int count = ForgeProductionConfigCategory.Instance.GetProductionConfigCount(unitLevel);
            self.AddUIScrollItems(ref self.ScrollItemProductions, count);
            self.View.E_ProductionLoopVerticalScrollRect.SetVisible(true, count);
        }

        public static void OnProductionRefreshHandler(this DlgForge self, Transform transform, int index)
        {
            Scroll_Item_production scrollItemProduction = self.ScrollItemProductions[index].BindTrans(transform);
            NumericComponent numericComponent = UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene());
            int unitLevel = numericComponent.GetAsInt(NumericType.Level);
            ForgeProductionConfig config = ForgeProductionConfigCategory.Instance.GetProductionByLevelIndex(unitLevel, index);

            scrollItemProduction.ES_EquipItem.RefreshShowItem(config.ItemConfigId);
            scrollItemProduction.E_ItemNameText.SetText(ItemConfigCategory.Instance.Get(config.ItemConfigId).Name);
            scrollItemProduction.E_ConsumeTypeText.SetText(config.ConsumeId == NumericType.IronStone ? "精铁：" : "皮革：");
            scrollItemProduction.E_ConsumeCountText.SetText(config.ConsumeCount.ToString());

            int matetialCount = numericComponent.GetAsInt(config.ConsumeId);
            scrollItemProduction.E_MakeButton.interactable = matetialCount >= config.ConsumeCount;
            scrollItemProduction.E_MakeButton.AddListenerAsync(() => { return self.OnStartProductionHandler(config.Id); });
        }

        public static async ETTask OnStartProductionHandler(this DlgForge self, int productionConfigId)
        {
            try
            {
                int errorCode = await ForgeHelper.StartProduction(self.ZoneScene(), productionConfigId);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }
                self.Refresh();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
