using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(HoldNode))]
    public class HoldNodeHandler : AHappenNodeHandler<Entity, HoldNode>
    {
        protected override bool Active(Entity entity, HoldNode node)
        {
            node.Continue(entity);
            return false;
        }
    }
}
