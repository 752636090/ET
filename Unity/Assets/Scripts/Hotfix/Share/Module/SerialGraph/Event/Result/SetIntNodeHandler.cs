using ET.Common;

namespace ET
{
    [ResultNodeHandler(typeof(SetIntNode))]
    public class SetIntNodeHandler : AResultNodeHandler<Entity, SetIntNode>
    {
        protected override bool OnResult(Entity entity, SetIntNode node)
        {
            SerialGraphBlackboard blackboard = (entity as IGraphEntity).Blackboard;
            blackboard.AddOrUpdate(node.Key, blackboard.Get<int>(node.Key) + node.Value);
            Log.Debug(blackboard.Get<int>(node.Key).ToString());
            return true;
        }
    }
}
