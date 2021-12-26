

namespace ET
{
	public class AppStartInitFinish_RemoveLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin);

            #region Learn
			Computer computer = args.ZoneScene.AddChild<Computer>();

			computer.AddComponent<PCCaseComponent>();
			computer.AddComponent<MonitorsComponent>();
			computer.AddComponent<KeyBoardComponent>();
			computer.AddComponent<MouseComponent>();

			computer.Start();
            #endregion
        }
	}
}
