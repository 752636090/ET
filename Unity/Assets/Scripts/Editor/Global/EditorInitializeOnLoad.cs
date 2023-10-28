using MongoDB.Bson.Serialization;
using System;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace ET
{
    [InitializeOnLoad]
    public class EditorInitializeOnLoad
    {
        static EditorInitializeOnLoad()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };
            ETTask.ExceptionHandler += Log.Error;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += EditorLogHelper.CheckCompolingFinish;
            //if (GameObject.Find("[YooAssets]") != null)
            //{
            //    UnityEngine.Object.DestroyImmediate(GameObject.Find("[YooAssets]"));
            //}
            EditorLogHelper.OnPlayModeStateChanged(PlayModeStateChange.EnteredEditMode);
            Init().Coroutine();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            EditorLogHelper.OnPlayModeStateChanged(change);
            if (!Application.isPlaying)
            {
                //World.Instance.Dispose();

                Init().Coroutine(); // 关闭游戏后重新生成World
            }
        }

        private static async ETTask Init()
        {
            Debug.Log("初始化EditorInitializeOnLoad");
            BsonSerializer.RegisterSerializationProvider(new Vector2SerializationProvider()); // 好像没用
            World.Instance.AddSingleton<Logger>().Log = new UnityLogger();
            World.Instance.AddSingleton<TimeInfo>();
            World.Instance.AddSingleton<FiberManager>();

            // 编辑器World不需要，加上会很恶心
            //await World.Instance.AddSingleton<ResourcesComponent>().CreatePackageAsync("DefaultPackage", true);

            CodeLoader codeLoader = World.Instance.AddSingleton<CodeLoader>();
            await codeLoader.DownloadAsync();

            codeLoader.Start(); // 里面最后一步是异步创建fiber
        }
    }
}
