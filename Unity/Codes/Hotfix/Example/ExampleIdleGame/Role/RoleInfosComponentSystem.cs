namespace ET
{
    public class RoleInfosComponentSystem : DestroySystem<RoleInfosComponent>
    {
        public override void Destroy(RoleInfosComponent self)
        {
            foreach (RoleInfo roleInfo in self.RoleInfos)
            {
                roleInfo?.Dispose();
            }
            self.RoleInfos.Clear();
            self.CurrentRoleId = 0;
        }
    }
}
