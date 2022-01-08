

namespace ET
{
	public class AppStartInitFinish_RemoveLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin);

			#region Learn
			// 放置第8节课会讲ZoneScene.AddChild和unitComponent.AddChildWithId的区别
			//Computer computer = args.ZoneScene.AddChild<Computer>();

			////Game.EventSystem.Publish(new EventType.InstallComputer() { Computer = computer });
			////Game.EventSystem.PublishAsync(new EventType.InstallComputer() { Computer = computer }).Coroutine();
			//await Game.EventSystem.PublishAsync(new EventType.InstallComputer() { Computer = computer });
			//computer.Start();

			////computer.AddComponent<PCCaseComponent>();
			////computer.AddComponent<MonitorsComponent>();
			////computer.AddComponent<KeyBoardComponent>();
			////computer.AddComponent<MouseComponent>();

			////computer.Start();

			////await TimerComponent.Instance.WaitAsync(3000);

			////computer.Dispose();

			////UnitConfig config = UnitConfigCategory.Instance.Get(1001);

			////Log.Debug(config.Name);

			////System.Collections.Generic.Dictionary<int, UnitConfig> allUnitConfig = UnitConfigCategory.Instance.GetAll();

			////foreach (UnitConfig unitConfig in allUnitConfig.Values)
			////         {
			////	Log.Debug(unitConfig.Name);
			////	Log.Debug(unitConfig.TestValue.ToString());
			////         }

			////UnitConfig heightConfig = UnitConfigCategory.Instance.GetUnitConfigByHeight(178);

			////Log.Debug(heightConfig.Name);

			Log.Debug("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

			//await this.TestAsync();
			//this.TestAsync().Coroutine();
			int result = await this.TestResultAsync();
			Log.Debug(result.ToString());

			Log.Debug("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
			#endregion
		}

		#region Learn
		public async ETTask TestAsync()
        {
			Log.Debug("1111111111111111111111111111111");
			await TimerComponent.Instance.WaitAsync(2000);
			Log.Debug("2222222222222222222222222222222");
			//await ETTask.CompletedTask; // 用了CompletedTask就可以认为是同步
		}

		/// <summary>
		/// 返回结果的异步方法肯定不能用Coroutine
		/// </summary>
		/// <returns></returns>
		public async ETTask<int> TestResultAsync()
        {
			Log.Debug("1111111111111111111111111111111");
			await TimerComponent.Instance.WaitAsync(2000);
			Log.Debug("2222222222222222222222222222222");

			return 10;
		}
        #endregion
    }
}
