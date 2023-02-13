using SaveData;
using System;
using System.Collections.Generic;
using Text;

namespace DataTable
{
	public class ChaoData : IComparable
	{
		public enum Rarity
		{
			NORMAL,
			RARE,
			SRARE,
			NONE
		}

		private const string SP_TEXT_COLOR = "[ffff00]";

		private const string SP_LOADING_TEXT_COLOR = "[e0b000]";

		private const string TEXT_WHITE_COLOR = "[ffffff]";

		public const int ID_NONE = -1;

		public const int ID_MIN = 0;

		public const int LEVEL_NONE = -1;

		public const int LEVEL_MIN = 0;

		public const int LEVEL_MAX = 10;

		private int m_currentAbility;

		private string[] m_abilityStatus;

		private ChaoDataAbilityStatus[] m_abilityStatusData;

		public int id
		{
			get;
			private set;
		}

		public Rarity rarity
		{
			get;
			private set;
		}

		public CharacterAttribute charaAtribute
		{
			get;
			private set;
		}

		public int currentAbility
		{
			get
			{
				return m_currentAbility;
			}
			set
			{
				m_currentAbility = value;
				if (m_currentAbility >= abilityNum)
				{
					m_currentAbility = abilityNum - 1;
				}
				if (m_currentAbility < 0)
				{
					m_currentAbility = 0;
				}
			}
		}

		public int abilityNum
		{
			get
			{
				return m_abilityStatus.Length;
			}
		}

		public ChaoAbility[] chaoAbilitys
		{
			get;
			set;
		}

		public string[] abilityStatus
		{
			get
			{
				return m_abilityStatus;
			}
			set
			{
				m_abilityStatus = value;
				m_abilityStatusData = new ChaoDataAbilityStatus[m_abilityStatus.Length];
				for (int i = 0; i < m_abilityStatus.Length; i++)
				{
					m_abilityStatusData[i] = new ChaoDataAbilityStatus(m_abilityStatus[i], id, i);
				}
			}
		}

		public ChaoAbility chaoAbility
		{
			get
			{
				return chaoAbilitys[currentAbility];
			}
		}

		public float[] abilityValue
		{
			get
			{
				return GetAbilityValues();
			}
		}

		public float[] bonusAbilityValue
		{
			get
			{
				return GetBonusAbilityValues();
			}
		}

		public int eventId
		{
			get
			{
				return GetAbilityEventId();
			}
		}

		public float extraValue
		{
			get
			{
				return GetAbilitiyExtraValue();
			}
		}

		public string name
		{
			get;
			set;
		}

		public string nameTwolines
		{
			get;
			set;
		}

		public string details
		{
			get
			{
				return m_abilityStatusData[currentAbility].details;
			}
		}

		public string loadingDetails
		{
			get
			{
				return m_abilityStatusData[currentAbility].loadingDetails;
			}
		}

		public string loadingLongDetails
		{
			get
			{
				return m_abilityStatusData[currentAbility].loadingLongDetails;
			}
		}

		public string growDetails
		{
			get
			{
				return m_abilityStatusData[currentAbility].growDetails;
			}
		}

		public string menuDetails
		{
			get
			{
				return m_abilityStatusData[currentAbility].menuDetails;
			}
		}

		public string bgmName
		{
			get
			{
				return m_abilityStatusData[currentAbility].bgmName;
			}
		}

		public string cueSheetName
		{
			get
			{
				return m_abilityStatusData[currentAbility].cueSheetName;
			}
		}

		public int index
		{
			get;
			set;
		}

		public List<int> eventIdList
		{
			get
			{
				List<int> list = null;
				if (m_abilityStatusData != null && m_abilityStatusData.Length > 0)
				{
					list = new List<int>();
					int num = m_abilityStatusData.Length;
					for (int i = 0; i < num; i++)
					{
						list.Add(m_abilityStatusData[i].eventId);
					}
				}
				return list;
			}
		}

