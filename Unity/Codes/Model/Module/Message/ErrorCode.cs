namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_NetworkError = 200002;
        public const int ERR_LoginInfoIsNull = 200003;
        public const int ERR_AccountNameFormError = 200004;
        public const int ERR_PasswordFormError = 200005;
        public const int ERR_AccountInBlackListError = 200006;
        public const int ERR_LoginPasswordError = 200007;
        public const int ERR_RequestRepeatedly = 200008;
        public const int ERR_TokenError = 200009;

        public const int ERR_RoleNameIsNull = 200010;
        public const int ERR_RoleNameSame = 200011;
        public const int ERR_RoleNotExist = 200012;

        public const int ERR_ConnectGateKeyError = 200013;

        public const int ERR_RequestSceneTypeError = 200014;

        public const int ERR_OtherAccountLogin = 200015;

        public const int ERR_SessionPlayerError = 200016;
        public const int ERR_NonePlayerError = 200017;
        public const int ERR_PlayerSessionError = 200018;
        public const int ERR_SessionStateError = 200019;
        public const int ERR_ReEnterGameError = 200020;

        public const int ERR_NumericTypeNotExist = 200021;
        public const int ERR_NumericTypeNotAddPoint = 200022;
        public const int ERR_AddPointNotEnough = 200023;

        public const int ERR_AlreadyAdventureState = 200024;
        public const int ERR_AdventureInDying = 200025;
        public const int ERR_AlreadyErrorLevel = 200026;
        public const int ERR_AdventureLevelNotEnough = 200027;
        public const int ERR_AdventureLevelIdError = 200028;
        public const int ERR_AdventureRoundError = 200029;
        public const int ERR_AdventureResultError = 200030;
        public const int ERR_AdventureWinResultError = 200031;
        public const int ERR_ExpNotEnough = 200032;
        public const int ERR_ExpNumError = 200033;

        public const int ERR_ItemNotExist = 200034;
    }
}