using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ET
{
    [FriendClass(typeof(DlgAdventure))]
    [FriendClass(typeof(Scroll_Item_battleLevel))]
	public static  class DlgAdventureSystem
	{

		public static void RegisterUIEvent(this DlgAdventure self)
		{
			self.RegisterCloseEvent<DlgAdventure>(self.View.E_CloseButton);
			self.View.E_BattleLevelLoopVerticalScrollRect.AddItemRefreshListener((Transform transform, int index) =>
			{
				self.OnBattleLevelItemRefresh(transform, index);
			});
		}

		public static void ShowWindow(this DlgAdventure self, Entity contextData = null)
		{
			self.View.EG_ContentRectTransform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.0f);
			self.View.EG_ContentRectTransform.DOScale(Vector3.one, 0.3f).onComplete += () => { self.Refresh(); };
		}

		public static void HideWindow(this DlgAdventure self)
        {
			self.View.E_BattleLevelLoopVerticalScrollRect.SetVisible(false);
			self.RemoveUIScrollItems(ref self.ScrollItemBattleLevels);
        }

		public static void Refresh(this DlgAdventure self)
        {
			int count = BattleLevelConfigCategory.Instance.GetAll().Count;
			self.AddUIScrollItems(ref self.ScrollItemBattleLevels, count);
			self.View.E_BattleLevelLoopVerticalScrollRect.SetVisible(true, count);
        }

		public static void OnBattleLevelItemRefresh(this DlgAdventure self, Transform transform, int index)
        {
			Scroll_Item_battleLevel scrollItemBattleLevel = self.ScrollItemBattleLevels[index].BindTrans(transform);
			BattleLevelConfig config = BattleLevelConfigCategory.Instance.GetConfigByIndex(index);
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.ZoneScene().CurrentScene());

			int unitLevel = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Level);
			bool isInAdventure = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.AdventureState) != 0;

			scrollItemBattleLevel.E_GoButton.SetVisible(unitLevel >= config.MiniEnterLevel[0] && !isInAdventure);
			scrollItemBattleLevel.E_InAdventureTipText.SetVisible(unitLevel >= config.MiniEnterLevel[0] && isInAdventure);
			scrollItemBattleLevel.E_LevelNotEnoughText.SetVisible(unitLevel < config.MiniEnterLevel[0]);
			scrollItemBattleLevel.E_LevelNameText.SetText($"{config.Name} Lv.{config.MiniEnterLevel[0]}~Lv.{config.MiniEnterLevel[1]}");
			scrollItemBattleLevel.E_GoButton.AddListenerAsync(() => { return self.OnStartGameLevelClickHandler(config.Id); });
        }

		public static async ETTask OnStartGameLevelClickHandler(this DlgAdventure self, int levelId)
        {
            try
            {
				int errorCode = await AdventureHelper.RequestStartGameLevel(self.ZoneScene(), levelId);
				if (errorCode != ErrorCode.ERR_Success)
                {
					return;
                }
				self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Adventure);

				self.ZoneScene().CurrentScene().GetComponent<AdventureComponent>().SetBattleRandomSeed(); // 猜的位置，可能不在这
				self.ZoneScene().CurrentScene().GetComponent<AdventureComponent>().StartAdventure().Coroutine();
            }
            catch (Exception e)
            {
				Log.Error(e.ToString());
            }
        }
	}
}
