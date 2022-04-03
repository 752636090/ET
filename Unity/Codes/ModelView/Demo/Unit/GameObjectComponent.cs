using UnityEngine;

namespace ET
{
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject;
        #region IdleGame
        public SpriteRenderer SpriteRenderer; 
        #endregion
    }
}