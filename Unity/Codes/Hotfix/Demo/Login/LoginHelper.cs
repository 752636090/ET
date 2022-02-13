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
                password = MD5Helper.StringMD5(password);
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
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>(); // 配合Kcp OnAccept挂的SessionIdleCheckerComponent

            zoneScene.GetComponent<AccountInfoComponent>().Token = a2CLoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2CLoginAccount.AccountId;

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetServerInfos(Scene zoneScene)
        {
            A2C_GetServerInfos a2CGetServerInfos = null;

            try
            {
                a2CGetServerInfos = (A2C_GetServerInfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetServerInfos()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token
                });

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2CGetServerInfos.Error != ErrorCode.ERR_Success)
            {
                return a2CGetServerInfos.Error;
            }

            foreach (ServerInfoProto serverInfoProto in a2CGetServerInfos.ServerInfosList)
            {
                ServerInfo serverInfo = zoneScene.GetComponent<ServerInfosComponent>().AddChild<ServerInfo>();
                serverInfo.FromMessage(serverInfoProto);
                zoneScene.GetComponent<ServerInfosComponent>().Add(serverInfo);
            }

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetRoles(Scene zoneScene)
        {
            A2C_GetRoles a2cGetRoles = null;

            try
            {
                a2cGetRoles = (A2C_GetRoles)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2cGetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2cGetRoles.Error.ToString());
                return a2cGetRoles.Error;
            }

            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Clear();
            foreach (RoleInfoProto roleInfoProto in a2cGetRoles.RoleInfo)
            {
                RoleInfo roleInfo = zoneScene.GetComponent<RoleInfosComponent>().AddChild<RoleInfo>();
                roleInfo.FromMessage(roleInfoProto);
                zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(roleInfo);
            }

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> CreateRole(Scene zoneScene, string name)
        {
            A2C_CreateRole a2CCreateRole = null;

            try
            {
                a2CCreateRole = (A2C_CreateRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    Name = name,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2CCreateRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CCreateRole.Error.ToString());
                return a2CCreateRole.Error;
            }
            RoleInfo newRoleInfo = zoneScene.GetComponent<RoleInfosComponent>().AddChild<RoleInfo>();
            newRoleInfo.FromMessage(a2CCreateRole.RoleInfo);

            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(newRoleInfo);

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> DeleteRole(Scene zoneScene)
        {
            A2C_DeleteRole a2CDeleteRoles = null;

            try
            {
                a2CDeleteRoles = (A2C_DeleteRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteRole()
                {
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    RoleInfoId = zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2CDeleteRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CDeleteRoles.Error.ToString());
                return a2CDeleteRoles.Error;
            }

            int index = zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.FindIndex((info) => { return info.Id == a2CDeleteRoles.DeletRoleInfoId; });

            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.RemoveAt(index);

            await ETTask.CompletedTask;
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