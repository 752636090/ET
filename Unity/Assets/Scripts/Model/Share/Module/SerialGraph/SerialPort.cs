using MongoDB.Bson.Serialization.Attributes;
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
        [BsonIgnore]
        [NonSerialized]
        public SerialNode Node;
        [BsonIgnore]
        [NonSerialized]
        // 逻辑层不要直接调用
        public List<SerialPort> Connections;
        [BsonIgnore]
        [NonSerialized]
        // 逻辑层不要直接调用
        public List<SerialNode> TargetNodes;
    }
}
