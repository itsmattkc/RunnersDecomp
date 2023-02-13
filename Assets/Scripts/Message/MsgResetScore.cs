namespace Message
{
	public class MsgResetScore
	{
		public int m_score;

		public int m_animal;

		public int m_ring;

		public int m_red_ring;

		public int m_final_score;

		public MsgResetScore()
		{
			m_score = 0;
			m_animal = 0;
			m_ring = 0;
			m_red_ring = 0;
			m_final_score = 0;
		}

		public MsgResetScore(int score, int animal, int ring)
		{
			m_score = score;
			m_animal = animal;
			m_ring = ring;
			m_red_ring = 0;
			m_final_score = 0;
		}
	}
}
