using ET.Common;
using ET.NodeDefine;
using System.Collections.Generic;

namespace ET
{
    [HappenNodeHandler(typeof(RandomNode))]
    //public class RandomNodeHandler : AHappenNodeHandler<RandomNode>
    public class RandomNodeHandler : AHappenNodeHandler<RandomNode>
    {
        protected override bool Active(RandomNode node)
        {
            // 判断生效条件
            return node.CheckCondition();
        }
    }
}
