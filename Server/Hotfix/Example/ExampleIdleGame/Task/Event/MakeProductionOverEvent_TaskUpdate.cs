using ET.EventType;

namespace ET
{
    public class MakeProductionOverEvent_TaskUpdate : AEvent<MakeProductionOver>
    {
        protected override void Run(MakeProductionOver args)
        {
            args.Unit.GetComponent<TasksComponent>().TriggerTaskAction(TaskActionType.MakeItem, count: 1, targetId: args.ProductionConfigId);
        }
    }
}
