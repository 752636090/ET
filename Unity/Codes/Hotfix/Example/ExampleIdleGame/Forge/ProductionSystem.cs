using UnityEngine;

namespace ET
{
    [FriendClass(typeof(Production))]
    public static class ProductionSystem
    {
        public static void FromMessage(this Production self, ProductionProto productionProto)
        {
            self.Id = productionProto.Id;
            self.ConfigId = productionProto.ConfigId;
            self.ProductionState = productionProto.ProductionState;
            self.StartTime = productionProto.StartTime;
            self.TargetTime = productionProto.TargetTime;
        }

        public static bool IsMakingState(this Production self)
        {
            return self.ProductionState == (int)ProductionState.Making;
        }

        public static bool IsMakeTimeOver(this Production self)
        {
            return self.TargetTime <= TimeHelper.ServerNow();
        }

        public static float GetRemainTimeValue(this Production self)
        {
            long RemainTime = self.TargetTime - TimeHelper.ServerNow();
            if (RemainTime <= 0)
            {
                return 0.0f;
            }

            long totalTime = self.TargetTime - self.StartTime;

            return RemainTime / (float)totalTime;
        }

        public static string GetRemainingTimeStr(this Production self)
        {
            long RemainTime = self.TargetTime - TimeHelper.ServerNow();

            if (RemainTime <= 0)
            {
                return "0时0分0秒";
            }

            RemainTime /= 1000;

            float h = Mathf.FloorToInt(RemainTime / 3600f);
            float m = Mathf.FloorToInt(RemainTime / 60f - h * 60f);
            float s = Mathf.FloorToInt(RemainTime - m * 60 - h * 3600f);
            return h.ToString("00") + "小时" + m.ToString("00") + "分" + s.ToString("00") + "秒";
        }
    }
}
