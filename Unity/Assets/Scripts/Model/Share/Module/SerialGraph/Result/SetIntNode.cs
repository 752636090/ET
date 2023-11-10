using System;
using ET.NodeDefine;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [NodeName("改变整数Int")]
    [Serializable]
    public class SetIntNode : ResultNode
    {
        public string Key;
        [LabelText("加值")]
        public int Value;
    }
}
