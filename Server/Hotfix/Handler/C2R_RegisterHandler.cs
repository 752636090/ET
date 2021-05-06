using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix.Handler
{
    [MessageHandler(AppType.Realm)]
    public class C2R_RegisterHandler : AMRpcHandler<C2R_Register, R2C_Register>
    {
        protected override async ETTask Run(Session session, C2R_Register request, R2C_Register response, Action reply)
        {
            string account = request.Account;
            string password = request.Password;

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                response.Error = ErrorCode.ERR_AccountOrPasswordError;
                reply();
                return;
            }

            try
            {
                var accountList = await Game.Scene.GetComponent<DBProxyComponent>().Query<AccountInfo>(d => account == d.AccountName);
                if (accountList.Count > 0)
                {
                    response.Error = ErrorCode.ERR_AccountOrPasswordError;
                    reply();
                    return;
                }
                
                AccountInfo accountInfo = ComponentFactory.CreateWithId<AccountInfo>(IdGenerater.GenerateId());
                accountInfo.AccountName = account;
                accountInfo.Password = password;
                await Game.Scene.GetComponent<DBProxyComponent>().Save(accountInfo);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                response.Error = ErrorCode.ERR_AccountOrPasswordError;
                response.Message = e.ToString();
                reply();
                return;
            }

            response.Error = ErrorCode.ERR_Success;
            reply();

            await ETTask.CompletedTask;
        }
    }
}
