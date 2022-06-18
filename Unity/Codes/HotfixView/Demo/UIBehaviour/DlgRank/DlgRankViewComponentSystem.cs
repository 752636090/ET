
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgRankViewComponentAwakeSystem : AwakeSystem<DlgRankViewComponent> 
	{
		public override void Awake(DlgRankViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgRankViewComponentDestroySystem : DestroySystem<DlgRankViewComponent> 
	{
		public override void Destroy(DlgRankViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
