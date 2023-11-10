using ET.NodeDefine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DataOrientation = ET.NodeDefine.Orientation;
using EditorOrientation = UnityEditor.Experimental.GraphView.Orientation;
using DataDirection = ET.NodeDefine.Direction;
using EditorDirection = UnityEditor.Experimental.GraphView.Direction;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using ET.Common;
using Sirenix.Utilities.Editor;
using System.Xml.Linq;
using Unity.CodeEditor;
using Sirenix.Utilities;

namespace ET
{
    public class EditorSerialNode : Node
    {
        //public EditorNode Parent; // 加载时或者运行时创建时设置
        //                          // 加载的时候从Node身上的数据还原出来,后续只在保存的时候把父子关系回写
        //public List<EditorNode> Childs = new();
        //public long NodeId; // 引用的Node,行为树编辑器界面要有个Dict存储        
        //public string Remark;

        public SerialNode SerialNode;
        private SerialNodeObject contentObject;
        public Editor ContentInspector { get; private set; }
        public Dictionary<int, Port> ViewPortDict = new Dictionary<int, Port>();

        public EditorSerialNode(SerialNode node, Vector2 position) : base()
        {
            SerialNode = node;
            //mainContainer.style.color = Color.green; // 测试，没用
            //contentContainer.style.color = Color.green; // 测试，没用
            //extensionContainer.style.color = Color.green; // 测试，没用
            //elementTypeColor = new Color(0, 1, 0, 0.5f); // 测试，没用
            //this.Q("node-border").style.color = Color.green; // 测试，没用
            //style.color = Color.green; // 测试，没用
            NodeTintAttribute nodeTint = node.GetType().GetCustomAttribute<NodeTintAttribute>(true);
            if (nodeTint != null)
            {
                Color color = node.GetType().GetCustomAttribute<NodeTintAttribute>()?.Color ?? Color.grey;
                style.backgroundColor = nodeTint.Color;
                mainContainer.style.backgroundColor = color;
                contentContainer.style.backgroundColor = color;
                inputContainer.style.backgroundColor = color;
                outputContainer.style.backgroundColor = color;
                titleContainer.style.backgroundColor = color;
            }
            NodeNameAttribute nodeName = node.GetType().GetCustomAttribute<NodeNameAttribute>();
            title = $"<size=12><b>{nodeName?.Name ?? SerialNode.GetType().Name}</b></size>";

            contentObject = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<SerialNodeObject>("Assets/Res/Editor/SerialNodeObject.asset"));
            contentObject.SerialNode = node;
            ContentInspector = Editor.CreateEditor(contentObject);
            extensionContainer.Add(new IMGUIContainer(ContentInspector.OnInspectorGUI));
            RefreshExpandedState();

            foreach (KeyValuePair<string, SerialPort> item in node.PortDict)
            {
                MemberInfo memberInfo = (MemberInfo)node.GetType().GetField(item.Key) ?? node.GetType().GetProperty(item.Key);
                PortAttribute portAttribute = memberInfo.GetCustomAttribute<PortAttribute>(true);

                //Port port = Port.Create<Edge>(EditorOrientation.Horizontal,
                //    portAttribute is InputAttribute ? EditorDirection.Input : EditorDirection.Output,
                //    (Port.Capacity)portAttribute.Capacity,
                //    memberInfo.GetReturnType());
                Port port = InstantiatePort(EditorOrientation.Horizontal,
                    portAttribute is InputAttribute ? EditorDirection.Input : EditorDirection.Output,
                    (Port.Capacity)portAttribute.Capacity,
                    memberInfo.GetReturnType());
                if (memberInfo.GetCustomAttribute<HideLabelAttribute>(true) == null)
                {
                    port.portName = memberInfo.GetCustomAttribute<LabelTextAttribute>()?.Text ?? item.Key; 
                }
                else
                {
                    port.portName = "";
                }
                port.portColor = memberInfo.GetReturnType().GetTypeColor();
                port.userData = item.Value;
                (portAttribute is InputAttribute ? inputContainer : outputContainer).Add(port);
                ViewPortDict[item.Value.Id] = port;
            }
            RefreshPorts();

