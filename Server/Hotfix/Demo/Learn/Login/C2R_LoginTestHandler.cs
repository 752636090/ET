using System;

namespace ET
{
    /// <summary>
    /// 需要回复的协议的handler继承AMRpcHandler
    /// </summary>
    [MessageHandler]
    public class C2R_LoginTestHandler : AMRpcHandler<C2R_LoginTest, R2C_LoginTest>
    {
        protected override async ETTask Run(Session session, C2R_LoginTest request, R2C_LoginTest response, Action reply)
        {
            response.key = "1111111111111111111";
            reply();
            await ETTask.CompletedTask;
        }
    }
}
