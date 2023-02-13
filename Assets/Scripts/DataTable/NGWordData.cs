namespace DataTable
{
	public class NGWordData
	{
		public string word
		{
			get;
			set;
		}

		public int param
		{
			get;
			set;
		}

		public NGWordData()
		{
		}

		public NGWordData(string word, int param)
		{
			this.word = word;
			this.param = param;
		}
	}
}
