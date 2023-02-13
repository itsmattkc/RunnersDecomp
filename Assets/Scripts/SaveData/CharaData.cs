namespace SaveData
{
	public class CharaData
	{
		private CharaAbility[] m_ability_array = new CharaAbility[29];

		private int[] m_status = new int[29];

		public CharaAbility[] AbilityArray
		{
			get
			{
				return m_ability_array;
			}
			set
			{
				m_ability_array = value;
			}
		}

		public int[] Status
		{
			get
			{
				return m_status;
			}
			set
			{
				m_status = value;
			}
		}

		public CharaData()
		{
			for (int i = 0; i < 29; i++)
			{
				if (i == 0)
				{
					m_status[i] = 1;
				}
				else
				{
					m_status[i] = 0;
				}
				m_ability_array[i] = new CharaAbility();
			}
		}
	}
}
