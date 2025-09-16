using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class StoryComponent : Entity, IGraphsComponent, IAwake, IDeserialize
    {
        [BsonIgnore]
        public SerialGraphType GraphType => SerialGraphType.Story;
        //[BsonIgnore]
        //[StaticField]
        //public static Dictionary<int, SerialGraph> GraphConfigDict { get; set; }
        [BsonIgnore]
        public Dictionary<int, StoryEntity> StoryDict = new();
        [BsonIgnore]
        public UnOrderMultiMap<Type, SerialPort> HoldPorts { get; set; } = new();
        // 事件被获知的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, SerialPort> OpenConditionPorts = new();
        // 事件获知前关闭的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, SerialPort> CloseConditionPorts = new();
        // 事件获知后关闭的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, SerialPort> CloseStartedConditionPorts = new();
        // 事件播放的条件按类型划分, 每项对应一组事件
        [BsonIgnore]
        public UnOrderMultiMap<Type, SerialPort> StartConditionPorts = new();
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
        public List<CheckAtStoryOutBase> ListTriggerLaterAtStoryOut = new List<CheckAtStoryOutBase>();
        [BsonIgnore]
        // 由于正在播放一个事件, 新申请的检测排队等待
        public List<StoryWaitCheckBase> CheckTypeToWait = new();


        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string, object> MiscValueDict = new();

        public HashSet<long> ProgressStories = new();
    }

    // 事件结束后优先进行的重新检测
    public class CheckAtStoryOut<TNode, TParam> : CheckAtStoryOutBase where TNode : ConditionNode where TParam : struct/* where TInvokeParam : struct*/
    {
        public TParam Param;

        //public Func<StoryEntity, SerialPort, TParam, List<ConditionNode>, bool, bool> CheckConditionFromRootFunc;

        public override bool CheckConditionFromRoot(StoryEntity storyEntity)
        {
            return SerialGraphHelper.CheckConditionFromRoot<StoryEntity, TNode, TParam>(storyEntity, Port, Param, SuccessList);
        }
    }
    public abstract class CheckAtStoryOutBase
    {
        public SerialPort Port;
        public Type ConditionType;
        public List<ConditionNode> SuccessList;
        public long InvokeType;

        public abstract bool CheckConditionFromRoot(StoryEntity storyEntity);
        //public abstract void CallFunc();
    }
    public struct CondNullParam
    {
        [StaticField]
        public static CondNullParam Instance = new();
    }

    public delegate Action<CheckStorySuccessResult> CheckStorySuccessFunc(SerialNode node, List<ConditionNode> successList = null);
    public class StoryWaitCheck<TNode, TParam> : StoryWaitCheckBase
    {
        public TParam Param;
        public Action<StoryComponent, TParam> CheckDelegate;

        public override void Check(StoryComponent storyComponent)
        {
            CheckDelegate.Invoke(storyComponent, Param);
        }
    }
    public abstract class StoryWaitCheckBase
    {
        ///// <summary>
        ///// 需要检测的条件类型
        ///// </summary>
        //public Type ConditionType;
        /// <summary>
        /// 需要检测的列表类型
        /// </summary>
        public StoryCheckDicType DicType;
        public abstract void Check(StoryComponent storyComponent);
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

    public struct CheckStorySuccessResult
    {
        public StoryCondSuccessInvokeParam Param;

        public CheckStorySuccessResult(StoryCondSuccessInvokeParam param)
        {
            Param = param;
        }
    }
}
