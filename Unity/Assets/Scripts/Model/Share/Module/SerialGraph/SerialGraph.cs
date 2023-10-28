using UnityEngine;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Sirenix.OdinInspector;

namespace ET
{
    [Serializable]
    public class SerialGraph
    {
        // 行为树的Id,生成后唯一
        [ReadOnly]
        public int Id;

        //[SerializeReference]
        //public SerialNode Head;
        //// 注意里面的所有嵌套结构,使用了基类的都加 [SerializeReference]
        //// 服务器代码加个SerializeReferenceAttribute防止报错即可

        [ReadOnly]
        public int HeadId;
        [SerializeReference]
        [ReadOnly]
        public List<SerialNode> Nodes = new List<SerialNode>();
        [ReadOnly]
        public List<SerialPort> Ports = new List<SerialPort>();

        [BsonIgnore]
        [NonSerialized] // 因为Undo也要把这个Undo了所以不能NonSerialized 但又不想增加数据量所以还是NonSerialized了
        //[SerializeReference]
        public Dictionary<int, SerialNode> NodeDict = new Dictionary<int, SerialNode>();
        [BsonIgnore]
        [NonSerialized]
        //[SerializeReference]
        public Dictionary<int, SerialPort> PortDict = new Dictionary<int, SerialPort>();

        [ReadOnly]
        public SerialGraphType Type;
    }

    public enum SerialGraphType
    {
        [LabelText("剧情")]
        Story = 0,
    }
}
