namespace Message
{
	public class MsgChaoState : MessageBase
	{
		public enum State
		{
			COME_IN,
			STOP,
			STOP_END,
			LAST_CHANCE,
			LAST_CHANCE_END
		}

		private State m_state;

		public State state
		{
			get
			{
				return m_state;
			}
		}

		public MsgChaoState(State state)
			: base(21760)
		{
			m_state = state;
		}
	}
}
