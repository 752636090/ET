using ET.Common;
using ET.Story;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(StoryEntity))]
    [FriendOf(typeof(StoryEntity))]
    public static partial class StoryEntitySystem
    {
        [EntitySystem]
        public static void Awake(this StoryEntity self, int graphId)
        {
            self.GraphId = graphId;
            self.Blackboard = new SerialGraphBlackboard();
            self.Deserialize();
        }

        [EntitySystem]
        public static void Deserialize(this StoryEntity self)
        {
            if (!StoryComponent.GraphBytesDict.TryGetValue(self.GraphId, out byte[] bytes))
            {
                Log.Error($"不存在剧情事件 Id:{self.GraphId}");
            }
            self.Graph = MongoHelper.Deserialize<SerialGraph>(bytes);
            self.Graph.AfterDeserialize();
            self.Blackboard.Cts = new ETCancellationToken();
            self.Blackboard.Entity = self;
            self.Graph.Blackboard = self.Blackboard;
            foreach (SerialNode node in self.Graph.Nodes)
            {
                bool found = false;
                if (node is StoryHeadInfoNode headInfoNode)
                {
                    self.HeadNode = headInfoNode;
                    found = true;
                }
                else if (node is StoryOpenNode openNode)
                {
                    self.OpenNode = openNode;
                    found = true;
                }
                else if (node is StoryStartNode startPlayNode)
                {
                    self.StartNode = startPlayNode;
                    found = true;
                }
                if (found && self.HeadNode != null && self.OpenNode != null && self.StartNode != null)
                {
                    break;
                }
            }
        }

        public static void StoryCompleted(this StoryEntity self)
        {
            Log.Debug($"剧情事件{self.GraphId}完成");
            if (self.State == StoryState.Completed
                || self.State == StoryState.CloseAfterOpen
                || self.State == StoryState.Close)
            {
                return;
            }
            //CloseTimeLimit(graphName);
            self.State = StoryState.Completed;
            //RemoveCheckNote(graphName);
            //StoryGraph graph = GetGraph(graphName);
            //if (graph != null)
            //{
            //    NewNoteList.Remove(graph);
            //}
            //UpdateRomingMainStoryText();

            //if (graph != null)
            //{
            //    graph.startnode.ClearHunter();
            //}

            //CheckCondition(typeof(HaveStoryFinishCondition));

            //GameEventManager.Instance.FireEvent(GameEventType.OnStoryComplated, new object[] { graphName });

            if (!self.GetParent<StoryComponent>().IsProcessingStory)
            {
                self.GetParent<StoryComponent>().ExitStory();    // 走一次清理
            }
        }

        public static void AnalysisGraphConfig(this StoryEntity self)
        {
            StoryState state = self.State;

            SerialPort openConditionRootPort = self.OpenNode.GetPort("Conditions");
            SerialPort openExitCondition = self.OpenNode.GetPort("ExitConditions");
            SerialPort startConditionRootPort = self.StartNode.GetPort("ConditionPort");
            SerialPort startExitCondition = self.StartNode.GetPort("ExitConditions");

            switch (state)
            {
                // 未获知
                case StoryState.NotOpen:
                    // 事件按发现条件归类至指定列表
                    self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().OpenConditionNodes, openConditionRootPort);
                    // 事件按关闭条件归类至指定列表
                    self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().CloseConditionNodes, openExitCondition);
                    break;

                //已获知
                case StoryState.Opened:
                    self.OpenNode.AddTime();
                    // 事件按播放条件归类至指定列表
                    self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().StartConditionNodes, startConditionRootPort);
                    // 事件按关闭条件归类至指定列表
                    self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().CloseStartedConditionNodes, startExitCondition);
                    break;

                // 已播放
                case StoryState.Started:
                    List<int> holdIdList = self.Blackboard.HoldNodes;
                    if (holdIdList != null && holdIdList.Count > 0)
                    {
                        foreach (HoldNode node in self.Graph.Nodes)
                        {
                            if (node == null || !holdIdList.Contains(node.Id))
                            {
                                continue;
                            }

                            self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().HoldNodes, node.GetPort("ConditionPort"));
                        }
                    }
                    //if (entity.StartNode.repeatTask && entity.HeadNode.taskLineType != StoryHeadInfoNode.TaskLineType.Cab)
                    //{
                    //    RecordConditionCheck(RepeatStoryForceFinishNodes, entity.StartNode.GetPort("forceFinish"));
                    //}
                    //// 时效标签生效
                    //if (data != null && data.TimeLimitSaveID > 0)
                    //{
                    //    ExistTimeLimitNodeGraphList.Add(graph);
                    //}
                    // 事件按关闭条件归类至指定列表
                    self.Graph.RecordConditionCheck(self.GetParent<StoryComponent>().CloseStartedConditionNodes, startExitCondition);
                    break;

                // 完成或失败
                case StoryState.Completed:
                case StoryState.Failed:
                    self.OpenNode.AddTime();
                    self.StartNode.AddTime();
                    break;
                // 逾期
                case StoryState.TimeOut:
                    break;
            }
        }
    }
}
