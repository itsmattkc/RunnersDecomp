namespace Message
{
	public class MsgMenuSequence : MessageBase
	{
		public enum SequeneceType
		{
			MAIN = 0,
			TITLE = 1,
			STAGE = 2,
			STAGE_CHECK = 3,
			EQUIP_ENTRANCE = 4,
			PRESENT_BOX = 5,
			DAILY_CHALLENGE = 6,
			DAILY_BATTLE = 7,
			CHARA_MAIN = 8,
			CHAO = 9,
			ITEM = 10,
			PLAY_ITEM = 11,
			OPTION = 12,
			RANKING = 13,
			RANKING_END = 14,
			INFOMATION = 0xF,
			ROULETTE = 0x10,
			CHAO_ROULETTE = 17,
			ITEM_ROULETTE = 18,
			SHOP = 19,
			EPISODE = 20,
			EPISODE_PLAY = 21,
			EPISODE_RANKING = 22,
			QUICK = 23,
			QUICK_RANKING = 24,
			PLAY_AT_EPISODE_PAGE = 25,
			MAIN_PLAY_BUTTON = 26,
			TUTORIAL_PAGE_MOVE = 27,
			CLOSE_DAILY_MISSION_WINDOW = 28,
			EVENT_TOP = 29,
			EVENT_SPECIAL = 30,
			EVENT_RAID = 0x1F,
			EVENT_COLLECT = 0x20,
			BACK = 33,
			NON = -1
		}

		private SequeneceType m_sequenece_type;

		public SequeneceType Sequenece
		{
			get
			{
				return m_sequenece_type;
			}
		}

		public MsgMenuSequence(SequeneceType sequenece_type)
			: base(57344)
		{
			m_sequenece_type = sequenece_type;
		}
	}
}
