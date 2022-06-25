namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class RankInfo : Entity, IAwake, IDestroy
    {
        public long UnitId;
        public string Name;
        public int Count;
    }
}
