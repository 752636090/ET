using System;
using UnityEngine;

namespace ET.Story
{
    [Serializable]
    public abstract class StoryDefaultNode : SerialNode, ISystemSerialNode, INodeActiveTimes
    {
        public string ActiveTimeKey { get; set; }
    }
}

namespace ET
{
    public interface ISystemSerialNode { }
}