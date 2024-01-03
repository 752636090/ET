using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(WaitTimeNode))]
    public class WaitTimeNodeHandler : AHappenNodeHandler<Entity, WaitTimeNode>
    {
        protected override bool Active(Entity entity, WaitTimeNode node)
        {
            Run(entity, node).Coroutine();
            return true;
        }

        private async ETTask Run(Entity entity, WaitTimeNode node)
        {
            await entity.Fiber().Root.GetComponent<TimerComponent>().WaitAsync(node.MilliSeconds);
            (entity as IGraphEntity).ContinueArrange(node, "OutPort");
        }
    }
}
