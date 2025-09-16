using ET.Client;
using ET.Common;
using ET.Story;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [EntitySystemOf(typeof(StoryComponent))]
    [FriendOf(typeof(StoryComponent))]
    [FriendOfAttribute(typeof(ET.StoryEntity))]
    public static partial class StoryComponentSystem
    {
        //[Invoke(TimerInvokeType.TestTick)]
        //public class StoryTestTimer : ATimer<StoryComponent>
        //{
        //    protected override void Run(StoryComponent self)
        //    {
        //        self.CheckCondition<TrueCondNode>();
        //    }
        //}

        [EntitySystem]
        public static void Awake(this StoryComponent self)
        {
            foreach (int id in IGraphsComponent.GraphConfigDict[SerialGraphType.Story].Keys)
            {
                self.AddStory(id);
            }
            self.CheckAfterLoading();
            // 临时强行触发，因为每个剧情事件都有条件才能触发
            self.StoryStart(self.StoryDict.Values.FirstOrDefault().StartNode);
        }

        //public static async ETTask AAA(this StoryComponent self)
        //{
        //    await self.Fiber().ScaledTimerComponent.WaitAsync(2000);

        //}

        [EntitySystem]
        public static void Deserialize(this StoryComponent self)
        {
            self.SetMiscValue<string>("CurrentPlace", null);
            foreach (StoryEntity entity in self.Children.Values)
            {
                self.StoryDict.Add(entity.GraphId, entity);
                entity.AnalysisGraphConfig();
            }
            self.CheckAfterLoading();
        }

        public static void AddStory(this StoryComponent self, int id)
        {
            StoryEntity entity = self.AddChild<StoryEntity, int>(id);
            entity.AnalysisGraphConfig();
            self.StoryDict.Add(id, entity);
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

            if (!self.IsProcessingStory)
            {
                self.ExitStory();    // 走一次清理
            }
        }

        public static void ExitStory(this StoryComponent self)
        {
            // 原项目有一堆对其它系统的处理
            self.IsProcessingStory = false;

            self.TriggerLater();
        }

        public static void CheckAfterLoading(this StoryComponent self)
        {
            if (!self.IsProcessingStory)
            {
                self.TriggerLater();
                //if (!self.IsProcessingStory)
                //{
                //    InvokeQuarterAtStoryOut();
                //}
            }

            self.CheckCondition<TrueCondNode>();
            //CheckCondition(typeof(LunarCalendarCondition));
            //CheckCondition(typeof(HaveTraitCondition));
            //CheckCondition(typeof(CheckBuildingBrokenCondition));

            self.CheckTypeAtStoryOut();
        }

        /// <summary>
        /// 检测是否满足了带有指定类型的条件链
        /// 【注意】(先不管这条注意事项)通用节点一定要把泛型TEntity声明成Entity(基类)
        /// </summary>
        public static void CheckCondition<TNode, TParam>(this StoryComponent self, TParam param) where TNode : ConditionNode where TParam : struct
        {
            // TODO 在触发事件的逻辑里应用IConditionNodeParam对象池

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
            self.CheckConditionNode<TNode, TParam>(self.CloseConditionPorts, param, StoryCondSuccessInvokeType.CheckExitConditionNode);
            // 检测事件是否被玩家获知
            self.CheckConditionNode<TNode, TParam>(self.OpenConditionPorts, param, StoryCondSuccessInvokeType.CheckOpenConditionNode);
            // 检测事件是否获知后关闭
            self.CheckConditionNode<TNode, TParam>(self.CloseStartedConditionPorts, param, StoryCondSuccessInvokeType.CheckExitConditionNode, false, false, false);
            //// 检测事件是否结束循环
            //self.CheckConditionNode(self.RepeatStoryForceFinishNodes, conditionType, extra, OnCheckRepeatStoryForceFinishNode, false, false, false);

            // 玩家一些情况下不触发播放
            if (!self.CheckGameStateCanActiveStory())
            {
                self.AddCheckTypeToWait<TNode, TParam>(param, StoryCheckDicType.Start);
                self.AddCheckTypeToWait<TNode, TParam>(param, StoryCheckDicType.Hold);
                return;
            }

            // 可以进行接下来的检测
            self.OnCheckStartAtStoryOut<TNode, TParam>(param);
            if (self.IsProcessingStory)
            {
                self.AddCheckTypeToWait<TNode, TParam>(param, StoryCheckDicType.Hold);
            }
            else
            {
                self.OnCheckHoldAtStoryOut<TNode, TParam>(param);
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

        public static void CheckCondition<TNode>(this StoryComponent self) where TNode : ConditionNode
        {
            self.CheckCondition<TNode, CondNullParam>(CondNullParam.Instance);
        }

        // 判断当前是否可以激活事件
        public static bool CheckGameStateCanActiveStory(this StoryComponent self)
        {
            return true;
        }

        /// <summary>
        /// 【注意】(先不管这条注意事项)通用节点一定要把泛型TEntity声明成Entity(基类)
        /// </summary>
        private static void CheckConditionNode<TNode, TParam>(this StoryComponent self, UnOrderMultiMap<Type, SerialPort> graphConditionNodes,
            TParam param, long invokeType,
            bool onlyOneTrigger = false, bool tryTriggerAtStoryOut = false, bool checkNodeTimes = true) where TNode : ConditionNode where TParam : struct/* where TInvokeParam : struct*/
        {
            Type conditionType = typeof(TNode);
            if (!graphConditionNodes.TryGetValue(conditionType, out List<SerialPort> ports))
            {
                return;
            }
            List<CheckAtStoryOut<TNode, TParam>> triggerList = new List<CheckAtStoryOut<TNode, TParam>>();
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
                bool trigger = story.CheckConditionFromRoot<StoryEntity, TNode, TParam>(port, param, successList, checkNodeTimes);
                if (trigger)
                {
                    CheckAtStoryOut<TNode, TParam> check = ObjectPool.Instance.Fetch<CheckAtStoryOut<TNode, TParam>>();
                    check.Port = port;
                    check.ConditionType = conditionType;
                    check.InvokeType = invokeType;
                    check.SuccessList = successList;
                    check.Param = param;
                    //check.CheckConditionFromRootFunc = story.CheckConditionFromRoot<StoryEntity, TNode, TParam>;
                    triggerList.Add(check);
                }
            }

            using ListComponent<long> resultList = ListComponent<long>.Create();
            using ListComponent<CheckStorySuccessResult> resultObjsList = ListComponent<CheckStorySuccessResult>.Create();
            int logName = 0;
            for (int i = 0; i < triggerList.Count; i++)
            {
                CheckAtStoryOut<TNode, TParam> check = triggerList[i];
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
                StoryCondSuccessInvokeParam invokeParam = new(self, check.Port.Node, check.SuccessList);
                long result = EventSystem.Instance.Invoke<StoryCondSuccessInvokeParam, long>(invokeType, invokeParam);
                if (result > 0)
                {
                    resultList.Add(result);
                    CheckStorySuccessResult resultObjs = new(invokeParam);
                    //resultObjs.Add(check.Port.Node);
                    //resultObjs.Add(check.SuccessList);
                    resultObjsList.Add(resultObjs);
                    logName = check.Port.Node.Graph.Id;
                }
            }
            for (int i = 0; i < resultList.Count && i < resultObjsList.Count; i++)
            {
                long result = resultList[i];
                CheckStorySuccessResult resultObjs = resultObjsList[i];
                Log.Debug($"触发剧情事件判断成功回调{result}");
                EventSystem.Instance.Invoke(result, resultObjs);
            }
        }

        public static void AddCheckTypeToWait<TNode, TParam>(this StoryComponent self, TParam param, StoryCheckDicType dicType) where TNode : ConditionNode where TParam : struct
        {
            Type conditionType = typeof(TParam);
            foreach (StoryWaitCheckBase waitCheckBase in self.CheckTypeToWait)
            {
                if (waitCheckBase is StoryWaitCheck<TNode, TParam> waitCheck
                    //&& waitCheck.ConditionType == conditionType
                    && waitCheck.DicType == dicType
                    && waitCheck.Param.Equals(param))
                {
                    return;
                }
            }
            StoryWaitCheck<TNode, TParam> w = new();
            //w.ConditionType = conditionType;
            w.Param = param;
            w.DicType = dicType;
            switch (dicType)
            {
                case StoryCheckDicType.Start:
                    w.CheckDelegate = OnCheckStartAtStoryOut<TNode, TParam>;
                    break;
                case StoryCheckDicType.Hold:
                    w.CheckDelegate = OnCheckHoldAtStoryOut<TNode, TParam>; ;
                    break;
                case StoryCheckDicType.All:
                    w.CheckDelegate = CheckCondition<TNode, TParam>; ;
                    break;
                default:
                    Log.Error("???");
                    break;
            }
            self.CheckTypeToWait.Add(w);
        }

        /// <summary>
        /// 事件播放时不能进行的检测(Start节点)
        /// 【注意】(先不管这条注意事项)通用节点一定要把泛型TEntity声明成Entity(基类)
        /// </summary>
        public static void OnCheckStartAtStoryOut<TNode, TParam>(this StoryComponent self, TParam param) where TNode : ConditionNode where TParam : struct
        {
            // 检测事件是否播放
            self.CheckConditionNode<TNode, TParam>(self.StartConditionPorts, param, StoryCondSuccessInvokeType.CheckStartAtStoryOut, true, true);
        }

        public static bool StoryStart(this StoryComponent self, StoryStartNode startNode)
        {
            //if (RoamingManager.Instance.player == null)
            //{
            //    Debug.LogWarningFormat("事件触发了但是主角没有创建{0}", startNode.graph.name);
            //    return false;
            //}
            StoryEntity entity = self.StoryDict[startNode.Graph.Id];
            
            self.IsProcessingStory = true;

            //FindTalkingNpc(startNode.GetInputPort("conditionPort"), (npcID) =>
            //{
            //    RemoveTalkingNpc(npcID);
            //});

            entity.Blackboard.AddActiveTime(startNode);   // 标记已触发
            entity.State = StoryState.Started;
            Log.Warning($"触发事件 {startNode.Graph.Id}");
            //entity.RemoveRedDot();

            //// 判断有生效中的时效标签
            //TimeLimitNode limitNode = (startNode.graph as StoryGraph).GetActiveTimeLimitNode(data.TimeLimitSaveID);
            //if (limitNode != null)
            //{
            //    limitNode.Continue();
            //    Debug.LogErrorFormat("会有这种情况?事件{0}进入时, 有时效标签{1}生效", startNode.graph.name, data.TimeLimitSaveID);
            //}
            //else
            {
                entity.ContinueArrange(startNode, "PlayPort");
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
            using ListComponent<CheckAtStoryOutBase> triggerList = ListComponent<CheckAtStoryOutBase>.Create();
            foreach (CheckAtStoryOutBase check in self.ListTriggerLaterAtStoryOut)
            {
                SerialPort port = check.Port;
                long callfunc = check.InvokeType;
                StoryEntity entity = self.StoryDict[port.Node.Graph.Id];
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
                bool trigger = check.CheckConditionFromRoot(entity);
                if (trigger)
                {
                    triggerList.Add(check); // 只保留条件通过的
                }
            }
            self.ListTriggerLaterAtStoryOut.Clear();

            int logName = 0;
            long result = 0;
            CheckStorySuccessResult resultobjs = default;
            for (int i = 0; i < triggerList.Count; i++)
            {
                CheckAtStoryOutBase check = triggerList[i];
                if (self.IsProcessingStory == false && result == 0)
                {
                    // 只触发第一个
                    StoryCondSuccessInvokeParam invokeParam = new(self, check.Port.Node, check.SuccessList);
                    result = EventSystem.Instance.Invoke<StoryCondSuccessInvokeParam, long>(check.InvokeType, invokeParam);
                    if (result > 0)
                    {
                        resultobjs.Param = invokeParam;
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
            if (result > 0)
            {
                Log.Debug($"触发剧情事件判断成功回调{result}");
                EventSystem.Instance.Invoke(result, resultobjs);
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

                StoryWaitCheckBase w = self.CheckTypeToWait[0];

                if (checkAll)
                {
                    if (w.DicType == StoryCheckDicType.All)
                    {
                        self.CheckTypeToWait.RemoveAt(0);
                        w.Check(self);
                    }
                }
                else
                {
                    self.CheckTypeToWait.RemoveAt(0);
                    w.Check(self);
                }
            }
        }

        /// <summary>
        /// 事件播放时不能进行的检测(Hold节点)
        /// </summary>
        public static void OnCheckHoldAtStoryOut<TNode, TParam>(this StoryComponent self, TParam param) where TNode : ConditionNode where TParam : struct
        {
            self.CheckConditionNode<TNode, TParam>(self.HoldPorts, param, StoryCondSuccessInvokeType.CheckHoldAtStoryOut, true, true);
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
            //entity.RemoveRedDot();
            SerialGraph graph = entity.Graph;
            //// 撤销追踪
            //RemoveCheckNote(graphName);
            //if (graph != null && NewNoteList.Contains(graph))
            //{
            //    NewNoteList.Remove(graph);
            //}
            //UpdateRomingMainStoryText();
            // 不发奖励
            entity.Blackboard.Results?.Clear();
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

        public static void StoryOpen(this StoryComponent self, StoryOpenNode openNode)
        {
            if (openNode == null)
            {
                return;
            }
            StoryEntity entity = self.StoryDict[openNode.Graph.Id];
            entity.Blackboard.AddActiveTime(openNode);   // 标记已触发
            SerialGraph graph = openNode.Graph;
            entity.State = StoryState.Opened;

            //// 记录获知时间
            //data.OpenedTime = dataContainer.initedObject.TimeSort++;
            //data.LimitQuaterFrom = InGameTimeManager.Instance.TotalQuaterPassedAsInt;

            //if (graph.headnode.taskLineType != StoryHeadInfoNode.TaskLineType.Cab)
            //{
            if (entity.StartNode.CheckCloseCondition(entity) == false)
            {
                // 事件按播放条件归类至指定列表
                SerialPort startConditionRootPort = entity.StartNode.GetPort("ConditionPort");
                entity.RecordConditionCheck(self.StartConditionPorts, startConditionRootPort);

                // 事件按关闭条件归类至指定列表
                entity.RecordConditionCheck(self.CloseStartedConditionPorts, /*graph.headnode.taskLineType == StoryHeadInfoNode.TaskLineType.Cab ? null : */entity.StartNode.GetPort("ExitConditions"));

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

                //entity.CheckOpenRedDot();
            }
            else
            {
                // 获知后满足关闭条件, 直接关闭
                self.StoryClose(graph.Id);
            }
            //}
        }

        public static void SetMiscValue<T>(this StoryComponent self, string key, T value)
        {
            // 防止装箱拆箱问题
            if (typeof(T).IsValueType)
            {
                //这里要接入一次池化
                if (!self.MiscValueDict.TryGetValue(key, out object obj))
                {
                    obj = new ValueObject<T>();
                    self.MiscValueDict.Add(key, obj);
                }
                (obj as ValueObject<T>).Value = value;
                return;
            }
            self.MiscValueDict[key] = value;
        }

        public static T GetMiscValue<T>(this StoryComponent self, string key)
        {
            if (!self.MiscValueDict.TryGetValue(key, out object obj))
            {
                // 抛出异常
                return default;
            }
            if (typeof(T).IsValueType)
            {
                ValueObject<T> value = obj as ValueObject<T>;
                return value.Value;
            }
            return (T)obj;
        }

        public static bool ContainsMiscValue(this StoryComponent self, string key)
        {
            return self.MiscValueDict.ContainsKey(key);
        }

        public static void RemoveMiscValue<T>(this StoryComponent self, string key)
        {
            if (self.MiscValueDict.TryGetValue(key, out object obj))
            {
                if (obj is IValueObject valueObject)
                {
                    valueObject.Dispose();
                }
                self.MiscValueDict.Remove(key);
            }
        }

        public static void ClearMiscValue<T>(this StoryComponent self)
        {
            foreach (object obj in self.MiscValueDict.Values)
            {
                if (obj is IValueObject valueObject)
                {
                    valueObject.Dispose();
                }
            }
            self.MiscValueDict.Clear();
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