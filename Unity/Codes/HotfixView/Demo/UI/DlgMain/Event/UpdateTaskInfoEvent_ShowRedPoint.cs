using ET.EventType;

namespace ET
{
    public class UpdateTaskInfoEvent_ShowRedPoint : AEvent<EventType.UpdateTaskInfo>
    {
        protected override void Run(UpdateTaskInfo args)
        {
            bool isExist = args.ZoneScene.GetComponent<TasksComponent>().IsExitTaskComplete();
            if (isExist)
            {
                RedDotHelper.ShowRedDotNode(args.ZoneScene, "Task");
            }
            else
            {
                if (RedDotHelper.IsLogicAlreadyShow(args.ZoneScene, "Task"))
                {
                    RedDotHelper.HideRedDotNode(args.ZoneScene, "Task");
                }
            }
            args.ZoneScene.GetComponent<UIComponent>()?.GetDlgLogic<DlgTask>()?.Refresh();
        }
    }
}
