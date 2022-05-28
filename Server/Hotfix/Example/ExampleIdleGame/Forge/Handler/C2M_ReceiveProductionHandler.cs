using System;

namespace ET
{
    [FriendClass(typeof(Production))]
    public class C2M_ReceiveProductionHandler : AMActorLocationRpcHandler<Unit, C2M_ReceiveProduction, M2C_ReceiveProduction>
    {
        protected override async ETTask Run(Unit unit, C2M_ReceiveProduction request, M2C_ReceiveProduction response, Action reply)
        {
            ForgeComponent forgeComponent = unit.GetComponent<ForgeComponent>();
            BagComponent bagComponent = unit.GetComponent<BagComponent>();

            if (bagComponent.IsMaxLoad())
            {
                response.Error = ErrorCode.ERR_BagMaxLoad;
                reply();
                return;
            }

            if (!forgeComponent.IsExistProductionOverQueue(request.ProductionId))
            {
                response.Error = ErrorCode.ERR_NoMakeQueueOver;
                reply();
                return;
            }

            Production production = forgeComponent.GetProductionById(request.ProductionId);

            ForgeProductionConfig config = ForgeProductionConfigCategory.Instance.Get(production.ConfigId);
            if (!BagHelper.AddItemByConfigId(unit, config.ItemConfigId))
            {
                response.Error = ErrorCode.ERR_AddBagItemError;
                reply();
                return;
            }
            Game.EventSystem.Publish(new EventType.MakeProductionOver() { Unit = unit, ProductionConfigId = production.ConfigId });
            production.Reset();

            response.ProductionProto = production.ToMessage();
            reply();

            await ETTask.CompletedTask;
        }
    }
}
