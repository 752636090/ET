namespace ET
{
    // 应该没用了
    [HappenNodeHandler(typeof(HappenNode))]
    public class HappenNodeHandler : AHappenNodeHandler<HappenNode>
    {
        protected override bool Active(HappenNode node)
        {
            node.Graph.SetCurrentNode(node);
            return true;
        }
    }

    [AbstractDeclare]
    public abstract class AHappenNodeHandler<T> : AContinueNodeHandler<T>, IHappenNodeHandler where T : HappenNode
    {

    }
}
