using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public partial class ItemConfigCategory
    {
        public int GetMaxAddHp()
        {
            int maxValue = (this.GetOne() as ItemConfig).AddHp;
            foreach (var info in this.dict)
            {
                ItemConfig config = info.Value as ItemConfig;
                if (maxValue < config.AddHp)
                {
                    maxValue = config.AddHp;
                }
            }
            return maxValue;
        }
    }
}
