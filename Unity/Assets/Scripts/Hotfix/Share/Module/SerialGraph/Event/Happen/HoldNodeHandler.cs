using ET.Common;

namespace ET
{
    [HappenNodeHandler(typeof(HoldNode))]
    public class HoldNodeHandler : AHappenNodeHandler<HoldNode>
    {
        protected override bool Active(HoldNode node)
        {
            node.Continue();
            return false;
        }
    }
}
