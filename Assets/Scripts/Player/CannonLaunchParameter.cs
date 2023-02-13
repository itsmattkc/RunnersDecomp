using UnityEngine;

namespace Player
{
	public class CannonLaunchParameter : StateEnteringParameter
	{
		public Vector3 m_position = Vector3.zero;

		public Quaternion m_rotation = Quaternion.identity;

		public float m_firstSpeed;

		public float m_height;

		public float m_outOfControlTime;

		public override void Reset()
		{
		}

		public void Set(Vector3 pos, Quaternion rot, float firstSpeed, float height, float outOfcontrol)
		{
			m_position = pos;
			m_rotation = rot;
			m_firstSpeed = firstSpeed;
			m_height = height;
			m_outOfControlTime = outOfcontrol;
		}
	}
}
