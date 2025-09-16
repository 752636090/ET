namespace ET
{
    // 应该没用了
    //[HappenNodeHandler(typeof(HappenNode))]
    //public class HappenNodeHandler : AHappenNodeHandler<HappenNode>
    //{
    //    protected override bool Active(HappenNode node)
    //    {
    //        node.Graph.SetCurrentNode(node);
    //        return true;
    //    }
    //}

    [AbstractDeclare]
    public abstract class AHappenNodeHandler<TEntity, TNode> : AContinueNodeHandler<TEntity, TNode>, IHappenNodeHandler where TEntity : Entity where TNode : HappenNode
    {

    }

    [AbstractDeclare]
    public abstract class AWaitableHappenNodeHandler<TEntity, TNode> : IWaitableHappenNodeHandler where TEntity : Entity where TNode : HappenNode
    {
        public async ETTask HandleStartWait(Entity entity, HappenNode node, ETCancellationToken cancellationToken = null)
        {
            await StartWait(entity as TEntity, node as TNode, cancellationToken);
        }

        protected abstract ETTask StartWait(TEntity entity, TNode node, ETCancellationToken cancellationToken = null);
    }
}
