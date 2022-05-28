namespace ET
{
    public class ForgeComponentDeserializeSystem : DeserializeSystem<ForgeComponent>
    {
        public override void Deserialize(ForgeComponent self)
        {
            foreach (Entity entity in self.Children.Values)
            {
                self.ProductionList.Add(entity as Production);
            }
        }
    }

    [FriendClass(typeof(ForgeComponent))]
    [FriendClass(typeof(Production))]
    public static class ForgeComponentSystem
    {
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

        public static bool IsExistProductionOverQueue(this ForgeComponent self, long productionId)
        {
            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                if (self.ProductionList[i].Id == productionId
                    && self.ProductionList[i].ProductionState == (int)ProductionState.Making
                    && self.ProductionList[i].TargetTime <= TimeHelper.ServerNow())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在空闲的
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsExistFreeQueue(this ForgeComponent self)
        {
            if (self.ProductionList.Count < 2)
            {
                return true;
            }

            Production production = self.GetFreeProduction();

            if (production != null)
            {
                return true;
            }
            return false;
        }

        public static Production StartProduction(this ForgeComponent self, int configId)
        {
            Production production = self.GetFreeProduction();
            if (production == null)
            {
                production = self.AddChild<Production>();
                self.ProductionList.Add(production);
            }

            production.ConfigId = configId;
            production.ProductionState = (int)ProductionState.Making;
            production.StartTime = TimeHelper.ServerNow();
            production.TargetTime = TimeHelper.ServerNow() + (ForgeProductionConfigCategory.Instance.Get(configId).ProductionTime * 1000);

            return production;
        }

        public static Production GetFreeProduction(this ForgeComponent self)
        {
            for (int i = 0; i < self.ProductionList.Count; i++)
            {
                if (self.ProductionList[i].ProductionState == (int)ProductionState.Received)
                {
                    return self.ProductionList[i];
                }
            }
            return null;
        }
    }
}
