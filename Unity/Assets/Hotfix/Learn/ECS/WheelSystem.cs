using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [ObjectSystem]
    public class WheelComponentAwakeSystem : AwakeSystem<WheelComponent>
    {
        public override void Awake(WheelComponent self)
        {
            Log.Debug("Car WheelComponent Awake");
        }
    }

    public static class WheelSystem
    {
        public static void Roll(this WheelComponent self, float speed)
        {
            Log.Debug($"wheel speed {speed}");
            Log.Debug("wheel start roll!");
        }
    }
}