		public bool IsValidate
		{
			get
			{
				return id != -1 && level != -1;
			}
		}

		public int level
		{
			get
			{
				return (id == -1) ? (-1) : GetChaoLevel();
			}
		}

		public string spriteNameSuffix
		{
			get
			{
				return id.ToString("D4");
			}
		}

		public ChaoData()
		{
		}

		public ChaoData(ChaoData src)
		{
			id = src.id;
			rarity = src.rarity;
			charaAtribute = src.charaAtribute;
			currentAbility = src.currentAbility;
			chaoAbilitys = new ChaoAbility[src.chaoAbilitys.Length];
			for (int i = 0; i < src.chaoAbilitys.Length; i++)
			{
				chaoAbilitys[i] = src.chaoAbilitys[i];
			}
			m_abilityStatus = new string[src.m_abilityStatus.Length];
			for (int j = 0; j < src.m_abilityStatus.Length; j++)
			{
				m_abilityStatus[j] = src.m_abilityStatus[j];
			}
			m_abilityStatusData = new ChaoDataAbilityStatus[src.m_abilityStatusData.Length];
			for (int k = 0; k < src.m_abilityStatusData.Length; k++)
			{
				m_abilityStatusData[k] = src.m_abilityStatusData[k];
			}
			name = src.name;
			nameTwolines = src.nameTwolines;
			index = src.index;
		}

		public int CompareTo(object obj)
		{
			return id - ((ChaoData)obj).id;
		}

		public static string GetSpriteNameSuffix(int id)
		{
			return id.ToString("D4");
		}

