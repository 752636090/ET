using System.Collections.Generic;

namespace ET
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgRank :Entity,IAwake,IUILogic
	{

		public DlgRankViewComponent View { get => this.Parent.GetComponent<DlgRankViewComponent>();}

		public Dictionary<int, Scroll_Item_rank> ScrollItemRanks = new Dictionary<int, Scroll_Item_rank>();
		public long Timer;
	}
}
