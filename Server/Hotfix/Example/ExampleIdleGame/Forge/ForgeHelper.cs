namespace ET
{
    [FriendClass(typeof(ForgeComponent))]
    public static class ForgeHelper
    {
        public static void SyncAllProduction(Unit unit)
        {
            ForgeComponent forgeComponent = unit.GetComponent<ForgeComponent>();

            M2C_AllProductionList m2CAllProductionList = new M2C_AllProductionList();
            for (int i = 0; i < forgeComponent.ProductionList.Count; i++)
            {
                m2CAllProductionList.ProductionProtoList.Add(forgeComponent.ProductionList[i].ToMessage());
            }
            MessageHelper.SendToClient(unit, m2CAllProductionList);
        }
    }
}
