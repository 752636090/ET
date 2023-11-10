using ET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [SameClassParallelHandler(typeof(RandomNode))]
    public class RandomParallelHandler : ASameClassParallelHandler
    {
        protected override void Continue(SerialGraph graph, List<SerialNode> nodes)
        {
            using ListComponent<RandomNode> randomNodes = ListComponent<RandomNode>.Create();
            int totalWeight = 0;
            foreach (RandomNode node in nodes)
            {
                if (SerialGraphEventSystem.Instance.Active(node))
                {
                    randomNodes.Add(node);
                    totalWeight += node.Weight;
                }
            }
            int ran = RandomGenerator.RandomNumber(0, totalWeight);
            int pass = 0;
            foreach (RandomNode randomNode in nodes)
            {
                pass += randomNode.Weight;
                if (ran < pass)
                {
                    randomNode.Continue();
                    break;
                }
            }
        }
    }
}
