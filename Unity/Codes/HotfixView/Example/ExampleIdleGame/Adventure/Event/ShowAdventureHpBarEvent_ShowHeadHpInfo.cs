using ET.EventType;

namespace ET
{
    public class ShowAdventureHpBarEvent_ShowHeadHpInfo : AEvent<EventType.ShowAdventureHpBar>
    {
        protected override void Run(ShowAdventureHpBar args)
        {
            args.Unit.GetComponent<HeadHpViewComponent>().SetVisible(args.isShow);
            args.Unit.GetComponent<HeadHpViewComponent>().SetHp();
        }
    }
}
