using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ET
{
    [CustomEditor(typeof(SerialNode))]
    public class SerialNodeInspector : OdinEditor
    {
        public override void SaveChanges()
        {
            base.SaveChanges();
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
        }
    }
}
