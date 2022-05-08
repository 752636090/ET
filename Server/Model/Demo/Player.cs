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




	public sealed class Player : Entity, IAwake<string>, IAwake<long, long>
	{
		public long AccountId { get; set; }
		
		public long UnitId { get; set; }

		public PlayerState PlayerState { get; set; }

		public Session ClientSesison { get; set; }
	}
}