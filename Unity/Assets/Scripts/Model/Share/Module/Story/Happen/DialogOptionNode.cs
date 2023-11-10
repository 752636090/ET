using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET.Story
{
    [Serializable]
    [NodeWidth(300)]
    public class DialogOptionNode : HappenNode, IReset
    {
        public override bool IsSaveNode => true;

        [Input(typeConstraint: TypeConstraint.Strict)]
        [LabelText("")]
        public DialogNodePort ChoicePort;

        [LabelText("本地化Key")]
        [OnValueChanged("OnChangeConfigID")]
        public string TextConfigKey = "";

        [NonSerialized]
        [ShowInInspector]
        [EnableIf("@false")]
        [LabelText("")]
        [MultiLineProperty(lines: 2)]
        [BsonIgnore]
        public string Tip;

        [LabelText("显示顺序")]
        [LabelWidth(65)]
        [HorizontalGroup(width: 80)]
        public int Order = 0;

        [LabelText("可反复选择")]
        [LabelWidth(64)]
        [HorizontalGroup(marginLeft: 26)]
        public bool ShowSelected = false;

        [Input(typeConstraint: TypeConstraint.Strict)]
        [HorizontalGroup("1", 100)]
        [LabelText("此项出现的条件"), LabelWidth(100)]
        public ConditionPort ConditionPort;

        [HorizontalGroup("1")]
        [LabelText("条件不足半透显示")]
        [LabelWidth(100)]
        public bool ShowIfIgnore = true;

        [Output(typeConstraint: TypeConstraint.Inherited)]
        [LabelText("结果")]
        public DialogNodePort OnChoicePort;

        [LabelText("底部对话(只生效一个)"), LabelWidth(144)]
        public bool ShowBottomTalk = false;

        public enum SelectionMode
        {
            ID,
            Team
        }
        [SerializeField]
        [EnumToggleButtons]
        [ShowIf("@ShowBottomTalk")]
        public SelectionMode Mode = SelectionMode.ID;


        [ShowIf("@ShowBottomTalk && Mode==SelectionMode.ID")]
        [OnValueChanged("OnNpcIDChanged")]
        public int NpcID = 0;

#if UNITY_EDITOR
        [ShowInInspector]
        [LabelText("")]
        [HorizontalGroup("BottomName")]
        [ShowIf("@ShowBottomTalk && Mode==SelectionMode.ID")]
        [EnableIf("@false")]
        [BsonIgnore]
        private string NpcName = ""; 
#endif

        [ShowIf("@ShowBottomTalk && Mode==SelectionMode.Team")]
        [LabelText("队中位置")]
        public int Index = 0;

        [LabelText("改名(可不填)")]
        [ShowIf("@ShowBottomTalk")]
        [OnValueChanged("OnNpcIDChanged")]
        public string ShowNpcName = "";

        [LabelText("显示立绘")]
        [HorizontalGroup("BottomImage"), LabelWidth(60)]
        [ShowIf("@ShowBottomTalk")]
        public bool ShowPortrait = true;

        [LabelText("立绘后缀")]
        [ShowIf("@ShowBottomTalk && ShowPortrait")]
        [HorizontalGroup("BottomImage"), LabelWidth(50)]
        public string PortraitSuffix;

        [LabelText("对话key")]
        [ShowIf("@ShowBottomTalk")]
        [OnValueChanged("OnChangeText")]
        public string TextKey = "";

        [System.NonSerialized]
        [ShowInInspector]
        [ShowIf("@ShowBottomTalk")]
        [DisableIf("@true")]
        [LabelText("")]
        [MultiLineProperty(lines: 4)]
        [BsonIgnore]
        private string MsgTip;

        // 标记玩家已选过
        [System.NonSerialized]
        [BsonIgnore]
        private bool Selected = false;

        [System.NonSerialized]
        [BsonIgnore]
        public bool ConditionSuccess = false;

        public void UnSelect()
        {
            Selected = false;
        }


        public bool IsSelected()
        {
            return Selected;
        }

        public void Reset()
        {
            Selected = false;
            ConditionSuccess = false;
        }

        //        protected override void Init()
        //        {
        //            if (this.name.Length <= 0) this.name = "选项";
        //            OnChangeConfigID();
        //            IsSaveNode = true;
        //        }

        //        private void OnChangeConfigID()
        //        {
        //#if UNITY_EDITOR
        //            Tip = GetString(TextConfigKey);
        //#endif
        //        }

        //        public override Node OnActive()
        //        {
        //            // 判断显示条件
        //            List<NodePort> conditions = this.GetPort("ConditionPort").GetConnections();
        //            ConditionSuccess = conditions.Count <= 0;
        //            foreach (NodePort port in conditions)
        //            {
        //                ConditionNode conditionNode = port.node as ConditionNode;
        //                if (conditionNode == null)
        //                {
        //                    continue;
        //                }
        //                ConditionSuccess = conditionNode.CheckAllConnectNode(NodePort.IO.Input);
        //                if (ConditionSuccess)
        //                {
        //                    break;
        //                }
        //            }
        //            if ((ConditionSuccess || ShowIfIgnore) && (Selected == false || (Selected && ShowSelected)))
        //            {
        //                return this;
        //            }
        //            return null;
        //        }

        //        public void OnSelected()
        //        {
        //            Selected = true;

        //            base.Continue("OnChoicePort");
        //        }


        //        private void OnChangeText()
        //        {
        //#if UNITY_EDITOR
        //            MsgTip = GetString(TextKey);
        //#endif
        //        }

        //        private void OnNpcIDChanged()
        //        {
        //#if UNITY_EDITOR
        //            if (NpcID == 0)
        //            {
        //                NpcName = "(取玩家名)";
        //            }
        //            else
        //            {
        //                if (ShowNpcName.Length > 0)
        //                {
        //                    NpcName = GetString(ShowNpcName);
        //                }
        //                else
        //                {
        //                    CharacterPoolData data = BaseDataClass.GetGameData<CharacterPoolDataScriptObject>()?.GetData(NpcID);
        //                    if (data != null)
        //                    {
        //                        if (data.IsUnique)
        //                        {
        //                            NpcName = data.UName;
        //                        }
        //                        else
        //                        {
        //                            NpcName = data.UName + " (非独特NPC)";
        //                        }
        //                        return;
        //                    }
        //                    NpcName = string.Format("没有找到{0}", NpcID);
        //                }
        //            }
        //#endif
        //        }

    }
}
