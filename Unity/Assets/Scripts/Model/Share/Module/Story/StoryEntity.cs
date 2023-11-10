using ET.Story;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ET
{
    [ChildOf(typeof(StoryComponent))]
    public class StoryEntity : Entity, IAwake<int>, ISerializeToEntity, IGraphEntity, IDeserialize
    {
        public int GraphId { get; set; }
        public SerialGraphBlackboard Blackboard { get; set; }
        public StoryState State { get; set; } = StoryState.NotOpen;
        public List<int> TurnedOffOptions = new();

        [BsonIgnore]
        public SerialGraph Graph { get; set; }

        [BsonIgnore]
        public StoryHeadInfoNode HeadNode;
        [BsonIgnore]
        public StoryOpenNode OpenNode;
        [BsonIgnore]
        public StoryStartNode StartNode;
    }
}
