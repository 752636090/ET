using System;

namespace ET
{
    public class C2A_LoginAccountIGHandler : AMRpcHandler<C2A_LoginAccountIG, A2C_LoginAccountIG>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccountIG request, A2C_LoginAccountIG response, Action reply)
        {


            await ETTask.CompletedTask;
        }
    }
}
