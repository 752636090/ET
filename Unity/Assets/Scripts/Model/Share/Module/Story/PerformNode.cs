using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET
{
    [Serializable]
    public abstract class PerformNode : ContinueNode
    {
        [Input(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("")]
        public PerformNode InPort;

        [Output(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("")]
        public PerformNode OutPort;
    }
}
