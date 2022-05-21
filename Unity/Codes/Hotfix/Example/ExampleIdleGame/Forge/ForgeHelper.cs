using System;

namespace ET
{
    public static class ForgeHelper
    {
        // 请求开始生产物品
        public static async ETTask<int> StartProduction(Scene ZoneScene, int productionConfigId)
        {
            // 判定生成配方是否存在
            if (!ForgeProductionConfigCategory.Instance.Contain(productionConfigId))
            {
                return ErrorCode.ERR_MakeConfigNotExist;
            }

            // 判定生产材料数量是否满足
            ForgeProductionConfig config = ForgeProductionConfigCategory.Instance.Get(productionConfigId);
            int materialCount = UnitHelper.GetMyUnitNumericComponent(ZoneScene.CurrentScene()).GetAsInt(config.ConsumeId);
            if (materialCount < config.ConsumeCount)
            {
                return ErrorCode.ERR_Success;
            }

            M2C_StartProduction m2CStartProduction = null;

            try
            {
                m2CStartProduction = (M2C_StartProduction)await ZoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_StartProduction() { ConfigId = productionConfigId });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            ZoneScene?.GetComponent<ForgeComponent>().AddOrUpdateProductionQueue(m2CStartProduction.ProductionProto);
            return ErrorCode.ERR_Success;
        }

        // 请求获取生产好的物品
        //public static async ETTask<int> ReceivedProductionItem(Scene ZoneScene, long productionId)
        //{

        //}
    }
}
