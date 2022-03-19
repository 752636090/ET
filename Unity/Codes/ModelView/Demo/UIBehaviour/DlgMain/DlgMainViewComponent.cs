
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgMainViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text E_RoleLevelText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleLevelText == null )
     			{
		    		this.m_E_RoleLevelText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Top/Image/E_RoleLevel");
     			}
     			return this.m_E_RoleLevelText;
     		}
     	}

		public UnityEngine.UI.Text E_GoldText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_GoldText == null )
     			{
		    		this.m_E_GoldText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Top/Image (1)/E_Gold");
     			}
     			return this.m_E_GoldText;
     		}
     	}

		public UnityEngine.UI.Text E_ExpText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_ExpText == null )
     			{
		    		this.m_E_ExpText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Top/Image (2)/E_Exp");
     			}
     			return this.m_E_ExpText;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_RoleLevelText = null;
			this.m_E_GoldText = null;
			this.m_E_ExpText = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_RoleLevelText = null;
		private UnityEngine.UI.Text m_E_GoldText = null;
		private UnityEngine.UI.Text m_E_ExpText = null;
		public Transform uiTransform = null;
	}
}
