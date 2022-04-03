using UnityEngine;

namespace ET
{
    public class AfterUnitCreate_CreateUnitView : AEvent<EventType.AfterUnitCreate>
    {
        protected override void Run(EventType.AfterUnitCreate args)
        {
            #region IdleGame
            // Unit View层
            // 这里可以改成异步加载，demo就不搞了
            ResourcesComponent.Instance.LoadBundle("Knight.unity3d");
            GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset("Knight.unity3d", "Knight");
            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            go.transform.SetParent(GlobalComponent.Instance.Unit, true);

            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            args.Unit.AddComponent<AnimatorComponent>();
            args.Unit.Position = Vector3.zero;
            #endregion

            #region Learn
            //args.Unit.UnitType = UnitType.Player;
            //switch (args.Unit.UnitType)
            #endregion
            //{
            //    case UnitType.Player:
            //        {
            //            // Unit View层
            //            // 这里可以改成异步加载，demo就不搞了
            //            GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset("Unit.unity3d", "Unit");
            //            GameObject prefab = bundleGameObject.Get<GameObject>("Skeleton");

            //            GameObject go = UnityEngine.Object.Instantiate(prefab, GlobalComponent.Instance.Unit, true);
            //            go.transform.position = args.Unit.Position;
            //            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            //            args.Unit.AddComponent<AnimatorComponent>();
            //        }
            //        break;
            //    case UnitType.Monster:
            //        {
            //            // Unit View层
            //            // 这里可以改成异步加载，demo就不搞了
            //            GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset("Unit.unity3d", "Unit");
            //            GameObject prefab = bundleGameObject.Get<GameObject>("Skeleton");

            //            GameObject go = UnityEngine.Object.Instantiate(prefab, GlobalComponent.Instance.Unit, true);
            //            go.transform.position = args.Unit.Position;
            //            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            //            args.Unit.AddComponent<AnimatorComponent>();
            //        }
            //        break;
            //    case UnitType.Npc:
            //        {

            //        }
            //        break;
            //    case UnitType.DropItem:
            //        {

            //        }
            //        break;
            //    case UnitType.Box:
            //        {

            //        }
            //        break;
            //    default:
            //        break;
            //}

        }
    }
}