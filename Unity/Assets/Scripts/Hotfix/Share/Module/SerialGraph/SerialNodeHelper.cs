using ET.Common;
using ET.NodeDefine;
using System;
using System.Collections.Generic;

namespace ET
{
    public static class SerialNodeHelper
    {
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

        public static bool CheckCondition(this SerialNode node, string portName = "ConditionPort")
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
                if (SerialGraphEventSystem.Instance.CheckAllConnectNode(conditionNode, Direction.Input))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ConditionNode
        public static bool CheckAllExceptSelf(this ConditionNode self, Direction io, List<ConditionNode> line = null)
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
                    return SerialGraphEventSystem.Instance.CheckAllConnectNode(self, io, line);
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
        public static bool CheckConditionLineWithType(this ConditionNode self, Type conditionType, IConditionNodeParam param, Direction io, List<ConditionNode> line)
        {
            if (self.GetType() == conditionType && SerialGraphEventSystem.Instance.CheckCondition(self, param))
            {
                return SerialGraphEventSystem.Instance.CheckAllConnectNode(self, io, line) && self.CheckAllExceptSelf(io == Direction.Input ? Direction.Output : Direction.Input, line);
            }
            // 找出指定类型的条件
            if (io == Direction.Output)
            {
                return self.GetPort("State").GetConnections().Exists(n => (n.Node as ConditionNode).CheckConditionLineWithType(conditionType, param, io, line));
            }
            return self.GetPort("StateIn").GetConnections().Exists(n => (n.Node as ConditionNode).CheckConditionLineWithType(conditionType, param, io, line));
        }

        public static bool BaseCheckAllConnectNode(this ConditionNode self, Direction io, List<ConditionNode> line = null)
        {
            // 这里是基类的处理, 直接判断后续节点, this的条件是否能通过应当子类override这个函数去处理, 成功后base到这里
            line?.Add(self); // 走到基类这里的肯定是已经成功的
            bool result = self.CheckAllExceptSelf(io, line);
            if (result == false)
            {
                line?.Remove(self);
            }
            return result;
        }
        #endregion

        #region ContinueNode
        public static void Continue(this ContinueNode self, string outPort = null)
        {
            if (self is HoldNode)
            {
                Log.Debug($"触发{self.Graph.Id}流程判断{self.Id} UTC时间{TimeInfo.Instance.ToDateTime(TimeInfo.Instance.ServerNow())}");
                if (self.Graph.GetEntity() is StoryEntity story)
                {
                    story.GetParent<StoryComponent>().IsProcessingStory = true; 
                }
            }
            self.Graph.ContinueArrange(self, outPort ?? self.DefaultOutPort);
        }
        #endregion

        #region HoldNode
        public static void Hold(this HoldNode self)
        {
            // 重新等待检测
            self.ClearTimes();
            // 等待条件满足再继续
            SerialPort conditionPort = self.GetPort("ConditionPort");
            self.Graph.RecordHold(conditionPort);
        }

        public static void TrySendResult(this HoldNode self)
        {
            // 发放奖励
            if (self.DoResult)
            {
                self.Graph.SendResult();
            }
        }
        #endregion
    }
}
