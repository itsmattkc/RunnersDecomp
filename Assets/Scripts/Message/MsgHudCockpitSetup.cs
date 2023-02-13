namespace Message
{
	public class MsgHudCockpitSetup : MessageBase
	{
		public BossType m_bossType;

		public bool m_spCrystal;

		public bool m_animal;

		public bool m_backMainMenuCheck;

		public bool m_firstTutorial;

		public MsgHudCockpitSetup(BossType bossType, bool spCrystal, bool animal, bool backMainMenuCheck, bool firstTutorial)
			: base(49155)
		{
			m_bossType = bossType;
			m_spCrystal = spCrystal;
			m_animal = animal;
			m_backMainMenuCheck = backMainMenuCheck;
			m_firstTutorial = firstTutorial;
		}
	}
}
