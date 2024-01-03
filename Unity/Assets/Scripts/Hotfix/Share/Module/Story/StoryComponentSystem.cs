using ET.Common;
using ET.Story;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [EntitySystemOf(typeof(StoryComponent))]
    [FriendOf(typeof(StoryComponent))]
    [FriendOfAttribute(typeof(ET.StoryEntity))]
    public static partial class StoryComponentSystem
    {
        [EntitySystem]
        public static void Awake(this StoryComponent self)
        {
            foreach (int id in StoryComponent.GraphBytesDict.Keys)
            {
                self.AddStory(id);
            }
            self.CheckAfterLoading();
            // 临时强行触发，因为每个剧情事件都有条件才能触发
            self.StoryStart(self.StoryDict.Values.FirstOrDefault().StartNode);
        }

        //public static async ETTask AAA(this StoryComponent self)
        //{
        //    await self.Fiber().TimerComponent.WaitAsync(2000);

        //}

        [EntitySystem]
        public static void Deserialize(this StoryComponent self)
        {
            foreach (StoryEntity entity in self.Children.Values)
            {
                self.StoryDict.Add(entity.GraphId, entity);
                entity.AnalysisGraphConfig();
            }
        }

        public static void AddStory(this StoryComponent self, int id)
        {
            StoryEntity entity = self.AddChild<StoryEntity, int>(id);
            self.StoryDict.Add(id, entity);
        }

        public static bool IsOptionClosed(this StoryComponent self, SerialGraph graph, DialogOptionNode node)
        {
            int saveID = node.Id;
            if (node.ShowSelected)
            {
                // 为了兼容一开始不让反复选择, 之后又改为可重复选择的情况
                graph.Blackboard.GetEntity<StoryEntity>().TurnedOffOptions.Remove(saveID);
                return false;
            }
            if (graph.Blackboard.GetEntity<StoryEntity>().TurnedOffOptions.Contains(saveID))
            {
                return true;
            }
            return false;
        }

        public static void ShowDialogOptions(this StoryComponent self, List<DialogOptionNode> nodes)
        {
            // TODO
            Log.Debug("显示对话选项");
        }

        public static void StoryCompleted(this StoryComponent self, StoryEntity story)
        {
            Log.Debug($"剧情事件{story.GraphId}完成");
            if (story.State == StoryState.Completed
                || story.State == StoryState.CloseAfterOpen
                || story.State == StoryState.Close)
            {
                return;
            }
            //CloseTimeLimit(graphName);
            story.State = StoryState.Completed;
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

            if (!story.GetParent<StoryComponent>().IsProcessingStory)
            {
                story.GetParent<StoryComponent>().ExitStory();    // 走一次清理
            }
        }

        public static void ExitStory(this StoryComponent self)
        {
            // 原项目有一堆对其它系统的处理
            self.IsProcessingStory = false;
        }

        public static void CheckAfterLoading(this StoryComponent self)
        {
            Log.Debug("临时直接检测剧情事件");
            if (!self.IsProcessingStory)
            {
                self.TriggerLater();
                //if (!self.IsProcessingStory)
                //{
                //    InvokeQuarterAtStoryOut();
                //}
            }

            //CheckCondition(typeof(LunarCalendarCondition));
            //CheckCondition(typeof(HaveTraitCondition));
            //CheckCondition(typeof(CheckBuildingBrokenCondition));

            self.CheckTypeAtStoryOut();
        }

        /// <summary>
        /// 检测是否满足了带有指定类型的条件链
        /// </summary>
        public static void CheckCondition(this StoryComponent self, Type conditionType, IConditionNodeParam param = null)
        {
            //if (IsGameEnding)
            //{
            //    return;
            //}
            //if (RoamingManager.Instance.player == null)
            //{
            //    AddCheckTypeToWait(conditionType, extra, StoryDicType.All);
            //    return;
            //}

            // 检测事件是否获知前关闭
            self.CheckConditionNode(self.CloseConditionNodes, conditionType, param, self.OnCheckExitConditionNode);
            // 检测事件是否被玩家获知
            self.CheckConditionNode(self.OpenConditionNodes, conditionType, param, self.OnCheckOpenConditionNode);
            // 检测事件是否获知后关闭
            self.CheckConditionNode(self.CloseStartedConditionNodes, conditionType, param, self.OnCheckExitConditionNode, false, false, false);
            //// 检测事件是否结束循环
            //self.CheckConditionNode(self.RepeatStoryForceFinishNodes, conditionType, extra, OnCheckRepeatStoryForceFinishNode, false, false, false);

            // 玩家一些情况下不触发播放
            if (!self.CheckGameStateCanActiveStory())
            {
                self.AddCheckTypeToWait(conditionType, param, StoryCheckDicType.Start);
                self.AddCheckTypeToWait(conditionType, param, StoryCheckDicType.Hold);
                return;
            }

            // 可以进行接下来的检测
            self.OnCheckStartAtStoryOut(conditionType, param);
            if (self.IsProcessingStory)
            {
                self.AddCheckTypeToWait(conditionType, param, StoryCheckDicType.Hold);
            }
            else
            {
                self.OnCheckHoldAtStoryOut(conditionType, param);
            }

            //if (!self.IsProcessingStory)
            //{
            //    if (conditionType != typeof(CloseToNpcCondition) && conditionType != typeof(EnterAreaCondition))
            //    {
            //        // 检测是否显示附近npc的事件气泡
            //        UpdateAllNpcTalkEmojiShow(true);
            //    }
            //}
        }

        // 判断当前是否可以激活事件
        public static bool CheckGameStateCanActiveStory(this StoryComponent self)
        {
            return true;
        }

        private static void CheckConditionNode(this StoryComponent self, UnOrderMultiMap<Type, SerialPort> graphConditionNodes,
            Type conditionType, IConditionNodeParam param, ActionCheckStorySuccess callFunc,
            bool onlyOneTrigger = false, bool tryTriggerAtStoryOut = false, bool checkNodeTimes = true)
        {
            if (!graphConditionNodes.TryGetValue(conditionType, out List<SerialPort> ports))
            {
                return;
            }
            List<CheckAtStoryOut> triggerList = new List<CheckAtStoryOut>();
            foreach (SerialPort port in ports)
            {
                if (self.StoryDict.TryGetValue(port.Node.Graph.Id, out StoryEntity story))
                {
                    if (story.State == StoryState.Failed
                        || story.State == StoryState.Completed
                        || story.State == StoryState.Close
                        || story.State == StoryState.TimeOut
                        || story.State == StoryState.CloseAfterOpen)
                    {
                        continue;
                    }
                }

                List<ConditionNode> successList = new List<ConditionNode>();
                bool trigger = story.Graph.CheckConditionFromRoot(port, conditionType, param, successList, checkNodeTimes);
                if (trigger)
                {
                    CheckAtStoryOut check = ObjectPool.Instance.Fetch<CheckAtStoryOut>();
                    check.Port = port;
                    check.ConditionType = conditionType;
                    check.Param = param;
                    check.CallFunc = callFunc;
                    check.SuccessList = successList;
                    triggerList.Add(check);
                }
            }

            List<Action<List<object>>> resultList = new List<Action<List<object>>>();
            List<List<object>> resultObjsList = new List<List<object>>();
            int logName = 0;
            for (int i = 0; i < triggerList.Count; i++)
            {
                CheckAtStoryOut check = triggerList[i];
                if (resultList.Count > 0 && onlyOneTrigger)
                {
                    if (tryTriggerAtStoryOut)
                    {
                        // 加入"事件后重新检测"队列
                        check.SuccessList = null;
                        self.ListTriggerLaterAtStoryOut.Add(check);
                        Log.Warning($"由于正在播放事件{logName}, {check.Port.Node.Graph.Id}稍后再重新检测");
                    }
                    continue;
                }
                // 触发
                Action<List<object>> result = check.CallFunc(check.Port.Node, check.SuccessList);
                if (result != null)
                {
                    resultList.Add(result);
                    List<object> resultObjs = ObjectPool.Instance.Fetch<List<object>>();
                    resultObjs.Add(check.Port.Node);
                    resultObjs.Add(check.SuccessList);
                    logName = check.Port.Node.Graph.Id;
                }
            }
            for (int i = 0; i < resultList.Count && i < resultObjsList.Count; i++)
            {
                resultList[i].Invoke(resultObjsList[i]);
            }
        }

        public static void AddCheckTypeToWait(this StoryComponent self, Type conditionType, IConditionNodeParam param, StoryCheckDicType dicType)
        {
            foreach (StoryWaitCheck waitCheck in self.CheckTypeToWait)
            {
                if (waitCheck.ConditionType != conditionType || waitCheck.DicType != dicType)
                {
                    continue;
                }
                if ((param == null && waitCheck.Param != null) || (param != null && waitCheck.Param == null))
                {
                    continue;
                }
                else if (param == null && waitCheck.Param == null)
                {
                    return;
                }
                if (param.GetType() != waitCheck.Param.GetType() || !param.Equals(waitCheck.Param))
                {
                    continue;
                }
                return;
            }
            StoryWaitCheck w = new StoryWaitCheck();
            w.ConditionType = conditionType;
            w.Param = param;
            w.DicType = dicType;
            self.CheckTypeToWait.Add(w);
        }

        /// <summary>
        /// 事件播放时不能进行的检测(Start节点)
        /// </summary>
        public static void OnCheckStartAtStoryOut(this StoryComponent self, Type conditionType, IConditionNodeParam param)
        {
            // 检测事件是否播放
            self.CheckConditionNode(self.StartConditionNodes, conditionType, param, self.OnCheckStartAtStoryOut_Cb, true, true);
        }
        private static Action<List<object>> OnCheckStartAtStoryOut_Cb(this StoryComponent self, SerialNode node, List<ConditionNode> successList)
        {
            //SaveObjectStory.GraphData data = dataContainer.initedObject.GetOrCreateGraphData(node.graph.name);
            StoryStartNode startNode = node as StoryStartNode;
            //if (startNode.repeatTask && (node.graph as StoryGraph).headnode.taskLineType != StoryHeadInfoNode.TaskLineType.Cab)
            //{
            //    int LastPlayTime = data.LastPlayTime;
            //    if (LastPlayTime >= 0 && InGameTimeManager.Instance.TotalQuaterPassedAsInt - LastPlayTime < startNode.MinTimeDiv)
            //    {
            //        // CD时间内不播
            //        return null;
            //    }
            //    data.LastPlayTime = InGameTimeManager.Instance.TotalQuaterPassedAsInt;// 记录循环时刻
            //}
            return (List<object> objs) => { self.StoryStart(startNode); };
        }

        public static bool StoryStart(this StoryComponent self, StoryStartNode startNode)
        {
            //if (RoamingManager.Instance.player == null)
            //{
            //    Debug.LogWarningFormat("事件触发了但是主角没有创建{0}", startNode.graph.name);
            //    return false;
            //}
            StoryEntity entity = startNode.Graph.GetEntity<StoryEntity>();
            self.IsProcessingStory = true;

            //FindTalkingNpc(startNode.GetInputPort("conditionPort"), (npcID) =>
            //{
            //    RemoveTalkingNpc(npcID);
            //});

            startNode.AddTime();   // 标记已触发
            entity.State = StoryState.Started;
            Log.Warning($"触发事件 {startNode.Graph.Id}");

            //// 判断有生效中的时效标签
            //TimeLimitNode limitNode = (startNode.graph as StoryGraph).GetActiveTimeLimitNode(data.TimeLimitSaveID);
            //if (limitNode != null)
            //{
            //    limitNode.Continue();
            //    Debug.LogErrorFormat("会有这种情况?事件{0}进入时, 有时效标签{1}生效", startNode.graph.name, data.TimeLimitSaveID);
            //}
            //else
            {
                startNode.Graph.ContinueArrange(startNode, "PlayPort");
            }
            return true;
        }

        public static void TriggerLater(this StoryComponent self)
        {
            //if (IsGameEnding)
            //{
            //    return;
            //}
            if (self.IsProcessingStory)
            {
                return;
            }
            using ListComponent<CheckAtStoryOut> triggerList = ListComponent<CheckAtStoryOut>.Create();
            foreach (CheckAtStoryOut check in self.ListTriggerLaterAtStoryOut)
            {
                SerialPort port = check.Port;
                ActionCheckStorySuccess callfunc = check.CallFunc;
                StoryEntity entity = port.Node.Graph.GetEntity<StoryEntity>();
                if (entity != null)
                {
                    if (entity.State == StoryState.Failed
                        || entity.State == StoryState.Close
                        || entity.State == StoryState.TimeOut
                        || entity.State == StoryState.CloseAfterOpen)
                    {
                        continue;
                    }
                }

                List<ConditionNode> successList = ObjectPool.Instance.Fetch<List<ConditionNode>>();
                check.SuccessList = successList;
                bool trigger = entity.Graph.CheckConditionFromRoot(port, check.ConditionType, check.Param, successList);
                if (trigger)
                {
                    triggerList.Add(check); // 只保留条件通过的
                }
            }
            self.ListTriggerLaterAtStoryOut.Clear();

            int logName = 0;
            Action<List<object>> result = null;
            List<object> resultobjs = null;
            for (int i = 0; i < triggerList.Count; i++)
            {
                CheckAtStoryOut check = triggerList[i];
                if (self.IsProcessingStory == false && result == null)
                {
                    // 只触发第一个
                    result = check.CallFunc(check.Port.Node, check.SuccessList);
                    if (result != null)
                    {
                        resultobjs = ObjectPool.Instance.Fetch<List<object>>();
                        resultobjs.Add(check.Port.Node);
                        resultobjs.Add(check.SuccessList);
                        logName = check.Port.Node.Graph.Id;
                    }
                }
                else
                {
                    // 加入"事件后重新检测"队列
                    check.SuccessList = null;
                    self.ListTriggerLaterAtStoryOut.Add(check);
                    Log.Debug($"由于正在播放事件{logName}, 稍后再重新检测{check.Port.Node.Graph.Id}");
                }
            }
            result?.Invoke(resultobjs);
            if (resultobjs != null)
            {
                resultobjs.Clear();
                ObjectPool.Instance.Recycle(resultobjs);
            }
        }

        /// <summary>
        /// CheckTypeToWait保存了事件演出中各种type数据改变, 此时处理这些type, 检测是否触发事件
        /// </summary>
        public static void CheckTypeAtStoryOut(this StoryComponent self, bool checkAll = false)
        {
            while (self.CheckTypeToWait.Count > 0)
            {
                if (self.CheckGameStateCanActiveStory() == false)
                {
                    return;
                }

                StoryWaitCheck w = self.CheckTypeToWait[0];

                if (checkAll)
                {
                    if (w.DicType == StoryCheckDicType.All)
                    {
                        self.CheckTypeToWait.RemoveAt(0);
                        self.CheckCondition(w.ConditionType, w.Param);
                    }
                }
                else
                {
                    self.CheckTypeToWait.RemoveAt(0);
                    if (w.DicType == StoryCheckDicType.All)
                    {
                        self.CheckCondition(w.ConditionType, w.Param);
                    }
                    else if (w.DicType == StoryCheckDicType.Start)
                    {
                        self.OnCheckStartAtStoryOut(w.ConditionType, w.Param);
                    }
                    else if (w.DicType == StoryCheckDicType.Hold)
                    {
                        self.OnCheckHoldAtStoryOut(w.ConditionType, w.Param);
                    }
                }
            }
        }

        /// <summary>
        /// 事件播放时不能进行的检测(Hold节点)
        /// </summary>
        public static void OnCheckHoldAtStoryOut(this StoryComponent self, Type conditionType, IConditionNodeParam param)
        {
            self.CheckConditionNode(self.HoldNodes, conditionType, param, self.OnCheckHoldAtStoryOut_Cb, true, true);
        }
        private static Action<List<object>> OnCheckHoldAtStoryOut_Cb(this StoryComponent self, SerialNode node, List<ConditionNode> successList)
        {
            HoldNode holdnode = node as HoldNode;
            List<int> holdlist = node.Graph.Blackboard.HoldNodes;
            if (holdlist == null || holdlist.Contains(holdnode.Id) == false)
            {
                Log.Warning($"触发{node.Graph.Id}流程判断{holdnode.Id}时, 没有找到存档数据");
                return null;
            }

            // 标记所有同源的hold节点都失效
            return (List<object> objs) =>
            {
                //FindTalkingNpc(holdnode.GetOutputPort("conditionPort"), (npcNode) =>
                //{
                //    RemoveTalkingNpc(npcNode);
                //});
                holdnode.Graph.AddHoldNodeTimesIfSameSource(holdnode);
                holdnode.Continue();
            };
        }

        public static Action<List<object>> OnCheckExitConditionNode(this StoryComponent self, SerialNode node, List<ConditionNode> successList)
        {
            return (List<object> objs) =>
            {
                //if ((node.Graph as StoryGraph).taskLineType == StoryHeadInfoNode.TaskLineType.Hunter)
                //{
                //    BountyHunterManager.Instance.CloseByStoryHunter(node.graph.name);
                //}
                self.StoryClose(node.Graph.Id);
            };
        }

        public static void StoryClose(this StoryComponent self, int graphId)
        {
            Log.Debug($"剧情事件{graphId}关闭");
            StoryEntity entity = self.StoryDict.GetValueOrDefault(graphId);
            if (entity == null || entity.State == StoryState.Close || entity.State == StoryState.CloseAfterOpen)
            {
                return;
            }
            //CloseTimeLimit(graphName);
            if (entity.State == StoryState.NotOpen)
            {
                entity.State = StoryState.Close;
            }
            else
            {
                entity.State = StoryState.CloseAfterOpen;
            }
            SerialGraph graph = entity.Graph;
            //// 撤销追踪
            //RemoveCheckNote(graphName);
            //if (graph != null && NewNoteList.Contains(graph))
            //{
            //    NewNoteList.Remove(graph);
            //}
            //UpdateRomingMainStoryText();
            // 不发奖励
            entity.Blackboard.Results.Clear();
            //// 悬赏
            //if (graph != null && graph.headnode.taskLineType == StoryHeadInfoNode.TaskLineType.Hunter)
            //{
            //    graph.startnode.ClearHunter();
            //}

            //if (entity.State == StoryState.CloseAfterOpen)
            //{
            //    self.CheckCondition(typeof(HaveStoryCloseAfterOpenCondition));
            //}

            if (!self.IsProcessingStory)
            {
                self.ExitStory();    // 走一次清理
            }
        }

        public static Action<List<object>> OnCheckOpenConditionNode(this StoryComponent self, SerialNode node, List<ConditionNode> successList)
        {
            self.StoryOpen(node as StoryOpenNode);
            return null;
        }

        public static void StoryOpen(this StoryComponent self, StoryOpenNode openNode)
        {
            if (openNode == null)
            {
                return;
            }
            StoryEntity entity = openNode.Graph.GetEntity<StoryEntity>();
            openNode.AddTime();   // 标记已触发
            SerialGraph graph = openNode.Graph;
            entity.State = StoryState.Opened;

            //// 记录获知时间
            //data.OpenedTime = dataContainer.initedObject.TimeSort++;
            //data.LimitQuaterFrom = InGameTimeManager.Instance.TotalQuaterPassedAsInt;

            //if (graph.headnode.taskLineType != StoryHeadInfoNode.TaskLineType.Cab)
            //{
            if (entity.StartNode.CheckCondition("ExitConditions") == false)
            {
                // 事件按播放条件归类至指定列表
                SerialPort startConditionRootPort = entity.StartNode.GetPort("conditionPort");
                graph.RecordConditionCheck(self.StartConditionNodes, startConditionRootPort);

                // 事件按关闭条件归类至指定列表
                graph.RecordConditionCheck(self.CloseStartedConditionNodes, /*graph.headnode.taskLineType == StoryHeadInfoNode.TaskLineType.Cab ? null : */entity.StartNode.GetPort("ExitConditions"));

                //// 事件按跳出条件归类至指定列表
                //if (graph.startnode.repeatTask)
                //{
                //    NodePort repeatFinishConditionRootPort = graph.startnode.GetPort("forceFinish");
                //    RecordConditionCheck(RepeatStoryForceFinishNodes, repeatFinishConditionRootPort);
                //}

                //// 记录NPC交互
                //FindTalkingNpc(graph.startnode.GetInputPort("conditionPort"), (node) =>
                //{
                //    AddTalkingNpc(node);
                //});

                //// 写到记事本界面和追踪
                //if ((graph.headnode.taskLineType == StoryHeadInfoNode.TaskLineType.Hunter || (graph.headnode.isToNode && graph.headnode.isAutoAddNote)))
                //{
                //    NoteActive(graph);
                //}
            }
            else
            {
                // 获知后满足关闭条件, 直接关闭
                self.StoryClose(graph.Id);
            }
            //}
        }


    }
}

