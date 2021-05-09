using System;
using ETModel;

namespace ETHotfix
{
    public static class LoginHelper
    {
        public static async ETVoid OnLoginAsync(string account, string password)
        {
            try
            {
                // 创建一个ETModel层的Session
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);
				
                // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_Login r2CLogin = (R2C_Login) await realmSession.Call(new C2R_Login() { Account = account, Password = password });
                realmSession.Dispose();

                if (r2CLogin.Error != ErrorCode.ERR_Success)
                {
                    ETModel.Log.Error("登录失败");
                    return;
                }

                // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);
                ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;
				
                // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
                Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
				
                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

                Log.Info("登陆gate成功!");

                // 创建Player
                Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
                PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
                playerComponent.MyPlayer = player;

                Game.EventSystem.Run(EventIdType.LoginFinish);

                // 测试消息有成员是class类型
                G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());

                C2R_TestHelloMsg testHelloMsg = new C2R_TestHelloMsg()
                {
                    SayMessage = "后端你好"
                };
                SessionComponent.Instance.Session.Send(testHelloMsg);

                C2A_Login c2A_Login = new C2A_Login()
                {
                    //TestInfo = 
                };
                c2A_Login.TestIds.Add(1);
                c2A_Login.TestIds.Add(2);
                c2A_Login.TestIds.Add(3);

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 

        public static async ETVoid OnRegisterAsync(string account, string password)
        {
            try
            {
                // 创建一个ETModel层的Session
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

                // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_Register r2CRegister = (R2C_Register)await realmSession.Call(new C2R_Register() { Account = account, Password = password });
                realmSession.Dispose();

                if (r2CRegister.Error != ErrorCode.ERR_Success)
                {
                    ETModel.Log.Error("注册失败！");
                    return;
                }
                ETModel.Log.Info("注册成功！");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 
    }
}