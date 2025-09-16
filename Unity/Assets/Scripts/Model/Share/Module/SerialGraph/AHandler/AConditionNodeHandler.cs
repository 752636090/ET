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
    public abstract class AConditionNodeHandler<TEntity, TNode, TParam> : ASerialNodeHandler<TEntity, TNode>, IConditionNodeHandler where TEntity : Entity where TNode : ConditionNode where TParam : struct
    {
        protected abstract bool CheckParam(TEntity entity, TNode node, TParam param);

        public bool HandleCheckParam(Entity entity, ConditionNode node, TParam param)
        {
            return CheckParam((TEntity)entity, (TNode)node, param);
        }

        protected abstract bool Check(TEntity entity, TNode node, NodeDefine.Direction direction, List<ConditionNode> line = null);

        public bool HandleCheck(Entity entity, ConditionNode node, NodeDefine.Direction direction, List<ConditionNode> line = null)
        {
            return Check((TEntity)entity, (TNode)node, direction);
            //bool result = Check((TEntity)entity, (TNode)node, direction);
            //if (node is IProgressNode progressNode)
            //{

            //}
        }
    }
}
