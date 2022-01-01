namespace ET
{
    public class ComputerAwakeSystem : AwakeSystem<Computer>
    {
        public override void Awake(Computer self)
        {
            Log.Debug("ComputerAwake!!!!");
        }
    }

    public class ComputerUpdateSystem : UpdateSystem<Computer>
    {
        public override void Update(Computer self)
        {
            Log.Debug("Computer update!!!!");
        }
    }

    public class ComputerDestroySystem : DestroySystem<Computer>
    {
        public override void Destroy(Computer self)
        {
            Log.Debug("Computer Destroy");
        }
    }

    public static class ComputerSystem
    {
        public static void Start(this Computer self)
        {
            Log.Debug("Computer Start!!!!!!!");
            self.GetComponent<PCCaseComponent>().StartPower();
            self.GetComponent<MonitorsComponent>().Display();

            #region ZoneScene DemainScene 讲解
            //self.ZoneScene(); // 客户端才能编译通过
            //self.DomainScene();
            //var domain = self.Domain; // 不建议直接这么获取
            #endregion
        }
    }
}
