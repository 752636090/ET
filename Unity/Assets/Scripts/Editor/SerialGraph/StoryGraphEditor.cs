using ET.Story;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public class StoryGraphEditor : SerialGraphEditor
    {
        public override SerialGraphType SerialGraphType => SerialGraphType.Story;

        [MenuItem("Tools/连连看/打开剧情事件编辑器", priority = 0)]
        public static void Open()
        {
            SerialGraphEditor window = GetWindow<StoryGraphEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 700);
        }

        protected override void InitNewGraph()
        {
            base.InitNewGraph();
            EditorSerialNode headNode = EditorSerialGraph.AddNode(typeof(StoryHeadInfoNode), new Vector2(0, -40));
            EditorSerialNode openNode = EditorSerialGraph.AddNode(typeof(StoryOpenNode), new Vector2(250, 200));
            EditorSerialNode startNode = EditorSerialGraph.AddNode(typeof(StoryStartNode), new Vector2(650, 300));
            EditorSerialGraph.Connect(openNode.SerialNode.GetPort("Enter"), headNode.SerialNode.GetPort("StartPort"));
            EditorSerialGraph.Connect(startNode.SerialNode.GetPort("Enter"), openNode.SerialNode.GetPort("Next"));
        }
    }
}
