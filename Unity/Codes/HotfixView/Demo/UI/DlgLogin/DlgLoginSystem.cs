﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgLoginSystem
	{

		public static void RegisterUIEvent(this DlgLogin self)
		{
			// AddListenerAsync 登录流程走完才能点下一次，防止频繁点击登录
			self.View.E_LoginButton.AddListenerAsync(() => { return self.OnLoginClickHandler();});
		}

		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{
			
		}
		
		public static async ETTask OnLoginClickHandler(this DlgLogin self)
		{
            try
            {
                int errorCode = await LoginHelper.Login(
                        self.DomainScene(),
                        ConstValue.LoginAddress,
                        self.View.E_AccountInputField.GetComponent<InputField>().text,
                        self.View.E_PasswordInputField.GetComponent<InputField>().text);
				if (errorCode != ErrorCode.ERR_Success)
                {
					Log.Error(errorCode.ToString());
					return;
                }
				// TODO 限时登录之后的页面逻辑

            }
            catch (Exception e)
            {
				Log.Error(e.ToString());
            }
		}
		
		public static void HideWindow(this DlgLogin self)
		{

		}
	}
}
