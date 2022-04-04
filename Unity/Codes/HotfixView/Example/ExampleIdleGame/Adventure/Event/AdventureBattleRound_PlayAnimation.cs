using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class AdventureBattleRound_PlayAnimation : AEventAsync<EventType.AdventureBattleRound>
    {
        protected override async ETTask Run(EventType.AdventureBattleRound args)
        {
            if (!args.AttackUnit.IsAlive() || !args.TargetUnit.IsAlive())
            {
                return;
            }

            args.AttackUnit?.GetComponent<AnimatorComponent>().Play(MotionType.Attack);
            args.TargetUnit?.GetComponent<AnimatorComponent>().Play(MotionType.Hurt);

            long instanceId = args.TargetUnit.InstanceId;

            args.TargetUnit.GetComponent<GameObjectComponent>().SpriteRenderer.color = Color.red;

            await TimerComponent.Instance.WaitAsync(300);

            if (instanceId != args.TargetUnit.InstanceId) // 怕被释放掉
            {
                return;
            }

            args.TargetUnit.GetComponent<GameObjectComponent>().SpriteRenderer.color = Color.white;

            await ETTask.CompletedTask;
        }
    }
}
