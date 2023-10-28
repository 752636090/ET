using ET.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "SerialNodeObject", menuName = "Temp/SerialNodeObject")]
    public class SerialNodeObject : ScriptableObject
    {
        [SerializeReference]
        [HideReferenceObjectPicker]
        [HideLabel]
        public SerialNode SerialNode = new SetIntNode();
    }
}
