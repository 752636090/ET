using ET.EventType;

namespace ET
{
    public class InstallComputer_AddComponent : AEvent<EventType.InstallComputer>
    {
        protected async override ETTask Run(InstallComputer arg)
        {
            await TimerComponent.Instance.WaitAsync(1000);
            Computer computer = arg.Computer;

            computer.AddComponent<PCCaseComponent>();
            computer.AddComponent<MonitorsComponent>();
            computer.AddComponent<MouseComponent>();
            computer.AddComponent<KeyBoardComponent>();
            await ETTask.CompletedTask;
        }
    }
}
