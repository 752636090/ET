using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public partial class BattleLevelConfigCategory
    {
        public BattleLevelConfig GetConfigByIndex(int index)
        {
            if (index < 0 || index >= this.list.Count)
            {
                Log.Error($"Get BattleLevelConfig Index Error: {index}");
                return null;
            }
            return this.list[index];
        }
    }
}
