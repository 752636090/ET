using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Story
{
    [Serializable]
    [NodeWidth(200), NodeTint(100, 70, 70)]
    public class StoryHeadInfoNode : StoryDefaultNode, IHeadSerialNode
    {
        [Output(typeConstraint: TypeConstraint.Strict, capacity: Capacity.Single)]
        [LabelText("-")]
        public StoryHeadAloneNodePort StartPort;
    }
}
