namespace Message
{
	public class MsgGetPrizeWheelSpinGeneralSucceed : MessageBase
	{
		public ServerPrizeState prizeState;

		public MsgGetPrizeWheelSpinGeneralSucceed()
			: base(61465)
		{
		}
	}
}
