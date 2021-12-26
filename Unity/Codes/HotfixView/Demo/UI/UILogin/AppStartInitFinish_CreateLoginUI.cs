

namespace ET
{
	public class AppStartInitFinish_RemoveLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin);

			#region Learn
			// 放置第8节课会讲ZoneScene.AddChild和unitComponent.AddChildWithId的区别
			Computer computer = args.ZoneScene.AddChild<Computer>();

			computer.AddComponent<PCCaseComponent>();
			computer.AddComponent<MonitorsComponent>();
			computer.AddComponent<KeyBoardComponent>();
			computer.AddComponent<MouseComponent>();

			computer.Start();

			await TimerComponent.Instance.WaitAsync(3000);

			computer.Dispose();
            #endregion
        }
	}
}
