namespace Message
{
	public class MsgPrepareStageReplace : MessageBase
	{
		public PlayerSpeed m_speedLevel;

		public string m_stageName;

		public MsgPrepareStageReplace(PlayerSpeed speedLevel, string stagename)
			: base(12310)
		{
			m_speedLevel = speedLevel;
			m_stageName = stagename;
		}
	}
}
