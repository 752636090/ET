
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgForgeViewComponentAwakeSystem : AwakeSystem<DlgForgeViewComponent> 
	{
		public override void Awake(DlgForgeViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgForgeViewComponentDestroySystem : DestroySystem<DlgForgeViewComponent> 
	{
		public override void Destroy(DlgForgeViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
