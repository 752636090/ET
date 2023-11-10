using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET
{
    [System.Serializable]
    [NodeTint(20, 120, 40)]
    public abstract class ResultNode : ContinueNode
    {
        [Input(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("")]
        public DialogNodePort InPort;

        [Output(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("")]
        public DialogNodePort OutPort;

        public override bool IsSaveNode => true;
        public override bool CanParallel => false;
    }
}
