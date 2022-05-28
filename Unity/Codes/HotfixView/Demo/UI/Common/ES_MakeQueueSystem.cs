using System;

namespace ET
{
    [FriendClass(typeof(Production))]
    [FriendClass(typeof(ES_MakeQueue))]
    public static class ES_MakeQueueSystem
    {
        public static void Refresh(this ES_MakeQueue self, Production production)
        {
            if (production == null || !production.IsMakingState())
            {
                self.uiTransform.SetVisible(false);
                return;
            }

            self.uiTransform.SetVisible(true);

            int itemConfigId = ForgeProductionConfigCategory.Instance.Get(production.ConfigId).ItemConfigId;
            self.ES_EquipItem.RefreshShowItem(itemConfigId);

            bool isCanReceive = production.IsMakeTimeOver() && production.IsMakingState();

            self.E_MakeTimeText.SetText(production.GetRemainingTimeStr());
            self.E_LeftTimeSlider.value = production.GetRemainTimeValue();

            self.E_LeftTimeSlider.SetVisible(!isCanReceive);
            self.E_MakeTimeText.SetVisible(!isCanReceive);
            self.E_MakeTipText.SetVisible(!isCanReceive);
            self.E_MakeOverTipText.SetVisible(isCanReceive);
            self.E_ReceiveButton.SetVisible(isCanReceive);
            self.E_ReceiveButton.AddListenerAsync(() => { return self.OnReceiveButtonHandler(production.Id); });
        }

        public static async ETTask OnReceiveButtonHandler(this ES_MakeQueue self, long productionId)
        {
            try
            {
                int errorCode = await ForgeHelper.ReceivedProductionItem(self.ZoneScene(), productionId);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }
                self.ZoneScene().GetComponent<UIComponent>().GetDlgLogic<DlgForge>().RefreshMakeQueue();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
