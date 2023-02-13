using UnityEngine;

namespace Message
{
	public class MsgPushCamera : MessageBase
	{
		public CameraType m_type;

		public Object m_parameter;

		public float m_interpolateTime;

		public MsgPushCamera(CameraType type, float interpolateTime, Object parameter = null)
			: base(32768)
		{
			m_type = type;
			m_parameter = parameter;
			m_interpolateTime = interpolateTime;
		}
	}
}
