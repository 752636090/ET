using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ET
{
    //[ComponentOf]
    //public class SerialGraphsDataComponent : Entity, IAwake, IDestroy
    //{
    //    public Dictionary<int, List<SGSaveData>> SaveDict = new();

    //    // 事件暂停阶段再继续的条件按类型划分, 每项对应一组事件
    //    [BsonIgnore]
    //    public UnOrderMultiMap<Type, SerialPort> HoldNodes = ObjectPool.Instance.Fetch<UnOrderMultiMap<Type, SerialPort>>();
    //}

    //public class SGSaveData : IReset, IDisposable
    //{


    //    // 事件Hold(需要参与判断的)列表
    //    public List<int> Holds = new();
    //    // 结果库
    //    public List<int> Results = new();

    //    public UnOrderMultiMap<Type, int> OtherNodes

    //    public void Reset()
    //    {
    //        Holds.Clear();
    //        Results.Clear();
    //    }

    //    public static SGSaveData Create()
    //    {
    //        return ObjectPool.Instance.Fetch<SGSaveData>();
    //    }

    //    public void Dispose()
    //    {
    //        ObjectPool.Instance.Recycle(this);
    //    }
    //}
}
