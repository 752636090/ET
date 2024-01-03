using ET.Common;

namespace ET
{
    [SerialGraphHandler(SerialGraphType.Story)]
    public class StoryGraphHandler : ASerialGraphHandler
    {
        protected override async ETTask EnterHold(Entity entity, HoldNode holdNode)
        {
            //FindTalkingNpc(holdNode.GetPort("ConditionPort"), (node) =>
            //{
            //    AddTalkingNpc(node);
            //});
            await ETTask.CompletedTask;
        }

        protected override async ETTask ExitHold(Entity entity, HoldNode holdNode)
        {
            //FindTalkingNpc(holdNode.GetPort("ConditionPort"), (npcNode) =>
            //{
            //    RemoveTalkingNpc(npcNode);
            //});
            await ETTask.CompletedTask;
        }

        protected override void CheckComplete(Entity entity)
        {
            //if (IsHunterClearProgress)
            //{
            //    return;
            //}
            StoryEntity story = entity as StoryEntity;
            if (story.State != StoryState.Failed
                && story.State != StoryState.CloseAfterOpen
                && story.State != StoryState.Completed
                && story.State != StoryState.TimeOut)
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
                    entity.GetParent<StoryComponent>().StoryCompleted(story);
                }
            }

            //if (IsGameEnding == false)
            {
                entity.GetParent<StoryComponent>().ExitStory();
            }
        }

        protected override void Exit(Entity entity)
        {
            entity.GetParent<StoryComponent>().ExitStory();
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
