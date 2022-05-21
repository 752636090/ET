using System.Collections.Generic;

namespace ET
{
    public class DlgForge : Entity, IAwake, IUILogic
    {
        public DlgForgeViewComponent View { get => this.Parent.GetComponent<DlgForgeViewComponent>(); }

        public Dictionary<int, Scroll_Item_production> ScrollItemProductions;

        public long MakeQueueTimer = 0;
    }
}
