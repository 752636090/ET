using ET.NodeDefine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ET
{
    // 应该没用了
    //[ConditionNodeHandler(typeof(ConditionNode))]
    //public class ConditionNodeHandler : AConditionNodeHandler<ConditionNode>
    //{
    //    protected override bool Check(ConditionNode node, IConditionNodeParam param)
    //    {
    //        return true;
    //    }

    //    protected override bool CheckAllConnectNode(ConditionNode node, Direction direction, List<ConditionNode> line = null)
    //    {
    //        return node.BaseCheckAllConnectNode(direction, line);
    //    }
    //}

    [AbstractDeclare]
    public abstract class AConditionNodeHandler<TEntity, TNode> : ASerialNodeHandler<TEntity, TNode>, IConditionNodeHandler where TEntity : Entity where TNode : ConditionNode
    {
        protected abstract bool Check(TEntity entity, TNode node, IConditionNodeParam param);

        public bool HandleCheck(Entity entity, ConditionNode node, IConditionNodeParam param)
        {
            return Check((TEntity)entity, (TNode)node, param);
        }

        protected abstract bool CheckAllConnectNode(TEntity entity, TNode node, Direction direction, List<ConditionNode> line = null);

        public bool HandleCheckAllConnectNode(Entity entity, ConditionNode node, Direction direction, List<ConditionNode> line = null)
        {
            return CheckAllConnectNode((TEntity)entity, (TNode)node, direction);
        }
    }
}
