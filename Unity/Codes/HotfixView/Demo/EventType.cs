using UnityEngine;

namespace ET
{
    namespace EventType
    {
        /// <summary>
        /// 定义在显示层的EventType必须在显示层订阅和发布
        /// </summary>
        public struct CreateUnitView
        {
            public GameObject GameObject;
        }

        public struct StartGameLevel
        {
            public Scene ZoneScene;
        }
    }
}
