using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ET
{
    [System.Serializable]
    public abstract class ContinueNode : SerialNode
    {
#if UNITY_EDITOR
        [BsonIgnore]
        private Color SaveTextColor = Color.white;
        [BsonIgnore]
        private bool EnableSaveEdit = false;
#endif

        //[LabelText("存档点"), LabelWidth(38)]
        //[EnableIf("@EnableSaveEdit")]
        //[HorizontalGroup("SaveGroup")]
        //[GUIColor("@SaveTextColor")]
        //[OnValueChanged("OnSaveIDChanged")]
        //[ShowIf("@IsSaveNode")]
        //public int SaveID = 0;

        [BsonIgnore]
        public virtual bool IsSaveNode => false;

        [BsonIgnore]
        public virtual string DefaultOutPort => "OutPort";

#if UNITY_EDITOR
        //[HorizontalGroup("SaveGroup", width: 38)]
        //[ShowIf("@IsSaveNode")]
        //[Button("@SaveButtonName()")]
        private void OnClickSaveButton()
        {
            if (EnableSaveEdit && CheckSaveID() == false)
            {
                WarningSaveEdit();
                return;
            }
            SaveTextColor = Color.white;
            EnableSaveEdit = !EnableSaveEdit;
        }

        public void WarningSaveEdit()
        {
            SaveTextColor = Color.red;
        }

        private string SaveButtonName()
        {
            if (EnableSaveEdit)
            {
                return "保存";
            }
            return "修改";
        }
        private void OnSaveIDChanged()
        {
            SaveTextColor = Color.white;
        }

        public bool CheckSaveID()
        {
            if (Id == 0)
            {
                return true;
            }
            foreach (ContinueNode childnode in Graph.Nodes)
            {
                if (childnode == null || childnode == this)
                {
                    continue;
                }
                int childSaveID = 0;
                if (childnode.IsSaveNode)
                {
                    childSaveID = childnode.Id;
                }
                else
                {
                    continue;
                }
                if (childSaveID == Id)
                {
                    Log.Error($"存档点ID与 Id为{childnode.Id}的节点)重复");
                    return false;
                }
            }
            return true;
        } 
#endif
    }

    //public interface ISaveNode
    //{
    //    [LabelText("存档点"), LabelWidth(38)]
    //    [EnableIf("@EnableSaveEdit")]
    //    [HorizontalGroup("SaveGroup")]
    //    [GUIColor("@SaveTextColor")]
    //    [OnValueChanged("OnSaveIDChanged")]
    //    [ShowIf("@IsSaveNode")]
    //    [SerializeField]
    //    public int SaveID { get; set; }
    //}
}