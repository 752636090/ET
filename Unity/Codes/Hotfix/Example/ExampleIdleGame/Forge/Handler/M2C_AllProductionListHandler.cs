namespace ET
{
    public class M2C_AllProductionListHandler : AMHandler<M2C_AllProductionList>
    {
        protected override void Run(Session session, M2C_AllProductionList message)
        {
            for (int i = 0; i < message.ProductionProtoList.Count; i++)
            {
                session.ZoneScene().GetComponent<ForgeComponent>().AddOrUpdateProductionQueue(message.ProductionProtoList[i]);
            }
        }
    }
}
