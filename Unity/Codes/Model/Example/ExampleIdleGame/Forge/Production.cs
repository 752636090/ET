namespace ET
{
    public enum ProductionConsumeType
    {
        IronStone = 1, // 精铁
        Fur = 2, // 皮毛
    }

    public enum ProductionState
    {
        Received = 0, // 已领取
        Making = 1, // 正在制造
    }

#if SERVER
    public class Production : Entity, IAwake, IDestroy, IDeserializeToEntity 
#else
    public class Production : Entity, IAwake, IDestroy
#endif
    {
        public long StartTime = 0;
        public long TargetTime = 0;
        public int ConfigId = 0;
        public int ProductionState = 0;
    }
}
