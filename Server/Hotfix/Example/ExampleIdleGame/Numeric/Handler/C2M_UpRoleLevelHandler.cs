using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class C2M_UpRoleLevelHandler : AMActorLocationRpcHandler<Unit, C2M_UpRoleLevel, M2C_UpRoleLevel>
    {
        protected override async ETTask Run(Unit unit, C2M_UpRoleLevel request, M2C_UpRoleLevel response, Action reply)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            int UnitLevel = numericComponent.GetAsInt(NumericType.Level);

            PlayerLevelConfig config = PlayerLevelConfigCategory.Instance.Get(UnitLevel);

            long exp = numericComponent.GetAsLong(NumericType.Exp);

            if (exp < config.NeedExp)
            {
                response.Error = ErrorCode.ERR_ExpNotEnough;
                reply();
                return;
            }

            long newExp = exp - config.NeedExp;
            if (newExp < 0)
            {
                response.Error = ErrorCode.ERR_ExpNumError;
                reply();
                return;
            }

            numericComponent[NumericType.Exp] = newExp;
            numericComponent[NumericType.Level] = UnitLevel + 1;
            numericComponent[NumericType.AttributePoint] += 1;

            reply();
            await ETTask.CompletedTask;
        }
    }
}
