namespace ET
{
    [FriendClass(typeof(Production))]
    public static class ProductionSystem
    {
        public static void FromMessage(this Production self, ProductionProto productionProto)
        {
            self.Id = productionProto.Id;
            self.ConfigId = productionProto.ConfigId;
            self.ProductionState = productionProto.ProductionState;
            self.StartTime = productionProto.StartTime;
            self.TargetTime = productionProto.TargetTime;
        }

        public static bool IsMakingState(this Production self)
        {
            return self.ProductionState == (int)ProductionState.Making;
        }

        public static bool IsMakeTimeOver(this Production self)
        {
            return self.TargetTime <= TimeHelper.ServerNow();
        }

        //public static float GetRemainTimeValue(this Production self)
        //{

        //}
    }
}
