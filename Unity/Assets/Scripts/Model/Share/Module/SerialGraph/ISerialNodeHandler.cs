using System;
using System.Collections.Generic;
using ET.Common;
using ET.NodeDefine;

namespace ET
{
    /// <summary>
    /// 解决分析器报错
    /// </summary>
    public class AbstractDeclareAttribute : BaseAttribute
    {

    }

    public abstract class TypeKeyBaseAttribute : BaseAttribute
    {
        public Type Type { get; }

        public TypeKeyBaseAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SerialNodeHandlerAttribute : TypeKeyBaseAttribute
    {
        public SerialNodeHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface ISerialNodeHandler
    {
    }

    public interface IConditionNodeParam : IEquatable<IConditionNodeParam>
    {

    }

    public abstract class ASerialNodeHandler<T> : ISerialNodeHandler
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConditionNodeHandlerAttribute : SerialNodeHandlerAttribute
    {
        public ConditionNodeHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface IConditionNodeHandler : ISerialNodeHandler
    {
        bool HandleCheck(ConditionNode node, IConditionNodeParam param);

        bool HandleCheckAllConnectNode(ConditionNode node, Direction direction, List<ConditionNode> line = null);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ContinueNodeHandlerAttribute : SerialNodeHandlerAttribute
    {
        public ContinueNodeHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface IContinueNodeHandler : ISerialNodeHandler
    {
        bool HandleActive(ContinueNode node);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class HappenNodeHandlerAttribute : ContinueNodeHandlerAttribute
    {
        public HappenNodeHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface IHappenNodeHandler : IContinueNodeHandler
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ResultNodeHandlerAttribute : ContinueNodeHandlerAttribute
    {
        public ResultNodeHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface IResultNodeHandler : IContinueNodeHandler
    {
        bool HandleOnResult(ResultNode node);
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class SameClassParallelHandlerAttribute : TypeKeyBaseAttribute
    {
        public SameClassParallelHandlerAttribute(Type type) : base(type)
        {
        }
    }

    public interface ISameClassParallelHandler
    {
    }

    public abstract class ASameClassParallelHandler : ISameClassParallelHandler
    {
        public void ContinueArrange(SerialGraph graph, List<SerialNode> nodes)
        {
            Continue(graph, nodes);
        }

        protected abstract void Continue(SerialGraph graph, List<SerialNode> nodes);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SerialGraphHandlerAttribute : BaseAttribute
    {
        public SerialGraphType Type;

        public SerialGraphHandlerAttribute(SerialGraphType type)
        {
            Type = type;
        }
    } 

    public interface ISerialGraphHandler
    {
    }

    public abstract class ASerialGraphHandler : ISerialGraphHandler
    {
        protected abstract ETTask EnterHold(SerialGraph graph, HoldNode holdNode);
        protected abstract ETTask ExitHold(SerialGraph graph, HoldNode holdNode);
        protected abstract void CheckComplete(SerialGraph graph);
        protected abstract void Exit(SerialGraph graph);

        public async ETTask HandleAfterHold(SerialGraph graph, HoldNode holdNode)
        {
            try
            {
                await EnterHold(graph, holdNode);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async ETTask HandleBeforeHold(SerialGraph graph, HoldNode holdNode)
        {
            try
            {
                await ExitHold(graph, holdNode);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void HandleCheckComplete(SerialGraph graph)
        {
            try
            {
                CheckComplete(graph);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void HandleExit(SerialGraph graph)
        {
            try
            {
                Exit(graph);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
