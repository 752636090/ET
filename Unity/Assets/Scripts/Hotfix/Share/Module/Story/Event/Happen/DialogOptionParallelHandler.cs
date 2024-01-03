using ET.Story;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [SameClassParallelHandler(typeof(DialogOptionNode))]
    public class DialogOptionParallelHandler : ASameClassParallelHandler
    {
        protected override void Continue(SerialGraph graph, List<SerialNode> nodes)
        {
            Log.Debug($"{nodes.Count} --Cast<DialogOptionNode>--> {nodes.Cast<DialogOptionNode>().Count()}");
            StoryComponent storyComponent = graph.Blackboard.Entity as StoryComponent;
            ListComponent<DialogOptionNode> optionNodes = ListComponent<DialogOptionNode>.Create();
            foreach (DialogOptionNode optionNode in nodes.Cast<DialogOptionNode>())
            {
                bool canActive = SerialGraphEventSystem.Instance.Active(optionNode);

                if ((graph.Blackboard.Entity as StoryComponent).IsOptionClosed(graph, optionNode) == false
                    && canActive)
                {
                    optionNodes.Add(optionNode);
                } 
            }

            using OptionNodeSorter sorter = OptionNodeSorter.Create(nodes);
            optionNodes.Sort(sorter);

            storyComponent.ShowDialogOptions(optionNodes);
        }
    }
}
