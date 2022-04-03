using ET.EventType;

namespace ET
{
    public class NumericChangeEvent_NoticeClient : AEvent<EventType.NumbericChange>
    {
        protected override void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            unit.GetComponent<NumericNoticeComponent>()?.NoticeImmediately(args);
        }
    }
}
