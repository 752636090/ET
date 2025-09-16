using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ET
{
    [Serializable]
    [NodeTint(120, 100, 30)]
    public abstract class ConditionNode : SerialNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [HideLabel]
        public ConditionPort StateIn;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [HideLabel]
        public ConditionPort State;
    }

    //public interface IProgressNode
    //{
    //    [BsonIgnore]
    //    TaskProgressType ProgressType { get; }

    //    [HideLabel]
    //    [HideReferenceObjectPicker]
    //    [SerializeField]
    //    ProgressTaskInfo TaskInfo { get; set; }
    //}

    //public struct ProgressTaskInfo
    //{
    //    [LabelText("类型")]
    //    [LabelWidth(50)]
    //    public TaskActionType ActionType;
    //    [LabelText("目标ID")]
    //    [LabelWidth(50)]
    //    public int TargetId;
    //    [LabelText("目标数量")]
    //    [LabelWidth(50)]
    //    public int TargetCount;
    //}
}
