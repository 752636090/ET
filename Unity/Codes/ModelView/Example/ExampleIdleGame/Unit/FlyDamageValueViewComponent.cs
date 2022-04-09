﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class FlyDamageValueViewComponent : Entity, IAwake, IDestroy
    {
        public HashSet<GameObject> FlyingDamageSet = new HashSet<GameObject>();
    }
}
