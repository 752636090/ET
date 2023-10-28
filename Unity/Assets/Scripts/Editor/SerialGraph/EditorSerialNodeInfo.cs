using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ET
{
    public class EditorSerialNodeInfo : IEquatable<EditorSerialNodeInfo>
    {
        public string Remark;
        [BsonSerializer(typeof(Vector2Serializer))]
        public Vector2 Position;

        public bool Equals(EditorSerialNodeInfo other)
        {
            return Remark == other.Remark && Position == other.Position;
        }
    }

    public class Vector2Serializer : SerializerBase<Vector2>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector2 value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("x");
            context.Writer.WriteDouble(value.x);
            context.Writer.WriteName("y");
            context.Writer.WriteDouble(value.y);
            context.Writer.WriteEndDocument();
        }

        public override Vector2 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            context.Reader.ReadName();
            float x = (float)context.Reader.ReadDouble();
            context.Reader.ReadName();
            float y = (float)context.Reader.ReadDouble();
            context.Reader.ReadEndDocument();
            return new Vector2(x, y);
        }
    }

    public class Vector2SerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Vector2)) return new Vector2Serializer();
            return null;
        }
    }
}
