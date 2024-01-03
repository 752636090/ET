using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ET
{
    public class SerialGraphBlackboard : IDisposable // 不继承Entity 实现下IDispose把Value<T> 扔池里
    {
        // 在哪个实体身上运行的,技能的就是Spell,任务的就是Quest
        [BsonIgnore]
        public Entity Entity { get; set; }
        //public BTEventType EventType { get; init; }
        //为了方便各种异步情况下行为树的运行,节点的执行返回类型设计为ETTask,参数       中携带一个取消标记ETCancelTokenSource,方便外部及时中断整颗树的运行.
        [BsonIgnore]
        public ETCancellationToken Cts { get; set; }
        private Dictionary<string, object> values = new();
        //public long InstanceId { get; }
        [BsonIgnore]
        private HappenNode CurrentNode;
        //private SerialGraphsDataComponent ModuleSaveData;
        [BsonIgnoreIfNull]
        public List<int> HoldNodes;
        /// <summary>
        /// 结果库
        /// </summary>
        [BsonIgnoreIfNull]
        public List<int> Results;

        [BsonIgnore]
        public SerialGraph Graph
        {
            get
            {
                return (Entity as IGraphEntity).Graph;
            }
        }

        private SerialGraphBlackboard() { }

        public SerialGraphBlackboard(Entity entity)
        {
            Cts = new ETCancellationToken();
            Entity = entity;
        }

        // 要注意针对值类型的处理
        public void AddOrUpdate<T>(string key, T value)
        {
            // 防止装箱拆箱问题
            if (typeof(T).IsValueType)
            {
                //这里要接入一次池化
                if (!values.TryGetValue(key, out object obj))
                {
                    obj = new ValueObject<T>();
                    values.Add(key, obj);
                }
                (obj as ValueObject<T>).Value = value;
                return;
            }
            values[key] = value;
        }
        // 如果想要获取的类型在values中找不到,抛出异常
        public T Get<T>(string key)
        {
            if (!values.TryGetValue(key, out object obj))
            {
                // 抛出异常
                return default;
            }
            if (typeof(T).IsValueType)
            {
                var value = obj as ValueObject<T>;
                return value.Value;
            }
            return (T)obj;

        }

        public void SetCurrentNode(HappenNode node)
        {
            //currentNodeId = node.Id;
            CurrentNode = node;
        }

        public HappenNode GetCurrentNode()
        {
            return CurrentNode;
        }

        //public void OnDeserialize(SerialGraph graph, Entity entity)
        //{
        //    if (currentNodeId > 0)
        //    {
        //        CurrentNode = graph.NodeDict[currentNodeId] as HappenNode; 
        //    }
        //}

        public void Dispose()
        {
            foreach (object value in values)
            {
                if (value is IValueObject)
                {
                    ObjectPool.Instance.Recycle(value);
                }
            }
        }

        public T GetEntity<T>() where T : Entity
        {
            return Entity as T;
        }

        //public SerialGraphsDataComponent GetModuleSaveData()
        //{
        //    return ModuleSaveData;
        //}

        public void AddActiveTime(INodeActiveTimes node)
        {
            if (!values.TryGetValue(node.ActiveTimeKey, out object value))
            {
                AddOrUpdate(node.ActiveTimeKey, 0);
            }

            AddOrUpdate(node.ActiveTimeKey, 1);
        }

        public int GetActiveTime(INodeActiveTimes node)
        {
            if (!values.TryGetValue(node.ActiveTimeKey, out object value))
            {
                return 0;
            }

            return Get<int>(node.ActiveTimeKey);
        }

        public void ClearActiveTime(INodeActiveTimes node)
        {
            AddOrUpdate(node.ActiveTimeKey, 0);
        }




        interface IValueObject : IReset { }
        class ValueObject<T> : IValueObject
        {
            public T Value;

            public void Reset()
            {
                Value = default;
            }
        }
    }
}
