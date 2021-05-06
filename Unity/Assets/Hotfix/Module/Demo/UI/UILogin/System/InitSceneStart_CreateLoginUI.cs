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

            #region ECS
            //Car carEntity = ComponentFactory.Create<Car>(); // Awake 
            //carEntity.AddComponent<WheelComponent>(); // 轮子组件Awake
            //carEntity.AddComponent<EngineComponent>();
            //carEntity.Forward();
            //// Start 游戏循环后的那一帧
            //// Update Start之后的游戏循环每一帧进行调用

            //carEntity.Dispose(); // Destroy 实体身上每个挂载的组件都会调用Destroy
            //carEntity = null; // Destroy 
            #endregion

            #region 事件
            //Game.EventSystem.Run(EventIdType.ReadBook, "Game"); 
            #endregion

            #region Excel游戏数据的配置与获取
            //ItemConfig itemConfig = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(ItemConfig), 2001) as ItemConfig;
            //Log.Debug(itemConfig.Name);
            //        ItemConfig[] itemConfigs = (ItemConfig[])Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(ItemConfig));

            //        foreach (var config in itemConfigs)
            //        {
            //Log.Debug(config.Name);
            //        }

            //ItemConfigCategory itemConfigCategory = Game.Scene.GetComponent<ConfigComponent>().GetCategory(typeof(ItemConfig)) as ItemConfigCategory;
            //Log.Debug("maxAddHp : " + itemConfigCategory.GetMaxAddHp().ToString()); 
            #endregion


        }
	}
}
