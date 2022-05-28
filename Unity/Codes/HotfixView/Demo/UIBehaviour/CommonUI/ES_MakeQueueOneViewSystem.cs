
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class ES_MakeQueueOneAwakeSystem : AwakeSystem<ES_MakeQueueOne,Transform> 
	{
		public override void Awake(ES_MakeQueueOne self,Transform transform)
		{
			self.uiTransform = transform;
		}
	}


	[ObjectSystem]
	public class ES_MakeQueueOneDestroySystem : DestroySystem<ES_MakeQueueOne> 
	{
		public override void Destroy(ES_MakeQueueOne self)
		{
			self.DestroyWidget();
		}
	}
}
