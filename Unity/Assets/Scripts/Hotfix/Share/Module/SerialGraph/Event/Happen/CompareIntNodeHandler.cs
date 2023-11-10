using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(CompareIntNode))]
    public class CompareIntNodeHandler : AHappenNodeHandler<CompareIntNode>
    {
        protected override bool Active(CompareIntNode node)
        {
            int value = node.Graph.Blackboard.Get<int>(node.Key);
            if (value > node.Value)
            {
                node.Continue("MorePort");
            }
            else if (value < node.Value)
            {
                node.Continue("LessPort");
            }
            else
            {
                node.Continue("EqualPort");
            }
            return true;
        }
    }
}
