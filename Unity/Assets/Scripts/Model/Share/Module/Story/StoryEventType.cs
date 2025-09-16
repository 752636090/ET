using System.Collections.Generic;

namespace ET
{
    [UniqueId]
    public static class StoryCondSuccessInvokeType
    {
        public const long CheckExitConditionNode = 1;
        public const long CheckOpenConditionNode = 2;
        public const long CheckStartAtStoryOut = 3;
        public const long CheckHoldAtStoryOut = 4;
    }

    [UniqueId]
    public static class StoryCondSuccessInvokeResultType
    {
        public const long CheckExitConditionNode = 10;
        public const long CheckStartAtStoryOut = 30;
        public const long CheckHoldAtStoryOut = 40;
    }

    public struct StoryCondSuccessInvokeParam
    {
        public StoryComponent StoryComponent;
        public SerialNode Node;
        public List<ConditionNode> SuccessList;

        public StoryCondSuccessInvokeParam(StoryComponent storyComponent, SerialNode node, List<ConditionNode> successList)
        {
            StoryComponent = storyComponent;
            Node = node;
            SuccessList = successList;
        }
    }

    public struct CheckExitConditionNodeResult
    {
        public StoryComponent StoryComponent;
        public SerialNode Node;
    }

    //public abstract class AStoryInvoke<T> : AInvokeHandler<StoryCondSuccessInvokeParam> where T : class
    //{
    //    public override void Handle(StoryCondSuccessInvokeParam a)
    //    {
    //        this.Run(a.Args as T);
    //    }

    //    protected abstract void Run(T t);
    //}

    //public struct CheckExitConditionNode
    //{
    //    public StoryComponent StoryComponent;
    //    public SerialNode Node;
    //}

    //public struct CheckOpenConditionNode
    //{
    //    public SerialNode Node;
    //    public List<ConditionNode> SuccessList;
    //}

    //public struct CheckStartAtStoryOut
    //{
    //    public SerialNode Node;
    //    public List<ConditionNode> SuccessList;
    //}

    //public struct CheckHoldAtStoryOut
    //{
    //    public SerialNode Node;
    //    public List<ConditionNode> SuccessList;
    //}
}
