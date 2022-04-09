using UnityEngine;

namespace ET
{
    public class AfterUnitCreate_CreateUnitView : AEventAsync<EventType.AfterUnitCreate>
    {
        protected override async ETTask Run(EventType.AfterUnitCreate args)
        {
            #region IdleGame
            // Unit View层

            await ResourcesComponent.Instance.LoadBundleAsync($"{args.Unit.Config.PrefabName}.unity3d");
            GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset($"{args.Unit.Config.PrefabName}.unity3d", args.Unit.Config.PrefabName); ;
            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            go.transform.SetParent(GlobalComponent.Instance.Unit, true);

            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            args.Unit.GetComponent<GameObjectComponent>().SpriteRenderer = go.GetComponent<SpriteRenderer>();
            args.Unit.AddComponent<AnimatorComponent>();
            args.Unit.AddComponent<HeadHpViewComponent>();

            args.Unit.Position = args.Unit.Type == UnitType.Player ? new Vector3(-1.5f, 0, 0) : new Vector3(1.5f, RandomHelper.RandomNumber(-1, 1), 0);

            await ETTask.CompletedTask;
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