namespace ET
{
    [FriendClass(typeof(Player))]
    public class PlayerSystem : AwakeSystem<Player, long, long>
	{
		public override void Awake(Player self, long a, long roleId)
		{
			self.AccountId = a;
			self.UnitId = roleId;
		}
	}

    public class PlayerDestroySystem : DestroySystem<Player>
    {
        public override void Destroy(Player self)
        {
            self.AccountId = 0;
            self.UnitId = 0;
            self.ChatInfoInstanceId = 0;
            self.PlayerState = PlayerState.Disconnect;
            self.ClientSesison?.Dispose();
        }
    }
}