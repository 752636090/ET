namespace ET
{
    public static class DisconnectHelper
    {
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            long instanceId = self.InstanceId;

            await TimerComponent.Instance.WaitAsync(1000);

            if (self.InstanceId != instanceId) // 说明已经Dispose，InstanceId归零了
            {
                return;
            }
            self.Dispose();
        }

        /// <param name="player">网关上对游戏角色的映射</param>
        public static async ETTask KickPlayer(Player player)
        {
            if (player == null || player.IsDisposed)
            {
                return;
            }
            long instanceId = player.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.Account.GetHashCode()))
            {
                if (player.IsDisposed || instanceId != player.InstanceId) // 防多次进入
                {
                    return;
                }

                switch (player.PlayerState)
                {
                    case PlayerState.Disconnect:
                        break;
                    case PlayerState.Gate:
                        break;
                    case PlayerState.Game:
                        // TODO 通知游戏逻辑服下线Unit角色逻辑，并将数据存入数据库    还要通知账号服务器移除

                        break;
                }

                player.PlayerState = PlayerState.Disconnect;
                player.DomainScene().GetComponent<PlayerComponent>()?.Remove(player.Account);
                player?.Dispose();
                await TimerComponent.Instance.WaitAsync(300); // 为了防止Player身上有异步操作
            }
        }
    }
}
