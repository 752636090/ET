using ET.Story;
using UnityEngine;

namespace ET
{
    public class StoryGraphEditor : SerialGraphEditor
    {
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
