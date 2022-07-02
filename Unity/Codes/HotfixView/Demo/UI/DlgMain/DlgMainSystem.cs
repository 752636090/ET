using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public static class DlgMainSystem
    {

        public static void RegisterUIEvent(this DlgMain self)
        {
            self.View.E_RoleButton.AddListenerAsync(() => { return self.OnRoleButtonClickHandler(); });
            self.View.E_BattleButton.AddListenerAsync(() => { return self.OnBattleButtonClickHandler(); });
            self.View.E_BagButton.AddListenerAsync(() => { return self.OnBagButtonClickHandler(); }); // 猜的
            self.View.E_MakeButton.AddListenerAsync(() => { return self.OnMakeButtonClickHandler(); }); // 猜的
            self.View.E_TaskButton.AddListenerAsync(() => { return self.OnTaskButtonClickHandler(); }); // 猜的
            self.View.E_RankButton.AddListenerAsync(() => { return self.OnRankButtonClickHandler(); }); // 猜的
            self.View.E_ChatButton.AddListenerAsync(() => { return self.OnChatButtonClickHandler(); }); // 猜的
            RedDotHelper.AddRedDotNodeView(self.ZoneScene(), "Role", self.View.E_RoleButton.gameObject, Vector3.one, new Vector3(75, 55, 0));
            RedDotHelper.AddRedDotNodeView(self.ZoneScene(), "Forge", self.View.E_MakeButton.gameObject, Vector3.one, new Vector3(75, 55, 0)); // 猜的
            RedDotHelper.AddRedDotNodeView(self.ZoneScene(), "Task", self.View.E_TaskButton.gameObject, Vector3.one, new Vector3(75, 55, 0)); // 猜的
        }

        public static void OnUnLoadWindow(this DlgMain self)
        {
            RedDotMonoView redDotMonoView = self.View.E_RoleButton.GetComponent<RedDotMonoView>();
            RedDotHelper.RemoveRedDotView(self.ZoneScene(), "Role", out redDotMonoView);
        }

        public static void ShowWindow(this DlgMain self, Entity contextData = null)
        {
            self.Refresh().Coroutine();
        }

        public static async ETTask Refresh(this DlgMain self)
        {
            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.ZoneScene().CurrentScene());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            self.View.E_RoleLevelText.SetText($"Lv.{numericComponent.GetAsInt((int)NumericType.Level)}");
            self.View.E_GoldText.SetText(numericComponent.GetAsInt((int)NumericType.Gold).ToString());
            self.View.E_ExpText.SetText(numericComponent.GetAsInt((int)NumericType.Exp).ToString());

            await ETTask.CompletedTask;
        }

        public static async ETTask OnRoleButtonClickHandler(this DlgMain self)
        {
            //try
            //{
            //    int error = await NumericHelper.TestUpdateNumeric(self.ZoneScene());
            //    if (error != ErrorCode.ERR_Success)
            //    {
            //        return;
            //    }

            //    Log.Debug("发送更新属性测试消息成功");
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e.ToString());
            //}

            // 都是猜的
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_RoleInfo);
            await ETTask.CompletedTask;
        }

        // 里面是猜的
        public static async ETTask OnBattleButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Adventure);
            await ETTask.CompletedTask;
        }

        // 整个都是猜的
        public static async ETTask OnBagButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Bag);
            await ETTask.CompletedTask;
        }

        // 整个都是猜的
        public static async ETTask OnMakeButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Forge);
            await ETTask.CompletedTask;
        }

        // 整个都是猜的
        public static async ETTask OnTaskButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Task);
            await ETTask.CompletedTask;
        }

        // 整个都是猜的
        public static async ETTask OnRankButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Rank);
            await ETTask.CompletedTask;
        }

        // 整个都是猜的
        public static async ETTask OnChatButtonClickHandler(this DlgMain self)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Chat);
            await ETTask.CompletedTask;
        }
    }
}
