using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class AdventureHelper
    {
        public static async ETTask<int> RequestStartGameLevel(Scene zoneScene, int levelId)
        {
            M2C_StartGameLevel m2CStartGameLevel = null;
            try
            {
                m2CStartGameLevel = (M2C_StartGameLevel)await zoneScene.GetComponent<SessionComponent>().Session.Call(
                    new C2M_StartGameLevel() { LevelId = levelId });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (m2CStartGameLevel.Error != ErrorCode.ERR_Success)
            {
                Log.Error(m2CStartGameLevel.Error.ToString());
                return m2CStartGameLevel.Error;
            }

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> RequestEndGameLevel(Scene zoneScene)
        {

        }
    }
}
