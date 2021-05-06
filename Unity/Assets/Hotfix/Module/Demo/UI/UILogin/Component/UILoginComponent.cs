using System;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiLoginComponentSystem : AwakeSystem<UILoginComponent>
	{
		public override void Awake(UILoginComponent self)
		{
			self.Awake();
		}
	}
	
	public class UILoginComponent: Component
	{
		private GameObject account;
		private GameObject password;
		private GameObject loginBtn;
		private GameObject registerBtn;

		public void Awake()
		{
			ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			
			loginBtn = rc.Get<GameObject>("LoginBtn");
			loginBtn.GetComponent<Button>().onClick.Add(OnLogin);

			registerBtn = rc.Get<GameObject>("RegisterBtn");
			registerBtn.GetComponent<Button>().onClick.Add(OnRegister);

			this.account = rc.Get<GameObject>("Account");
			this.password = rc.Get<GameObject>("Password");
		}

        public void OnLogin()
		{
			LoginHelper.OnLoginAsync(this.account.GetComponent<InputField>().text,
				this.password.GetComponent<InputField>().text).Coroutine();
		}

		private void OnRegister()
		{
			LoginHelper.OnRegisterAsync(this.account.GetComponent<InputField>().text,
				this.password.GetComponent<InputField>().text).Coroutine();
		}
	}
}
