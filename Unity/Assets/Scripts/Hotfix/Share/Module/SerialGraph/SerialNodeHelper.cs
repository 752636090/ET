using System.Collections.Generic;

namespace ET
{
    public static class SerialNodeHelper
    {
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
            if (node.CacheTargetNodeDict.TryGetValue(name, out List<SerialNode> nodes))
            {
                return nodes;
            }

            SerialPort port = GetPort(node, name);
            if (port == null)
            {
                return null;
            }

            node.CacheTargetNodeDict[name] = nodes = new List<SerialNode>();
            foreach (int targetPortId in port.TargetIds)
            {
                SerialPort targetPort = node.Graph.PortDict[targetPortId];
                nodes.Add(node.Graph.NodeDict[targetPort.NodeId]);
            }

            return nodes;
        }

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
    }
}
