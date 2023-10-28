using System;
using Sirenix.OdinInspector;

namespace ET.Common
{
    [Serializable]
    public class WaitTimeNode : ResultNode
    {
        [LabelText("毫秒")]
        public int MilliSeconds;
    }
}
