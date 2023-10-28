using ET.NodeDefine;
using Sirenix.OdinInspector;
using System;

namespace ET
{
    [Serializable]
    public abstract class ConditionNode : SerialNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public ConditionPort StateIn;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public ConditionPort State;
    }
}
