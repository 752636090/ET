using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    //[Serializable]
    public class ClassA
    {
        public int A;
    }

    //[Serializable]
    public class ClassB
    {
        public ClassA A1;
        public ClassA A2;

        public ClassB()
        {
            A1 = A2 = new ClassA();
        }
    }
}
