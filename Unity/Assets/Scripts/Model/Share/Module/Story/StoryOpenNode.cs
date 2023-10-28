using ET.NodeDefine;
using Sirenix.OdinInspector;
using System;

namespace ET.Story
{
    [System.Serializable]
    [NodeWidth(160), NodeTint(100, 70, 70)]
    public class StoryOpenNode : StoryDefaultNode
    {
        /// <summary>
        /// 状态入口
        /// </summary>
        [Input(typeConstraint: TypeConstraint.Strict, capacity: Capacity.Single)]
        [LabelText("")]
        public StoryHeadAloneNodePort Enter;

        [Output(typeConstraint: TypeConstraint.Strict)]
        [LabelText("(获知前)关闭")]
        public ConditionPort ExitConditions;

        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("设置条件")]
        public ConditionPort EnterConditions;

        [Output(typeConstraint: TypeConstraint.Strict, capacity: Capacity.Single)]
        [LabelText("")]
        public StoryOpenAloneNodePort Next;    // 下一个状态
    }
}
