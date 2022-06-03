
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgMainViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button E_RoleButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleButton == null )
     			{
		    		this.m_E_RoleButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Role");
     			}
     			return this.m_E_RoleButton;
     		}
     	}

		public UnityEngine.UI.Image E_RoleImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleImage == null )
     			{
		    		this.m_E_RoleImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Role");
     			}
     			return this.m_E_RoleImage;
     		}
     	}

		public UnityEngine.UI.Button E_BagButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BagButton == null )
     			{
		    		this.m_E_BagButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Bag");
     			}
     			return this.m_E_BagButton;
     		}
     	}

		public UnityEngine.UI.Image E_BagImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BagImage == null )
     			{
		    		this.m_E_BagImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Bag");
     			}
     			return this.m_E_BagImage;
     		}
     	}

		public UnityEngine.UI.Button E_BattleButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BattleButton == null )
     			{
		    		this.m_E_BattleButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Battle");
     			}
     			return this.m_E_BattleButton;
     		}
     	}

		public UnityEngine.UI.Image E_BattleImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BattleImage == null )
     			{
		    		this.m_E_BattleImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Battle");
     			}
     			return this.m_E_BattleImage;
     		}
     	}

		public UnityEngine.UI.Button E_MakeButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_MakeButton == null )
     			{
		    		this.m_E_MakeButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Make");
     			}
     			return this.m_E_MakeButton;
     		}
     	}

		public UnityEngine.UI.Image E_MakeImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_MakeImage == null )
     			{
		    		this.m_E_MakeImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Make");
     			}
     			return this.m_E_MakeImage;
     		}
     	}

		public UnityEngine.UI.Button E_TaskButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_TaskButton == null )
     			{
		    		this.m_E_TaskButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Task");
     			}
     			return this.m_E_TaskButton;
     		}
     	}

		public UnityEngine.UI.Image E_TaskImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_TaskImage == null )
     			{
		    		this.m_E_TaskImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Center/Group_Bottom/E_Task");
     			}
     			return this.m_E_TaskImage;
     		}
     	}

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
		    		this.m_E_RoleLevelText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Group_Top/Group_Head/E_RoleLevel");
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
		    		this.m_E_GoldText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Group_Top/Group_GoldTip/E_Gold");
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
		    		this.m_E_ExpText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Center/Group_Top/Group_ExpTip/E_Exp");
     			}
     			return this.m_E_ExpText;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_RoleButton = null;
			this.m_E_RoleImage = null;
			this.m_E_BagButton = null;
			this.m_E_BagImage = null;
			this.m_E_BattleButton = null;
			this.m_E_BattleImage = null;
			this.m_E_MakeButton = null;
			this.m_E_MakeImage = null;
			this.m_E_TaskButton = null;
			this.m_E_TaskImage = null;
			this.m_E_RoleLevelText = null;
			this.m_E_GoldText = null;
			this.m_E_ExpText = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_E_RoleButton = null;
		private UnityEngine.UI.Image m_E_RoleImage = null;
		private UnityEngine.UI.Button m_E_BagButton = null;
		private UnityEngine.UI.Image m_E_BagImage = null;
		private UnityEngine.UI.Button m_E_BattleButton = null;
		private UnityEngine.UI.Image m_E_BattleImage = null;
		private UnityEngine.UI.Button m_E_MakeButton = null;
		private UnityEngine.UI.Image m_E_MakeImage = null;
		private UnityEngine.UI.Button m_E_TaskButton = null;
		private UnityEngine.UI.Image m_E_TaskImage = null;
		private UnityEngine.UI.Text m_E_RoleLevelText = null;
		private UnityEngine.UI.Text m_E_GoldText = null;
		private UnityEngine.UI.Text m_E_ExpText = null;
		public Transform uiTransform = null;
	}
}
