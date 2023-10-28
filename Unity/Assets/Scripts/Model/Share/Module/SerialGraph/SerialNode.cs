using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [Serializable]
    public abstract class SerialNode
    {
        // 行为树内唯一,不需要保证连续性.从0开始.
        // 加载一棵树的时候,记录一下当前最大的节点id值,后续新增节点+1即可
        [HideInInspector]
        public int Id = 1; // 最低从1开始

        // 操，嵌套了几层，SerializedScriptableObject就序列化不出来字典了
        //[HideInInspector]
        //public Dictionary<string, int> PortIdDict = new Dictionary<string, int>();

        [BsonIgnore]
        [NonSerialized] // 因为Undo也要把这个Undo了所以不能NonSerialized 但又不想增加数据量所以还是NonSerialized了
        //[SerializeReference]
        [HideInInspector]
        public Dictionary<string, SerialPort> PortDict = new Dictionary<string, SerialPort>();

        [BsonIgnore]
        [NonSerialized]
        //[SerializeReference]
        [HideInInspector]
        public Dictionary<string, List<SerialNode>> CacheTargetNodeDict = new Dictionary<string, List<SerialNode>>();

        [BsonIgnore]
        [NonSerialized]
        //[SerializeReference]
        [HideInInspector]
        public SerialGraph Graph;

        public SerialNode()
        {

        }

        public virtual void OnReset() { }
    }

    public interface IHeadSerialNode { }
}
