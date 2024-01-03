using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class StoryComponent : Entity, IGraphsComponent, IAwake, ISerializeToEntity, IDeserialize
    {
        [BsonIgnore]
        public SerialGraphType GraphType => SerialGraphType.Story;
        //[BsonIgnore]
        //[StaticField]
        //public static Dictionary<int, SerialGraph> GraphConfigDict { get; set; }
        [BsonIgnore]
        public Dictionary<int, StoryEntity> StoryDict = new();
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> HoldNodes { get; set; }
        // 事件被获知的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> OpenConditionNodes = new();
        // 事件获知前关闭的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> CloseConditionNodes = new();
        // 事件获知后关闭的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> CloseStartedConditionNodes = new();
        // 事件播放的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, long> StartConditionNodes = new();
        [BsonIgnore]
        private bool isProcessingStory;
        [BsonIgnore]
        public bool IsProcessingStory
        {
            get { return isProcessingStory; }
            set { isProcessingStory = value; }
        }

        /// <summary>
        /// 一个触发条件的检测引发了多个事件的播放, 则排队等待第一个事件播放完后优先重新检测此队列
        /// </summary>
        [BsonIgnore]
        public List<CheckAtStoryOut> ListTriggerLaterAtStoryOut = new List<CheckAtStoryOut>();
        [BsonIgnore]
        // 由于正在播放一个事件, 新申请的检测排队等待
        public List<StoryWaitCheck> CheckTypeToWait = new();



    }

    // 事件结束后优先进行的重新检测
    public class CheckAtStoryOut
    {
        public SerialPort Port;
        public Type ConditionType;
        public IConditionNodeParam Param;
        public ActionCheckStorySuccess CallFunc;
        public List<ConditionNode> SuccessList;
    }

    public delegate Action<List<object>> ActionCheckStorySuccess(SerialNode node, List<ConditionNode> successList = null);
    public class StoryWaitCheck
    {
        /// <summary>
        /// 需要检测的条件类型
        /// </summary>
        public Type ConditionType;
        public IConditionNodeParam Param;
        /// <summary>
        /// 需要检测的列表类型
        /// </summary>
        public StoryCheckDicType DicType;
    }
    /// <summary>
    /// 释放玩家操作后需要进行的条件检测
    /// </summary>
    public enum StoryCheckDicType
    {
        Start,
        Hold,
        All
    }
}
