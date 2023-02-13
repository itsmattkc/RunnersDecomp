namespace Player
{
	public class JumpParameter : StateEnteringParameter
	{
		public bool m_onAir;

		public override void Reset()
		{
			m_onAir = false;
		}

		public void Set(bool onair)
		{
			m_onAir = onair;
		}
	}
}
