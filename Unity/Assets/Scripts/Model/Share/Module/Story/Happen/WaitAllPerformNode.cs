using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [Serializable]
    public class WaitAllPerformNode : HappenNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public PerformNodePort InPort;

        [Output(typeConstraint: TypeConstraint.Strict, capacity: Capacity.Single)]
        [LabelText("")]
        public DialogNodePort OutPort;
    }
}
