

namespace ET
{
	public static class SessionPlayerComponentSystem
	{
		public class SessionPlayerComponentDestroySystem: DestroySystem<SessionPlayerComponent>
		{
			public override void Destroy(SessionPlayerComponent self) // TODO 作业 判断是二次登录(不要执行KickPlayer)还是顶号/主动断开(要执行KickPlayer) // 最后透露（不知道是不是指这里）：根据Player的状态和Session的映射关系的替换
			{
				// 发送断线消息
				ActorLocationSenderComponent.Instance.Send(self.PlayerId, new G2M_SessionDisconnect());
				self.Domain.GetComponent<PlayerComponent>()?.Remove(self.AccountId);
			}
		}

		public static Player GetMyPlayer(this SessionPlayerComponent self)
		{
			return self.Domain.GetComponent<PlayerComponent>().Get(self.AccountId);
		}
	}
}
