using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ET
{
    public enum StoryState
    {
        None = -1,
        /// <summary>
        /// 未获知
        /// </summary>
        NotOpen = 0,
        /// <summary>
        /// 关闭(在获知前关闭)
        /// </summary>
        Close = 1,
        /// <summary>
        /// 已获知
        /// </summary>
        Opened = 2,
        /// <summary>
        /// 已播放
        /// </summary>
        Started = 3,
        /// <summary>
        /// 完成
        /// </summary>
        Completed = 4,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 5,
        /// <summary>
        /// 逾期
        /// </summary>
        TimeOut = 6,
        /// <summary>
        /// 关闭(在获知后关闭)
        /// </summary>
        CloseAfterOpen = 7,
    }

    public interface IGraphEntity
    {
        public int GraphId { get; set; }
        //public GraphState State { get; set; }
        public SerialGraphBlackboard Blackboard { get; set; }

        [BsonIgnore]
        public SerialGraph Graph { get; set; }
    }

    public interface IGraphsComponent
    {
        //public Dictionary<string, List<int>> GraphHoldList = new Dictionary<string, List<int>>();

        [BsonIgnore]
        public SerialGraphType GraphType { get; }

        /// <summary>
        /// Value：Port的运行时在组件内的唯一Id
        /// </summary>
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> HoldNodes { get; set; }

        [BsonIgnore]
        [StaticField]
        public static MultiDictionary<SerialGraphType, int, SerialGraph> GraphConfigDict = new();
    }
}
