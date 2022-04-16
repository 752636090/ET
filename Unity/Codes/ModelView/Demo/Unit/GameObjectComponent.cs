using UnityEngine;

namespace ET
{
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject { get; set; }
        #region IdleGame
        public SpriteRenderer SpriteRenderer { get; set; }
        #endregion
    }
}