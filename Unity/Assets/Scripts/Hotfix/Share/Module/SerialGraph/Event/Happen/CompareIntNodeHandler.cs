using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(CompareIntNode))]
    public class CompareIntNodeHandler : AHappenNodeHandler<Entity, CompareIntNode>
    {
        protected override bool Active(Entity entity, CompareIntNode node)
        {
            int value = (entity as IGraphEntity).Blackboard.Get<int>(node.Key);
            if (value > node.Value)
            {
                node.Continue(entity, "MorePort");
            }
            else if (value < node.Value)
            {
                node.Continue(entity, "LessPort");
            }
            else
            {
                node.Continue(entity, "EqualPort");
            }
            return true;
        }
    }
}
