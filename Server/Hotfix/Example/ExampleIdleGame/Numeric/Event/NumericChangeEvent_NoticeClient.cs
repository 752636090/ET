using ET.EventType;

namespace ET
{
    public class NumericChangeEvent_NoticeClient : AEventClass<EventType.NumbericChange>
    {
        protected override void Run(object a)
        {
            EventType.NumbericChange args = a as EventType.NumbericChange;
            if (args.Parent is not Unit unit)
            {
                return;
            }
            unit.GetComponent<NumericNoticeComponent>()?.NoticeImmediately(args);
        }
    }
}
