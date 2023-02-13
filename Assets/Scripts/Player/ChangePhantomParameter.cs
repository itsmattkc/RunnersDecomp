namespace Player
{
	public class ChangePhantomParameter : StateEnteringParameter
	{
		public PhantomType m_changeType;

		public float m_time;

		public PhantomType ChangeType
		{
			get
			{
				return m_changeType;
			}
		}

		public float Timer
		{
			get
			{
				return m_time;
			}
		}

		public override void Reset()
		{
			m_changeType = PhantomType.NONE;
			m_time = 0f;
		}

		public void Set(PhantomType type, float time)
		{
			m_changeType = type;
			m_time = time;
		}

		public void Set(PhantomType type)
		{
			m_changeType = type;
		}
	}
}
