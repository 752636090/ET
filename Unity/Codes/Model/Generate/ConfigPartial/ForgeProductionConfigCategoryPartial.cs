namespace ET
{
    public partial class ForgeProductionConfigCategory
    {
        public int GetProductionConfigCount(int unitLevel)
        {
            int count = 0;

            foreach (ForgeProductionConfig config in this.list)
            {
                if (config.NeedLevel <= unitLevel)
                {
                    ++count;
                }
            }
            return count;
        }

        public ForgeProductionConfig GetProductionByLevelIndex(int unitLevel, int index)
        {
            int tempIndex = 0;

            foreach (ForgeProductionConfig config in this.list)
            {
                if (config.NeedLevel <= unitLevel && index == tempIndex)
                {
                    return config;
                }

                if (config.NeedLevel <= unitLevel)
                {
                    ++tempIndex;
                }
            }
            return null;
        }
    }
}
