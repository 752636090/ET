using ET.Common;
using ET.NodeDefine;
using System.Collections.Generic;

namespace ET
{
    [HappenNodeHandler(typeof(RandomNode))]
    //public class RandomNodeHandler : AHappenNodeHandler<RandomNode>
    public class RandomNodeHandler : AHappenNodeHandler<Entity, RandomNode>
    {
        protected override bool Active(Entity entity, RandomNode node)
        {
            // 判断生效条件
            return node.CheckCondition(entity);
        }
    }
}
