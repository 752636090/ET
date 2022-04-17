using ET.EventType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [NumericWatcher(NumericType.AttributePoint)]
    [NumericWatcher(NumericType.Exp)]
    public class NumericWatcher_AddExp : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            unit = args.Parent as Unit;

            if (args.NumericType == NumericType.Exp)
            {
                NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

                int unitLevel = numericComponent.GetAsInt(NumericType.Level);

                if (PlayerLevelConfigCategory.Instance.Contain(unitLevel))
                {
                    long needExp = PlayerLevelConfigCategory.Instance.Get(unitLevel).NeedExp;

                    if (args.New >= needExp)
                    {
                        RedDotHelper.ShowRedDotNode(unit.ZoneScene(), "UpLevelButton");
                    }
                    else
                    {
                        if (RedDotHelper.IsLogicAlreadyShow(unit.ZoneScene(), "UpLevelButton"))
                        {
                            RedDotHelper.HideRedDotNode(unit.ZoneScene(), "UpLevelButton");
                        }
                    }
                }
            }

            if (args.NumericType == NumericType.AttributePoint)
            {
                if (args.New >= 0)
                {
                    RedDotHelper.ShowRedDotNode(unit.ZoneScene(), "AddAttribute");
                }
                else
                {
                    if (RedDotHelper.IsLogicAlreadyShow(unit.ZoneScene(), "AddAttribute"))
                    {
                        RedDotHelper.HideRedDotNode(unit.ZoneScene(), "AddAttribute");
                    }
                }
            }

            unit.ZoneScene().GetComponent<UIComponent>().GetDlgLogic<DlgRoleInfo>()?.Refresh();
        }
    }
}
