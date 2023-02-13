using DataTable;
using Message;
using System.Collections.Generic;
using UnityEngine;

public class StageAbilityManager : MonoBehaviour
{
	public struct BonusRate
	{
		public float score;

		public float animal;

		public float ring;

		public float red_ring;

		public float distance;

		public float sp_crystal;

		public float raid_boss_ring;

		public float final_score;

		public void Reset()
		{
			score = 0f;
			animal = 0f;
			ring = 0f;
			red_ring = 0f;
			distance = 0f;
			sp_crystal = 0f;
			raid_boss_ring = 0f;
			final_score = 0f;
		}
	}

	private class ChaoAbilityInfo
	{
		public class AbilityData
		{
			public ChaoAbility ability = ChaoAbility.UNKNOWN;

			public float bonus;

			public float normal;

			public float extra;
		}

		public List<AbilityData> abilityDatas = new List<AbilityData>();

		public CharacterAttribute attribute = CharacterAttribute.UNKNOWN;

		public void Init()
		{
			abilityDatas.Clear();
			attribute = CharacterAttribute.UNKNOWN;
		}

		public void AddAbility(ChaoAbility ability, float bonus, float normal, float extra)
		{
			AbilityData abilityData = new AbilityData();
			abilityData.ability = ability;
			abilityData.bonus = bonus;
			abilityData.normal = normal;
			abilityData.extra = extra;
			abilityDatas.Add(abilityData);
		}
	}

	private enum ResType
	{
		CHAO,
		CHARA,
		NUM
	}

	private const float CHARA_ABILITY_BONUS_VALUE = 20f;

	[Header("debugFlag にチェックを入れると、指定した値で設定できます")]
	[SerializeField]
	private bool m_debugFlag;

	[SerializeField]
	private float m_debugCampaignBonusValue;

	private bool[] m_boostItemValidFlag = new bool[3];

	private ChaoAbilityInfo m_mainChaoInfo = new ChaoAbilityInfo();

	private ChaoAbilityInfo m_subChaoInfo = new ChaoAbilityInfo();

	private float[] m_mainCharaAbilityValue = new float[10];

	private float[] m_subCharaAbilityValue = new float[10];

	private float[] m_mainCharaOverlapBonus = new float[5];

	private float[] m_subCharaOverlapBonus = new float[5];

	private float[] m_mainTeamAbilityBonusValue = new float[6];

	private float[] m_subTeamAbilityBonusValue = new float[6];

	private TeamAttributeCategory m_mainTeamAttributeCategory = TeamAttributeCategory.NONE;

	private TeamAttributeCategory m_subTeamAttributeCategory = TeamAttributeCategory.NONE;

	private float m_boostBonusValue;

	private float m_campaignBonusValue;

	private BonusRate m_count_chao_bonus_value_rate;

	private BonusRate m_main_chao_bonus_value_rate;

	private BonusRate m_sub_chao_bonus_value_rate;

	private BonusRate m_main_chara_bonus_value_rate;

	private BonusRate m_sub_chara_bonus_value_rate;

	private BonusRate m_bonus_value_rate;

	private BonusRate m_mileage_bonus_score_rate;

	private PlayerInformation m_playerInformation;

	private float m_chaoCountBonus;

	private int m_chaoCount;

	private int m_getMainChaoRecoveryRingCount;

	private int m_getSubChaoRecoveryRingCount;

	private bool m_initFlag;

	private static StageAbilityManager instance = null;

	private static string CHAODATA_SCENENAME = "ChaoDataTable";

	private static string CHAODATA_NAME = "ChaoTable";

	private static string CHARADATA_SCENENAME = "CharaAbilityDataTable";

	private static string CHARADATA_NAME = "ImportAbilityTable";

