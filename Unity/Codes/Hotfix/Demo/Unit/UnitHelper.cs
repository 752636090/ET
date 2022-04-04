using System;

namespace ET
{
    public static class UnitHelper
    {
        public static Unit GetMyUnitFromZoneScene(Scene zoneScene)
        {
            PlayerComponent playerComponent = zoneScene.GetComponent<PlayerComponent>();
            Scene currentScene = zoneScene.GetComponent<CurrentScenesComponent>().Scene;
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
        
        public static Unit GetMyUnitFromCurrentScene(Scene currentScene)
        {
            PlayerComponent playerComponent = currentScene.Parent.Parent.GetComponent<PlayerComponent>();
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }

        public static NumericComponent GetMyUnitNumericComponent(Scene currentScene)
        {
            PlayerComponent playerComponent = currentScene?.ZoneScene().GetComponent<PlayerComponent>();
            if (null == playerComponent)
            {
                return null;
            }
            return currentScene.GetComponent<UnitComponent>()?.Get(playerComponent.MyId)?.GetComponent<NumericComponent>();
        }

        public static bool IsAlive(this Unit unit)
        {
            if (unit == null || unit.IsDisposed)
            {
                return false;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (null == numericComponent)
            {
                return false;
            }

            return numericComponent.GetAsInt(NumericType.IsAlive) == 1;
        }

        public static void SetAlive(this Unit unit, bool isAlive)
        {
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (null == numericComponent)
            {
                return;
            }

            numericComponent.Set(NumericType.IsAlive, isAlive ? 1 : 0);
        }
    }
}