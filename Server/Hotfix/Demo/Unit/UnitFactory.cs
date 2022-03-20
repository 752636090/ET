using System;
using UnityEngine;

namespace ET
{
    public static class UnitFactory
    {
        public static Unit Create(Scene scene, long id, UnitType unitType)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                {
                    Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1001);
                    //unit.AddComponent<MoveComponent>();
                    //unit.Position = new Vector3(-10, 0, -10);
			
                    NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                    //numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
                    //numericComponent.Set(NumericType.AOI, 15000); // 视野15米
                    UnitConfig unitConfig = UnitConfigCategory.Instance.Get(1001);
                    numericComponent.SetNoEvent(NumericType.Postion, unitConfig.Position); // 补充：配表中浮点数要写成万分比(防止误差)，直接填浮点数是小白做法
                    numericComponent.SetNoEvent(NumericType.Height, unitConfig.Height);
                    numericComponent.SetNoEvent(NumericType.Weight, unitConfig.Weight);
                    
                    unitComponent.Add(unit);
                    //// 加入aoi
                    //unit.AddComponent<AOIEntity, int, Vector3>(9 * 1000, unit.Position);
                    return unit;
                }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }
    }
}