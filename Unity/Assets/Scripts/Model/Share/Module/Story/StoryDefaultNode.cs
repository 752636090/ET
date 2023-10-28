using System;
using UnityEngine;

namespace ET.Story
{
    [Serializable]
    public abstract class StoryDefaultNode : SerialNode, ISystemSerialNode
    {
        /// <summary>
        /// 标记是否已经激活
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public int ActiveTimes = 0;
    }
}

namespace ET
{
    public interface ISystemSerialNode { }
}