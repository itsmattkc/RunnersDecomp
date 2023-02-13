namespace Message
{
	public class MsgNotifyDead : MessageBase
	{
		private int playerNo;

		public MsgNotifyDead()
			: base(20480)
		{
		}
	}
}
