using System.Collections.Generic;

namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	public class DlgRoleInfo :Entity,IAwake,IUILogic
	{

		public DlgRoleInfoViewComponent View { get => this.Parent.GetComponent<DlgRoleInfoViewComponent>();}

		// 自己猜的定义
		public Dictionary<int, Scroll_Item_attribute> ScrollItemAttributes = new Dictionary<int, Scroll_Item_attribute>();
	}
}
