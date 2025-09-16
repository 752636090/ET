using Sirenix.OdinInspector;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [CreateAssetMenu(fileName = "New", menuName = "SerialGraph/通用SO")]
    public class CommonScriptableObject : SerializedScriptableObject
    {
        [ReadOnly]
        public int Id;

#if UNITY_EDITOR
        [PropertyOrder(0)]
        [Button("保存")]
        public void Save()
        {
            OnBeforeSave();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        protected override void OnBeforeSerialize()
        {
            if (UnityEditor.Selection.activeObject != this)
            {
                return;
            }

            OnBeforeSave();

            base.OnBeforeSerialize();
        }

        public void OnBeforeSave()
        {
            if (Value is IScriptableObjectId id)
            {
                bool needChangeId = Id == 0;

                string selfPath = UnityEditor.AssetDatabase.GetAssetPath(this);
                string folder = Path.GetDirectoryName(selfPath);
                while (Path.GetDirectoryName(folder).Replace("\\", "/") != "Assets/Bundles/CustomConfig")
                {
                    if (folder.Length < 10)
                    {
                        Log.Error($"{Path.GetFileNameWithoutExtension(selfPath)}不在Assets/Bundles/CustomConfig中，将产生错误");
                        return;
                    }
                    folder = Path.GetDirectoryName(folder);
                }

                int maxId = 0;
                string[] paths = Directory.GetFiles(folder, "*.asset", SearchOption.AllDirectories);
                foreach (string path in paths)
                {
                    CommonScriptableObject so = UnityEditor.AssetDatabase.LoadAssetAtPath<CommonScriptableObject>(path);
                    if (so != this && so.Id == Id)
                    {
                        needChangeId = true;
                    }
                    maxId = math.max(maxId, so.Id);
                }
                if (needChangeId)
                {
                    Id = maxId + 1;
                    id.Id = Id;
                }
            }

            if (Value is IScriptableObjectSaveHandler saveHandler)
            {
                saveHandler.BeforeSave();
            }
        }
#endif

        [HideLabel]
        public object Value;
    }
}
