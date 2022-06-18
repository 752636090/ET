using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [Timer(TimerType.RankUI)]
    public class RankUITimer : ATimer<DlgRank>
    {
        public override void Run(DlgRank t)
        {
			t?.RefreshRankInfo().Coroutine();
        }
    }

    [FriendClass(typeof(DlgRank))]
    [FriendClass(typeof(RankInfo))]
	public static  class DlgRankSystem
	{

		public static void RegisterUIEvent(this DlgRank self)
		{
			self.RegisterCloseEvent<DlgRank>(self.View.E_CloseButton);
			self.View.E_RankLoopVerticalScrollRect.AddItemRefreshListener(self.OnRankItemLoopHandler);
		}

		public static void ShowWindow(this DlgRank self, Entity contextData = null)
		{
			self.RefrshRankInfo().Coroutine();
			self.Timer = TimerComponent.Instance.NewRepeatedTimer(5000, TimerType.RankUI, self);
		}

		public static void HideWindow(this DlgRank self, Entity contextData = null)
        {
			TimerComponent.Instance.Remove(ref self.Timer);
        }

		public static void OnRankItemLoopHandler(this DlgRank self, Transform transform, int index)
        {
			Scroll_Item_rank scrollItemRank = self.ScrollItemRanks[index].Bind(transform);
			RankInfo rankInfo = self.ZoneScene().GetComponent<RankComponent>().GetRankInfoByIndex(index);

			int order = index + 1;
			scrollItemRank.E_RankOrderText.SetText($"第 {order} 名");
			scrollItemRank.E_NameText.SetText(rankInfo.Name.ToString());
			scrollItemRank.E_LevelText.SetText("Lv." + rankInfo.Count.ToString());
        }

		public static async ETTask RefreshRankInfo(this DlgRank self)
        {
            try
            {
				Scene ZoneScene = self.ZoneScene();
				int errorCode = await RankHelper.GetRankInfo(ZoneScene);
				if (errorCode != ErrorCode.ERR_Success)
                {
					Log.Error(errorCode.ToString());
					return;
                }

				if (!ZoneScene.GetComponent<UIComponent>().IsWindowVisible(WindowID.WindowID_Rank))
                {
					return;
                }

				int count = self.ZoneScene().GetComponent<RankComponent>().GetRankCount();
				self.AddUIScrollItems(ref self.ScrollItemRank, count);
				self.View.E_RankLoopVerticalScrollRect.SetVisible(true, count);
            }
            catch (Exception e)
            {
				Log.Error(e.ToString());
            }
        }
	}
}
