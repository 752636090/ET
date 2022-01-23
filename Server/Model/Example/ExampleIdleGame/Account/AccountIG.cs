namespace ET
{
    public enum AccountTypeIG
    {
        General,

        BlackList = 1,
    }

    public class AccountIG : Entity, IAwake
    {
        public string AccountName;

        public string Password;

        public long CreateTime;

        public int AccountType; // 内测账号/机器人账号/白名单账号/黑名单账号……


    }
}
