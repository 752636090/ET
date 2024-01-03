namespace ET
{
    //public struct RecordResultNode
    //{
    //    public ResultNode Node;
    //}
    [AbstractDeclare]
    public abstract class AResultNodeHandler<T> : AContinueNodeHandler<T>, IResultNodeHandler where T : ResultNode
    {
        protected override bool Active(T node)
        {
            node.Graph.RecordPrize(node);
            return true;
        }

        public bool HandleOnResult(ResultNode node)
        {
            return OnResult(node as T);
        }

        protected abstract bool OnResult(T node);
    }
}
