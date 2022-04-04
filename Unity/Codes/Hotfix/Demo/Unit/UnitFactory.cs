﻿using UnityEngine;

namespace ET
{
    public static class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        //unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
	        //unit.Forward = new Vector3(unitInfo.ForwardX, unitInfo.ForwardY, unitInfo.ForwardZ);
	        
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
	        for (int i = 0; i < unitInfo.Ks.Count; ++i)
	        {
		        numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i]);
	        }
	        
	        //unit.AddComponent<MoveComponent>();
	        //if (unitInfo.MoveInfo != null)
	        //{
		       // if (unitInfo.MoveInfo.X.Count > 0)
		       // {
			      //  using (ListComponent<Vector3> list = ListComponent<Vector3>.Create())
			      //  {
				     //   list.Add(unit.Position);
				     //   for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i)
				     //   {
					    //    list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
				     //   }

				     //   unit.MoveToAsync(list).Coroutine();
			      //  }
		       // }
	        //}

	        unit.AddComponent<ObjectWait>();

	        //unit.AddComponent<XunLuoPathComponent>();
	        
	        Game.EventSystem.PublishAsync(new EventType.AfterUnitCreate() {Unit = unit});
            return unit;
        }

        #region IdleGame
        public static async ETTask<Unit> CreateMonster(Scene currentScene, int configId)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), configId);
            unitComponent.Add(unit);

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            numericComponent.SetNoEvent(NumericType.DamageValue, unit.Config.DamageValue);
            numericComponent.SetNoEvent(NumericType.MaxHp, unit.Config.MaxHp);
            numericComponent.SetNoEvent(NumericType.Hp, unit.Config.MaxHp);

            unit.AddComponent<ObjectWait>();

            await Game.EventSystem.PublishAsync(new EventType.AfterUnitCreate() { Unit = unit });
            return unit;
        }
        #endregion

        #region Learn
        public static Unit CreatePlayer(Entity domain, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Player;
            unit.AddComponent<MoveComponent>();

            unitComponent.Add(unit);
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit });
            return unit;
        }

        public static Unit CreateNpc(Entity domain, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = domain.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Npc;

            unitComponent.Add(unit);
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit });
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
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { Unit = unit });
            return unit;
        } 
        #endregion
    }
}
