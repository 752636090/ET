
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgTaskViewComponentAwakeSystem : AwakeSystem<DlgTaskViewComponent> 
	{
		public override void Awake(DlgTaskViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgTaskViewComponentDestroySystem : DestroySystem<DlgTaskViewComponent> 
	{
		public override void Destroy(DlgTaskViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
