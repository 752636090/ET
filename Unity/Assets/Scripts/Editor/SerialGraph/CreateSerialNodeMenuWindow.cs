using ET.NodeDefine;
using ET.Story;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public class CreateSerialNodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            SerialGraphEditor window = SerialGraphEditor.Instance;
            if (window.EditorSerialGraph == null)
            {
                EditorUtility.DisplayDialog("错误", "未选择文件", "确定");
                return null;
            }

            List<SearchTreeEntry> menuTree = new List<SearchTreeEntry>();
            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("创建节点")));

            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("条件"), 1));
            foreach (Type type in typeof(SerialNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ConditionNode)) && (t.Namespace == "ET.Common" || t.Namespace == $"ET.{window.SerialGraphType}")))
            {
                string name = type.Name;
                if (type.GetCustomAttribute<NodeNameAttribute>(false) != null)
                {
                    name = type.GetCustomAttribute<NodeNameAttribute>(false).Name;
                }
                menuTree.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = type });
            }
            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("执行"), 1));
            foreach (Type type in typeof(SerialNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(HappenNode)) && (t.Namespace == "ET.Common" || t.Namespace == $"ET.{window.SerialGraphType}")))
            {
                string name = type.Name;
                if (type.GetCustomAttribute<NodeNameAttribute>(false) != null)
                {
                    name = type.GetCustomAttribute<NodeNameAttribute>(false).Name;
                }
                menuTree.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = type });
            }
            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("演出"), 1));
            foreach (Type type in typeof(SerialNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PerformNode)) && (t.Namespace == "ET.Common" || t.Namespace == $"ET.{window.SerialGraphType}")))
            {
                string name = type.Name;
                if (type.GetCustomAttribute<NodeNameAttribute>(false) != null)
                {
                    name = type.GetCustomAttribute<NodeNameAttribute>(false).Name;
                }
                menuTree.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = type });
            }
            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("结果"), 1));
            foreach (Type type in typeof(SerialNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ResultNode)) && (t.Namespace == "ET.Common" || t.Namespace == $"ET.{window.SerialGraphType}")))
            {
                string name = type.Name;
                if (type.GetCustomAttribute<NodeNameAttribute>(false) != null)
                {
                    name = type.GetCustomAttribute<NodeNameAttribute>(false).Name;
                }
                menuTree.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = type });
            }
            menuTree.Add(new SearchTreeGroupEntry(new GUIContent("系统"), 1));
            foreach (Type type in typeof(SerialNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(ISystemSerialNode).IsAssignableFrom(t) && (t.Namespace == "ET.Common" || t.Namespace == $"ET.{window.SerialGraphType}")))
            {
                string name = type.Name;
                if (type.GetCustomAttribute<NodeNameAttribute>(false) != null)
                {
                    name = type.GetCustomAttribute<NodeNameAttribute>(false).Name;
                }
                menuTree.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = type });
            }
            return menuTree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            SerialGraphEditor window = SerialGraphEditor.Instance;
            // window to graph position
            VisualElement windowRoot = window.rootVisualElement;
            Vector2 windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
            Vector2 graphMousePosition = window.GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            Type nodeType = (Type)searchTreeEntry.userData;

            window.RegisterCompleteObjectUndo("Added " + nodeType);
            EditorSerialNode editorNode = window.EditorSerialGraph.AddNode(nodeType, graphMousePosition);
            window.GraphView.AddElement(editorNode);
            //Vector2 size = new Vector2(nodeType.GetCustomAttribute<NodeWidthAttribute>(true)?.Width ?? 100, 100);
            //editorNode.SetPosition(new Rect(graphMousePosition, size));

            return true;
        }
    }
}
