using ET.EventType;

namespace ET
{
    [Timer(TimerType.MakeQueueOver)]
    public class MakeQueueOverTimer : ATimer<ForgeComponent>
    {
        public override void Run(ForgeComponent t)
        {
            Game.EventSystem.Publish(new MakeQueueOver() { ZoneScene = t.ZoneScene() });
        }
    }

    public class ForgeComponentAwakeSystem : AwakeSystem<ForgeComponent>
    {
        public override void Awake(ForgeComponent self)
        {

        }
    }

    public class ForgeComponentDestroySystem : DestroySystem<ForgeComponent>
    {
        public override void Destroy(ForgeComponent self)
        {
            foreach (Production production in self.ProductionList)
            {
                production?.Dispose();
            }

            ForeachHelper.Foreach<long, long>(self.ProductionTimeDict, (key, value) =>
            {
                TimerComponent.Instance?.Remove(ref value);
            });
        }
    }

    [FriendClass(typeof(ForgeComponent))]
    [FriendClass(typeof(Production))]
    public static class ForgeComponentSystem
    {
        public static bool IsExistMakeQueueOver(this ForgeComponent self)
        {
            bool isCanReceive = false;

            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                Production production = self.ProductionList[i];
                if (production.IsMakingState() && production.IsMakeTimeOver())
                {
                    isCanReceive = true;
                    break;
                }
            }
            return isCanReceive;
        }

        public static void AddOrUpdateProductionQueue(this ForgeComponent self, ProductionProto productionProto)
        {
            Production production = self.GetProductionById(productionProto.Id);

            if (production == null)
            {
                production = self.AddChild<Production>();
                self.ProductionList.Add(production);
            }
            production.FromMessage(productionProto);

            if (self.ProductionTimeDict.TryGetValue(production.Id, out long timeId))
            {
                TimerComponent.Instance.Remove(ref timeId);
                self.ProductionTimeDict.Remove(production.Id);
            }

            if (production.IsMakingState() && !production.IsMakeTimeOver())
            {
                Log.Debug($"启动一个定时器!!!!:{production.TargetTime}");
                timeId = TimerComponent.Instance.NewOnceTimer(production.TargetTime, TimerType.MakeQueueOver, self);
                self.ProductionTimeDict.Add(production.Id, timeId);
            }

            Game.EventSystem.Publish(new MakeQueueOver() { ZoneScene = self.ZoneScene() });
        }

        public static Production GetProductionById(this ForgeComponent self, long productionId)
        {
            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                if (self.ProductionList[i].Id == productionId)
                {
                    return self.ProductionList[i];
                }
            }
            return null;
        }

        public static Production GetProductionByIndex(this ForgeComponent self, int index)
        {
            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                if (index == i)
                {
                    return self.ProductionList[i];
                }
            }
            return null;
        }

        public static int GetMakingProductionQueueCount(this ForgeComponent self)
        {
            int count = 0;
            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                Production production = self.ProductionList[i];
                if (production.ProductionState == (int)ProductionState.Making)
                {
                    ++count;
                }
            }
            return count;
        }
    }
}
