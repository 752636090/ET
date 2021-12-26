using UnityEngine;

namespace ET
{
    public static class UnitFactory
    {
        public static Unit Create(Entity domain, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
	        
	        unit.AddComponent<MoveComponent>();
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
	        for (int i = 0; i < unitInfo.Ks.Count; ++i)
	        {
		        numericComponent.Set((NumericType)unitInfo.Ks[i], unitInfo.Vs[i]);
	        }

	        unit.AddComponent<ObjectWait>();

	        unit.AddComponent<XunLuoPathComponent>();
	        
	        Game.EventSystem.Publish(new EventType.AfterUnitCreate() {Unit = unit}).Coroutine();
            return unit;
        }

        #region Learn
        public static Unit CreatePlayer(Entity domain, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Player;
            unit.AddComponent<MoveComponent>();

            unitComponent.Add(unit);
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit }).Coroutine();
            return unit;
        }

        public static Unit CreateNpc(Entity domain, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Npc;

            unitComponent.Add(unit);
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit }).Coroutine();
            return unit;
        } 

        public static Unit CreateMonster(Entity domain, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Monster;

            unit.AddComponent<MoveComponent>();
            unit.AddComponent<XunLuoPathComponent>();

            unitComponent.Add(unit);
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit }).Coroutine();
            return unit;
        } 
        #endregion
    }
}