	private static readonly List<ResourceSceneLoader.ResourceInfo> m_loadInfo = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, CHAODATA_SCENENAME, true, false, true, CHAODATA_NAME),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, CHARADATA_SCENENAME, true, false, true, CHARADATA_NAME)
	};

	public static StageAbilityManager Instance
	{
		get
		{
			return instance;
		}
	}

	public BonusRate BonusValueRate
	{
		get
		{
			return m_bonus_value_rate;
		}
	}

	public BonusRate MainChaoBonusValueRate
	{
		get
		{
			return m_main_chao_bonus_value_rate;
		}
	}

	public BonusRate SubChaoBonusValueRate
	{
		get
		{
			return m_sub_chao_bonus_value_rate;
		}
	}

	public BonusRate CountChaoBonusValueRate
	{
		get
		{
			return m_count_chao_bonus_value_rate;
		}
	}

	public BonusRate MainCharaBonusValueRate
	{
		get
		{
			return m_main_chara_bonus_value_rate;
		}
	}

	public BonusRate SubCharaBonusValueRate
	{
		get
		{
			return m_sub_chara_bonus_value_rate;
		}
	}

	public float CampaignValueRate
	{
		get
		{
			return m_campaignBonusValue;
		}
	}

	public BonusRate MileageBonusScoreRate
	{
		get
		{
			return m_mileage_bonus_score_rate;
		}
	}

	public float SpecialCrystalRate
	{
		get
		{
			return m_bonus_value_rate.sp_crystal;
		}
	}

	public float RadBossRingRate
	{
		get
		{
			return m_bonus_value_rate.raid_boss_ring;
		}
	}

	public float[] CharaAbility
	{
		get
		{
			CheckPlayerInformation();
			if (m_playerInformation != null)
			{
				if (m_playerInformation.PlayingCharaType == PlayingCharacterType.MAIN)
				{
					return m_mainCharaAbilityValue;
				}
				return m_subCharaAbilityValue;
			}
			return m_mainCharaAbilityValue;
		}
	}

	public bool[] BoostItemValidFlag
	{
		get
		{
			return m_boostItemValidFlag;
		}
		set
		{
			m_boostItemValidFlag = value;
		}
	}

	public void RequestPlayChaoEffect(ChaoAbility ability)
	{
		PlayChaoEffect(ability);
	}

	public void RequestPlayChaoEffect(ChaoAbility ability, ChaoType chaoType)
	{
		PlayChaoEffect(ability, chaoType);
	}

	public void RequestPlayChaoEffect(ChaoAbility[] abilities)
	{
		foreach (ChaoAbility ability in abilities)
		{
			PlayChaoEffect(ability);
		}
	}

	public void RequestPlayChaoEffect(ChaoAbility[] abilities, ChaoType chaoType)
	{
		foreach (ChaoAbility ability in abilities)
		{
			PlayChaoEffect(ability, chaoType);
		}
	}

	public void RequestStopChaoEffect(ChaoAbility ability)
	{
		StopChaoEffect(ability);
	}

	public void RequestStopChaoEffect(ChaoAbility[] abilities)
	{
		foreach (ChaoAbility ability in abilities)
		{
			StopChaoEffect(ability);
		}
	}

	public bool HasChaoAbility(ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			return HasChaoAbility(m_mainChaoInfo, ability) || HasChaoAbility(m_subChaoInfo, ability);
		}
		return false;
	}

	public bool HasChaoAbility(ChaoAbility ability, ChaoType type)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			switch (type)
			{
			case ChaoType.MAIN:
				return HasChaoAbility(m_mainChaoInfo, ability);
			case ChaoType.SUB:
				return HasChaoAbility(m_subChaoInfo, ability);
			}
		}
		return false;
	}

	private bool HasChaoAbility(ChaoAbilityInfo info, ChaoAbility ability)
	{
		foreach (ChaoAbilityInfo.AbilityData abilityData in info.abilityDatas)
		{
			if (abilityData.ability == ability)
			{
				return true;
			}
		}
		return false;
	}

	public int GetChaoAbliltyValue(ChaoAbility ability, int src_value)
	{
		if (ability < ChaoAbility.NUM)
		{
			return (int)CalcPlusAbliltyBonusValue(ability, src_value);
		}
		return 0;
	}

	public float GetChaoAbliltyValue(ChaoAbility ability, float src_value)
	{
		if (ability < ChaoAbility.NUM)
		{
			return CalcPlusAbliltyBonusValue(ability, src_value);
		}
		return 0f;
	}

	public float GetChaoAbilityValue(ChaoAbility ability)
	{
		float chaoAbilityValue = GetChaoAbilityValue(m_mainChaoInfo, ability);
		return chaoAbilityValue + GetChaoAbilityValue(m_subChaoInfo, ability);
	}

	public float GetChaoAbilityValue(ChaoAbility ability, ChaoType type)
	{
		switch (type)
		{
		case ChaoType.MAIN:
			return GetChaoAbilityValue(m_mainChaoInfo, ability);
		case ChaoType.SUB:
			return GetChaoAbilityValue(m_subChaoInfo, ability);
		default:
			return 0f;
		}
	}

	private float GetChaoAbilityValue(ChaoAbilityInfo info, ChaoAbility ability)
	{
		foreach (ChaoAbilityInfo.AbilityData abilityData in info.abilityDatas)
		{
			if (abilityData.ability == ability)
			{
				return (!IsSameAttribute(info.attribute)) ? abilityData.normal : abilityData.bonus;
			}
		}
		return 0f;
	}

	public float GetChaoAbilityExtraValue(ChaoAbility ability)
	{
		return GetChaoAbilityExtraValue(ability, ChaoType.MAIN) + GetChaoAbilityExtraValue(ability, ChaoType.SUB);
	}

	public float GetChaoAbilityExtraValue(ChaoAbility ability, ChaoType type)
	{
		ChaoAbilityInfo chaoAbilityInfo = null;
		switch (type)
		{
		case ChaoType.MAIN:
			chaoAbilityInfo = m_mainChaoInfo;
			break;
		case ChaoType.SUB:
			chaoAbilityInfo = m_subChaoInfo;
			break;
		}
		if (chaoAbilityInfo != null)
		{
			foreach (ChaoAbilityInfo.AbilityData abilityData in chaoAbilityInfo.abilityDatas)
			{
				if (abilityData.ability == ability)
				{
					return abilityData.extra;
				}
			}
		}
		return 0f;
	}

	public float GetCharacterOverlapBonusValue(OverlapBonusType bonusType)
	{
		if (OverlapBonusType.SCORE <= bonusType && bonusType < OverlapBonusType.NUM)
		{
			return m_mainCharaOverlapBonus[(int)bonusType] + m_subCharaOverlapBonus[(int)bonusType];
		}
		return 0f;
	}

	public float GetCharacterOverlapBonusValue(OverlapBonusType bonusType, bool mainChara)
	{
		if (OverlapBonusType.SCORE <= bonusType && bonusType < OverlapBonusType.NUM)
		{
			if (mainChara)
			{
				return m_mainCharaOverlapBonus[(int)bonusType];
			}
			return m_subCharaOverlapBonus[(int)bonusType];
		}
		return 0f;
	}

	public int GetChaoAndTeamAbliltyScoreValue(List<ChaoAbility> abilityList, TeamAttributeBonusType bonusType, int src_value)
	{
		float num = 0f;
		int count = abilityList.Count;
		for (int i = 0; i < count; i++)
		{
			num += GetChaoAbilityValue(m_mainChaoInfo, abilityList[i]);
			num += GetChaoAbilityValue(m_subChaoInfo, abilityList[i]);
		}
		num += GetTeamAbilityBonusValue(bonusType);
		return (int)GetPlusPercentBonusValue(num, src_value);
	}

	public int GetChaoAndEnemyScoreValue(List<ChaoAbility> abilityList, int src_value)
	{
		float num = 0f;
		int count = abilityList.Count;
		for (int i = 0; i < count; i++)
		{
			num += GetChaoAbilityValue(m_mainChaoInfo, abilityList[i]);
			num += GetChaoAbilityValue(m_subChaoInfo, abilityList[i]);
		}
		num += GetTeamAbilityBonusValue(TeamAttributeBonusType.ENEMY);
		num += GetCharacterOverlapBonusValue(OverlapBonusType.ENEMY);
		return (int)GetPlusPercentBonusValue(num, src_value);
	}

	private float GetTeamAbilityBonusValue(TeamAttributeBonusType bonusType)
	{
		if (bonusType < TeamAttributeBonusType.NUM && bonusType != TeamAttributeBonusType.NONE)
		{
			return m_mainTeamAbilityBonusValue[(int)bonusType] + m_subTeamAbilityBonusValue[(int)bonusType];
		}
		return 0f;
	}

	public long GetTeamAbliltyResultScore(long score, int coefficient)
	{
		float num = 1f;
		int num2 = 0;
		if (m_mainTeamAttributeCategory == TeamAttributeCategory.EASY_SPEED)
		{
			num2++;
		}
		if (m_subTeamAttributeCategory == TeamAttributeCategory.EASY_SPEED)
		{
			num2++;
		}
		switch (num2)
		{
		case 1:
			num = 0.8f;
			break;
		case 2:
			num = 0.64f;
			break;
		}
		double num3 = (double)coefficient * (double)num;
		double num4 = (double)score * num3;
		long num5 = (long)num4;
		if ((double)num5 < num4)
		{
			num4 += 1.0;
		}
		return (long)num4;
	}

	public bool IsEasySpeed(PlayingCharacterType type)
	{
		switch (type)
		{
		case PlayingCharacterType.MAIN:
			if (m_mainTeamAttributeCategory == TeamAttributeCategory.EASY_SPEED)
			{
				return true;
			}
			break;
		case PlayingCharacterType.SUB:
			if (m_subTeamAttributeCategory == TeamAttributeCategory.EASY_SPEED)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public float GetTeamAbliltyTimeScale(float timeScale)
	{
		float num = (m_mainTeamAbilityBonusValue[5] + m_subTeamAbilityBonusValue[5]) * 0.01f;
		return timeScale - num;
	}

	public float GetItemTimePlusAblityBonus(ItemType itemType)
	{
		return CalcItemPlusAbliltyBonusValue(itemType);
	}

	public float GetChaoCountBonusValue()
	{
		return m_chaoCountBonus;
	}

	public int GetChaoCount()
	{
		return m_chaoCount;
	}

	public void RecalcAbilityVaue()
	{
		InitParam();
		CalcChaoCountBonus();
		SetCharacterAbility();
		SetCharacterOverlapBonus();
		SetTeamAbility();
		SetChaoAbility();
		SetChaoBonusValueRate();
		SetCharacterBonusValueRate();
		SetPampaignBonusValueRate();
	}

	public GameObject GetLostRingChao()
	{
		GameObject result = null;
		if (HasChaoAbility(ChaoAbility.RECOVERY_RING))
		{
			bool flag = true;
			bool flag2 = false;
			int num = (int)GetChaoAbilityValue(m_mainChaoInfo, ChaoAbility.RECOVERY_RING);
			int num2 = (int)GetChaoAbilityValue(m_subChaoInfo, ChaoAbility.RECOVERY_RING);
			if (m_getMainChaoRecoveryRingCount < num)
			{
				flag2 = true;
			}
			else if (m_getSubChaoRecoveryRingCount < num2)
			{
				flag2 = true;
				flag = false;
			}
			string b = (!flag) ? "SubChao" : "MainChao";
			if (flag2)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Chao");
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					if (gameObject.name == b)
					{
						result = gameObject;
						break;
					}
				}
			}
		}
		return result;
	}

	public void SetLostRingCount(int ring)
	{
		bool flag = false;
		int num = 0;
		int num2 = (int)GetChaoAbilityValue(m_mainChaoInfo, ChaoAbility.RECOVERY_RING);
		int num3 = (int)GetChaoAbilityValue(m_subChaoInfo, ChaoAbility.RECOVERY_RING);
		if (m_getMainChaoRecoveryRingCount < num2)
		{
			int num4 = num2 - m_getMainChaoRecoveryRingCount;
			if (num4 < ring)
			{
				num = num4;
				ring -= num4;
			}
			else
			{
				num = ring;
				ring = 0;
			}
			m_getMainChaoRecoveryRingCount += num;
			flag = true;
		}
		if (ring > 0 && m_getSubChaoRecoveryRingCount < num3)
		{
			int num5 = num3 - m_getSubChaoRecoveryRingCount;
			if (num5 < ring)
			{
				num += num5;
				m_getSubChaoRecoveryRingCount += num5;
			}
			else
			{
				num += ring;
				m_getSubChaoRecoveryRingCount += ring;
			}
			flag = true;
		}
		if (flag)
		{
			MsgAddStockRing value = new MsgAddStockRing(num);
			GameObjectUtil.SendDelayedMessageFindGameObject("HudCockpit", "OnAddStockRing", value);
			if (StageScoreManager.Instance != null)
			{
				StageScoreManager.Instance.AddRecoveryRingCount(num);
			}
		}
	}

	private void Setup()
	{
		RecalcAbilityVaue();
		SetMileageBonusScoreRate();
	}

	protected void Awake()
	{
		base.tag = "Manager";
		SetInstance();
	}

	private void Start()
	{
		InitParam();
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child != null)
			{
				GameObject gameObject = child.gameObject;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
		}
		for (int j = 0; j < 3; j++)
		{
			m_boostItemValidFlag[j] = false;
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void InitParam()
	{
		for (int i = 0; i < 6; i++)
		{
			m_mainTeamAbilityBonusValue[i] = 0f;
			m_subTeamAbilityBonusValue[i] = 0f;
		}
		for (int j = 0; j < 10; j++)
		{
			m_mainCharaAbilityValue[j] = 0f;
			m_subCharaAbilityValue[j] = 0f;
		}
		for (int k = 0; k < 5; k++)
		{
			m_mainCharaOverlapBonus[k] = 0f;
			m_subCharaOverlapBonus[k] = 0f;
		}
		m_mainTeamAttributeCategory = TeamAttributeCategory.NONE;
		m_subTeamAttributeCategory = TeamAttributeCategory.NONE;
		m_playerInformation = null;
		m_mainChaoInfo.Init();
		m_subChaoInfo.Init();
		m_chaoCountBonus = 0f;
		m_chaoCount = 0;
		m_count_chao_bonus_value_rate.Reset();
		m_main_chao_bonus_value_rate.Reset();
		m_sub_chao_bonus_value_rate.Reset();
		m_main_chara_bonus_value_rate.Reset();
		m_sub_chara_bonus_value_rate.Reset();
		m_bonus_value_rate.Reset();
		m_campaignBonusValue = 0f;
		m_boostBonusValue = 0f;
		if (!m_initFlag)
		{
			m_mileage_bonus_score_rate.Reset();
		}
		m_getMainChaoRecoveryRingCount = 0;
		m_getSubChaoRecoveryRingCount = 0;
		m_initFlag = true;
	}

	private void SetCharacterAbility()
	{
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (!(saveDataManager != null))
		{
			return;
		}
		CharaType mainChara = saveDataManager.PlayerData.MainChara;
		if (CheckCharaType(mainChara))
		{
			SetCharaAbilityValue(ref m_mainCharaAbilityValue, saveDataManager.CharaData.AbilityArray[(int)mainChara]);
		}
		if (m_boostItemValidFlag[2])
		{
			CharaType subChara = saveDataManager.PlayerData.SubChara;
			if (CheckCharaType(subChara))
			{
				SetCharaAbilityValue(ref m_subCharaAbilityValue, saveDataManager.CharaData.AbilityArray[(int)subChara]);
			}
		}
	}

	private void SetTeamAbility()
	{
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (!(saveDataManager != null) || !(CharacterDataNameInfo.Instance != null))
		{
			return;
		}
		CharaType mainChara = saveDataManager.PlayerData.MainChara;
		if (CheckCharaType(mainChara))
		{
			SetTeamAbilityBonusValue(ref m_mainTeamAbilityBonusValue, ref m_mainTeamAttributeCategory, CharacterDataNameInfo.Instance.GetDataByID(mainChara));
		}
		if (m_boostItemValidFlag[2])
		{
			CharaType subChara = saveDataManager.PlayerData.SubChara;
			if (CheckCharaType(subChara))
			{
				SetTeamAbilityBonusValue(ref m_subTeamAbilityBonusValue, ref m_subTeamAttributeCategory, CharacterDataNameInfo.Instance.GetDataByID(subChara));
			}
		}
	}

	private void SetCharacterOverlapBonus()
	{
		if (!(ResourceManager.Instance != null))
		{
			return;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ETC, "OverlapBonusTable");
		if (!(gameObject != null))
		{
			return;
		}
		OverlapBonusTable component = gameObject.GetComponent<OverlapBonusTable>();
		if (!(component != null) || !(SaveDataManager.Instance != null))
		{
			return;
		}
		CharaType mainChara = SaveDataManager.Instance.PlayerData.MainChara;
		if (CheckCharaType(mainChara))
		{
			SetOverlapBonusValue(ref m_mainCharaOverlapBonus, mainChara, component);
		}
		if (m_boostItemValidFlag[2])
		{
			CharaType subChara = SaveDataManager.Instance.PlayerData.SubChara;
			if (CheckCharaType(subChara))
			{
				SetOverlapBonusValue(ref m_subCharaOverlapBonus, subChara, component);
			}
		}
	}

	private void SetOverlapBonusValue(ref float[] overlapBonus, CharaType charaType, OverlapBonusTable overlapBonusTable)
	{
		if (ServerInterface.PlayerState == null || !(overlapBonusTable != null))
		{
			return;
		}
		ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(charaType);
		if (serverCharacterState == null)
		{
			return;
		}
		int star = serverCharacterState.star;
		if (overlapBonus.Length == 5)
		{
			for (int i = 0; i < 5; i++)
			{
				overlapBonus[i] = overlapBonusTable.GetStarBonusList(charaType, star, (OverlapBonusType)i);
			}
		}
	}

	private bool CheckCharaType(CharaType chara_type)
	{
		return CharaType.SONIC <= chara_type && chara_type < CharaType.NUM;
	}

	private void SetCharaAbilityValue(ref float[] ability_value, CharaAbility ability)
	{
		if (ability == null)
		{
			return;
		}
		ImportAbilityTable importAbilityTable = ImportAbilityTable.GetInstance();
		if (importAbilityTable != null)
		{
			for (int i = 0; i < 10; i++)
			{
				ability_value[i] = importAbilityTable.GetAbilityPotential((AbilityType)i, (int)ability.Ability[i]);
			}
		}
	}

	private void SetTeamAbilityBonusValue(ref float[] bonusValue, ref TeamAttributeCategory category, CharacterDataNameInfo.Info info)
	{
		for (int i = 0; i < 6; i++)
		{
			bonusValue[i] = 0f;
		}
		if (info != null)
		{
			category = info.m_teamAttributeCategory;
			if (info.m_mainAttributeBonus != TeamAttributeBonusType.NONE && info.m_mainAttributeBonus < TeamAttributeBonusType.NUM)
			{
				bonusValue[(int)info.m_mainAttributeBonus] = info.TeamAttributeValue;
			}
			if (info.m_subAttributeBonus != TeamAttributeBonusType.NONE && info.m_subAttributeBonus < TeamAttributeBonusType.NUM)
			{
				bonusValue[(int)info.m_subAttributeBonus] = info.TeamAttributeSubValue;
			}
		}
	}

	private void SetChaoAbility()
	{
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (saveDataManager != null)
		{
			bool mainChaoFlag = true;
			SetChaoAbilityData(saveDataManager.PlayerData.MainChaoID, mainChaoFlag);
			bool mainChaoFlag2 = false;
			SetChaoAbilityData(saveDataManager.PlayerData.SubChaoID, mainChaoFlag2);
		}
	}

	private void SetChaoAbilityData(int chaoId, bool mainChaoFlag)
	{
		ChaoData chaoData = ChaoTable.GetChaoData(chaoId);
		if (chaoData == null)
		{
			return;
		}
		int level = chaoData.level;
		if (level < 0)
		{
			return;
		}
		int abilityNum = chaoData.abilityNum;
		for (int i = 0; i < abilityNum; i++)
		{
			chaoData.currentAbility = i;
			bool flag = false;
			int eventId = chaoData.eventId;
			if (eventId > 0)
			{
				if (EventManager.IsVaildEvent(eventId) && EventManager.Instance != null)
				{
					switch (chaoData.chaoAbility)
					{
					case ChaoAbility.SPECIAL_CRYSTAL_COUNT:
					case ChaoAbility.COMBO_ALL_SPECIAL_CRYSTAL:
					case ChaoAbility.SPECIAL_CRYSTAL_RATE:
						if (EventManager.Instance.IsSpecialStage())
						{
							flag = true;
						}
						break;
					case ChaoAbility.RAID_BOSS_RING_COUNT:
					case ChaoAbility.AGGRESSIVITY_UP_FOR_RAID_BOSS:
						if (EventManager.Instance.IsRaidBossStage())
						{
							flag = true;
						}
						break;
					}
				}
			}
			else
			{
				flag = true;
				if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
				{
					ChaoAbility chaoAbility = chaoData.chaoAbility;
					if (chaoAbility == ChaoAbility.RING_COUNT || chaoAbility == ChaoAbility.COMBO_SUPER_RING || chaoAbility == ChaoAbility.BOSS_SUPER_RING_RATE || chaoAbility == ChaoAbility.RECOVERY_RING)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				float num = chaoData.bonusAbilityValue[level];
				float num2 = chaoData.abilityValue[level];
				float extraValue = chaoData.extraValue;
				if (chaoData.chaoAbility == ChaoAbility.RECOVERY_RING)
				{
					num = Mathf.Ceil(num);
					num2 = Mathf.Ceil(num2);
				}
				if (mainChaoFlag)
				{
					m_mainChaoInfo.AddAbility(chaoData.chaoAbility, num, num2, extraValue);
					m_mainChaoInfo.attribute = chaoData.charaAtribute;
				}
				else
				{
					m_subChaoInfo.AddAbility(chaoData.chaoAbility, num, num2, extraValue);
					m_subChaoInfo.attribute = chaoData.charaAtribute;
				}
			}
		}
		chaoData.currentAbility = 0;
	}

	private void SetBonusVale(ref BonusRate bonusRate, ChaoAbilityInfo info)
	{
		foreach (ChaoAbilityInfo.AbilityData abilityData in info.abilityDatas)
		{
			float num = 0f;
			switch (abilityData.ability)
			{
			case ChaoAbility.ALL_BONUS_COUNT:
			case ChaoAbility.SCORE_COUNT:
			case ChaoAbility.RING_COUNT:
			case ChaoAbility.RED_RING_COUNT:
			case ChaoAbility.ANIMAL_COUNT:
			case ChaoAbility.RUNNIGN_DISTANCE:
			case ChaoAbility.SPECIAL_CRYSTAL_COUNT:
			case ChaoAbility.RAID_BOSS_RING_COUNT:
				num = ((!IsSameAttributeFromSaveData(info.attribute, true)) ? abilityData.normal : abilityData.bonus);
				num *= 0.01f;
				break;
			}
			switch (abilityData.ability)
			{
			case ChaoAbility.ALL_BONUS_COUNT:
				bonusRate.score += num;
				bonusRate.ring += num;
				bonusRate.animal += num;
				bonusRate.distance += num;
				break;
			case ChaoAbility.SCORE_COUNT:
				bonusRate.score += num;
				break;
			case ChaoAbility.RING_COUNT:
				bonusRate.ring += num;
				break;
			case ChaoAbility.ANIMAL_COUNT:
				bonusRate.animal += num;
				break;
			case ChaoAbility.RUNNIGN_DISTANCE:
				bonusRate.distance += num;
				break;
			case ChaoAbility.SPECIAL_CRYSTAL_COUNT:
				bonusRate.sp_crystal += num;
				break;
			case ChaoAbility.RAID_BOSS_RING_COUNT:
				bonusRate.raid_boss_ring += num;
				break;
			}
		}
	}

	private bool IsSameAttribute(CharacterAttribute chaoAtribute)
	{
		CheckPlayerInformation();
		if (m_playerInformation != null)
		{
			return m_playerInformation.PlayerAttribute == chaoAtribute;
		}
		return false;
	}

	private bool IsSameAttributeFromSaveData(CharacterAttribute attribute, bool subCharaCompare)
	{
		CharaType charaType = CharaType.UNKNOWN;
		CharaType charaType2 = CharaType.UNKNOWN;
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (saveDataManager != null)
		{
			charaType = saveDataManager.PlayerData.MainChara;
			if (m_boostItemValidFlag[2])
			{
				charaType2 = saveDataManager.PlayerData.SubChara;
			}
		}
		if (IsSameCharaAbility(charaType, attribute))
		{
			return true;
		}
		if (subCharaCompare && charaType2 != CharaType.UNKNOWN)
		{
			return IsSameCharaAbility(charaType2, attribute);
		}
		return false;
	}

	private bool IsSameCharaAbility(CharaType charaType, CharacterAttribute chaoAttribute)
	{
		CharacterAttribute characterAttribute = CharaTypeUtil.GetCharacterAttribute(charaType);
		if (characterAttribute == chaoAttribute && chaoAttribute != CharacterAttribute.UNKNOWN)
		{
			return true;
		}
		return false;
	}

	private void SetChaoBonusValueRate()
	{
		m_count_chao_bonus_value_rate.score = m_chaoCountBonus * 0.01f;
		SetBonusVale(ref m_main_chao_bonus_value_rate, m_mainChaoInfo);
		SetBonusVale(ref m_sub_chao_bonus_value_rate, m_subChaoInfo);
		m_bonus_value_rate.score += m_count_chao_bonus_value_rate.score;
		m_bonus_value_rate.score += m_main_chao_bonus_value_rate.score + m_sub_chao_bonus_value_rate.score;
		m_bonus_value_rate.animal += m_main_chao_bonus_value_rate.animal + m_sub_chao_bonus_value_rate.animal;
		m_bonus_value_rate.ring += m_main_chao_bonus_value_rate.ring + m_sub_chao_bonus_value_rate.ring;
		m_bonus_value_rate.red_ring = 0f;
		m_bonus_value_rate.distance += m_main_chao_bonus_value_rate.distance + m_sub_chao_bonus_value_rate.distance;
		m_bonus_value_rate.sp_crystal += m_main_chao_bonus_value_rate.sp_crystal + m_sub_chao_bonus_value_rate.sp_crystal;
		m_bonus_value_rate.raid_boss_ring += m_main_chao_bonus_value_rate.raid_boss_ring + m_sub_chao_bonus_value_rate.raid_boss_ring;
	}

	private void SetCharacterBonusValueRate()
	{
		m_main_chara_bonus_value_rate.score = (m_mainTeamAbilityBonusValue[1] + m_mainCharaOverlapBonus[0]) * 0.01f;
		m_sub_chara_bonus_value_rate.score = (m_subTeamAbilityBonusValue[1] + m_subCharaOverlapBonus[0]) * 0.01f;
		m_main_chara_bonus_value_rate.animal = (m_mainCharaAbilityValue[6] + m_mainTeamAbilityBonusValue[3] + m_mainCharaOverlapBonus[2]) * 0.01f;
		m_sub_chara_bonus_value_rate.animal = (m_subCharaAbilityValue[6] + m_subTeamAbilityBonusValue[3] + m_subCharaOverlapBonus[2]) * 0.01f;
		m_main_chara_bonus_value_rate.ring = (m_mainCharaAbilityValue[3] + m_mainTeamAbilityBonusValue[2] + m_mainCharaOverlapBonus[1]) * 0.01f;
		m_sub_chara_bonus_value_rate.ring = (m_subCharaAbilityValue[3] + m_subTeamAbilityBonusValue[2] + m_subCharaOverlapBonus[1]) * 0.01f;
		m_main_chara_bonus_value_rate.distance = (m_mainCharaAbilityValue[4] + m_mainTeamAbilityBonusValue[0] + m_mainCharaOverlapBonus[3]) * 0.01f;
		m_sub_chara_bonus_value_rate.distance = (m_subCharaAbilityValue[4] + m_subTeamAbilityBonusValue[0] + m_subCharaOverlapBonus[3]) * 0.01f;
		if (m_boostItemValidFlag[0])
		{
			m_boostBonusValue = 1f;
			m_main_chara_bonus_value_rate.score += m_boostBonusValue;
			if (m_boostItemValidFlag[2])
			{
				SaveDataManager saveDataManager = SaveDataManager.Instance;
				if (saveDataManager != null && CheckCharaType(saveDataManager.PlayerData.SubChara))
				{
					m_sub_chara_bonus_value_rate.score += m_boostBonusValue;
				}
			}
		}
		m_bonus_value_rate.distance += m_main_chara_bonus_value_rate.distance + m_sub_chara_bonus_value_rate.distance;
		m_bonus_value_rate.score += m_main_chara_bonus_value_rate.score + m_sub_chara_bonus_value_rate.score;
		m_bonus_value_rate.animal += m_main_chara_bonus_value_rate.animal + m_sub_chara_bonus_value_rate.animal;
		m_bonus_value_rate.ring += m_main_chara_bonus_value_rate.ring + m_sub_chara_bonus_value_rate.ring;
	}

	private void SetPampaignBonusValueRate()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerCampaignData campaignInSession = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.BankedRingBonus);
			if (campaignInSession != null)
			{
				float num = m_campaignBonusValue = campaignInSession.fContent;
			}
			else
			{
				m_campaignBonusValue = 0f;
			}
		}
	}

	private void SetMileageBonusScoreRate()
	{
		GameObject gameObject = GameObject.Find("MileageBonusInfo");
		if (gameObject != null)
		{
			MileageBonusInfo component = gameObject.GetComponent<MileageBonusInfo>();
			if (component != null)
			{
				m_mileage_bonus_score_rate.Reset();
				MileageBonusData bonusData = component.BonusData;
				float num = bonusData.value - 1f;
				MileageBonusData bonusData2 = component.BonusData;
				switch (bonusData2.type)
				{
				case MileageBonus.SCORE:
					m_mileage_bonus_score_rate.score = num;
					break;
				case MileageBonus.ANIMAL:
					m_mileage_bonus_score_rate.animal = num;
					break;
				case MileageBonus.RING:
					m_mileage_bonus_score_rate.ring = num;
					break;
				case MileageBonus.DISTANCE:
					m_mileage_bonus_score_rate.distance = num;
					break;
				case MileageBonus.FINAL_SCORE:
					m_mileage_bonus_score_rate.final_score = num;
					break;
				}
			}
			Object.Destroy(gameObject);
		}
		else
		{
			m_mileage_bonus_score_rate.Reset();
		}
	}

	private float GetPlusPercentBonusValue(float percent, float src_value)
	{
		return Mathf.Ceil(src_value + src_value * percent * 0.01f);
	}

	private float GetPlusPercentBonusTime(float percent, float src_value)
	{
		return src_value + src_value * percent * 0.01f;
	}

	private float CalcPlusAbliltyBonusValue(ChaoAbility ability, float src_value)
	{
		float num = src_value;
		switch (ability)
		{
		case ChaoAbility.COLOR_POWER_SCORE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.ASTEROID_SCORE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.DRILL_SCORE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.LASER_SCORE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.COLOR_POWER_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.ITEM_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.COMBO_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.TRAMPOLINE_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.MAGNET_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.ASTEROID_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.DRILL_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.LASER_TIME:
			num = GetPlusPercentBonusTime(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.BOSS_STAGE_TIME:
			num = GetChaoAbilityValue(ability) + src_value;
			break;
		case ChaoAbility.BOSS_RED_RING_RATE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			num = Mathf.Clamp(num, 0f, 100f);
			break;
		case ChaoAbility.BOSS_SUPER_RING_RATE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			num = Mathf.Clamp(num, 0f, 100f);
			break;
		case ChaoAbility.RARE_ENEMY_UP:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			num = Mathf.Clamp(num, 0f, 100f);
			break;
		case ChaoAbility.SPECIAL_CRYSTAL_RATE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			num = Mathf.Clamp(num, 0f, 100f);
			break;
		case ChaoAbility.AGGRESSIVITY_UP_FOR_RAID_BOSS:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.LAST_CHANCE:
			num = GetChaoAbilityValue(ChaoAbility.LAST_CHANCE);
			break;
		case ChaoAbility.MAP_BOSS_DAMAGE:
			num = src_value - GetChaoAbilityValue(ChaoAbility.MAP_BOSS_DAMAGE);
			if (num < 1f)
			{
				num = 1f;
			}
			break;
		case ChaoAbility.MAGNET_RANGE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.COMBO_RECEPTION_TIME:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.TRANSFER_DOUBLE_RING:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		case ChaoAbility.ENEMY_SCORE:
			num = GetPlusPercentBonusValue(GetChaoAbilityValue(ability), src_value);
			break;
		}
		return num;
	}

	private float CalcItemPlusAbliltyBonusValue(ItemType itemType)
	{
		float result = 0f;
		float num = 0f;
		switch (itemType)
		{
		case ItemType.INVINCIBLE:
			result = CharaAbility[9];
			num = GetChaoAbilityValue(ChaoAbility.ITEM_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.MAGNET:
			result = CharaAbility[8];
			num = GetChaoAbilityValue(ChaoAbility.MAGNET_TIME) + GetChaoAbilityValue(ChaoAbility.ITEM_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.COMBO:
			result = CharaAbility[7];
			num = GetChaoAbilityValue(ChaoAbility.COMBO_TIME) + GetChaoAbilityValue(ChaoAbility.ITEM_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.TRAMPOLINE:
			result = CharaAbility[5];
			num = GetChaoAbilityValue(ChaoAbility.TRAMPOLINE_TIME) + GetChaoAbilityValue(ChaoAbility.ITEM_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.LASER:
			result = CharaAbility[0];
			num = GetChaoAbilityValue(ChaoAbility.LASER_TIME) + GetChaoAbilityValue(ChaoAbility.COLOR_POWER_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.DRILL:
			result = CharaAbility[1];
			num = GetChaoAbilityValue(ChaoAbility.DRILL_TIME) + GetChaoAbilityValue(ChaoAbility.COLOR_POWER_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		case ItemType.ASTEROID:
			result = CharaAbility[2];
			num = GetChaoAbilityValue(ChaoAbility.ASTEROID_TIME) + GetChaoAbilityValue(ChaoAbility.COLOR_POWER_TIME);
			result = GetPlusPercentBonusTime(num, result);
			break;
		}
		return result;
	}

	private void CalcChaoCountBonus()
	{
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState == null)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0.1f;
		float num5 = 0.3f;
		float num6 = 1f;
		List<ServerChaoState> chaoStates = playerState.ChaoStates;
		foreach (ServerChaoState item in chaoStates)
		{
			if (item != null && item.Status > ServerChaoState.ChaoStatus.NotOwned)
			{
				switch (item.Rarity)
				{
				case 0:
					num += num4 * (float)item.NumAcquired;
					break;
				case 1:
					num2 += num5 * (float)item.NumAcquired;
					break;
				case 2:
					num3 += num6 * (float)item.NumAcquired;
					break;
				}
				m_chaoCount += item.NumAcquired;
			}
		}
		float value = num + num2 + num3;
		m_chaoCountBonus = Mathf.Clamp(value, 0f, 200f);
	}

	private ChaoEffect.TargetType GetChaoEffectTagetType(ChaoAbility ability)
	{
		ChaoEffect.TargetType targetType = ChaoEffect.TargetType.Unknown;
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			if (HasChaoAbility(m_mainChaoInfo, ability))
			{
				targetType = ChaoEffect.TargetType.MainChao;
			}
			if (HasChaoAbility(m_subChaoInfo, ability))
			{
				targetType = ((targetType != 0) ? ChaoEffect.TargetType.SubChao : ChaoEffect.TargetType.BothChao);
				if (ability == ChaoAbility.RECOVERY_RING && targetType == ChaoEffect.TargetType.BothChao)
				{
					targetType = ChaoEffect.TargetType.MainChao;
				}
			}
		}
		return targetType;
	}

	private void PlayChaoEffect(ChaoAbility ability)
	{
		ChaoEffect.TargetType chaoEffectTagetType = GetChaoEffectTagetType(ability);
		if (chaoEffectTagetType != ChaoEffect.TargetType.Unknown)
		{
			ChaoEffect chaoEffect = ChaoEffect.Instance;
			if (chaoEffect != null)
			{
				chaoEffect.RequestPlayChaoEffect(chaoEffectTagetType, ability);
			}
		}
	}

	private void PlayChaoEffect(ChaoAbility ability, ChaoType chaoType)
	{
		ChaoEffect.TargetType chaoEffectTagetType = GetChaoEffectTagetType(ability);
		switch (chaoType)
		{
		case ChaoType.MAIN:
			if (chaoEffectTagetType == ChaoEffect.TargetType.MainChao || chaoEffectTagetType == ChaoEffect.TargetType.BothChao)
			{
				ChaoEffect chaoEffect2 = ChaoEffect.Instance;
				if (chaoEffect2 != null)
				{
					chaoEffect2.RequestPlayChaoEffect(ChaoEffect.TargetType.MainChao, ability);
				}
			}
			break;
		case ChaoType.SUB:
			if (chaoEffectTagetType == ChaoEffect.TargetType.SubChao || chaoEffectTagetType == ChaoEffect.TargetType.BothChao)
			{
				ChaoEffect chaoEffect = ChaoEffect.Instance;
				if (chaoEffect != null)
				{
					chaoEffect.RequestPlayChaoEffect(ChaoEffect.TargetType.SubChao, ability);
				}
			}
			break;
		}
	}

	private void StopChaoEffect(ChaoAbility ability)
	{
		ChaoEffect.TargetType chaoEffectTagetType = GetChaoEffectTagetType(ability);
		if (chaoEffectTagetType != ChaoEffect.TargetType.Unknown)
		{
			ChaoEffect chaoEffect = ChaoEffect.Instance;
			if (chaoEffect != null)
			{
				chaoEffect.RequestStopChaoEffect(chaoEffectTagetType, ability);
			}
		}
	}

	private void CheckPlayerInformation()
	{
		if (m_playerInformation == null)
		{
			m_playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
		}
	}

	public static void LoadAbilityDataTable(ResourceSceneLoader loaderComponent)
	{
		if (loaderComponent != null)
		{
			if (!IsExistDataObject(CHAODATA_NAME))
			{
				loaderComponent.AddLoadAndResourceManager(m_loadInfo[0]);
			}
			if (!IsExistDataObject(CHARADATA_NAME))
			{
				loaderComponent.AddLoadAndResourceManager(m_loadInfo[1]);
			}
		}
	}

	public static void SetupAbilityDataTable()
	{
		StageAbilityManager stageAbilityManager = Instance;
		if (stageAbilityManager != null)
		{
			stageAbilityManager.Setup();
		}
	}

	private static bool IsExistDataObject(string name)
	{
		GameObject x = GameObject.Find(name);
		if (x != null)
		{
			return true;
		}
		return false;
	}
}
