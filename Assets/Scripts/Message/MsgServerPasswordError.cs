namespace Message
{
	public class MsgServerPasswordError : MessageBase
	{
		public string m_key;

		public string m_userId;

		public string m_password;

		public MsgServerPasswordError()
			: base(61516)
		{
		}
	}
}
