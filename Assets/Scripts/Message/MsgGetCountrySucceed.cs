namespace Message
{
	public class MsgGetCountrySucceed : MessageBase
	{
		public int m_countryId;

		public string m_countryCode;

		public MsgGetCountrySucceed()
			: base(61443)
		{
		}
	}
}
