using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [NodeName("随机项")]
    //public class RandomNode : HappenNode
    public class RandomNode : HappenNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public DialogNodePort ChoicePort;

        [LabelText("权重")]
        public int Weight = 0;

        [Input(typeConstraint: TypeConstraint.Strict)]
        [HorizontalGroup("1", 100)]
        [LabelText("此项生效的条件"), LabelWidth(100)]
        public ConditionPort ConditionPort;

        [Output(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("结果")]
        public DialogNodePort OutPort;

        public override string DefaultOutPort => "OutPort";
    }
}
