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
}