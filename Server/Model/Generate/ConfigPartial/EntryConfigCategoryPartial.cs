using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public partial class EntryConfigCategory
    {
        private Dictionary<int, MultiMap<int, EntryConfig>> EntryConfigDict = new Dictionary<int, MultiMap<int, EntryConfig>>();

        public override void AfterEndInit()
        {
            base.AfterEndInit();

            foreach (EntryConfig config in this.dict.Values)
            {
                if (!this.EntryConfigDict.ContainsKey(config.EntryType))
                {
                    this.EntryConfigDict.Add(config.EntryType, new MultiMap<int, EntryConfig>());
                }
                this.EntryConfigDict[config.EntryType].Add(config.EntryLevel, config);
            }
        }

        public EntryConfig GetRandomEntryConfigByLevel(int entryType, int level)
        {
            if (!this.EntryConfigDict.ContainsKey(entryType))
            {
                return null;
            }

            MultiMap<int, EntryConfig> entryConfigMap = this.EntryConfigDict[entryType];
            if (!entryConfigMap.ContainsKey(level))
            {
                return null;
            }

            List<EntryConfig> configList = entryConfigMap[level];
            int index = RandomHelper.RandomNumber(0, configList.Count);
            return configList[index];
        }
    }
}
