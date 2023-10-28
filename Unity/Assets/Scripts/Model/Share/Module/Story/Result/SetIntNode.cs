using System;
using ET.NodeDefine;

namespace ET.Common
{
    [NodeName("改变整数Int")]
    [Serializable]
    public class SetIntNode : ResultNode
    {
        public string Key;
    }
}