            Vector2 size = new Vector2(node.GetType().GetCustomAttribute<NodeWidthAttribute>(true)?.Width ?? 100, 100);
            base.SetPosition(new Rect(position, size));
        }

        public override void SetPosition(Rect newPos)
        {
            SerialGraphEditor.Instance.RegisterCompleteObjectUndo("Node SetPosition");
            base.SetPosition(newPos);
            SerialGraphEditor.Instance.EditorSerialGraph.EditorNodeInfoDict[SerialNode.Id].Position = newPos.position;
        }

        public override void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
        {
            base.CollectElements(collectedElementSet, conditionFunc);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
        }

        public override Rect GetPosition()
        {
            return base.GetPosition();
        }

        public override Port InstantiatePort(EditorOrientation orientation, EditorDirection direction, Port.Capacity capacity, Type type)
        {
            Port port = base.InstantiatePort(orientation, direction, capacity, type);
            
            return port;
        }

        protected override void OnPortRemoved(Port port)
        {
            base.OnPortRemoved(port);
            Debug.Log("OnPortRemoved");
        }

        //public override string title
        //{
        //    get => SerialNode.GetType().Name;
        //    //set => base.title=value;
        //}

        protected override void ToggleCollapse()
        {
            base.ToggleCollapse();
        }

    }

    public static class XNodeTool
    {
        private static Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        public static Color GetTypeColor(this Type type)
        {
            if (type == typeof(StoryHeadAloneNodePort) || type == typeof(StoryOpenAloneNodePort))
            {
                return new Color(100f / 255 * 1.5f, 70f / 255 * 1.5f, 70f / 255 * 1.5f);
            }
            else if (type == typeof(ConditionPort))
            {
                return new Color(140f / 255, 100f / 255, 10f / 255);
            }
            else if (type == typeof(DialogNodePort))
            {
                return new Color(100f / 255, 130f / 255, 190f / 255);
            }

            if (type == null) return Color.gray;
            Color col;
            if (!typeColors.TryGetValue(type, out col))
            {
                string typeName = type.PrettyName();
                UnityEngine.Random.State oldState = UnityEngine.Random.state;
                UnityEngine.Random.InitState(typeName.GetHashCode());
                col = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                typeColors.Add(type, col);
                UnityEngine.Random.state = oldState;
            }
            return col;
        }

        /// <summary> Return a prettiefied type name. </summary>
        public static string PrettyName(this Type type)
        {
            if (type == null) return "null";
            if (type == typeof(System.Object)) return "object";
            if (type == typeof(float)) return "float";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(bool)) return "bool";
            else if (type.IsGenericType)
            {
                string s = "";
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(List<>)) s = "List";
                else s = type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                string[] stypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    stypes[i] = types[i].PrettyName();
                }
                return s + "<" + string.Join(", ", stypes) + ">";
            }
            else if (type.IsArray)
            {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++)
                {
                    rank += ",";
                }
                Type elementType = type.GetElementType();
                if (!elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";
                else
                {
                    string s = elementType.PrettyName();
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }
            else return type.ToString();
        }
    }

    //public class InputAttributeDrawer : OdinAttributeDrawer<InputAttribute>
    //{
    //    protected override bool CanDrawAttributeProperty(InspectorProperty property)
    //    {
    //        //SkipWhenDrawing = true;
    //        return false;
    //    }
    //}

    //public class OutputAttributeDrawer : OdinAttributeDrawer<OutputAttribute>
    //{
    //    protected override bool CanDrawAttributeProperty(InspectorProperty property)
    //    {
    //        //SkipWhenDrawing = true;
    //        return false;
    //    }
    //}

    //public class InputAttributeProcessor : OdinAttributeProcessor<InputAttribute>
    //{
    //    public override bool CanProcessSelfAttributes(InspectorProperty property)
    //    {
    //        return false;
    //    }

    //    public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
    //    {
    //        return false;
    //    }
    //}

    //public class OutputAttributeProcessor : OdinAttributeProcessor<OutputAttribute>
    //{
    //    public override bool CanProcessSelfAttributes(InspectorProperty property)
    //    {
    //        return false;
    //    }

    //    public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
    //    {
    //        return false;
    //    }
    //}
}
