namespace ET
{
    /// <summary>
    /// 防止客户端突然没电
    /// </summary>
    public class AccountCheckOutTimeComponent : Entity, IAwake<long>, IDestroy
    {
        public long Timer = 0;

        public long AccountId = 0;
    }
}
