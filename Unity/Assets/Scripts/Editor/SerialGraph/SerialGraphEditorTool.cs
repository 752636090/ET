using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace ET
{
    public static class SerialGraphEditorTool
    {
        [MenuItem("Tools/连连看/导出所有Graph配置(用于增删字段)")]
        public static void ExportAll()
        {
            string[] paths = Directory.GetFiles("Assets/Res/Editor/Graphs", "*.asset", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
                EditorSerialGraph asset = AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path);
                foreach (SerialNode node in asset.SerialGraph.Nodes)
                {
                    if (node is not IBeforeSaveNode beforeSave)
                    {
                        continue;
                    }
                    string error = beforeSave.BeforeSave();
                    if (error != null)
                    {
                        Log.Error($"{asset.name}出现以下错误：{error}");
                    }
                }
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
                asset.Export();
            }
        }

        [MenuItem("Tools/连连看/检测连线缺失数据")]
        public static void CheckMissingLine()
        {
            string[] paths = Directory.GetFiles("Assets/Res/Editor/Graphs", "*.asset", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
                EditorSerialGraph asset = AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path);
                asset.CheckMissingLine();
            }
            Log.Debug("检测完毕");
        }

        [MenuItem("Tools/连连看/检测连线重复数据")]
        public static void CheckRepeatedLine()
        {
            string[] paths = Directory.GetFiles("Assets/Res/Editor/Graphs", "*.asset", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
                EditorSerialGraph asset = AssetDatabase.LoadAssetAtPath<EditorSerialGraph>(path);
                foreach (SerialPort port in asset.SerialGraph.Ports)
                {
                    HashSet<int> targets = new();
                    foreach (int targetId in port.TargetIds)
                    {
                        if (targets.Contains(targetId))
                        {
                            SerialPort target = asset.SerialGraph.GetPort(targetId);
                            Log.Error($"{path}: {port.Node.Id}.{port.Name} 与 {target.Node.Id}.{target.Name} 连接重复");
                        }
                        else
                        {
                            targets.Add(targetId);
                        }
                    }
                }
            }
            Log.Debug("检测完毕");
        }
    }
}
