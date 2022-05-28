using System;
using System.Collections.Generic;

namespace ET
{
    public class C2M_EndGameLevelHandler : AMActorLocationRpcHandler<Unit, C2M_EndGameLevel, M2C_EndGameLevel>
    {
        protected override async ETTask Run(Unit unit, C2M_EndGameLevel request, M2C_EndGameLevel response, Action reply)
        {
            // 检测关卡信息是否正常
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            int levelId = numericComponent.GetAsInt(NumericType.AdventureState);
            if (levelId == 0 || !BattleLevelConfigCategory.Instance.Contain(levelId))
            {
                response.Error = ErrorCode.ERR_AdventureLevelIdError;
                reply();
                return;
            }

            // 检测上传的回合数是否正常
            if (request.Round <= 0)
            {
                response.Error = ErrorCode.ERR_AdventureRoundError;
                reply();
                return;
            }

            // 战斗失败直接进入垂死状态
            if (request.BattleResult == (int)BattleRoundResult.LoseBattle)
            {
                numericComponent.Set(NumericType.DyingState, 1);
                numericComponent.Set(NumericType.AdventureState, 0);
                reply();
                return;
            }

            if (request.BattleResult != (int)BattleRoundResult.WinBattle)
            {
                response.Error = ErrorCode.ERR_AdventureResultError;
                reply();
                return;
            }

            // 检测战斗胜利结果是否正常
            if (!unit.GetComponent<AdventureCheckComponent>().CheckBattleWinResult(request.Round))
            {
                response.Error = ErrorCode.ERR_AdventureWinResultError;
                reply();
                return;
            }

            numericComponent.Set(NumericType.AdventureState, 0);
            reply();

            // 战斗胜利增加经验值
            numericComponent[NumericType.Exp] += BattleLevelConfigCategory.Instance.Get(levelId).RewardExp;

            // 背包测试代码 随机下发闯关成功的奖励（30个）
            for (int i = 0; i < 30; i++)
            {
                if (!BagHelper.AddItemByConfigId(unit, RandomHelper.RandomNumber(1002, 1018)))
                {
                    Log.Error("增加背包物品失败");
                }
            }

            // 猜的，增加精铁和皮革
            numericComponent[NumericType.IronStone] += 1000;
            numericComponent[NumericType.Fur] += 1000;
            
            await ETTask.CompletedTask;
        }
    }
}
