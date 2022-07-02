using System;

namespace ET
{
    [FriendClassAttribute(typeof(ET.ChatInfoUnitsComponent))]
    [FriendClassAttribute(typeof(ET.ChatInfoUnit))]
    public class C2Chat_SendChatInfoHandler : AMActorRpcHandler<ChatInfoUnit, C2Chat_SendChatInfo, Chat2C_SendChatInfo>
    {
        // 注意：ChatInfoUnit加上MailBoxComponent才能作为消息处理的实体
        protected override async ETTask Run(ChatInfoUnit chatInfoUnit, C2Chat_SendChatInfo request, Chat2C_SendChatInfo response, Action reply)
        {
            if (string.IsNullOrEmpty(request.ChatMessage))
            {
                response.Error = ErrorCode.ERR_ChatMessageEmpty;
                reply();
                return;
            }

            ChatInfoUnitsComponent chatInfoUnitsComponent = chatInfoUnit.DomainScene().GetComponent<ChatInfoUnitsComponent>();
            foreach (ChatInfoUnit otherUnit in chatInfoUnitsComponent.ChatInfoUnitDict.Values) // 作业：解决性能压力
            {
                MessageHelper.SendActor(otherUnit.GateSessionActorId, new Chat2C_NoticeChatInfo()
                {
                    Name = chatInfoUnit.Name, ChatMessage = request.ChatMessage
                });
            }

            reply();
            await ETTask.CompletedTask;
        }
    }
}
