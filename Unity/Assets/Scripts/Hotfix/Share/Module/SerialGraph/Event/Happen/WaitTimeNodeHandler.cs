using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(WaitTimeNode))]
    public class WaitTimeNodeHandler : AHappenNodeHandler<WaitTimeNode>
    {
        protected override bool Active(WaitTimeNode node)
        {
            Run(node).Coroutine();
            return true;
        }

        private async ETTask Run(WaitTimeNode node)
        {
            await node.Graph.GetEntity().Fiber().TimerComponent.WaitAsync(node.MilliSeconds);
            node.Graph.ContinueArrange(node, "OutPort");
        }
    }
}
