namespace DataTable
{
	public class InformationData
	{
		public string tag
		{
			get;
			set;
		}

		public string url
		{
			get;
			set;
		}

		public string sfx
		{
			get;
			set;
		}

		public InformationData()
		{
		}

		public InformationData(string tag, string url, string sfx)
		{
			this.tag = tag;
			this.url = url;
			this.sfx = sfx;
		}
	}
}
