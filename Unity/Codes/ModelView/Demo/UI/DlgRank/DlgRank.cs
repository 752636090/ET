namespace ET
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgRank :Entity,IAwake,IUILogic
	{

		public DlgRankViewComponent View { get => this.Parent.GetComponent<DlgRankViewComponent>();} 

		 

	}
}
