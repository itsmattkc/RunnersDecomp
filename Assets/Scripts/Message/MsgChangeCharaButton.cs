namespace Message
{
	public class MsgChangeCharaButton : MessageBase
	{
		public bool value;

		public bool pause;

		public MsgChangeCharaButton(bool value_, bool pause_)
			: base(12315)
		{
			value = value_;
			pause = pause_;
		}
	}
}