		private int GetChaoLevel()
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null && instance.ChaoData != null && instance.ChaoData.Info != null)
			{
				SaveData.ChaoData.ChaoDataInfo[] info = instance.ChaoData.Info;
				for (int i = 0; i < info.Length; i++)
				{
					SaveData.ChaoData.ChaoDataInfo chaoDataInfo = info[i];
					if (chaoDataInfo.chao_id == id)
					{
						return chaoDataInfo.level;
					}
				}
			}
			return -1;
		}

		private int GetAbilityEventId()
		{
			int num = 0;
			ChaoDataAbilityStatus chaoDataAbilityStatus = m_abilityStatusData[currentAbility];
			return chaoDataAbilityStatus.eventId;
		}

		private float GetAbilitiyExtraValue()
		{
			float num = 0f;
			ChaoDataAbilityStatus chaoDataAbilityStatus = m_abilityStatusData[currentAbility];
			return chaoDataAbilityStatus.extraValue;
		}

		private float[] GetAbilityValues()
		{
			ChaoDataAbilityStatus chaoDataAbilityStatus = m_abilityStatusData[currentAbility];
			return chaoDataAbilityStatus.abilityValues;
		}

		private float[] GetBonusAbilityValues()
		{
			ChaoDataAbilityStatus chaoDataAbilityStatus = m_abilityStatusData[currentAbility];
			return chaoDataAbilityStatus.bonusAbilityValues;
		}

		public string GetFeaturedDetail()
		{
			string cellID = "featured_footnote" + id.ToString("D4");
			return TextUtility.GetChaoText("Chao", cellID);
		}

		public string GetDetailLevelPlusSP(int chaoLevel)
		{
			return GetDetailLevelPlusSP(chaoLevel, "[ffff00]");
		}

		public string GetLoadingPageDetailLevelPlusSP(int chaoLevel)
		{
			return GetLoadingPageDetailLevelPlusSP(chaoLevel, "[e0b000]");
		}

		private string GetDetailLevelPlusSP(int chaoLevel, string color)
		{
			string sPDetailsLevel = GetSPDetailsLevel(chaoLevel);
			if (string.IsNullOrEmpty(sPDetailsLevel))
			{
				return GetDetailsLevel(chaoLevel);
			}
			sPDetailsLevel = color + sPDetailsLevel + "[-]";
			sPDetailsLevel += "\n";
			return sPDetailsLevel + GetDetailsLevel(chaoLevel);
		}

		private string GetLoadingPageDetailLevelPlusSP(int chaoLevel, string color)
		{
			string sPLoadingLongDetailsLevel = GetSPLoadingLongDetailsLevel(chaoLevel);
			if (string.IsNullOrEmpty(sPLoadingLongDetailsLevel))
			{
				return GetLoadingLongDetailsLevel(chaoLevel);
			}
			sPLoadingLongDetailsLevel = color + sPLoadingLongDetailsLevel + "[-]";
			sPLoadingLongDetailsLevel += "\n";
			return sPLoadingLongDetailsLevel + GetLoadingLongDetailsLevel(chaoLevel);
		}

		public string GetSPLoadingLongDetailsLevel(int chaoLevel)
		{
			string result = string.Empty;
			for (int i = 0; i < abilityNum; i++)
			{
				currentAbility = i;
				if (EventManager.IsVaildEvent(eventId))
				{
					result = GetLoadingLongDetailsLevel(chaoLevel);
					break;
				}
			}
			currentAbility = 0;
			return result;
		}

		private bool IsRateText(string text)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(text) && text.IndexOf("{RATE") != -1)
			{
				result = true;
			}
			return result;
		}

		private Dictionary<string, string> CreateReplacesDic(string targetText, float param1, float param2)
		{
			List<float> list = new List<float>();
			list.Add(param1);
			list.Add(param2);
			return CreateReplacesDic(targetText, list);
		}

		private Dictionary<string, string> CreateReplacesDic(string targetText, float param1, float param2, float param3, float param4)
		{
			List<float> list = new List<float>();
			list.Add(param1);
			list.Add(param2);
			list.Add(param3);
			list.Add(param4);
			return CreateReplacesDic(targetText, list);
		}

		private Dictionary<string, string> CreateReplacesDic(string targetText, List<float> paramList)
		{
			int num = 0;
			if (paramList == null || paramList.Count <= 0)
			{
				return null;
			}
			num = paramList.Count;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool flag = IsRateText(targetText);
			int num2 = 0;
			int num3 = 0;
			float num4 = 0f;
			for (int i = 0; i < num; i++)
			{
				num2 = i + 1;
				dictionary.Add(value: ((int)paramList[i]).ToString(), key: "{PARAM" + num2 + "}");
				if (flag)
				{
					dictionary.Add(value: ((paramList[i] + 100f) / 100f).ToString(), key: "{RATE" + num2 + "}");
				}
			}
			return dictionary;
		}

		public string GetLoadingLongDetailsLevel(int chaoLevel)
		{
			if (ChaoTableUtility.IsKingArthur(id))
			{
				return GetKingArtherDetailsLevel(chaoLevel, true);
			}
			string loadingLongDetails = this.loadingLongDetails;
			float param = 0f;
			float param2 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(loadingLongDetails, param, param2);
			return TextUtility.Replaces(loadingLongDetails, replaceDic);
		}

		public string GetDetailsLevel(int chaoLevel)
		{
			if (ChaoTableUtility.IsKingArthur(id))
			{
				return GetKingArtherDetailsLevel(chaoLevel, false);
			}
			string details = this.details;
			float param = 0f;
			float param2 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(details, param, param2);
			return TextUtility.Replaces(details, replaceDic);
		}

		public string GetKingArtherDetailsLevel(int chaoLevel, bool loadingLoingDetalFlag)
		{
			string text = (!loadingLoingDetalFlag) ? details : loadingLongDetails;
			float param = 0f;
			float param2 = 0f;
			float param3 = 0f;
			float param4 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 1;
				if (chaoLevel < abilityValue.Length)
				{
					param3 = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param4 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 0;
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(text, param, param2, param3, param4);
			return TextUtility.Replaces(text, replaceDic);
		}

		public string GetSPDetailsLevel(int chaoLevel)
		{
			string result = string.Empty;
			for (int i = 0; i < abilityNum; i++)
			{
				currentAbility = i;
				if (EventManager.IsVaildEvent(eventId))
				{
					result = GetDetailsLevel(chaoLevel);
					break;
				}
			}
			currentAbility = 0;
			return result;
		}

		public string GetGrowDetailLevelPlusSP(int chaoLevel)
		{
			string sPGrowDetailsLevel = GetSPGrowDetailsLevel(chaoLevel);
			if (string.IsNullOrEmpty(sPGrowDetailsLevel))
			{
				return GetGrowDetailsLevel(chaoLevel);
			}
			sPGrowDetailsLevel += "\n\n";
			return sPGrowDetailsLevel + GetGrowDetailsLevel(chaoLevel);
		}

		public string GetGrowDetailsLevel(int chaoLevel)
		{
			if (ChaoTableUtility.IsKingArthur(id))
			{
				return GetKingArtherGrowDetailsLevel(chaoLevel);
			}
			float param = 0f;
			float param2 = 0f;
			float param3 = 0f;
			float param4 = 0f;
			int num = chaoLevel - 1;
			if (num >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
				if (num < abilityValue.Length)
				{
					param3 = abilityValue[num];
				}
				if (num < bonusAbilityValue.Length)
				{
					param4 = bonusAbilityValue[num];
				}
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(growDetails, param, param2, param3, param4);
			return TextUtility.Replaces(growDetails, replaceDic);
		}

		private string GetKingArtherGrowDetailsLevel(int chaoLevel)
		{
			float[] array = new float[8];
			int num = chaoLevel - 1;
			if (num >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					array[0] = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					array[1] = bonusAbilityValue[chaoLevel];
				}
				if (num < abilityValue.Length)
				{
					array[2] = abilityValue[num];
				}
				if (num < bonusAbilityValue.Length)
				{
					array[3] = bonusAbilityValue[num];
				}
				currentAbility = 1;
				if (chaoLevel < abilityValue.Length)
				{
					array[4] = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					array[5] = bonusAbilityValue[chaoLevel];
				}
				if (num < abilityValue.Length)
				{
					array[6] = abilityValue[num];
				}
				if (num < bonusAbilityValue.Length)
				{
					array[7] = bonusAbilityValue[num];
				}
				currentAbility = 0;
			}
			List<float> paramList = new List<float>(array);
			Dictionary<string, string> replaceDic = CreateReplacesDic(growDetails, paramList);
			return TextUtility.Replaces(growDetails, replaceDic);
		}

		public string GetSPGrowDetailsLevel(int chaoLevel)
		{
			string text = string.Empty;
			for (int i = 0; i < abilityNum; i++)
			{
				currentAbility = i;
				if (EventManager.IsVaildEvent(eventId))
				{
					text = GetGrowDetailsLevel(chaoLevel);
					if (!string.IsNullOrEmpty(text))
					{
						text = "[ffff00]" + text + "[ffffff]";
					}
					break;
				}
			}
			currentAbility = 0;
			return text;
		}

		public string GetLoadingDetailsLevel(int chaoLevel)
		{
			if (ChaoTableUtility.IsKingArthur(id))
			{
				return GetKingArtherLoadingDetailsLevel(chaoLevel);
			}
			float param = 0f;
			float param2 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(loadingDetails, param, param2);
			return TextUtility.Replaces(loadingDetails, replaceDic);
		}

		public string GetKingArtherLoadingDetailsLevel(int chaoLevel)
		{
			float param = 0f;
			float param2 = 0f;
			float param3 = 0f;
			float param4 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 1;
				if (chaoLevel < abilityValue.Length)
				{
					param3 = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param4 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 0;
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(loadingDetails, param, param2, param3, param4);
			return TextUtility.Replaces(loadingDetails, replaceDic);
		}

		public string GetSPLoadingDetailsLevel(int chaoLevel)
		{
			string result = string.Empty;
			for (int i = 0; i < abilityNum; i++)
			{
				currentAbility = i;
				if (EventManager.IsVaildEvent(eventId))
				{
					result = GetLoadingDetailsLevel(chaoLevel);
					break;
				}
			}
			currentAbility = 0;
			return result;
		}

		public string GetMainMenuDetailsLevel(int chaoLevel)
		{
			if (ChaoTableUtility.IsKingArthur(id))
			{
				return GetKingArtherMainMenuDetailsLevel(chaoLevel);
			}
			float param = 0f;
			float param2 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(menuDetails, param, param2);
			return TextUtility.Replaces(menuDetails, replaceDic);
		}

		public string GetKingArtherMainMenuDetailsLevel(int chaoLevel)
		{
			float param = 0f;
			float param2 = 0f;
			float param3 = 0f;
			float param4 = 0f;
			if (chaoLevel >= 0)
			{
				if (chaoLevel < abilityValue.Length)
				{
					param = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param2 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 1;
				if (chaoLevel < abilityValue.Length)
				{
					param3 = abilityValue[chaoLevel];
				}
				if (chaoLevel < bonusAbilityValue.Length)
				{
					param4 = bonusAbilityValue[chaoLevel];
				}
				currentAbility = 0;
			}
			Dictionary<string, string> replaceDic = CreateReplacesDic(menuDetails, param, param2, param3, param4);
			return TextUtility.Replaces(menuDetails, replaceDic);
		}

		public string GetSPMainMenuDetailsLevel(int chaoLevel)
		{
			string text = string.Empty;
			for (int i = 0; i < abilityNum; i++)
			{
				currentAbility = i;
				if (EventManager.IsVaildEvent(eventId))
				{
					text = GetMainMenuDetailsLevel(chaoLevel);
					if (!string.IsNullOrEmpty(text))
					{
						text = "[ffff00]" + text + "[-]";
					}
					break;
				}
			}
			currentAbility = 0;
			if (string.IsNullOrEmpty(text))
			{
				text = GetMainMenuDetailsLevel(chaoLevel);
			}
			return text;
		}

		public void StatusUpdate()
		{
			for (int i = 0; i < m_abilityStatusData.Length; i++)
			{
				m_abilityStatusData[i].update(id);
			}
		}

		public bool SetChaoAbility(int abilityEventId)
		{
			bool result = false;
			if (abilityNum > 1)
			{
				for (int i = 0; i < abilityNum; i++)
				{
					ChaoDataAbilityStatus chaoDataAbilityStatus = m_abilityStatusData[i];
					if (abilityEventId == chaoDataAbilityStatus.eventId)
					{
						currentAbility = i;
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool SetChaoAbilityIndex(int abilityIndex)
		{
			bool result = false;
			if (abilityNum > 1 && abilityIndex >= 0 && abilityNum > abilityIndex)
			{
				currentAbility = abilityIndex;
				result = true;
			}
			return result;
		}

		public void accept(ref ChaoDataVisitorBase visitor)
		{
			visitor.visit(this);
		}

		public static int ChaoCompareById(ChaoData x, ChaoData y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			return x.id - y.id;
		}

		public void SetDummyData()
		{
			id = 0;
			rarity = Rarity.NORMAL;
			charaAtribute = CharacterAttribute.SPEED;
			chaoAbilitys = new ChaoAbility[1];
			chaoAbilitys[0] = ChaoAbility.ALL_BONUS_COUNT;
			m_abilityStatus = new string[1];
			m_abilityStatus[0] = "dummy";
			m_abilityStatusData = new ChaoDataAbilityStatus[1];
			m_abilityStatusData[0] = new ChaoDataAbilityStatus("0,0", 0, 0);
			currentAbility = 0;
		}
	}
}
