using System;
using System.Collections.Generic;

namespace ET
{
    //[EntitySystemOf(typeof(SerialGraphsDataComponent))]
    //[FriendOf(typeof(SerialGraphsDataComponent))]
    //public static partial class SerialGraphsDataComponentSystem
    //{
    //    [EntitySystem]
    //    public static void Awake(this SerialGraphsDataComponent self)
    //    {

    //    }

    //    [EntitySystem]
    //    public static void Destroy(this SerialGraphsDataComponent self)
    //    {
    //        foreach (List<SGSaveData> saveDatas in self.SaveDict.Values)
    //        {
    //            foreach (SGSaveData saveData in saveDatas)
    //            {
    //                saveData.Dispose(); 
    //            }
    //            saveDatas.Clear();
    //            ObjectPool.Instance.Recycle(saveDatas);
    //        }
    //        self.SaveDict.Clear();
    //    }
    //}
}
