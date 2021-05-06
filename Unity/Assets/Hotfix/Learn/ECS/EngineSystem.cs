using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public static class EngineSystem
    {
        public static void Driver(this EngineComponent self)
        {
            Log.Debug("engine is driving!");
        }
    }
}
