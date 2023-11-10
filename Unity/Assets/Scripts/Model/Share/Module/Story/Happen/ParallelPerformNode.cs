using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Story
{
    [Serializable]
    public class ParallelPerformNode : HappenNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public DialogNodePort InPort;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public PerformNodePort OutPort;
    }
}
