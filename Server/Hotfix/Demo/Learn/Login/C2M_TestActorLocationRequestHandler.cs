using System;

namespace ET
{
    //[ActorMessageHandler]
    public class C2M_TestActorLocationRequestHandler : AMActorLocationRpcHandler<Unit, C2M_TestActorLocationRequest, M2C_TestActorLocationResponse>
    {
        // 正常的第一个参数是session，Actor消息第一个参数是unit
        protected override async ETTask Run(Unit unit, C2M_TestActorLocationRequest request, M2C_TestActorLocationResponse response, Action reply)
        {
            Log.Debug(request.Content);
            response.Content = "aaaaaaaaa";

            reply();
            await ETTask.CompletedTask;
        }
    }
}
