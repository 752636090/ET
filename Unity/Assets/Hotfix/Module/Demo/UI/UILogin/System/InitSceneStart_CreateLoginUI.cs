using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginUI: AEvent
	{
		public override void Run()
		{
			UI ui = UILoginFactory.Create();
			Game.Scene.GetComponent<UIComponent>().Add(ui);

			Car carEntity = ComponentFactory.Create<Car>(); // Awake 
			carEntity.AddComponent<WheelComponent>(); // 轮子组件Awake
			carEntity.AddComponent<EngineComponent>();
			carEntity.Forward();
			// Start 游戏循环后的那一帧
			// Update Start之后的游戏循环每一帧进行调用

			carEntity.Dispose(); // Destroy 实体身上每个挂载的组件都会调用Destroy
			carEntity = null; // Destroy
		}
	}
}
