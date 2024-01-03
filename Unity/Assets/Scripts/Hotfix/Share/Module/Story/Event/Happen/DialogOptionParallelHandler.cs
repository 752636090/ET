using ET.Story;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [SameClassParallelHandler(typeof(DialogOptionNode))]
    public class DialogOptionParallelHandler : ASameClassParallelHandler
    {
        protected override void Continue(Entity entity, List<SerialNode> nodes)
        {
            Log.Debug($"{nodes.Count} --Cast<DialogOptionNode>--> {nodes.Cast<DialogOptionNode>().Count()}");
            StoryEntity story = entity as StoryEntity;
            StoryComponent storyComponent = (entity as StoryEntity).GetParent<StoryComponent>();
            ListComponent<DialogOptionNode> optionNodes = ListComponent<DialogOptionNode>.Create();
            foreach (DialogOptionNode optionNode in nodes.Cast<DialogOptionNode>())
            {
                bool canActive = SerialGraphEventSystem.Instance.Active(story, optionNode);

                if (storyComponent.IsOptionClosed(story, optionNode) == false
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
