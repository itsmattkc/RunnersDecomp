using Text;

namespace DataTable
{
	public class ChaoDataAbilityStatus
	{
		private int m_chaoId;

		private int m_abilityIndex;

		private string m_orgAbilityStatus;

		public int maxLevel
		{
			get;
			private set;
		}

		public int eventId
		{
			get;
			private set;
		}

		public float extraValue
		{
			get;
			private set;
		}

		public string bgmName
		{
			get;
			private set;
		}

		public string cueSheetName
		{
			get;
			private set;
		}

		public float[] abilityValues
		{
			get;
			private set;
		}

		public float[] bonusAbilityValues
		{
			get;
			private set;
		}

		public string details
		{
			get;
			private set;
		}

		public string loadingDetails
		{
			get;
			private set;
		}

		public string loadingLongDetails
		{
			get;
			private set;
		}

		public string growDetails
		{
			get;
			private set;
		}

		public string menuDetails
		{
			get;
			private set;
		}

		public ChaoDataAbilityStatus(string status, int id, int abilityIndex)
		{
			m_chaoId = id;
			m_abilityIndex = abilityIndex;
			m_orgAbilityStatus = status;
			update(id);
		}

		public ChaoDataAbilityStatus(ChaoDataAbilityStatus src)
		{
			m_chaoId = src.m_chaoId;
			m_orgAbilityStatus = src.m_orgAbilityStatus;
			maxLevel = src.maxLevel;
			eventId = src.eventId;
			extraValue = src.extraValue;
			bgmName = src.bgmName;
			cueSheetName = src.cueSheetName;
			abilityValues = new float[src.abilityValues.Length];
			for (int i = 0; i < src.abilityValues.Length; i++)
			{
				abilityValues[i] = src.abilityValues[i];
			}
			bonusAbilityValues = new float[src.bonusAbilityValues.Length];
			for (int j = 0; j < src.bonusAbilityValues.Length; j++)
			{
				bonusAbilityValues[j] = src.bonusAbilityValues[j];
			}
			details = src.details;
			loadingDetails = src.loadingDetails;
			growDetails = src.growDetails;
			menuDetails = src.menuDetails;
		}

		public void update(int id)
		{
			m_chaoId = id;
			int num = 4;
			string[] array = m_orgAbilityStatus.Split(',');
			maxLevel = (array.Length - num) / 2;
			if (array.Length > 0)
			{
				eventId = int.Parse(array[0]);
			}
			if (array.Length > 1)
			{
				extraValue = float.Parse(array[1]);
			}
			if (array.Length > 2)
			{
				if (array[2] == "non")
				{
					cueSheetName = string.Empty;
				}
				else
				{
					cueSheetName = array[2];
				}
			}
			if (array.Length > 3)
			{
				if (array[3] == "non")
				{
					bgmName = string.Empty;
				}
				else
				{
					bgmName = array[3];
				}
			}
			if (maxLevel > 0 && array.Length >= maxLevel * 2 + num)
			{
				abilityValues = new float[maxLevel];
				bonusAbilityValues = new float[maxLevel];
				for (int i = 0; i < maxLevel; i++)
				{
					abilityValues[i] = float.Parse(array[i + num]);
					bonusAbilityValues[i] = float.Parse(array[i + num + maxLevel]);
				}
			}
			if (!ChaoTableUtility.IsKingArthur(id) || m_abilityIndex != 1)
			{
				details = GetDetailsText("details");
				growDetails = GetDetailsText("grow_details");
				loadingDetails = GetDetailsText("loading_details");
				menuDetails = GetDetailsText("main_menu_details");
				loadingLongDetails = GetDetailsText("loading_long_details");
			}
		}

		private string GetDetailsText(string callId)
		{
			if (m_abilityIndex == 0)
			{
				return TextUtility.GetChaoText("Chao", callId + m_chaoId.ToString("D4"));
			}
			return TextUtility.GetChaoText("Chao", callId + m_chaoId.ToString("D4") + "_" + m_abilityIndex);
		}
	}
}
