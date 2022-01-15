namespace ET
{
    //[MessageHandler]
    public class M2C_TestActorMessageHandler : AMHandler<M2C_TestActorMessage>
    {
        protected override async ETTask Run(Session session, M2C_TestActorMessage message)
        {

            Log.DebugColor(message.Content, "FFFF00");
            await ETTask.CompletedTask;
        }
    }
}
