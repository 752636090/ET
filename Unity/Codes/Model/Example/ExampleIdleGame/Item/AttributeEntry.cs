using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if SERVER
using MongoDB.Bson.Serialization.Attributes;
#endif

namespace ET
{
    public enum EntryType
    {
        Common = 1, // 普通词条
        Special = 2, // 特殊词条
    }

#if SERVER
    public class AttributeEntry : Entity, IAwake, IDestroy, ISerializeToEntity
#else
    public class AttributeEntry : Entity, IAwake, IDestroy
#endif
    {
        /// <summary>
        /// 词条数值属性类型
        /// </summary>
        public int Key;

        /// <summary>
        /// 词条数值属性值
        /// </summary>
        public long Value;

        /// <summary>
        /// 词条类型
        /// </summary>
        public EntryType Type;
    }
}
