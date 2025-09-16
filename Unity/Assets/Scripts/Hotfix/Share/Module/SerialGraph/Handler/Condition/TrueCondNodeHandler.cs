using ET.Common;
using System.Collections.Generic;

namespace ET.Client
{
    [ConditionNodeHandler(typeof(TrueCondNode))]
    public class TrueCondNodeHandler : AConditionNodeHandler<Entity, TrueCondNode, CondNullParam>
    {
        protected override bool CheckParam(Entity entity, TrueCondNode node, CondNullParam param)
        {
            return true;
        }

        protected override bool Check(Entity entity, TrueCondNode node, NodeDefine.Direction direction, List<ConditionNode> line = null)
        {
            return true;
        }
    }
}
