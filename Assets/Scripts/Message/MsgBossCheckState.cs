namespace Message
{
	public class MsgBossCheckState : MessageBase
	{
		public enum State
		{
			IDLE,
			ATTACK_OK
		}

		private State m_state;

		public MsgBossCheckState(State state)
			: base(12323)
		{
			m_state = state;
		}

		public bool IsAttackOK()
		{
			return m_state == State.ATTACK_OK;
		}
	}
}
