using ET.NodeDefine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ET
{
    // 应该没用了
    [ConditionNodeHandler(typeof(ConditionNode))]
    public class ConditionNodeHandler : AConditionNodeHandler<ConditionNode>
    {
        protected override bool Check(ConditionNode node, IConditionNodeParam param)
        {
            return true;
        }

        protected override bool CheckAllConnectNode(ConditionNode node, Direction direction, List<ConditionNode> line = null)
        {
            return node.BaseCheckAllConnectNode(direction, line);
        }
    }

    [AbstractDeclare]
    public abstract class AConditionNodeHandler<T> : ASerialNodeHandler<T>, IConditionNodeHandler where T : ConditionNode
    {
        protected abstract bool Check(T node, IConditionNodeParam param);

        public bool HandleCheck(ConditionNode node, IConditionNodeParam param)
        {
            return Check((T)node, param);
        }

        protected abstract bool CheckAllConnectNode(T node, Direction direction, List<ConditionNode> line = null);

        public bool HandleCheckAllConnectNode(ConditionNode node, Direction direction, List<ConditionNode> line = null)
        {
            return CheckAllConnectNode((T)node, direction);
        }
    }
}
