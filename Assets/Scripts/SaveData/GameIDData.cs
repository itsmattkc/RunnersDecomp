namespace SaveData
{
	public class GameIDData
	{
		public const string NoUserID = "0";

		public const string KeyID = "aa7329ab4330306fbdd6dbe9b85c96be";

		public const string KeyPass = "48521cd1266052bfc25718720e91fa83";

		public string id
		{
			get;
			set;
		}

		public string password
		{
			get;
			set;
		}

		public string device
		{
			get;
			set;
		}

		public string takeoverId
		{
			get;
			set;
		}

		public string takeoverPassword
		{
			get;
			set;
		}

		public GameIDData()
		{
			Init();
		}

		public void Init()
		{
			id = "0";
			password = string.Empty;
			device = string.Empty;
			takeoverId = string.Empty;
			takeoverPassword = string.Empty;
		}
	}
}
