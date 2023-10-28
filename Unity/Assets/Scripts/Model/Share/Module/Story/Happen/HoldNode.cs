using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Common
{
    [System.Serializable]
    [NodeWidth(150)]
    [NodeName("流程判断")]
    public class HoldNode : HappenNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public DialogNodePort InPort;

        [LabelText("立即结算"), LabelWidth(50)]
        public bool DoResult = false;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("继续的条件")]
        public ConditionPort ConditionPort;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("继续")]
        public DialogNodePort OutPort;

        [System.NonSerialized]
        [BsonIgnore]
        public int checkSucessTimes = 0;
    }
}
