using System;


namespace ET
{
    public static class LoginHelper
    {
        //public static async ETTask Login(Scene zoneScene, string address, string account, string password)
        //{
        //    try
        //    {
        //        // 创建一个ETModel层的Session
        //        R2C_Login r2CLogin;
        //        Session session = null;
        //        try
        //        {
        //            session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
        //            {
        //                r2CLogin = (R2C_Login) await session.Call(new C2R_Login() { Account = account, Password = password });
        //            }
        //        }
        //        finally
        //        {
        //            session?.Dispose();
        //        }

        //        // 创建一个gate Session,并且保存到SessionComponent中
        //        Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLogin.Address));
        //        gateSession.AddComponent<PingComponent>();
        //        zoneScene.AddComponent<SessionComponent>().Session = gateSession;

        //        G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
        //            new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId});

        //        Log.Debug("登陆gate成功!");

        //        await Game.EventSystem.PublishAsync(new EventType.LoginFinish() {ZoneScene = zoneScene});
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //    }
        //}

        #region IdleGame
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            A2C_LoginAccount a2CLoginAccount = null;
            Session accountSession = null;

            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                a2CLoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount() { AccountName = account, Password = password });
            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                return a2CLoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;

            zoneScene.GetComponent<AccountInfoComponent>().Token = a2CLoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2CLoginAccount.AccountId;

            return ErrorCode.ERR_Success;
        } 
        #endregion

        #region Learn
        public static async ETTask LoginTest(Scene zoneScene, string address)
        {
            try
            {
                Session session = null;
                R2C_LoginTest r2CLoginTest = null;
                try
                {
                    session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                    {
                        r2CLoginTest = (R2C_LoginTest)await session.Call(new C2R_LoginTest() { Account = "", Password = "" });
                        Log.Debug(r2CLoginTest.key);
                        session.Send(new C2R_SayHello() { Hello = "Hello Server!" });
                        await TimerComponent.Instance.WaitAsync(2000);
                    }
                }
                finally
                {
                    session?.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        } 
        #endregion
    }
}