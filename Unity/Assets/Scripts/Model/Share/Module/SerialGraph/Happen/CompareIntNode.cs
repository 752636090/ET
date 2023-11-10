using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [System.Serializable]
    //[NodeWidth(200), NodeTint(120, 100, 30)]
    [NodeName("比较Int")]
    public class CompareIntNode : HappenNode
    {
        [Input(TypeConstraint.Strict)]
        [HideLabel]
        public DialogNodePort InPort; 
        [Output(TypeConstraint.Strict)]
        public DialogNodePort LessPort;
        [Output(TypeConstraint.Strict)]
        public DialogNodePort EqualPort;
        [Output(TypeConstraint.Strict)]
        public DialogNodePort MorePort;

        [LabelText("键")]
        public string Key;
        [LabelText("值")]
        public int Value;
    }
}
