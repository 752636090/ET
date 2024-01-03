using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient: AEvent<Scene, EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EntryEvent3 args)
        {
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            root.AddComponent<UIGlobalComponent>();
            root.AddComponent<UIComponent>();
            root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();

            IGraphsComponent.GraphConfigDict = new();
            SerialGraph graph = MongoHelper.Deserialize<SerialGraph>((await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>("Assets/Bundles/Graphs/Story/10001.bytes")).bytes);
            graph.AfterDeserialize();
            IGraphsComponent.GraphConfigDict.Add(SerialGraphType.Story, 10001, graph);
            root.AddComponent<StoryComponent>();

            // 根据配置修改掉Main Fiber的SceneType
            SceneType sceneType = EnumHelper.FromString<SceneType>(globalComponent.GlobalConfig.AppType.ToString());
            root.SceneType = sceneType;

            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}