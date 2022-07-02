namespace ET
{
    #region IdleGame
	public enum PlayerState
    {
		Disconnect,
		Gate,
		Game,
    }
    #endregion




	public sealed class Player : Entity, IAwake<string>, IAwake<long, long>, IDestroy
	{
		public long AccountId { get; set; }
		
		public long UnitId { get; set; }

		public PlayerState PlayerState { get; set; }

		public Session ClientSesison { get; set; }

		public long ChatInfoInstanceId { get; set; }
	}
}