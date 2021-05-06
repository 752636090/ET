using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.ClientH |  AppType.ClientM | AppType.Gate | AppType.Map))]
	public partial class ItemConfigCategory : ACategory<ItemConfig>
	{
	}

	public class ItemConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public int AddHp;
		public int AddMagic;
		public double ClientTest;
		public string Test;
	}
}
