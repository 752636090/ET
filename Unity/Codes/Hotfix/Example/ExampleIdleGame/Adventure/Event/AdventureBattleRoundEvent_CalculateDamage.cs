using ET.EventType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class AdventureBattleRoundEvent_CalculateDamage : AEventAsync<EventType.AdventureBattleRound>
    {
        protected override async ETTask Run(AdventureBattleRound args)
        {
            if (!args.AttackUnit.IsAlive() || !args.TargetUnit.IsAlive())
            {
                return;
            }

            int damage = args.AttackUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.DamageValue);

            int HP = args.TargetUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp);

            HP -= damage;

            if (HP <= 0)
            {
                HP = 0;
                args.TargetUnit.SetAlive(false);
            }
            args.TargetUnit.GetComponent<NumericComponent>().Set(NumericType.Hp, HP);

            Log.Debug($"********** {args.AttackUnit.Type}攻击造成伤害：{damage} *********");
            Log.Debug($"********** {args.TargetUnit.Type}被攻击剩余血量：{HP} *********");
            await ETTask.CompletedTask;
        }
    }
}
