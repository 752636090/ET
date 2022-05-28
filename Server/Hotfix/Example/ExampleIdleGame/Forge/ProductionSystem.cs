namespace ET
{
    [FriendClass(typeof(Production))]
    public static class ProductionSystem
    {
        public static ProductionProto ToMessage(this Production self)
        {
            ProductionProto productionProto = new ProductionProto();
            productionProto.Id = self.Id;
            productionProto.ConfigId = self.ConfigId;
            productionProto.StartTime = self.StartTime;
            productionProto.TargetTime = self.TargetTime;
            productionProto.ProductionState = self.ProductionState;
            return productionProto;
        }

        public static void Reset(this Production self)
        {
            self.ConfigId = 0;
            self.ProductionState = (int)ProductionState.Received;
            self.TargetTime = 0;
        }
    }
}
