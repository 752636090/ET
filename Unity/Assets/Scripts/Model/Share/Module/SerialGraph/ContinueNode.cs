using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ET
{
    [System.Serializable]
    public abstract class ContinueNode : SerialNode
    {
        //[HideInInspector]
        //public Color saveTextColor = Color.white;

        public bool EnableSaveEdit = false;
        [LabelText("存档点"), LabelWidth(38)]
        [EnableIf("@EnableSaveEdit")]
        [HorizontalGroup("SaveGroup")]
        //[GUIColor("@saveTextColor")]
        //[OnValueChanged("OnSaveIDChanged")]
        [ShowIf("@IsSaveNode")]
        public int SaveID = 0;

        [BsonIgnore]
        public virtual bool IsSaveNode => false;
    }
}