using ET.Common;

namespace ET
{
    [SerialGraphHandler(SerialGraphType.Story)]
    public class StoryGraphHandler : ASerialGraphHandler
    {
        protected override async ETTask EnterHold(SerialGraph graph, HoldNode holdNode)
        {
            //FindTalkingNpc(holdNode.GetPort("ConditionPort"), (node) =>
            //{
            //    AddTalkingNpc(node);
            //});
            await ETTask.CompletedTask;
        }

        protected override async ETTask ExitHold(SerialGraph graph, HoldNode holdNode)
        {
            //FindTalkingNpc(holdNode.GetPort("ConditionPort"), (npcNode) =>
            //{
            //    RemoveTalkingNpc(npcNode);
            //});
            await ETTask.CompletedTask;
        }

        protected override void CheckComplete(SerialGraph graph)
        {
            //if (IsHunterClearProgress)
            //{
            //    return;
            //}
            StoryEntity entity = graph.Blackboard.GetEntity<StoryEntity>();
            if (entity.State != StoryState.Failed
                && entity.State != StoryState.CloseAfterOpen
                && entity.State != StoryState.Completed
                && entity.State != StoryState.TimeOut)
            {
                //if ((entity as StoryEntity).OpenNode.IsRepeat)
                //{
                //    if (data.TimeLimitSaveID > 0)
                //    {
                //        CloseTimeLimit(graph.name);
                //    }
                //    ResetSelectedTimes(graph);
                //}
                //else
                {
                    entity.StoryCompleted();
                }
            }

            //if (IsGameEnding == false)
            {
                entity.GetParent<StoryComponent>().ExitStory();
            }
        }

        protected override void Exit(SerialGraph graph)
        {
            graph.GetEntity<StoryEntity>().GetParent<StoryComponent>().ExitStory();
        }

        //if (IsGameEnding)
        //{
        //    IsGameEnding = false;
        //    UICabMapManager.Instance.OnExitStory(true);
        //    // 回到初始界面
        //    GameManager.QuitToMenu();
        //}
    }
}
