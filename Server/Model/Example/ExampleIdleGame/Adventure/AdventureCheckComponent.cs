using System.Collections.Generic;

namespace ET
{
    public class AdventureCheckComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<int, int> EnemyHpDictionary = new Dictionary<int, int>();
        public int Round = 0;
        public int AnimationTotalTime = 0;
        public int MonsterTotalDamage = 0;
        public int UnitTotalDamage = 0;
        public int TotalMonsterHp = 0;
    }
}
