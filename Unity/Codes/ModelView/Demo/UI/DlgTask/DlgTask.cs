using System.Collections.Generic;

namespace ET
{
	public  class DlgTask :Entity,IAwake,IUILogic
	{
        public DlgTaskViewComponent View { get => this.Parent.GetComponent<DlgTaskViewComponent>();} 

        public Dictionary<int, Scroll_Item_task> ScrollItemTasks; // 猜的


	}
}
