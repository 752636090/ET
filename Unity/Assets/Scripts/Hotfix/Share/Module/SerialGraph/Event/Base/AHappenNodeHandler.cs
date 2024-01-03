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
}
