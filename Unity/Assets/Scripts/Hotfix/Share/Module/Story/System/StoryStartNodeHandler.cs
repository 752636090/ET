using ET.Story;

namespace ET
{
    public static class StoryStartNodeHelper
    {
        public static bool CheckCloseCondition(this StoryStartNode self, StoryEntity entity)
        {
            SerialPort conditionPort = self.GetPort("ExitConditions");
            if (conditionPort.GetConnections().Count == 0)
            {
                return false;
            }

            foreach (SerialPort port in conditionPort.GetConnections())
            {
                if (SerialGraphEventSystem.Instance.CheckAllConnectNode(entity, port.Node as ConditionNode, NodeDefine.Direction.Output))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [SerialNodeHandler(typeof(StoryStartNode))]
    public class StoryStartNodeHandler : ASerialNodeHandler<StoryEntity, StoryStartNode>
    {
    }
}
