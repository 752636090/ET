using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ComponentOf(typeof(IGraphEntity))]
    public class HappenNodeWaitingComponent : Entity, IAwake<HappenNode>, IDestroy, ISerializeToEntity, IDeserialize
    {
        public int NodeId;

        [BsonIgnore]
        public HappenNode Node
        {
            get
            {
                return (HappenNode)(Parent as IGraphEntity).Graph.GetNode(NodeId);
            }
            set
            {
                NodeId = value.Id;
            }
        }

        [BsonIgnore]
        public ETCancellationToken CancellationToken;
    }
}
