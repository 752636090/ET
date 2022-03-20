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



    [ObjectSystem]
	public class PlayerSystem : AwakeSystem<Player, long, long>
	{
		public override void Awake(Player self, long a, long roleId)
		{
			self.AccountId = a;
			self.UnitId = roleId;
		}
	}

	public sealed class Player : Entity, IAwake<string>, IAwake<long, long>
	{
		public long AccountId { get; set; }
		
		public long UnitId { get; set; }

		public PlayerState PlayerState { get; set; }

		public Session ClientSesison { get; set; }
	}
}