﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public class SerialGraphAssetPostprocessor : AssetPostprocessor
    {
        //private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        //{
        //    foreach (string path in importedAssets)
        //    {
        //        if (path.StartsWith("Assets/Res/Editor/Graphs"))
        //        {
        //            AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path).Export();
        //        }
        //    }
        //}
    }
}
