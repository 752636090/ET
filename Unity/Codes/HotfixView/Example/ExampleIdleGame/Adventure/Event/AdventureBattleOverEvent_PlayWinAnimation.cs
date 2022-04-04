using ET.EventType;

namespace ET
{
    public class AdventureBattleOverEvent_PlayWinAnimation : AEventAsync<EventType.AdventureBattleOver>
    {
        protected override async ETTask Run(AdventureBattleOver args)
        {
            args.WinUnit?.GetComponent<AnimatorComponent>().Play(MotionType.Win);
            await ETTask.CompletedTask;
        }
    }
}
