namespace Message
{
	public class MsgLoginSucceed : MessageBase
	{
		public string m_userId;

		public string m_password;

		public string m_countryCode = string.Empty;

		public MsgLoginSucceed()
			: base(61440)
		{
		}
	}
}
