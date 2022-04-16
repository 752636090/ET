namespace ET
{
    /// <summary>
    /// 不需要回复的协议的handler继承AMHandler
    /// </summary>
    [MessageHandler]
    public class C2R_SayHelloHandler : AMHandler<C2R_SayHello>
    {
        protected override void Run(Session session, C2R_SayHello message)
        {
            Log.Debug(message.Hello);

            session.Send(new R2C_SayGoodBye() { GoodBye = "goodBye" }); // 这是主动发的不算是回复
        }
    }
}
