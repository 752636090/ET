namespace ET
{
	public  class DlgForge :Entity,IAwake,IUILogic
	{

		public DlgForgeViewComponent View { get => this.Parent.GetComponent<DlgForgeViewComponent>();} 

		 

	}
}
