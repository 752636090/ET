namespace ET
{
    [EntitySystemOf(typeof(HappenNodeWaitingComponent))]
    [FriendOfAttribute(typeof(ET.HappenNodeWaitingComponent))]
    public static partial class HappenNodeWaitingComponentSystem
    {
        [EntitySystem]
        private static void Awake(this HappenNodeWaitingComponent self, HappenNode node)
        {
            self.Node = node;
            self.StartWait(false).Coroutine();
        }

        [EntitySystem]
        private static void Deserialize(this HappenNodeWaitingComponent self)
        {
            self.StartWait(true).Coroutine();
        }

        [EntitySystem]
        private static void Destroy(this HappenNodeWaitingComponent self)
        {
            self.CancellationToken?.Cancel();
        }

        private static async ETTask StartWait(this HappenNodeWaitingComponent self, bool waitFrame)
        {
            if (waitFrame)
            {
                await self.Root().GetComponent<TimerComponent>().WaitFrameAsync();
            }
            self.CancellationToken = new();
            await SerialGraphEventSystem.Instance.HappenStartWait(self.Parent, self.Node, self.CancellationToken);
            if (self.CancellationToken.IsCancel())
            {
                return;
            }
            self.CancellationToken = null;
            self.Dispose();
        }
    }
}
