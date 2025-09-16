using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [System.Serializable]
    [NodeTint(30, 90, 120)]
    public abstract class HappenNode : ContinueNode
    {
        [BsonIgnore]
        public virtual bool IsWaitable => false;
    }
}
