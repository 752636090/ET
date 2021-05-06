using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [ObjectSystem]
    public class CarAwakeSystem : AwakeSystem<Car>
    {
        public override void Awake(Car self)
        {
            Log.Debug("Car Awake!");
        }
    }

    [ObjectSystem]
    public class CarStartSystem : StartSystem<Car>
    {
        public override void Start(Car self)
        {
            Log.Debug("Car Start");
        }
    }

    [ObjectSystem]
    public class CarUpdateSystem : UpdateSystem<Car>
    {
        public override void Update(Car self)
        {
            Log.Debug("Car Update");
        }
    }

    // LateUpdate 略

    [ObjectSystem]
    public class CarDestroySystem : DestroySystem<Car>
    {
        public override void Destroy(Car self)
        {
            Log.Debug("Car Destroy");
        }
    }

    public static class CarSystem
    {
        public static void Forward(this Car self)
        {
            Log.Debug("car forward");
        }

        public static void Backward(this Car self)
        {
            Log.Debug("car backward");
        }
    }
}
