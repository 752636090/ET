using ET.Common;

namespace ET
{
    [ResultNodeHandler(typeof(SetIntNode))]
    public class SetIntNodeHandler : AResultNodeHandler<SetIntNode>
    {
        protected override bool OnResult(SetIntNode node)
        {
            node.Graph.Blackboard.AddOrUpdate(node.Key, node.Graph.Blackboard.Get<int>(node.Key) + node.Value);
            Log.Debug(node.Graph.Blackboard.Get<int>(node.Key).ToString());
            return true;
        }
    }
}
