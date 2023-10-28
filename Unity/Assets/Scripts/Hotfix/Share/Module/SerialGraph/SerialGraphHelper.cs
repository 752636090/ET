using System.Collections.Generic;

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
                node.PortDict.Clear() ;
            }

            foreach (SerialPort port in self.Ports)
            {
                self.PortDict[port.Id] = port;
                self.NodeDict[port.NodeId].PortDict[port.Name] = port;
            }
        }
    }
}
