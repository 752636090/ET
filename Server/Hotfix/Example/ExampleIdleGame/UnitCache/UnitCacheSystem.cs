using System;

namespace ET
{
    [ObjectSystem]
    public class UnitCacheDestroySystem : DestroySystem<UnitCache>
    {
        public override void Destroy(UnitCache self)
        {
            foreach (Entity entity in self.CacheComponentsDictionary.Values)
            {
                entity.Dispose();
            }
            self.CacheComponentsDictionary.Clear();
            self.key = null;
        }
    }

    public static class UnitCacheSystem
    {
        public static void AddOrUpdate(this UnitCache self, Entity entity)
        {
            if (entity == null || entity.IsDisposed)
            {
                return;
            }

            if (self.CacheComponentsDictionary.TryGetValue(entity.Id, out Entity oldEntity)) // entity.Id是跟unit的id保持一致的
            {
                if (entity != oldEntity)
                {
                    oldEntity.Dispose();
                }

                self.CacheComponentsDictionary.Remove(entity.Id);
            }

            self.CacheComponentsDictionary.Add(entity.Id, entity);
        }

        public static async ETTask<Entity> Get(this UnitCache self, long unitId)
        {
            Entity entity = null;
            if (!self.CacheComponentsDictionary.TryGetValue(unitId, out entity))
            {
                entity = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<Entity>(unitId, self.key);
                if (entity != null)
                {
                    self.AddOrUpdate(entity);
                }
            }
            return entity;
        }

        public static void Delete(this UnitCache self, long id)
        {
            if (self.CacheComponentsDictionary.TryGetValue(id, out Entity entity))
            {
                entity.Dispose();
                self.CacheComponentsDictionary.Remove(id);
            }
        }
    }
}
