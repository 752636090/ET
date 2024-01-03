using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    // 应该没用了
    [ContinueNodeHandler(typeof(ContinueNode))]
    public class ContinueNodeHandler : AContinueNodeHandler<ContinueNode>
    {
        protected override bool Active(ContinueNode node)
        {
            return true;
        }
    }

    [AbstractDeclare]
    public abstract class AContinueNodeHandler<T> : ASerialNodeHandler<T>, IContinueNodeHandler where T : ContinueNode
    {
        public bool HandleActive(ContinueNode node)
        {
            return Active(node as T);
        }

        protected abstract bool Active(T node);
    }
}
