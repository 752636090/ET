using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class ForgeProductionConfigCategory : ProtoObject, IMerge
    {
        public static ForgeProductionConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, ForgeProductionConfig> dict = new Dictionary<int, ForgeProductionConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<ForgeProductionConfig> list = new List<ForgeProductionConfig>();
		
        public ForgeProductionConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ForgeProductionConfigCategory s = o as ForgeProductionConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            foreach (ForgeProductionConfig config in list)
            {
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public ForgeProductionConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ForgeProductionConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (ForgeProductionConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ForgeProductionConfig> GetAll()
        {
            return this.dict;
        }

        public ForgeProductionConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class ForgeProductionConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>物品id</summary>
		[ProtoMember(2)]
		public int ItemConfigId { get; set; }
		/// <summary>制造时间</summary>
		[ProtoMember(3)]
		public int ProductionTime { get; set; }
		/// <summary>制造所需的资源Id</summary>
		[ProtoMember(4)]
		public int ConsumeId { get; set; }
		/// <summary>制造所需的资源数量</summary>
		[ProtoMember(5)]
		public int ConsumeCount { get; set; }
		/// <summary>制造所需等级</summary>
		[ProtoMember(6)]
		public int NeedLevel { get; set; }

	}
}
