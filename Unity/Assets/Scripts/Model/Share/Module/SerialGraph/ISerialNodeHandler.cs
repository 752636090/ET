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

    public abstract class ASerialNodeHandler<TEntity, TNode> : ISerialNodeHandler where TEntity : Entity
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
        bool HandleCheck(Entity entity, ConditionNode node, IConditionNodeParam param);

        bool HandleCheckAllConnectNode(Entity entity, ConditionNode node, Direction direction, List<ConditionNode> line = null);
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
        bool HandleActive(Entity entity, ContinueNode node);
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
        bool HandleOnResult(Entity entity, ResultNode node);
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
        public void ContinueArrange(Entity entity, List<SerialNode> nodes)
        {
            Continue(entity, nodes);
        }

        protected abstract void Continue(Entity entity, List<SerialNode> nodes);
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
        protected abstract ETTask EnterHold(Entity entity, HoldNode holdNode);
        protected abstract ETTask ExitHold(Entity entity, HoldNode holdNode);
        protected abstract void CheckComplete(Entity entity);
        protected abstract void Exit(Entity entity);

        public async ETTask HandleAfterHold(Entity entity, HoldNode holdNode)
        {
            try
            {
                await EnterHold(entity, holdNode);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async ETTask HandleBeforeHold(Entity entity, HoldNode holdNode)
        {
            try
            {
                await ExitHold(entity, holdNode);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void HandleCheckComplete(Entity entity)
        {
            try
            {
                CheckComplete(entity);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void HandleExit(Entity entity)
        {
            try
            {
                Exit(entity);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
