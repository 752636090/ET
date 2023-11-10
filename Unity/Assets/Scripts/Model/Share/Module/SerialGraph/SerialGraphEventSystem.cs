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

        public bool CheckCondition(ConditionNode node, IConditionNodeParam param)
        {
            if (!allConditionNodeHandlers.TryGetValue(node.GetType(), out IConditionNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有AConditionNodeHandler");
                return false;
            }

            return handler.HandleCheck(node, param);
        }

        public bool CheckAllConnectNode(ConditionNode node, Direction direction, List<ConditionNode> line = null)
        {
            if (!allConditionNodeHandlers.TryGetValue(node.GetType(), out IConditionNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有AConditionNodeHandler");
                return false;
            }

            return handler.HandleCheckAllConnectNode(node, direction);
        }

        public bool HasParallelHandler(Type nodeType)
        {
            return allSameClassParallelHandlers.ContainsKey(nodeType);
        }

        public void ContinueParallel(Type type, SerialGraph graph, List<SerialNode> nodes)
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

            aHandler.ContinueArrange(graph, nodes);
        }

        public bool Active(ContinueNode node)
        {
            if (!allContinueNodeHandlers.TryGetValue(node.GetType(), out IContinueNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IContinueNodeHandler");
                return false;
            }

            return handler.HandleActive(node);
        }

        public void OnResult(ResultNode node)
        {
            if (!allResultNodeHandlers.TryGetValue(node.GetType(), out IResultNodeHandler handler))
            {
                Log.Debug($"类型{node.GetType()}没有IResultNodeHandler");
                return;
            }

            handler.HandleOnResult(node);
        }

        public void EnterHold(SerialGraph graph, HoldNode holdNode)
        {
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

            aHandler.HandleAfterHold(graph, holdNode).Coroutine();
        }

        public void ExitHold(SerialGraph graph, HoldNode holdNode)
        {
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

            aHandler.HandleBeforeHold(graph, holdNode).Coroutine();
        }

        public void CheckComplete(SerialGraph graph)
        {
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

            aHandler.HandleCheckComplete(graph);
        }

        public void Exit(SerialGraph graph)
        {
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

            aHandler.HandleExit(graph);
        }
    }
}
