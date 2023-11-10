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
        public static void AfterDeserialize(this SerialGraph self)
        {
            self.PortDict.Clear();
            self.NodeDict.Clear();

            foreach (SerialNode node in self.Nodes)
            {
                self.NodeDict[node.Id] = node;
                node.PortDict.Clear();
                node.Graph = self;
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

        public static void ContinueArrange(this SerialGraph self, SerialNode node, string continuePort)
        {
            if (node == null || node.GetPort(continuePort) == null)
            {
                Log.Error("后续节点null");
                return;
            }
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
                    holdnode.TrySendResult();
                    if (findOneHoldNodePass == null && holdnode.CheckCondition())
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
                        SerialGraphEventSystem.Instance.Active(continueNode);
                    }
                    else
                    {
                        if (!hasMutex)
                        {
                            hasMutex = true;
                            SerialGraphEventSystem.Instance.Active(continueNode);
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
                SerialGraphEventSystem.Instance.ExitHold(self, findOneHoldNodePass);

                // 有满足条件的hold, 则其他可以略过
                foreach (HoldNode holdnode in waitingHoldList)
                {
                    holdnode.AddTime();
                    SerialGraphEventSystem.Instance.ExitHold(self, holdnode);
                }
                findOneHoldNodePass.AddTime();
                SerialGraphEventSystem.Instance.Active(findOneHoldNodePass);
            }
            else
            {
                // 没有满足条件的hold, 全部进入等待状态
                foreach (HoldNode holdnode in waitingHoldList)
                {
                    holdnode.Hold();
                }
            }

            foreach (KeyValuePair<Type, List<SerialNode>> item in specialParallelDict)
            {
                SerialGraphEventSystem.Instance.ContinueParallel(item.Key, self, item.Value);
            }

            // 没有后续节点, 发奖结束事件
            if (nextList.Count == 0)
            {
                self.SendResult();
                SerialGraphEventSystem.Instance.CheckComplete(self);
            }
            else
            {
                // 后续全是流程判断且都未满足, 暂时退出事件
                if (nextList.Count == waitingHoldList.Count)
                {
                    SerialGraphEventSystem.Instance.Exit(self);
                }
            }
        }

        public static void SetCurrentNode(this SerialGraph self, HappenNode node)
        {
            self.Blackboard.SetCurrentNode(node);
        }

        public static HappenNode GetCurrentNode(this SerialGraph self)
        {
            return self.Blackboard.GetCurrentNode();
        }

        public static Entity GetEntity(this SerialGraph self)
        {
            return self.Blackboard.Entity;
        }

        public static T GetEntity<T>(this SerialGraph self) where T : Entity
        {
            return self.Blackboard.Entity as T;
        }

        /// <summary>
        /// 一个事件申请进入等待阶段
        /// </summary>
        public static void RecordHold(this SerialGraph self, SerialPort conditionsPort)
        {
            if (!self.AddHoldSaveData(conditionsPort))
            {
                return;
            }
            self.RecordConditionCheck((self.GetEntity().Parent as IGraphsComponent).HoldNodes, conditionsPort);;
            SerialGraphEventSystem.Instance.EnterHold(self, conditionsPort.Node as HoldNode);
        }

        // 记录hold节点
        private static bool AddHoldSaveData(this SerialGraph self, SerialPort conditionsPort)
        {
            if (conditionsPort.GetConnections().Count <= 0)
            {
                return false;
            }
            //SerialGraphsComponent cacheData = self.GetCacheData();
            HoldNode holdnode = conditionsPort.Node as HoldNode;
            List<int> ports = self.Blackboard.HoldNodes;
            if (ports == null)
            {
                ports = new List<int>();
                self.Blackboard.HoldNodes = ports;
            }
            if (ports.Contains(holdnode.Id))
            {
                return false;
            }
            ports.Add(holdnode.Id);
            return true;
        }

        public static void RecordConditionCheck(this SerialGraph self, UnOrderMultiMap<Type, SerialPort> conditionPorts, SerialPort rootPort)
        {
            self.RecordConditionCheck(conditionPorts, rootPort, rootPort);
        }

        public static void RecordConditionCheck(this SerialGraph self, UnOrderMultiMap<Type, SerialPort> conditionPorts, SerialPort conditionsPort, SerialPort rootPort)
        {
            if (conditionPorts == null || conditionsPort == null || rootPort == null)
            {
                return;
            }
            foreach (SerialPort port in conditionsPort.GetConnections())
            {
                Type type = port.Node.GetType();
                if (port.Node is not ConditionNode)
                {
                    // 不是条件
                    continue;
                }
                if (!conditionPorts.Contains(type, rootPort))
                {
                    conditionPorts.Add(type, rootPort);
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
                    self.RecordConditionCheck(conditionPorts, nextPort, rootPort);
                }
            }
        }

        public static void RecordPrize(this SerialGraph self, ResultNode node)
        {
            List<int> nodelist = self.Blackboard.Results;
            if (nodelist == null)
            {
                nodelist = new List<int>();
                self.Blackboard.Results = nodelist;
            }
            nodelist.Add(node.Id);
            node.Continue();
        }

        public static void SendResult(this SerialGraph self)
        {
            List<int> idList = self.Blackboard.Results;
            bool success = self.ForeachNodeBySaveIds<ResultNode>(idList, node =>
            {
                SerialGraphEventSystem.Instance.OnResult(node);
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
                self.Blackboard.Results.Clear();
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

        public static bool CheckConditionFromRoot(this SerialGraph graph, SerialPort port, Type conditionType, IConditionNodeParam param, List<ConditionNode> successList, bool checkNodeTimes = true)
        {
            INodeActiveTimes node = port.Node as INodeActiveTimes;
            if (checkNodeTimes && node.GetTimes() > 0)
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
                if (rootCondition.CheckConditionLineWithType(conditionType, param, io, successList))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 标记所有同源的hold节点都已触发
        /// </summary>
        public static void AddHoldNodeTimesIfSameSource(this SerialGraph self, HoldNode holdBode)
        {
            List<SerialPort> fromList = holdBode.GetPort("InPort").GetConnections();
            if (fromList.Count == 0)
            {
                self.HoldNodeUsed(holdBode);
                return;
            }
            foreach (SerialPort from in fromList)
            {
                foreach (SerialPort port in from.GetConnections())
                {
                    if (port.Node is HoldNode)
                    {
                        HoldNode childnode = port.Node as HoldNode;
                        self.HoldNodeUsed(childnode);
                    }
                }
            }
        }

        public static void HoldNodeUsed(this SerialGraph self, HoldNode holdNode)
        {
            if (holdNode.GetTimes() <= 0)
            {
                holdNode.AddTime();
                self.Blackboard.HoldNodes.Remove(holdNode.Id);

                SerialGraphEventSystem.Instance.ExitHold(self, holdNode);
            }
        }
    }
}
