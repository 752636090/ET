using System.Collections.Generic;

namespace ET
{
	[ChildType(typeof(Player))]
	public class PlayerComponent : Entity, IAwake, IDestroy
	{
		public readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();

		public int Count
		{
			get
			{
				return this.idPlayers.Count;
			}
		}
	}
}