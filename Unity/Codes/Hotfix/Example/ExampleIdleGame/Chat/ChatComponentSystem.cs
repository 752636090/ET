namespace ET
{
    [FriendClassAttribute(typeof(ET.ChatComponent))]
    public static class ChatComponentSystem
    {


        public static int GetChatMessageCount(this ChatComponent self)
        {
            return self.ChatMessageQueue.Count;
        }

        public static ChatInfo GetChatMessageByIndex(this ChatComponent self, int index)
        {
            int tempIndex = 0;
            foreach (ChatInfo chatInfo in self.ChatMessageQueue)
            {
                if (tempIndex == index)
                {
                    return chatInfo;
                }
                ++tempIndex;
            }
            return null;
        }
    }
}
