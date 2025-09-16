using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    // 应该没用了
    //[ContinueNodeHandler(typeof(ContinueNode))]
    //public class ContinueNodeHandler : AContinueNodeHandler<ContinueNode>
    //{
    //    protected override bool Active(Entity entity, ContinueNode node)
    //    {
    //        return true;
    //    }
    //}

    [AbstractDeclare]
    public abstract class AContinueNodeHandler<TEntity, TNode> : ASerialNodeHandler<TEntity, TNode>, IContinueNodeHandler where TEntity : Entity where TNode : ContinueNode
    {
        public bool HandleActive(Entity entity, ContinueNode node)
        {
            return Active(entity as TEntity, node as TNode);
        }

        protected abstract bool Active(TEntity entity, TNode node);
    }
}
