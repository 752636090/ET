using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    // 猜的
    public partial class PlayerNumericConfigCategory
    {
        // 猜的
        private List<PlayerNumericConfig> showConfigList;

        // 整个方法都是自己猜的
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            showConfigList = new List<PlayerNumericConfig>(4);
            foreach (PlayerNumericConfig config in list)
            {
                if (config.isNeedShow == 1)
                {
                    showConfigList.Add(config);
                }
            }
        }

        public int GetShowConfigCount()
        {
            // 整个方法都是自己猜的
            return showConfigList.Count;
        }

        public PlayerNumericConfig GetConfigByIndex(int index)
        {
            // 整个方法都是自己猜的
            if (index < 0 || index >= showConfigList.Count)
            {
                Log.Error("法克");
                return showConfigList[0];
            }

            return showConfigList[index];
        }
    }
}
