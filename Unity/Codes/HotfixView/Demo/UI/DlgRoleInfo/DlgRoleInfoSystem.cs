using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[FriendClass(typeof(DlgRoleInfo))]
	[FriendClass(typeof(Scroll_Item_attribute))]
	public static  class DlgRoleInfoSystem
	{

		public static void RegisterUIEvent(this DlgRoleInfo self)
		{
			self.RegisterCloseEvent<DlgRoleInfo>(self.View.E_CloseButton);
			self.View.ES_AttributeItem.RegisterEvent(NumericType.Power);
			self.View.ES_AttributeItem1.RegisterEvent(NumericType.PhysicalStrength);
			self.View.ES_AttributeItem2.RegisterEvent(NumericType.Agile);
			self.View.ES_AttributeItem3.RegisterEvent(NumericType.Spirit);
			self.View.E_AttributsLoopVerticalScrollRect.AddItemRefreshListener((Transform transform, int index) => { self.OnAttributeItemRefreshHandler(transform, index); });
			self.View.E_UpLevelButton.AddListenerAsync(self.OnUpRoleLevelHandler);

			RedDotHelper.AddRedDotNodeView(self.ZoneScene(), "UpLevelButton", self.View.E_UpLevelButton.gameObject, Vector3.one, new Vector3(115f, 10f, 0));
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(), "AddAttribute", self.View.E_AttributePointText.gameObject, new Vector3(0.5f, 0.5f, 1), new Vector3(-17, 10f, 0));
		}

		public static void OnUnLoadWindow(this DlgRoleInfo self)
		{
			RedDotMonoView redDotMonoView = self.View.E_UpLevelButton.GetComponent<RedDotMonoView>();
			RedDotHelper.RemoveRedDotView(self.ZoneScene(), "UpLevelButton", out redDotMonoView);

			redDotMonoView = self.View.E_UpLevelButton.GetComponent<RedDotMonoView>();
			RedDotHelper.RemoveRedDotView(self.ZoneScene(), "AddAttribute", out redDotMonoView);
		}

		public static void ShowWindow(this DlgRoleInfo self, Entity contextData = null)
		{
			self.Refresh();
		}

		 
		public static void Refresh(this DlgRoleInfo self)
        {
			self.View.ES_AttributeItem.Refresh(NumericType.Power);
			self.View.ES_AttributeItem1.Refresh(NumericType.PhysicalStrength);
			self.View.ES_AttributeItem2.Refresh(NumericType.Agile);
			self.View.ES_AttributeItem3.Refresh(NumericType.Spirit);

			NumericComponent numericComponent = UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene());
			self.View.E_CombatEffectivenessText.text = "战力值:" + numericComponent.GetAsLong(NumericType.CombatEffectiveness).ToString();
			self.View.E_AttributePointText.text = numericComponent.GetAsInt(NumericType.AttributePoint).ToString();

			int count = PlayerNumericConfigCategory.Instance.GetShowConfigCount();
			self.AddUIScrollItems(ref self.ScrollItemAttributes, count);
			self.View.E_AttributsLoopVerticalScrollRect.SetVisible(true, count);
        }

		public static void OnAttributeItemRefreshHandler(this DlgRoleInfo self, Transform transform, int index)
        {
			Scroll_Item_attribute scrollItemAttribute = self.ScrollItemAttributes[index].BindTrans(transform);
			PlayerNumericConfig config = PlayerNumericConfigCategory.Instance.GetConfigByIndex(index);
			scrollItemAttribute.E_attributeNameText.text = config.Name + ":";
			scrollItemAttribute.E_attributeValueText.text = config.isPercent == 0 ?
				UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene()).GetAsLong(config.Id).ToString() :
				$"{UnitHelper.GetMyUnitNumericComponent(self.ZoneScene().CurrentScene()).GetAsFloat(config.Id)}%";
		}

		public static async ETTask OnUpRoleLevelHandler(this DlgRoleInfo self)
        {
            try
            {
				int errorCode = await NumericHelper.RequestUpRoleLevel(self.ZoneScene());

				if (errorCode != ErrorCode.ERR_Success)
                {
					return;
                }
            }
            catch (Exception e)
            {
				Log.Error(e.ToString());
            }
        }
	}
}
