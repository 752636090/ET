using ET.Story;
using System.Collections.Generic;
using System;

namespace ET
{
    public class OptionNodeSorter : IComparer<DialogOptionNode>, IDisposable
    {
        private Dictionary<DialogOptionNode, int> preOrderDict = new Dictionary<DialogOptionNode, int>();

        private OptionNodeSorter() { }

        public static OptionNodeSorter Create(List<SerialNode> unfilteredNodes)
        {
            OptionNodeSorter sorter = ObjectPool.Instance.Fetch<OptionNodeSorter>();
            for (int i = 0; i < unfilteredNodes.Count; i++)
            {
                DialogOptionNode node = (DialogOptionNode)unfilteredNodes[i];
                sorter.preOrderDict.Add(node, i);
            }
            return sorter;
        }

        public void Dispose()
        {
            preOrderDict.Clear();
            ObjectPool.Instance.Recycle(this);
        }

        public int Compare(DialogOptionNode x, DialogOptionNode y)
        {
            if (x.Order != y.Order)
            {
                return x.Order.CompareTo(y.Order);
            }

            return preOrderDict[x].CompareTo(preOrderDict[y]);
        }
    }
}
