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

        public void HandleOnResult(Entity entity, ResultNode node)
        {
            OnResult(entity as TEntity, node as TNode);
        }

        protected abstract void OnResult(TEntity entity, TNode node);
    }
}
