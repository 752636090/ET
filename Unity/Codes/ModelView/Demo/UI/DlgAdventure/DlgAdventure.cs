using System.Collections.Generic;

namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	public  class DlgAdventure : Entity,IAwake,IUILogic
	{

		public DlgAdventureViewComponent View { get => this.Parent.GetComponent<DlgAdventureViewComponent>();}

		// 猜的
		public Dictionary<int, Scroll_Item_battleLevel> ScrollItemBattleLevels = new Dictionary<int, Scroll_Item_battleLevel>();
	}
}
