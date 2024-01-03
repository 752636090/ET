using ET.Common;
using ET.Story;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(StoryEntity))]
    [FriendOf(typeof(StoryEntity))]
    [FriendOfAttribute(typeof(ET.StoryComponent))]
    public static partial class StoryEntitySystem
    {
        [EntitySystem]
        public static void Awake(this StoryEntity self, int graphId)
        {
            self.GraphId = graphId;
            self.Deserialize();
        }

        [EntitySystem]
        public static void Deserialize(this StoryEntity self)
        {
            if (!IGraphsComponent.GraphConfigDict[SerialGraphType.Story].TryGetValue(self.GraphId, out SerialGraph graph))
            {
                Log.Error($"不存在剧情事件 Id:{self.GraphId}");
            }
            self.Blackboard = new(self);
            self.Graph = graph;
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
                    self.RecordConditionCheck(self.GetParent<StoryComponent>().OpenConditionNodes, openConditionRootPort);
                    // 事件按关闭条件归类至指定列表
                    self.RecordConditionCheck(self.GetParent<StoryComponent>().CloseConditionNodes, openExitCondition);
                    break;

                //已获知
                case StoryState.Opened:
                    self.Blackboard.AddActiveTime(self.OpenNode);
                    // 事件按播放条件归类至指定列表
                    self.RecordConditionCheck(self.GetParent<StoryComponent>().StartConditionNodes, startConditionRootPort);
                    // 事件按关闭条件归类至指定列表
                    self.RecordConditionCheck(self.GetParent<StoryComponent>().CloseStartedConditionNodes, startExitCondition);
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

                            self.RecordConditionCheck(self.GetParent<StoryComponent>().HoldNodes, node.GetPort("ConditionPort"));
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
                    self.RecordConditionCheck(self.GetParent<StoryComponent>().CloseStartedConditionNodes, startExitCondition);
                    break;

                // 完成或失败
                case StoryState.Completed:
                case StoryState.Failed:
                    self.Blackboard.AddActiveTime(self.OpenNode);
                    self.Blackboard.AddActiveTime(self.StartNode);
                    break;
                // 逾期
                case StoryState.TimeOut:
                    break;
            }
        }
    }
}
