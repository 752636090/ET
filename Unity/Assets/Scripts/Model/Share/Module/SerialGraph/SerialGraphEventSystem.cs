using ET.Common;
using ET.NodeDefine;
using SharpCompress.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [Code]
    public class SerialGraphEventSystem : Singleton<SerialGraphEventSystem>, ISingletonAwake
    {
        private readonly Dictionary<Type, IConditionNodeHandler> allConditionNodeHandlers = new();
        private readonly Dictionary<Type, IContinueNodeHandler> allContinueNodeHandlers = new();
        private readonly Dictionary<Type, ISameClassParallelHandler> allSameClassParallelHandlers = new();
        private readonly Dictionary<Type, IHappenNodeHandler> allHappenNodeHandlers = new();
        private readonly Dictionary<Type, IWaitableHappenNodeHandler> allWaitableHappenNodeHandlers = new();
        private readonly Dictionary<SerialGraphType, ISerialGraphHandler> allGraphHandlers = new();
        private readonly Dictionary<Type, IResultNodeHandler> allResultNodeHandlers = new();

        public void Awake()
        {
            //CollectHandlers<SerialNodeHandlerAttribute, ISerialNodeHandler>(allHappenNodeHandlers);
            //CollectPolymHandlers(typeof(SerialNode), allHappenNodeHandlers);

            CollectHandlers<ConditionNodeHandlerAttribute, IConditionNodeHandler>(allConditionNodeHandlers);
            //CollectPolymHandlers(typeof(ConditionNode), allConditionNodeHandlers);

            CollectHandlers<ContinueNodeHandlerAttribute, IContinueNodeHandler>(allContinueNodeHandlers);
            //CollectPolymHandlers(typeof(ContinueNode), allContinueNodeHandlers);

            CollectHandlers<SameClassParallelHandlerAttribute, ISameClassParallelHandler>(allSameClassParallelHandlers);
            //CollectPolymHandlers(typeof(SerialNode), allSameClassParallelHandlers);

            CollectHandlers<HappenNodeHandlerAttribute, IHappenNodeHandler>(allHappenNodeHandlers);
            CollectHandlers<WaitableHappenNodeHandlerAttribute, IWaitableHappenNodeHandler>(allWaitableHappenNodeHandlers);
            //CollectPolymHandlers(typeof(HappenNode), allHappenNodeHandlers);

            CollectHandlers<ResultNodeHandlerAttribute, IResultNodeHandler>(allResultNodeHandlers);

            CollectGraphHandlers();
        }

        private void CollectHandlers<TAttr, TI>(Dictionary<Type, TI> list) where TAttr : TypeKeyBaseAttribute
        {
            CodeTypes codeTypes = CodeTypes.Instance;
            foreach (Type eachType in codeTypes.GetTypes().Values)
            {
                if (!typeof(TAttr).IsAssignableFrom(eachType))
                {
                    continue;
                }
                foreach (Type type in codeTypes.GetTypes(eachType))
                {
                    object[] attrs = type.GetCustomAttributes(typeof(TAttr), false);
                    TI obj = default;
                    if (!type.IsGenericType)
                    {
                        obj = (TI)Activator.CreateInstance(type)
                            ?? throw new Exception($"type is not {typeof(TI)}: {type.Name}");
                    }
                    foreach (object attr in attrs)
                    {
                        TAttr handlerAttribute = attr as TAttr;
                        if (type.IsGenericType)
                        {
                            obj = (TI)Activator.CreateInstance(type.MakeGenericType(handlerAttribute.Type))
                                ?? throw new Exception($"type is not {typeof(TI)}: {type.Name}<{handlerAttribute.Type}>");
                        }
                        list[handlerAttribute.Type] = obj;
                    }
                } 
            }
        }
        
        //private void CollectPolymHandlers<TI>(Type rootBaseType, Dictionary<Type, TI> list)
        //{
        //    CodeTypes codeTypes = CodeTypes.Instance;
        //    foreach (Type type in codeTypes.GetTypes().Values)
        //    {
        //        if (rootBaseType.IsAssignableFrom(type))
        //        {
        //            Type baseType = type;
        //            while (!list.ContainsKey(baseType))
        //            {
        //                baseType = baseType.BaseType;
        //            }
        //            list[type] = list[baseType];
        //        }
        //    }
        //}

        private void CollectGraphHandlers()
        {
            CodeTypes codeTypes = CodeTypes.Instance;
            foreach (Type type in codeTypes.GetTypes(typeof(SerialGraphHandlerAttribute)))
            {
                if (Activator.CreateInstance(type) is not ISerialGraphHandler obj)
                {
                    throw new Exception($"type not is ASerialGraphHandler: {type.Name}");
                }

                object[] attrs = type.GetCustomAttributes(typeof(SerialGraphHandlerAttribute), false);
                foreach (object attr in attrs)
                {
                    SerialGraphHandlerAttribute handlerAttribute = attr as SerialGraphHandlerAttribute;
                    //Type eventType = obj.Type;
                    SerialGraphType graphType = handlerAttribute.Type;

                    allGraphHandlers[graphType] = obj;
                }
            }
        }

        /// <summary>
        /// 注意(先不管这条注意事项)通用节点一定要把泛型TEntity声明成Entity(基类)
        /// </summary>
        /// <returns></returns>
        public bool CheckConditionParam<TEntity, TNode, TParam>(TEntity entity, TNode node, TParam param) where TEntity : Entity where TNode : ConditionNode where TParam : struct
        {
            if (!allConditionNodeHandlers.TryGetValue(node.GetType(), out IConditionNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IConditionNodeHandler");
                return false;
            }
            if (handler is AConditionNodeHandler<TEntity, TNode, TParam> aHandler1)
            {
                return aHandler1.HandleCheckParam(entity, node, param);
            }
            else if (handler is AConditionNodeHandler<Entity, TNode, TParam> aHandler2)
            {
                return aHandler2.HandleCheckParam(entity, node, param);
            }

            Log.Error($"没有AConditionNodeHandler<{entity.GetType()}, {node.GetType()}, {param.GetType()}> " +
                $"且没有AConditionNodeHandler<Entity, {node.GetType()}, {param.GetType()}>");
            Log.Error($"{handler.GetType().BaseType}\n{typeof(AConditionNodeHandler<TEntity, TNode, TParam>)}");
            Log.Debug($"{typeof(TNode)}");
            return false;
        }

        public bool CheckAllConnectNode(Entity entity, ConditionNode node, NodeDefine.Direction direction, List<ConditionNode> line = null)
        {
            if (!allConditionNodeHandlers.TryGetValue(node.GetType(), out IConditionNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有AConditionNodeHandler");
                return false;
            }

            if (!handler.HandleCheck(entity, node, direction, line))
            {
                return false;
            }

            line?.Add(node); // 走到基类这里的肯定是已经成功的
            bool result = node.CheckAllExceptSelf(entity, direction, line);
            if (result == false)
            {
                line?.Remove(node);
            }
            return result;
        }

        public bool HasParallelHandler(Type nodeType)
        {
            return allSameClassParallelHandlers.ContainsKey(nodeType);
        }

        public void ContinueParallel(Entity entity, Type type, List<SerialNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                Log.Error("操？");
                return;
            }

            if (!allSameClassParallelHandlers.TryGetValue(type, out ISameClassParallelHandler handler))
            {
                Log.Debug($"类型{type}没有ASameClassParallelHandler");
                return;
            }
            if (handler is not ASameClassParallelHandler aHandler)
            {
                Log.Error($"ContinueArrange error: {handler.GetType().FullName}");
                return;
            }

            aHandler.ContinueArrange(entity, nodes);
        }

        public bool Active(Entity entity, ContinueNode node)
        {
            if (!allContinueNodeHandlers.TryGetValue(node.GetType(), out IContinueNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IContinueNodeHandler");
                return false;
            }

            return handler.HandleActive(entity, node);
        }

        public async ETTask HappenStartWait(Entity entity, HappenNode node, ETCancellationToken cancellationToken = null)
        {
            if (!allWaitableHappenNodeHandlers.TryGetValue(node.GetType(), out IWaitableHappenNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IWaitableHappenNodeHandler");
                return;
            }

            await handler.HandleStartWait(entity, node, cancellationToken);
        }

        public void OnResult(Entity entity, ResultNode node)
        {
            if (!allResultNodeHandlers.TryGetValue(node.GetType(), out IResultNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IResultNodeHandler");
                return;
            }

            handler.HandleOnResult(entity, node);
        }

        public void EnterHold(Entity entity, HoldNode holdNode)
        {
            SerialGraph graph = (entity as IGraphEntity).Graph;
            if (!allGraphHandlers.TryGetValue(graph.Type, out ISerialGraphHandler handler))
            {
                Log.Debug($"{graph.Type}没有ISerialGraphHandler");
                return;
            }
            if (handler is not ASerialGraphHandler aHandler)
            {
                Log.Error($"Active error: {handler.GetType().FullName}");
                return;
            }

            aHandler.HandleAfterHold(entity, holdNode).Coroutine();
        }

        public void ExitHold(Entity entity, HoldNode holdNode)
        {
            SerialGraph graph = (entity as IGraphEntity).Graph;
            if (!allGraphHandlers.TryGetValue(graph.Type, out ISerialGraphHandler handler))
            {
                Log.Debug($"{graph.Type}没有ISerialGraphHandler");
                return;
            }
            if (handler is not ASerialGraphHandler aHandler)
            {
                Log.Error($"Active error: {handler.GetType().FullName}");
                return;
            }

            aHandler.HandleBeforeHold(entity, holdNode).Coroutine();
        }

        public void CheckComplete(Entity entity)
        {
            SerialGraph graph = (entity as IGraphEntity).Graph;
            if (!allGraphHandlers.TryGetValue(graph.Type, out ISerialGraphHandler handler))
            {
                Log.Debug($"{graph.Type}没有ISerialGraphHandler");
                return;
            }
            if (handler is not ASerialGraphHandler aHandler)
            {
                Log.Error($"Active error: {handler.GetType().FullName}");
                return;
            }

            aHandler.HandleCheckComplete(entity);
        }

        /// <summary>
        /// 暂时退出
        /// </summary>
        public void Exit(Entity entity)
        {
            SerialGraph graph = (entity as IGraphEntity).Graph;
            if (!allGraphHandlers.TryGetValue(graph.Type, out ISerialGraphHandler handler))
            {
                Log.Debug($"{graph.Type}没有ISerialGraphHandler");
                return;
            }
            if (handler is not ASerialGraphHandler aHandler)
            {
                Log.Error($"Active error: {handler.GetType().FullName}");
                return;
            }

            aHandler.HandleExit(entity);
        }
    }
}
