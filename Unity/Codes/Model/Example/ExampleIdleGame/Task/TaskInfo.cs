namespace ET
{
    public enum TaskState
    {
        None = -1,
        Doing = 0,
        Complete = 1,
        Received = 2,
    }

    public enum TaksActionType
    {
        UpLevel = 1,
        MakeItem = 2,
        Adventure = 3,
    }

    public enum TaskProgressType
    {
        Add = 1, // 增加
        Sub = 2, // 减少
        Update = 3, // 赋值
    }

#if SERVER
    public class TaskInfo : Entity, IAwake, IAwake<int>, IDestroy, ISerializeToEntity
#else
    public class TaskInfo : Entity, IAwake, IAwake<int>, IDestroy
#endif
    {
        public int ConfigId = 0;

        public int TaskState = 0;

        public int TaskProgress = 0;
    }
}
