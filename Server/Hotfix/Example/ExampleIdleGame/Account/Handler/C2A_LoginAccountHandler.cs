using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ET
{
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>(); // 不移除会在5秒后断开连接


            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply(); // 返回给客户端
                session.Dispose();
                return;
            }

            if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$"))
            {
                response.Error = ErrorCode.ERR_LoginInfoError; // 此处应该换个错误码
                reply();
                session.Dispose();
                return;
            }

            if (!Regex.IsMatch(request.Password.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$"))
            {
                response.Error = ErrorCode.ERR_LoginInfoError; // 此处应该换个错误码
                reply();
                session.Dispose();
                return;
            }

            List<Account> accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(
                d => d.AccountName.Equals(request.AccountName.Trim()));
            Account account = null;
            if (accountInfoList.Count > 0)
            {
                account = accountInfoList[0];
                session.AddChild(account);
                if (account.AccountType == (int)AccountType.BlackList)
                {
                    response.Error = ErrorCode.ERR_LoginInfoError;
                    reply();
                    session.Dispose();
                    return;
                }

                if (!account.Password.Equals(request.Password))
                {
                    response.Error = ErrorCode.ERR_LoginInfoError;
                    reply();
                    session.Dispose();
                    return;
                }
            }
            else // 自动注册
            {
                account = session.AddChild<Account>();
                account.AccountName = request.AccountName;
                account.Password = request.Password;
                account.CreateTime = TimeHelper.ServerNow();
                account.AccountType = (int)AccountType.General;
                await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account); // Zone是区服
            }

            string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();
            session.DomainScene().GetComponent<TokenComponent>().Remove(account.Id); // DomainScene拿到了账号服务器的Scene
            session.DomainScene().GetComponent<TokenComponent>().Add(account.Id, Token);

            response.AccountId = account.Id;
            response.Token = Token;

            reply();
            // 上面都await了，这里就不需要await了
        }
    }
}
