using System;
using System.Collections.Generic;

namespace ET
{
    [Serializable]
    public class SerialPort
    {
        public int Id = 1; // 从1开始
        public int NodeId;
        public List<int> TargetIds = new List<int>();
        public string Name; // Odin序列化不出来嵌套类里的字典(SerialNode.PortIdDict)，只能在这写个字段了
    }
}