/*
触发事件 Role1-1 
UnityEngine.Debug:LogFormat (string,object[])
WuLin.StoryManager:StoryStart (WuLin.StateMachine.StoryStartNode) (at Assets/__WuLin/Scripts/Global/StoryManager.cs:893)
WuLin.StoryManager/<>c__DisplayClass74_0:<OnCheckStartAtStoryOut>b__1 (object[]) (at Assets/__WuLin/Scripts/Global/StoryManager.cs:1241)
WuLin.StoryManager:CheckConditionNode (System.Collections.Generic.Dictionary`2<System.Type, System.Collections.Generic.List`1<XNode.NodePort>>,System.Type,object[],WuLin.StoryManager/ActionCheckSuccess,bool,bool,bool) (at Assets/__WuLin/Scripts/Global/StoryManager.cs:1650)
WuLin.StoryManager:OnCheckStartAtStoryOut (System.Type,object[]) (at Assets/__WuLin/Scripts/Global/StoryManager.cs:1227)
WuLin.StoryManager:CheckTypeAtStoryOut (bool) (at Assets/__WuLin/Scripts/Global/StoryManager.cs:854)
WuLin.StoryManager:CheckAfterLoading () (at Assets/__WuLin/Scripts/Global/StoryManager.cs:822)
WuLin.GameLoadingProgress:OnCheckLoading () (at Assets/__WuLin/Scripts/Global/GameLoadingProgress.cs:234)
WuLin.GameLoadingProgress:Update () (at Assets/__WuLin/Scripts/Global/GameLoadingProgress.cs:198)
WuLin.GameMain:Update () (at Assets/__WuLin/Scripts/GameMain.cs:36)

 */