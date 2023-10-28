using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [System.Serializable]
    [NodeWidth(200), NodeTint(120, 100, 30)]
    [NodeName("(临时)判断Int是否大于")]
    public class CheckIntNode : ConditionNode
    {
        [LabelText("键")]
        public string Key;
        [LabelText("值")]
        public int Value;
    }
}
