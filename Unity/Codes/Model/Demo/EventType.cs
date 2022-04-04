using UnityEngine;

namespace ET
{
    namespace EventType
    {
        public struct AppStart
        {
        }

        public struct SceneChangeStart
        {
            public Scene ZoneScene;
        }
        
        
        public struct SceneChangeFinish
        {
            public Scene ZoneScene;
            public Scene CurrentScene;
        }

        public struct ChangePosition
        {
            public Unit Unit;
        }

        public struct ChangeRotation
        {
            public Unit Unit;
        }

        public struct PingChange
        {
            public Scene ZoneScene;
            public long Ping;
        }
        
        public struct AfterCreateZoneScene
        {
            public Scene ZoneScene;
        }
        
        public struct AfterCreateCurrentScene
        {
            public Scene CurrentScene;
        }
        
        public struct AfterCreateLoginScene
        {
            public Scene LoginScene;
        }

        public struct AppStartInitFinish
        {
            public Scene ZoneScene;
        }

        public struct LoginFinish
        {
            public Scene ZoneScene;
        }

        public struct LoadingBegin
        {
            public Scene Scene;
        }

        public struct LoadingFinish
        {
            public Scene Scene;
        }

        public struct EnterMapFinish
        {
            public Scene ZoneScene;
        }

        public struct AfterUnitCreate
        {
            public Unit Unit;
        }
        
        public struct MoveStart
        {
            public Unit Unit;
        }

        public struct MoveStop
        {
            public Unit Unit;
        }

        public struct UnitEnterSightRange
        {
        }

        public struct InstallComputer
        {
            public Computer Computer;
        }

        public struct StartGameLevel // 抄来的，好像没地方用
        {
            public Scene ZoneScene;
        }

        public struct AdventureBattleRound
        {
            public Scene ZoneScene;
            public Unit AttackUnit;
            public Unit TargetUnit;
        }

        public struct UnitBattleView
        {
            public Scene ZoneScene;
            public Unit AttackUnit;
            public Unit TargetUnit;
        }

        public struct AdventureBattleOver
        {
            public Scene ZoneScene;
            public Unit WinUnit;
        }

        public struct AdventureBattleReport
        {
            public int Round;
            public BattleRoundResult BattleRoundResult;
            public Scene ZoneScene;
        }

        public struct AdventureRoundReset
        {
            public Scene ZoneScene;
        }
    }
}