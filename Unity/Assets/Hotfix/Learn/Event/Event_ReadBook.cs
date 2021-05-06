using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    //[Event(EventIdType.ReadBook)]
    //public class Event_ReadBook : AEvent
    //{
    //    public override void Run()
    //    {
    //        Log.Debug("小明读书");
    //    }
    //}

    // 不能同时有带参和无参的

    [Event(EventIdType.ReadBook)]
    public class Event_ReadBookWithName : AEvent<string>
    {
        public override void Run(string bookName)
        {
            Log.Debug($"小明读{bookName}");
        }
    }
}
