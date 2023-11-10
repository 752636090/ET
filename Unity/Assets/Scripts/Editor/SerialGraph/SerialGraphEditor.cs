using ET.Common;
using ET.NodeDefine;
using ET.Story;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace ET
{
    public abstract class SerialGraphEditor : OdinEditorWindow
    {
        //public Dictionary<Type, IComponentDrawer> DrawerDict = new Dictionary<Type, IComponentDrawer>();

        [HorizontalGroup("1")]
        [LabelText("分类")]
        [ReadOnly]
        public SerialGraphType SerialGraphType;
        private string graphName;
        [HorizontalGroup("1")]
        [LabelText("文件名")]
        [ValueDropdown("GetGraphNames")]
        [DisableIf("@hasUnsavedChanges")]
        [ShowInInspector]
        public string GraphName
        {
            get
            {
                return graphName;
            }
            set
            {
                graphName = value;
                OnSelectGraph();
            }
        }

        //private Dictionary<string, Type> nodeDict = new Dictionary<string, Type>();
        private CreateSerialNodeMenuWindow createNodeMenu;
        public SerialGraphView GraphView { get; private set; }
        public virtual Type GraphViewType => typeof(SerialGraphView);
        public EditorSerialGraph EditorSerialGraphAsset { get; private set; }
        public EditorSerialGraph EditorSerialGraph { get; private set; }
        public static SerialGraphEditor Instance { get; private set; }

        [MenuItem("Tools/连连看/打开剧情事件编辑器")]
        public static void TestOpen()
        {
            SerialGraphEditor window = GetWindow<StoryGraphEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 700);
        }

        private void Awake()
        {
            Instance = this;
            createNodeMenu = CreateInstance<CreateSerialNodeMenuWindow>();
            Undo.undoRedoEvent += UndoRedoEvent;
            string lastName = EditorPrefs.GetString(SerialGraphType.ToString());
            IEnumerable<string> names = GetGraphNames();
            if (names.Contains(lastName))
            {
                GraphName = lastName;
            }
            else
            {
                foreach (string name in GetGraphNames())
                {
                    GraphName = name;
                    break;
                }
            }
        }

        private void OnInspectorUpdate()
        {
            CheckDirty(); // 每个Node的Inspector没办法监听
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Undo.undoRedoEvent -= UndoRedoEvent;
        }

        private void OnFocus()
        {
            Instance = this;
        }

        //private void CreateGUI()
        private void CreateGraphView()
        {
            if (EditorSerialGraph == null)
            {
                return;
            }

            GraphView = new SerialGraphView()
            {
                style = { flexGrow = 1, top = 30 },
            };
            //GridBackground gridBackground = new GridBackground();
            //GraphView.Insert(0, gridBackground);
            GraphView.AddManipulator(new ContentDragger());
            GraphView.AddManipulator(new SelectionDragger());
            GraphView.AddManipulator(new ContentZoomer());
            GraphView.AddManipulator(new EdgeManipulator());
            GraphView.AddManipulator(new RectangleSelector());
            GraphView.AddManipulator(new SelectionDragger());
            GraphView.AddManipulator(new SelectionDropper());
            GraphView.nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), createNodeMenu);
            };
            GraphView.graphViewChanged += OnGraphViewChanged;
            GraphView.RegisterCallback<KeyDownEvent>(OnKeyDown);
            GraphView.serializeGraphElements = OnCopy;
            GraphView.canPasteSerializedData = CanPaste;
            GraphView.unserializeAndPaste = OnPaste;
            //Undo.undoRedoPerformed += OnUndoRedoPerformed;
            ReloadView();

            rootVisualElement.Add(GraphView);
        }

        private IEnumerable<string> GetGraphNames()
        {
            //操，GraphView把上面选项挡住了
            //return new string[] { "1", "2" };
            return Directory.GetFiles($"Assets/Res/Editor/Graphs/{SerialGraphType}", "*.asset", SearchOption.AllDirectories)
                .Select(path => /*path.Split('/').Last().Split('.')[0]*/Path.GetFileNameWithoutExtension(path));
        }

        [HorizontalGroup("1")]
        [Button("创建")]
        public void OnCrateButtonClicked()
        {
            CreateSerialGraphWindow.ShowWindow();
        }

        [HorizontalGroup("1")]
        [Button("删除")]
        [EnableIf("@EditorSerialGraphAsset != null")]
        public void OnDeleteButtonClicked()
        {
            if (!EditorUtility.DisplayDialog("二次确认", $"确定要删除 {EditorSerialGraphAsset.name} 吗？", "确定", "取消"))
            {
                return;
            }

            EditorSerialGraph = null;
            if (File.Exists(EditorSerialGraphAsset.BsonPath))
            {
                AssetDatabase.MoveAssetToTrash(EditorSerialGraphAsset.BsonPath); 
            }
            AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(EditorSerialGraphAsset));
            GraphName = null;
        }

        [HorizontalGroup("1", width: 80)]
        //[VerticalGroup("1/2")]
        [Button("折叠选中")]
        [EnableIf("@GraphView != null")]
        private void FoldSelections()
        {
            foreach (ISelectable item in GraphView.selection)
            {
                if (item is EditorSerialNode node)
                {
                    node.expanded = false;
                }
            }
        }
        [HorizontalGroup("1", width: 80)]
        //[VerticalGroup("1/2")]
        [Button("展开选中")]
        [EnableIf("@GraphView != null")]
        private void ExpandSelections()
        {
            foreach (ISelectable item in GraphView.selection)
            {
                if (item is EditorSerialNode node)
                {
                    node.expanded = true;
                }
            }
        }

        private void OnSelectGraph()
        {
            if (GraphView != null && rootVisualElement.Contains(GraphView))
            {
                rootVisualElement.Remove(GraphView);
            }
            string path = $"Assets/Res/Editor/Graphs/{SerialGraphType}/{GraphName}.asset";
            if (!File.Exists(path))
            {
                return;
            }
            EditorSerialGraphAsset = AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path);
            EditorSerialGraph = Instantiate(EditorSerialGraphAsset);
            EditorSerialGraph.SerialGraph.AfterDeserialize();
            EditorPrefs.SetString(SerialGraphType.ToString(), GraphName);
            CreateGraphView();
        }

        public bool CreateGraph(string name, int id)
        {
            string path = $"Assets/Res/Editor/Graphs/{SerialGraphType}/{name}.asset";
            if (File.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", $"已存在名为{name}的{SerialGraphType}配置", "确定");
                return false;
            }

            if (File.Exists($"Assets/Bundles/Graphs/{SerialGraphType}/{id}.bytes"))
            {
                EditorUtility.DisplayDialog("错误", $"已存在id为{id}的{SerialGraphType}配置", "确定");
                return false;
            }

            EditorSerialGraphAsset = CreateInstance<EditorSerialGraph>();
            EditorSerialGraphAsset.SerialGraph.Id = id;
            EditorSerialGraphAsset.name = name;
            AssetDatabase.CreateAsset(EditorSerialGraphAsset, path);
            GraphName = name;
            OnSelectGraph();
            InitNewGraph();
            ReloadView();
            return true;
        }

        protected virtual void InitNewGraph()
        {

        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (!hasFocus)
            {
                return;
            }

            if (e.ctrlKey && e.keyCode == KeyCode.S)
            {
                SaveChanges();
            }
        }

        private string OnCopy(IEnumerable<GraphElement> elements)
        {
            SerialGraphCopyData data = new SerialGraphCopyData()
            {
                GraphType = EditorSerialGraph.SerialGraph.Type,
                GraphId = EditorSerialGraph.SerialGraph.Id,
            };
            HashSet<int> copiedNodeIdSet = new HashSet<int>();
            foreach (GraphElement element in elements)
            {
                if (element is EditorSerialNode viewNode)
                {
                    copiedNodeIdSet.Add(viewNode.SerialNode.Id);
                    data.Nodes.Add(viewNode.SerialNode);
                    data.EditorNodeInfoDict[viewNode.SerialNode.Id] = EditorSerialGraph.EditorNodeInfoDict[viewNode.SerialNode.Id];
                }
            }
            foreach (SerialPort port in EditorSerialGraph.SerialGraph.Ports)
            {
                if (copiedNodeIdSet.Contains(port.NodeId))
                {
                    data.Ports.Add(port);
                }
            }
            data.Nodes.Sort((a, b) => a.Id.CompareTo(b.Id));
            data.Ports.Sort((a, b) => a.Id.CompareTo(b.Id));
            byte[] bson = MongoHelper.Serialize(data);
            return Convert.ToBase64String(bson);
        }

        private bool CanPaste(string dataStr)
        {
            try
            {
                SerialGraphCopyData data = MongoHelper.Deserialize<SerialGraphCopyData>(Convert.FromBase64String(dataStr));
                return EditorSerialGraph.SerialGraph.Type == data.GraphType && data.Nodes.Count > 0;
            }
            catch (Exception e)
            {
                //Debug.LogError(e);
                return false;
            }
        }

        private void OnPaste(string operationName, string dataStr)
        {
            Undo.RegisterCompleteObjectUndo(EditorSerialGraph, "Paste");

            GraphView.ClearSelection();

            SerialGraphCopyData data = MongoHelper.Deserialize<SerialGraphCopyData>(Convert.FromBase64String(dataStr));
            int maxNodeId = EditorSerialGraph.GetMaxNodeId();
            int maxPortId = EditorSerialGraph.GetMaxPortId();
            DoubleMap<int, int> src2NewNodeIdDict = new DoubleMap<int, int>();
            Dictionary<int, int> src2NewPortIdDict = new Dictionary<int, int>();
            Vector2 minPositionInData = Vector2.one * int.MaxValue;
            foreach (EditorSerialNodeInfo info in data.EditorNodeInfoDict.Values)
            {
                minPositionInData.x = Mathf.Min(info.Position.x, minPositionInData.x);
                minPositionInData.y = Mathf.Min(info.Position.y, minPositionInData.y);
            }
            foreach (SerialNode node in data.Nodes)
            {
                EditorSerialNodeInfo srcViewInfo = data.EditorNodeInfoDict[node.Id];
                Vector2 newPos = data.GraphId == EditorSerialGraph.SerialGraph.Id
                        ? srcViewInfo.Position/* + new Vector2(100, 100)*/
                        : GraphView.contentViewContainer.WorldToLocal(position.position) + srcViewInfo.Position - minPositionInData;
                while (EditorSerialGraph.EditorNodeInfoDict.Values.FirstOrDefault(a => a.Position == newPos) != default)
                {
                    newPos += new Vector2(100, 100);
                }
                EditorSerialGraph.EditorNodeInfoDict[maxNodeId + 1] = new EditorSerialNodeInfo()
                {
                    Remark = srcViewInfo.Remark,
                    Position = newPos,
                };
                src2NewNodeIdDict.Add(node.Id, maxNodeId + 1);
                node.Id = maxNodeId + 1;
                maxNodeId++;
                EditorSerialGraph.SerialGraph.Nodes.Add(node);
                //EditorSerialGraph.SerialGraph.NodeDict[node.Id] = node;
            }
            foreach (SerialPort port in data.Ports)
            {
                port.Id = src2NewPortIdDict[port.Id] = ++maxPortId;
                port.NodeId = src2NewNodeIdDict.GetValueByKey(port.NodeId);
                EditorSerialGraph.SerialGraph.Ports.Add(port);
            }

            #region 处理连线
            foreach (SerialPort port in data.Ports)
            {
                List<int> newTargetIds = new List<int>();
                for (int i = 0; i < port.TargetIds.Count; i++)
                {
                    if (src2NewPortIdDict.TryGetValue(port.TargetIds[i], out int newTargetId))
                    {
                        newTargetIds.Add(newTargetId);
                    }
                    else if (data.GraphId == EditorSerialGraph.SerialGraph.Id
                        && EditorSerialGraph.SerialGraph.PortDict.TryGetValue(port.TargetIds[i], out SerialPort targetPort))
                    {
                        SerialNode targetNode = EditorSerialGraph.SerialGraph.NodeDict[targetPort.NodeId];
                        if (targetNode.GetType().GetField(targetPort.Name).GetCustomAttribute<PortAttribute>().Capacity == Capacity.Multi)
                        {
                            newTargetIds.Add(targetPort.Id);
                            targetPort.TargetIds.Add(port.Id);
                        }
                    }
                }
                port.TargetIds = newTargetIds;
            }
            #endregion

            EditorSerialGraph.SerialGraph.AfterDeserialize();
            ReloadView();
            foreach (GraphElement item in GraphView.graphElements)
            {
                if (item is EditorSerialNode viewNode && src2NewNodeIdDict.ContainsValue(viewNode.SerialNode.Id))
                {
                    GraphView.AddToSelection(item);
                }
            }
            SetDirty();
        }

        public void ReloadView()
        {
            HashSet<SerialNode> selections = GraphView.selection.Where(a => a is EditorSerialNode)
                .Select(a => (a as EditorSerialNode).SerialNode).ToHashSet();

            GraphView.graphViewChanged -= OnGraphViewChanged;
            GraphView.DeleteElements(GraphView.graphElements);

            Dictionary<int, EditorSerialNode> viewNodeDict = new Dictionary<int, EditorSerialNode>();
            foreach (SerialNode node in EditorSerialGraph.SerialGraph.Nodes)
            {
                EditorSerialNode viewNode = EditorSerialGraph.AddNodeToView(node);
                GraphView.AddElement(viewNode);
                viewNodeDict[node.Id] = viewNode;
                if (selections.Contains(node))
                {
                    GraphView.AddToSelection(viewNode);
                }
            }

            foreach (SerialPort port in EditorSerialGraph.SerialGraph.Ports)
            {
                SerialNode node = EditorSerialGraph.SerialGraph.NodeDict[port.NodeId];
                string portName = node.PortDict.First(a => a.Value == port).Key;
                bool isInput = node.GetType().GetMember(portName)[0].GetCustomAttribute<PortAttribute>() is InputAttribute;
                foreach (int targetPortId in port.TargetIds)
                {
                    SerialPort targetPort = EditorSerialGraph.SerialGraph.PortDict[targetPortId];
                    SerialNode targetNode = EditorSerialGraph.SerialGraph.NodeDict[targetPort.NodeId];
                    Port inputViewPort = isInput ? viewNodeDict[node.Id].ViewPortDict[port.Id] : viewNodeDict[targetNode.Id].ViewPortDict[targetPort.Id];
                    Port outputViewPort = isInput ? viewNodeDict[targetNode.Id].ViewPortDict[targetPort.Id] : viewNodeDict[node.Id].ViewPortDict[port.Id];

                    Edge edge = new Edge()
                    {
                        input = inputViewPort,
                        output = outputViewPort,
                    };
                    edge.input.Connect(edge);
                    edge.output.Connect(edge);
                    GraphView.AddElement(edge);
                }
            }

            GraphView.graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RegisterCompleteObjectUndo(EditorSerialGraph, "Create Edge");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    Port viewPort1 = edge.input;
                    Port viewPort2 = edge.output;
                    SerialPort port1 = (SerialPort)viewPort1.userData;
                    SerialPort port2 = (SerialPort)viewPort2.userData;
                    EditorSerialGraph.Connect(port1, port2);
                } 
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RegisterCompleteObjectUndo(EditorSerialGraph, "Remove Elements");
                foreach (GraphElement item in graphViewChange.elementsToRemove)
                {
                    if (item is Edge edge)
                    {
                        Port viewPort1 = edge.input;
                        Port viewPort2 = edge.output;
                        SerialPort port1 = (SerialPort)viewPort1.userData;
                        SerialPort port2 = (SerialPort)viewPort2.userData;
                        EditorSerialGraph.Disconnect(port1, port2);
                    }
                    else if (item is EditorSerialNode viewNode)
                    {
                        EditorSerialGraph.RemoveNode(viewNode.SerialNode);
                    }
                } 
            }

            SetDirty();
            return graphViewChange;
        }

        public void RegisterCompleteObjectUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(EditorSerialGraph, name);

        }

        private void UndoRedoEvent(in UndoRedoInfo undo)
        {
            if (EditorSerialGraph == null)
            {
                return;
            }

            EditorSerialGraph.SerialGraph.AfterDeserialize();
            ReloadView();

            CheckDirty();
        }

        private void CheckDirty()
        {
            if (EditorSerialGraph == null)
            {
                hasUnsavedChanges = false;
                return;
            }

            //if (EditorSerialGraph.EditorNodeInfoDict.Count != EditorSerialGraphAsset.EditorNodeInfoDict.Count)
            //{
            //    SetDirty();
            //    return;
            //}

            //foreach (KeyValuePair<int, EditorSerialNodeInfo> item in EditorSerialGraph.EditorNodeInfoDict)
            //{
            //    if (!EditorSerialGraphAsset.EditorNodeInfoDict.TryGetValue(item.Key, out EditorSerialNodeInfo info)
            //        || !info.Equals(item.Value))
            //    {
            //        SetDirty();
            //        return;
            //    }
            //}

            if (MongoHelper.Serialize(EditorSerialGraph.EditorNodeInfoDict.Values).Hash() != MongoHelper.Serialize(EditorSerialGraphAsset.EditorNodeInfoDict.Values).Hash())
            {
                SetDirty();
                return;
            }

            if (MongoHelper.Serialize(EditorSerialGraph.SerialGraph).Hash() != MongoHelper.Serialize(EditorSerialGraphAsset.SerialGraph).Hash())
            {
                SetDirty();
                return;
            }

            hasUnsavedChanges = false;
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            SerialNode headNode = FindHead();
            if (headNode == null)
            {
                EditorUtility.DisplayDialog("错误", "找不到根节点，不会保存", "知道了");
                return;
            }
            EditorSerialGraph.SerialGraph.HeadId = headNode.Id;
            EditorSerialGraph.Export();
            #region 根据bson文件还原，废弃
            //EditorUtility.SetDirty(EditorSerialGraph);
            //AssetDatabase.SaveAssets(); 
            #endregion
            string path = AssetDatabase.GetAssetPath(EditorSerialGraphAsset);
            //AssetDatabase.MoveAsset(path, path.Replace(".asset", "_.asset"));
            AssetDatabase.CreateAsset(EditorSerialGraph, path);
            //AssetDatabase.DeleteAsset(path.Replace(".asset", "_.asset"));
            AssetDatabase.Refresh();
            EditorSerialGraphAsset = AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path);
            EditorSerialGraph = Instantiate(EditorSerialGraphAsset);
            hasUnsavedChanges = false;
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            #region 根据bson文件还原，废弃
            //try
            //{
            //    EditorSerialGraph.SerialGraph = (SerialGraph)MongoHelper.Deserialize(EditorSerialGraph.SerialGraph.GetType(),
            //            FileHelper.GetFileBuffer(EditorSerialGraph.BsonPath));
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError($"检测到上一次保存的bson数据是旧版的\n{e}");
            //    return;
            //}

            //List<int> tempList = new List<int>();
            //foreach (int id in EditorSerialGraph.EditorNodeInfoDict.Keys)
            //{
            //    if (!EditorSerialGraph.SerialGraph.NodeDict.ContainsKey(id))
            //    {
            //        tempList.Add(id);
            //    }
            //}
            //foreach (int id in tempList)
            //{
            //    EditorSerialGraph.EditorNodeInfoDict.Remove(id);
            //}

            //tempList.Clear();
            //foreach (SerialNode node in EditorSerialGraph.SerialGraph.Nodes)
            //{
            //    if (!EditorSerialGraph.EditorNodeInfoDict.ContainsKey(node.Id))
            //    {
            //        tempList.Add(node.Id);
            //    }
            //}
            //foreach (int id in tempList)
            //{
            //    EditorSerialGraph.EditorNodeInfoDict[id] = new EditorSerialNodeInfo()
            //    {
            //        Position = new Vector2(111, 111),
            //    };
            //} 

            //EditorUtility.SetDirty(EditorSerialGraph);
            //AssetDatabase.SaveAssets();
            #endregion
        }

        public new void SetDirty()
        {
            hasUnsavedChanges = true;
        }

        protected /*abstract*/ SerialNode FindHead()
        {
            foreach (SerialNode node in EditorSerialGraph.SerialGraph.Nodes)
            {
                if (node is IHeadSerialNode)
                {
                    return node;
                }
            }
            return null;
        }
    }

    public class SerialGraphCopyData
    {
        public List<SerialNode> Nodes = new List<SerialNode>();
        public List<SerialPort> Ports = new List<SerialPort>();
        public int GraphId;
        public SerialGraphType GraphType;
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, EditorSerialNodeInfo> EditorNodeInfoDict = new();
    }

    //public interface IComponentDrawer
    //{
    //    void Init(BehaviourTreeBaseGraph graph);
    //    void Draw(Rect rect);
    //}
}
