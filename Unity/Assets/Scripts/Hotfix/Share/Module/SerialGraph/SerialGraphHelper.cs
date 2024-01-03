using DotRecast.Detour;
using ET.Common;
using ET.NodeDefine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    public static class SerialGraphHelper
    {
        #region SerialGraph
        public static void AfterDeserialize(this SerialGraph self)
        {
            self.PortDict.Clear();
            self.NodeDict.Clear();

            foreach (SerialNode node in self.Nodes)
            {
                self.NodeDict[node.Id] = node;
                node.PortDict.Clear();
                node.Graph = self;
                if (node is INodeActiveTimes activeTimes)
                {
                    activeTimes.ActiveTimeKey = $"{node.Id}_ActiveTime";
                }
            }

            foreach (SerialPort port in self.Ports)
            {
                port.Connections?.Clear();
                port.TargetNodes?.Clear();
                port.Node = self.NodeDict[port.NodeId];
                self.PortDict[port.Id] = port;
                self.NodeDict[port.NodeId].PortDict[port.Name] = port;
            }

#if UNITY_EDITOR
            //Log.Debug($"测试存档点数据：GraphId为{self.Id}的Graph中SaveID大于0的节点有{self.Nodes.Where(a => a is ContinueNode { Id:> 0 }).Count()}个");
#endif
        }

        public static SerialPort GetPort(this SerialGraph self, int portId)
        {
            if (!self.PortDict.TryGetValue(portId, out SerialPort port))
            {
                Log.Error($"Id为{self.Id}的{self.Type}图不存在Id为{portId}的端口");
                return null;
            }
            return port;
        }

        public static SerialNode GetNode(this SerialGraph self, int nodeId)
        {
            if (!self.NodeDict.TryGetValue(nodeId, out SerialNode node))
            {
                Log.Error($"Id为{self.Id}的{self.Type}图不存在Id为{nodeId}的节点");
                return null;
            }
            return node;
        }

        public static SerialNode GetNodeByPortId(this SerialGraph self, int portId)
        {
            if (!self.PortDict.TryGetValue(portId, out SerialPort port))
            {
                Log.Error($"Id为{self.Id}的{self.Type}图不存在Id为{portId}的端口");
                return null;
            }
            return self.GetNode(port.NodeId);
        }

        public static void ContinueArrange(this IGraphEntity graphEntity, SerialNode node, string continuePort)
        {
            if (node == null || node.GetPort(continuePort) == null)
            {
                Log.Error("后续节点null");
                return;
            }
            Entity entity = graphEntity as Entity;
            // 取后续所有节点(已排序)
            List<SerialPort> nextList = node.GetPort(continuePort).GetConnections();
            Dictionary<Type, List<SerialNode>> specialParallelDict = new Dictionary<Type, List<SerialNode>>();
            bool hasMutex = false;
            HoldNode findOneHoldNodePass = null;
            using ListComponent<HoldNode> waitingHoldList = ListComponent<HoldNode>.Create();
            foreach (SerialPort port in nextList)
            {
                SerialNode nextNode = port.Node;
                Type nextType = nextNode.GetType();
                if (SerialGraphEventSystem.Instance.HasParallelHandler(nextType))
                {
                    if (!specialParallelDict.TryGetValue(nextType, out List<SerialNode> list))
                    {
                        list = ListComponent<SerialNode>.Create();
                        specialParallelDict[nextType] = list;
                    }
                    list.Add(nextNode);
                }
                else if (nextNode is HoldNode holdNode)
                {
                    // 流程判断
                    HoldNode holdnode = port.Node as HoldNode;
                    holdnode.TrySendResult(graphEntity);
                    if (findOneHoldNodePass == null && holdnode.CheckCondition(entity))
                    {
                        findOneHoldNodePass = holdnode;
                    }
                    else
                    {
                        waitingHoldList.Add(holdnode);
                    }
                }
                else if (nextNode is ContinueNode continueNode)
                {
                    if (nextNode.CanParallel)
                    {
                        SerialGraphEventSystem.Instance.Active(entity as Entity, continueNode);
                    }
                    else
                    {
                        if (!hasMutex)
                        {
                            hasMutex = true;
                            SerialGraphEventSystem.Instance.Active(entity as Entity, continueNode);
                        }
                        else
                        {
                            Log.Error("卧槽！！！");
                        }
                    }
                }
                else
                {
                    Log.Error("卧槽！！！！");
                }
            }

            // 处理流程判断
            if (findOneHoldNodePass != null)
            {
                SerialGraphEventSystem.Instance.ExitHold(entity, findOneHoldNodePass);

                // 有满足条件的hold, 则其他可以略过
                foreach (HoldNode holdnode in waitingHoldList)
                {
                    graphEntity.Blackboard.AddActiveTime(holdnode);
                    SerialGraphEventSystem.Instance.ExitHold(entity, holdnode);
                }
                graphEntity.Blackboard.AddActiveTime(findOneHoldNodePass);
                SerialGraphEventSystem.Instance.Active(entity, findOneHoldNodePass);
            }
            else
            {
                // 没有满足条件的hold, 全部进入等待状态
                foreach (HoldNode holdnode in waitingHoldList)
                {
                    holdnode.Hold(graphEntity);
                }
            }

            foreach (KeyValuePair<Type, List<SerialNode>> item in specialParallelDict)
            {
                SerialGraphEventSystem.Instance.ContinueParallel(entity, item.Key, item.Value);
            }

            // 没有后续节点, 发奖结束事件
            if (nextList.Count == 0)
            {
                graphEntity.SendResult();
                SerialGraphEventSystem.Instance.CheckComplete(entity);
            }
            else
            {
                // 后续全是流程判断且都未满足, 暂时退出事件
                if (nextList.Count == waitingHoldList.Count)
                {
                    SerialGraphEventSystem.Instance.Exit(entity);
                }
            }
        }

        public static void SetCurrentNode(this IGraphEntity entity, HappenNode node)
        {
            entity.Blackboard.SetCurrentNode(node);
        }

        public static HappenNode GetCurrentNode(this IGraphEntity entity)
        {
            return entity.Blackboard.GetCurrentNode();
        }

        /// <summary>
        /// 一个事件申请进入等待阶段
        /// </summary>
        public static void RecordHold(this IGraphEntity graphEntity, SerialPort conditionsPort)
        {
            Entity entity = graphEntity as Entity;
            if (!graphEntity.AddHoldSaveData(conditionsPort))
            {
                return;
            }
            graphEntity.RecordConditionCheck((entity.Parent as IGraphsComponent).HoldNodes, conditionsPort); ;
            SerialGraphEventSystem.Instance.EnterHold(entity, conditionsPort.Node as HoldNode);
        }

        // 记录hold节点
        private static bool AddHoldSaveData(this IGraphEntity entity, SerialPort conditionsPort)
        {
            if (conditionsPort.GetConnections().Count <= 0)
            {
                return false;
            }
            //SerialGraphsComponent cacheData = self.GetCacheData();
            HoldNode holdnode = conditionsPort.Node as HoldNode;
            List<int> ports = entity.Blackboard.HoldNodes;
            if (ports == null)
            {
                ports = new List<int>();
                entity.Blackboard.HoldNodes = ports;
            }
            if (ports.Contains(holdnode.Id))
            {
                return false;
            }
            ports.Add(holdnode.Id);
            return true;
        }

        public static void RecordConditionCheck(this IGraphEntity graphEntity, UnOrderMultiMap<Type, long> conditionPorts, SerialPort rootPort)
        {
            graphEntity.RecordConditionCheck(conditionPorts, rootPort, rootPort);
        }

        public static void RecordConditionCheck(this IGraphEntity graphEntity, UnOrderMultiMap<Type, long> conditionPorts, SerialPort conditionsPort, SerialPort rootPort)
        {
            if (conditionPorts == null || conditionsPort == null || rootPort == null)
            {
                return;
            }
            long rootPortInstanceId = rootPort.GetInstanceId();
            foreach (SerialPort port in conditionsPort.GetConnections())
            {
                Type type = port.Node.GetType();
                if (port.Node is not ConditionNode)
                {
                    // 不是条件
                    continue;
                }
                if (!conditionPorts.Contains(type, rootPortInstanceId))
                {
                    conditionPorts.Add(type, rootPortInstanceId);
                }
                SerialPort nextPort;
                if (port.Name == "State")
                {
                    nextPort = port.Node.GetPort("StateIn");
                }
                else
                {
                    nextPort = port.Node.GetPort("State");
                }
                if (nextPort != null)
                {
                    graphEntity.RecordConditionCheck(conditionPorts, nextPort, rootPort);
                }
            }
        }

        public static void RecordPrize(this IGraphEntity graphEntity, ResultNode node)
        {
            List<int> nodelist = graphEntity.Blackboard.Results;
            if (nodelist == null)
            {
                nodelist = new List<int>();
                graphEntity.Blackboard.Results = nodelist;
            }
            nodelist.Add(node.Id);
            node.Continue(graphEntity as Entity);
        }

        public static void SendResult(this IGraphEntity graphEntity)
        {
            List<int> idList = graphEntity.Blackboard.Results;
            bool success = graphEntity.Graph.ForeachNodeBySaveIds<ResultNode>(idList, node =>
            {
                SerialGraphEventSystem.Instance.OnResult(graphEntity as Entity, node);
                //if (node is StoryFailNode)
                //{
                //    StoryFailed(graph.name);
                //}
                //else if (node is GameEndingNode)
                //{
                //    IsGameEnding = true;
                //}
                //else if (node is StoryTimeOutNode)
                //{
                //    StoryTimeOut(graph.name);
                //}
            });
            if (success)
            {
                graphEntity.Blackboard.Results.Clear();
            }
        }

        public static bool ForeachNodeBySaveIds<TNode>(this SerialGraph self, List<int> saveIds, Action<TNode> action) where TNode : SerialNode
        {
            if (saveIds == null)
            {
                return false;
            }

            bool success = false;
            foreach (SerialNode node in self.Nodes)
            {
                if (node is not TNode tNode)
                {
                    continue;
                }

                if (saveIds.Contains(node.Id))
                {
                    action(tNode);
                    success = true;
                }
            }
            return success;
        }

        public static bool CheckConditionFromRoot(this IGraphEntity graphEntity, SerialPort port, Type conditionType, IConditionNodeParam param, List<ConditionNode> successList, bool checkNodeTimes = true)
        {
            INodeActiveTimes node = port.Node as INodeActiveTimes;
            if (checkNodeTimes && graphEntity.Blackboard.Get<int>(node.ActiveTimeKey) > 0)
            {
                return false;
            }
            List<SerialPort> connectPorts = port.GetConnections();    // 获取一个事件的条件端口下挂接的所有第一级条件                        
            // 第一层条件可能会有多个
            foreach (SerialPort conditionNodePort in connectPorts)
            {
                // 取出一个第一层条件
                ConditionNode rootCondition = conditionNodePort.Node as ConditionNode;
                Direction io = conditionNodePort.Name == "State" ? Direction.Input : Direction.Output;
                //Direction io = port.IsInput ? NodePort.IO.Input : NodePort.IO.Output;

                // 检测本层并向下一层继续检测是否有指定类型的条件, 并检测条件链是否满足
                if (rootCondition.CheckConditionLineWithType(graphEntity as Entity, conditionType, param, io, successList))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 标记所有同源的hold节点都已触发
        /// </summary>
        public static void AddHoldNodeTimesIfSameSource(this IGraphEntity graphEntity, HoldNode holdBode)
        {
            List<SerialPort> fromList = holdBode.GetPort("InPort").GetConnections();
            if (fromList.Count == 0)
            {
                graphEntity.HoldNodeUsed(holdBode);
                return;
            }
            foreach (SerialPort from in fromList)
            {
                foreach (SerialPort port in from.GetConnections())
                {
                    if (port.Node is HoldNode)
                    {
                        HoldNode childnode = port.Node as HoldNode;
                        graphEntity.HoldNodeUsed(childnode);
                    }
                }
            }
        }

        public static void HoldNodeUsed(this IGraphEntity graphEntity, HoldNode holdNode)
        {
            if (graphEntity.Blackboard.GetActiveTime(holdNode) <= 0)
            {
                graphEntity.Blackboard.AddActiveTime(holdNode);
                graphEntity.Blackboard.HoldNodes.Remove(holdNode.Id);

                SerialGraphEventSystem.Instance.ExitHold(graphEntity as Entity, holdNode);
            }
        }
        #endregion

        #region SerialPort
        public static List<SerialPort> GetConnections(this SerialPort self)
        {
            if (self.Connections == null/* && self.TargetIds.Count > 0*/)
            {
                self.Connections = new(self.TargetIds.Count);
                foreach (int targetId in self.TargetIds)
                {
                    self.Connections.Add(self.Node.Graph.GetPort(targetId));
                }
            }
            return self.Connections;
        }

        public static List<SerialNode> GetTargetNodes(this SerialPort self)
        {
            if (self.TargetNodes == null/* && self.TargetIds.Count > 0*/)
            {
                self.TargetNodes = new(self.TargetIds.Count);
                foreach (int targetId in self.TargetIds)
                {
                    self.TargetNodes.Add(self.Node.Graph.GetPort(targetId).Node);
                }
            }
            return self.TargetNodes;
        }

        /// <summary>
        /// 一个功能组件内的唯一ID
        /// </summary>
        public static long GetInstanceId(this SerialPort self)
        {
            return ((long)self.Node.Graph.Id << 32) | (long)self.Id;
        }

        public static SerialPort GetPortByInstanceId(IGraphsComponent graphsComponent, long instanceId)
        {
            int graphId = (int)(instanceId >> 32);
            int portId = (int)(instanceId - graphId);
            return GetGraph(graphsComponent.GraphType, graphId).GetPort(portId);
        }

        public static SerialGraph GetGraph(SerialGraphType type, int id)
        {
            return IGraphsComponent.GraphConfigDict[type][id];
        }
        #endregion

        #region SerialNode
        public static SerialPort GetPort(this SerialNode node, string name)
        {
            if (!node.PortDict.TryGetValue(name, out SerialPort port))
            {
                Log.Error($"Id为{node.Graph.Id}的Graph中Id为{node.Id}的节点的{name}没有Port信息");
                return null;
            }

            return port;
        }

        public static List<SerialNode> GetTargetNodes(this SerialNode node, string name)
        {
            #region 旧代码
            //if (node.CacheTargetNodeDict.TryGetValue(name, out List<SerialNode> nodes))
            //{
            //    return nodes;
            //}

            //SerialPort port = GetPort(node, name);
            //if (port == null)
            //{
            //    return null;
            //}

            //node.CacheTargetNodeDict[name] = nodes = new List<SerialNode>();
            //foreach (int targetPortId in port.TargetIds)
            //{
            //    SerialPort targetPort = node.Graph.PortDict[targetPortId];
            //    nodes.Add(node.Graph.NodeDict[targetPort.NodeId]);
            //}

            //return nodes; 
            #endregion

            SerialPort port = GetPort(node, name);
            if (port == null)
            {
                return null;
            }
            return port.GetTargetNodes();
        }

        /// <summary>
        /// 只会返回第一个目标节点，如果目标节点不是T就返回空
        /// </summary>
        public static T GetTargetNode<T>(this SerialNode node, string name) where T : SerialNode
        {
            SerialPort port = GetPort(node, name);
            if (port == null || port.TargetIds.Count == 0)
            {
                return null;
            }

            SerialPort targetPort = node.Graph.PortDict[port.TargetIds[0]];
            return node.Graph.NodeDict[targetPort.NodeId] as T;
        }

        public static bool CheckCondition(this SerialNode node, Entity entity, string portName = "ConditionPort")
        {
            List<SerialPort> conditions = node.GetPort(portName).GetConnections();
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }
            foreach (SerialPort port in conditions)
            {
                if (port.Node is not ConditionNode conditionNode)
                {
                    continue;
                }
                if (SerialGraphEventSystem.Instance.CheckAllConnectNode(entity, conditionNode, Direction.Input))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ConditionNode
        public static bool CheckAllExceptSelf(this ConditionNode self, Entity entity, Direction io, List<ConditionNode> line = null)
        {
            List<int> connections = null;
            if (io == Direction.Output)
            {
                connections = self.GetPort("State").TargetIds;
            }
            else
            {
                connections = self.GetPort("StateIn").TargetIds;
            }
            if (connections.Count > 0)
            {
                return connections.Exists(n =>
                {
                    SerialNode node = self.Graph.GetNodeByPortId(n);
                    if (node is ConditionNode == false)
                    {
                        return true;
                    }
                    return SerialGraphEventSystem.Instance.CheckAllConnectNode(entity, self, io, line);
                });
            }
            return true;
        }

        // 找到第一个发现的conditionType类型条件节点
        public static SerialNode FindConditionType(this ConditionNode self, Type conditionType, Direction io)
        {
            if (self.GetType() == conditionType)
            {
                return self;
            }
            // 找出指定类型的条件
            List<SerialPort> connections = null;
            if (io == Direction.Output)
            {
                connections = self.GetPort("State").GetConnections();
            }
            else
            {
                connections = self.GetPort("StateIn").GetConnections();
            }
            if (connections == null)
            {
                return null;
            }
            SerialNode node = null;
            foreach (SerialPort port in connections)
            {
                node = (port.Node as ConditionNode).FindConditionType(conditionType, io);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// 按io的方向查找是否有conditionType类型的条件, 找到后以此条件开始向两个方向判断所有条件是否满足, 返回结果
        /// </summary>
        public static bool CheckConditionLineWithType(this ConditionNode self, Entity entity, Type conditionType, IConditionNodeParam param, Direction io, List<ConditionNode> line)
        {
            if (self.GetType() == conditionType && SerialGraphEventSystem.Instance.CheckCondition(entity, self, param))
            {
                return SerialGraphEventSystem.Instance.CheckAllConnectNode(entity, self, io, line) && self.CheckAllExceptSelf(entity, io == Direction.Input ? Direction.Output : Direction.Input, line);
            }
            // 找出指定类型的条件
            if (io == Direction.Output)
            {
                return self.GetPort("State").GetConnections().Exists(n => (n.Node as ConditionNode).CheckConditionLineWithType(entity, conditionType, param, io, line));
            }
            return self.GetPort("StateIn").GetConnections().Exists(n => (n.Node as ConditionNode).CheckConditionLineWithType(entity, conditionType, param, io, line));
        }

        public static bool BaseCheckAllConnectNode(this ConditionNode self, Entity entity, Direction io, List<ConditionNode> line = null)
        {
            // 这里是基类的处理, 直接判断后续节点, this的条件是否能通过应当子类override这个函数去处理, 成功后base到这里
            line?.Add(self); // 走到基类这里的肯定是已经成功的
            bool result = self.CheckAllExceptSelf(entity, io, line);
            if (result == false)
            {
                line?.Remove(self);
            }
            return result;
        }
        #endregion

        #region ContinueNode
        public static void Continue(this ContinueNode self, Entity entity, string outPort = null)
        {
            if (self is HoldNode)
            {
                Log.Debug($"触发{self.Graph.Id}流程判断{self.Id} UTC时间{TimeInfo.Instance.ToDateTime(TimeInfo.Instance.ServerNow())}");
                if (entity is StoryEntity story)
                {
                    story.GetParent<StoryComponent>().IsProcessingStory = true;
                }
            }
            (entity as IGraphEntity).ContinueArrange(self, outPort ?? self.DefaultOutPort);
        }
        #endregion

        #region HoldNode
        public static void Hold(this HoldNode self, IGraphEntity graphEntity)
        {
            // 重新等待检测
            graphEntity.Blackboard.ClearActiveTime(self);
            // 等待条件满足再继续
            SerialPort conditionPort = self.GetPort("ConditionPort");
            graphEntity.RecordHold(conditionPort);
        }

        public static void TrySendResult(this HoldNode self, IGraphEntity graphEntity)
        {
            // 发放奖励
            if (self.DoResult)
            {
                graphEntity.SendResult();
            }
        }
        #endregion
    }
}
