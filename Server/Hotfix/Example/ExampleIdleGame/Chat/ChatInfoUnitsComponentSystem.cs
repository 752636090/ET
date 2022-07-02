namespace ET
{
    public class ChatInfoUnitsComponentDestroy : DestroySystem<ChatInfoUnitsComponent>
    {
        public override void Destroy(ChatInfoUnitsComponent self)
        {
            foreach (ChatInfoUnit chatInfoUnit in self.ChatInfoUnitDict.Values)
            {
                chatInfoUnit?.Dispose();
            }
        }
    }

    [FriendClassAttribute(typeof(ET.ChatInfoUnitsComponent))]
    public static class ChatInfoUnitsComponentSystem
    {
        public static void Add(this ChatInfoUnitsComponent self, ChatInfoUnit chatInfoUnit)
        {
            if (self.ChatInfoUnitDict.ContainsKey(chatInfoUnit.Id))
            {
                Log.Error($"chatInfoUnit is exist! : {chatInfoUnit.Id}");
                return;
            }
            self.ChatInfoUnitDict.Add(chatInfoUnit.Id, chatInfoUnit);
        }

        public static ChatInfoUnit Get(this ChatInfoUnitsComponent self, long id)
        {
            self.ChatInfoUnitDict.TryGetValue(id, out ChatInfoUnit chatInfoUnit);
            return chatInfoUnit;
        }

        public static void Remove(this ChatInfoUnitsComponent self, long id)
        {
            if (self.ChatInfoUnitDict.TryGetValue(id, out ChatInfoUnit chatInfoUnit))
            {
                self.ChatInfoUnitDict.Remove(id);
                chatInfoUnit?.Dispose();
            }
        }
    }
}
