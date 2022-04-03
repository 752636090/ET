using System.Collections.Generic;
using System.Numerics;

namespace ET
{
    public partial class UnitConfig
    {
        /// <summary>
        /// 注意是逻辑层，用的Vector3在System.Numerics里面。
        /// 但用UnityEngine的也可以，因为Vector3里面是纯数学，不涉及渲染（因此ThirdParty里UnityEngine有Vector3）
        /// </summary>
        public Vector3 TestValue;
    }

    public class TestVector3
    {
        public Vector3 TestValue;
    }

    public partial class UnitConfigCategory
    {
        public List<TestVector3> TestVector3List = new List<TestVector3>();

        public override void AfterEndInit()
        {
            base.AfterEndInit();

            foreach (UnitConfig config in this.dict.Values)
            {
                //config.TestValue = new Vector3(config.Position[0], config.Height, config.Weight);
                //this.TestVector3List.Add(new TestVector3() { TestValue = config.TestValue });
            }
        }

        //public UnitConfig GetUnitConfigByHeight(int height)
        //{
        //    UnitConfig unitConfig = null;

        //    foreach (UnitConfig info in this.dict.Values)
        //    {
        //        if (info.Height == height)
        //        {
        //            unitConfig = info;
        //            break;
        //        }
        //    }

        //    return unitConfig;
        //}
    }
}
