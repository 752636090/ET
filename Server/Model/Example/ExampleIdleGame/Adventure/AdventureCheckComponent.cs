using System.Collections.Generic;

namespace ET
{
    public class AdventureCheckComponent : Entity, IAwake, IDestroy
    {
        public int AnimationTotalTime = 0;

        public List<long> EnemyIdList = new List<long>();
        /// <summary>
        /// 用于复用
        /// </summary>
        public List<long> CacheEnemyIdList = new List<long>();

        public SRandom Random = null;
    }
}
