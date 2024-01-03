namespace ET
{
    //public struct RecordResultNode
    //{
    //    public ResultNode Node;
    //}
    [AbstractDeclare]
    public abstract class AResultNodeHandler<TEntity, TNode> : AContinueNodeHandler<TEntity, TNode>, IResultNodeHandler where TEntity : Entity where TNode : ResultNode
    {
        protected override bool Active(TEntity entity, TNode node)
        {
            (entity as IGraphEntity).RecordPrize(node);
            return true;
        }

        public bool HandleOnResult(Entity entity, ResultNode node)
        {
            return OnResult(entity as TEntity, node as TNode);
        }

        protected abstract bool OnResult(TEntity entity, TNode node);
    }
}
