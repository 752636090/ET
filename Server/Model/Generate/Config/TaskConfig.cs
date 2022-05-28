using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class TaskConfigCategory : ProtoObject, IMerge
    {
        public static TaskConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, TaskConfig> dict = new Dictionary<int, TaskConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<TaskConfig> list = new List<TaskConfig>();
		
        public TaskConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            TaskConfigCategory s = o as TaskConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            foreach (TaskConfig config in list)
            {
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public TaskConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TaskConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (TaskConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TaskConfig> GetAll()
        {
            return this.dict;
        }

        public TaskConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class TaskConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>c任务名字</summary>
		[ProtoMember(2)]
		public string TaskName { get; set; }
		/// <summary>c任务描述</summary>
		[ProtoMember(3)]
		public string TaskDesc { get; set; }
		/// <summary>任务行为类型</summary>
		[ProtoMember(4)]
		public int TaskActionType { get; set; }
		/// <summary>任务目标id</summary>
		[ProtoMember(5)]
		public int TaskTargetId { get; set; }
		/// <summary>任务目标数量</summary>
		[ProtoMember(6)]
		public int TaskTargetCount { get; set; }
		/// <summary>前置任务ID</summary>
		[ProtoMember(7)]
		public int TaskBeforeId { get; set; }
		/// <summary>任务奖励金币数量</summary>
		[ProtoMember(8)]
		public int RewardGoldCount { get; set; }

	}
}
