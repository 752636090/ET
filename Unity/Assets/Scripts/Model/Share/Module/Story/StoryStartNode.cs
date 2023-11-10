using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Story
{
    [Serializable]
    public class StoryStartNode : StoryDefaultNode
    {
        [Input(typeConstraint: TypeConstraint.Strict, capacity: Capacity.Single)]
        [LabelText("")]
        public StoryOpenAloneNodePort Enter;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("(获知后)关闭")]
        public ConditionPort ExitConditions;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("播放")]
        public DialogNodePort PlayPort;
    }
}
