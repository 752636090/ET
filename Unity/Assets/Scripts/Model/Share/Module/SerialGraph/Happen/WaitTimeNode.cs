using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [Serializable]
    [NodeWidth(120)]
    [NodeName("等待时间")]
    public class WaitTimeNode : HappenNode
    {
        [HideLabel]
        [Input(TypeConstraint.Strict)]
        public DialogNodePort InPort;
        [HideLabel]
        [Output(TypeConstraint.Strict)]
        public DialogNodePort OutPort;

        [LabelText("毫秒")]
        [LabelWidth(40)]
        public int MilliSeconds;

        public override string DefaultOutPort => "OutPort";
    }
}
