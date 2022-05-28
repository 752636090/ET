using System;

namespace ET
{
    public class C2M_StartProductionHandler : AMActorLocationRpcHandler<Unit, C2M_StartProduction, M2C_StartProduction>
    {
        protected override async ETTask Run(Unit unit, C2M_StartProduction request, M2C_StartProduction response, Action reply)
        {
            if (!ForgeProductionConfigCategory.Instance.Contain(request.ConfigId))
            {
                response.Error = ErrorCode.ERR_MakeConfigNotExist;
                reply();
                return;
            }

            ForgeComponent forgeComponent = unit.GetComponent<ForgeComponent>();

            // 是否有空闲的制造队列
            if (!forgeComponent.IsExistFreeQueue())
            {
                response.Error = ErrorCode.ERR_NoMakeFreeQueue;
                reply();
                return;
            }

            // 制造材料是否充足
            ForgeProductionConfig config = ForgeProductionConfigCategory.Instance.Get(request.ConfigId);
            int materialCount = unit.GetComponent<NumericComponent>().GetAsInt(config.ConsumeId);
            if (materialCount < config.ConsumeCount)
            {
                response.Error = ErrorCode.ERR_MakeConsumeError;
                reply();
                return;
            }

            unit.GetComponent<NumericComponent>()[config.ConsumeId] -= config.ConsumeCount;

            Production production = forgeComponent.StartProduction(request.ConfigId);
            response.ProductionProto = production.ToMessage();

            reply();

            await ETTask.CompletedTask;
        }
    }
}
