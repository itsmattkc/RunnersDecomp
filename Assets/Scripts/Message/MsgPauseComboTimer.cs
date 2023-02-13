namespace Message
{
	public class MsgPauseComboTimer : MessageBase
	{
		public enum State
		{
			PAUSE,
			PAUSE_TIMER,
			PLAY,
			PLAY_SET,
			PLAY_RESET
		}

		public State m_value;

		public float m_time;

		public MsgPauseComboTimer(State value)
			: base(12356)
		{
			m_value = value;
			m_time = -1f;
		}

		public MsgPauseComboTimer(State value, float time)
			: base(12356)
		{
			m_value = value;
			m_time = time;
		}
	}
}
