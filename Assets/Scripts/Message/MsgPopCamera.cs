namespace Message
{
	public class MsgPopCamera : MessageBase
	{
		public CameraType m_type;

		public float m_interpolateTime;

		public MsgPopCamera(CameraType type, float interpolateTime)
			: base(32769)
		{
			m_type = type;
			m_interpolateTime = interpolateTime;
		}
	}
}
