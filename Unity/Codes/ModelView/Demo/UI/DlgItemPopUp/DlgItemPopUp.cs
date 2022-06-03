using System.Collections.Generic;

namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
    public class DlgItemPopUp : Entity, IAwake, IUILogic
    {

        public DlgItemPopUpViewComponent View { get => this.Parent.GetComponent<DlgItemPopUpViewComponent>(); }

        public Dictionary<int, Scroll_Item_entry> ScrollItemEntries;

        public long ItemId = 0;
        public ItemContainerType ItemContainerType = ItemContainerType.Bag;
    }
}
