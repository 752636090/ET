using ET.EventType;

namespace ET
{
    public class SceneChangeStart_CreateRedDotLogic : AEvent<EventType.SceneChangeStart>
    {
        protected override void Run(SceneChangeStart args)
        {
            RedDotHelper.AddRedDotNode(args.ZoneScene, "Root", "Main", false);
            RedDotHelper.AddRedDotNode(args.ZoneScene, "Main", "Role", false);
            RedDotHelper.AddRedDotNode(args.ZoneScene, "Role", "UpLevelButton", false);
            RedDotHelper.AddRedDotNode(args.ZoneScene, "Role", "AddAttribute", false);
        }
    }
}
