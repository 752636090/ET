using UnityEngine;
using UnityEditor;

namespace ET
{
    public class CustomAssetModificationProcessor : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            //foreach (string path in paths)
            //{
            //    if (path.StartsWith("Assets/Res/Editor/Graphs"))
            //    {
            //        void Export()
            //        {
            //            AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path).Export();
            //            EditorApplication.projectChanged -= Export;
            //        }
            //        EditorApplication.projectChanged += Export;
            //    }
            //}
            return paths;
        }
    }
}
