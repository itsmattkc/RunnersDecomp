using GameScore;
using Message;
using System;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/GameMode/Stage")]
public class StageScoreManager : MonoBehaviour
{
	public enum DataType
	{
		StageCount,
		BonusCount,
		BonusCount_MainChao,
		BonusCount_SubChao,
		BonusCount_ChaoCount,
		BonusCount_MainChara,
		BonusCount_SubChara,
		BonusCount_Campaign,
		BonusCount_Rank,
		FinalCount,
		Score,
		MileageBonusScore,
		NUM
	}

	public struct MaskedInt
	{
		private int m_valueUp;

		private int m_valueDown;

		private int m_mask;

		private int m_addNum;

		public void Set(int input)
		{
			if (m_mask == 0)
			{
				m_mask = UnityEngine.Random.Range(15, int.MaxValue);
				m_addNum = UnityEngine.Random.Range(15, 1024);
			}
			input += m_addNum;
			m_valueUp = (input & m_mask);
			m_valueDown = (input & ~m_mask);
		}

		public int Get()
		{
			int num = m_valueUp | m_valueDown;
			return num - m_addNum;
		}
	}

	public struct MaskedLong
	{
		private long m_valueUp;

		private long m_valueDown;

		private long m_mask;

		private long m_addNum;

		public void Set(long input)
		{
			if (m_mask == 0L)
			{
				m_mask = UnityEngine.Random.Range(15, int.MaxValue);
				m_addNum = UnityEngine.Random.Range(15, 1024);
			}
			input += m_addNum;
			m_valueUp = (input & m_mask);
			m_valueDown = (input & ~m_mask);
		}

		public long Get()
		{
			long num = m_valueUp | m_valueDown;
			return num - m_addNum;
		}
	}

	public class ResultData
	{
		private MaskedLong m_score;

		private MaskedLong m_animal;

		private MaskedLong m_ring;

		private MaskedLong m_red_ring;

		private MaskedLong m_distance;

		private MaskedLong m_sp_crystal;

		private MaskedLong m_raid_boss_ring;

		private MaskedLong m_raid_boss_reward;

		private MaskedLong m_final_score;

		public long score
		{
			get
			{
				return m_score.Get();
			}
			set
			{
				m_score.Set(value);
			}
		}

		public long animal
		{
			get
			{
				return m_animal.Get();
			}
			set
			{
				m_animal.Set(value);
			}
		}

		public long ring
		{
			get
			{
				return m_ring.Get();
			}
			set
			{
				m_ring.Set(value);
			}
		}

		public long red_ring
		{
			get
			{
				return m_red_ring.Get();
			}
			set
			{
				m_red_ring.Set(value);
			}
		}

		public long distance
		{
			get
			{
				return m_distance.Get();
			}
			set
			{
				m_distance.Set(value);
			}
		}

		public long sp_crystal
		{
			get
			{
				return m_sp_crystal.Get();
			}
			set
			{
				m_sp_crystal.Set(value);
			}
		}

		public long raid_boss_ring
		{
			get
			{
				return m_raid_boss_ring.Get();
			}
			set
			{
				m_raid_boss_ring.Set(value);
			}
		}

		public long raid_boss_reward
		{
			get
			{
				return m_raid_boss_reward.Get();
			}
			set
			{
				m_raid_boss_reward.Set(value);
			}
		}

		public long final_score
		{
			get
			{
				return m_final_score.Get();
			}
			set
			{
				m_final_score.Set(value);
			}
		}

		public ResultData()
		{
			m_animal = default(MaskedLong);
			m_ring = default(MaskedLong);
			m_red_ring = default(MaskedLong);
			m_sp_crystal = default(MaskedLong);
			m_raid_boss_ring = default(MaskedLong);
			m_raid_boss_reward = default(MaskedLong);
			m_score = default(MaskedLong);
			m_distance = default(MaskedLong);
			m_final_score = default(MaskedLong);
		}

		public long Sum()
		{
			long num = 0L;
			num += score;
			num += animal;
			num += ring;
			num += red_ring;
			return num + distance;
		}
	}

	private const float FloatToInt = 1000000f;

	private readonly int m_stockRingCoefficient = Data.ResultRing;

	private readonly int m_scoreCoefficient = 1;

	private readonly int m_animalCoefficient = Data.ResultAnimal;

	private readonly int m_totaldistanceCoefficient = Data.ResultDistance;

	[SerializeField]
	private int m_continueRing = 1000;

	[SerializeField]
	private int m_continueRaidBossRing = 500;

	private MaskedLong m_score = default(MaskedLong);

	private MaskedLong m_animal = default(MaskedLong);

	private MaskedLong m_ring = default(MaskedLong);

	private MaskedLong m_red_ring = default(MaskedLong);

	private MaskedLong m_special_crystal = default(MaskedLong);

	private MaskedLong m_raid_boss_ring = default(MaskedLong);

	private MaskedLong m_final_score = default(MaskedLong);

	private MaskedLong m_collectEventCount = default(MaskedLong);

	private ResultData[] m_results;

	private PlayerInformation m_information;

	private LevelInformation m_levelInformation;

	private bool m_bossStage;

	private bool m_quickMode;

	private bool m_isFinalScore;

	private StageScorePool m_scorePool;

	private float m_rank_rate;

	private long m_realtime_score_back;

	private long m_animal_back;

	private long m_event_score_back;

	private long m_realtime_score_old;

	private long m_animal_old;

	private long m_event_score_old;

	private static StageScoreManager instance;

	public static StageScoreManager Instance
	{
		get
		{
			return instance;
		}
	}

	public ResultData ScoreData
	{
		get
		{
			return GetResultData(DataType.Score);
		}
	}

	public ResultData MileageBonusScoreData
	{
		get
		{
			return GetResultData(DataType.MileageBonusScore);
		}
	}

	public ResultData CountData
	{
		get
		{
			return GetResultData(DataType.StageCount);
		}
	}

	public ResultData BonusCountData
	{
		get
		{
			return GetResultData(DataType.BonusCount);
		}
	}

	public ResultData BonusCountMainChaoData
	{
		get
		{
			return GetResultData(DataType.BonusCount_MainChao);
		}
	}

	public ResultData BonusCountSubChaoData
	{
		get
		{
			return GetResultData(DataType.BonusCount_SubChao);
		}
	}

	public ResultData BonusCountChaoCountData
	{
		get
		{
			return GetResultData(DataType.BonusCount_ChaoCount);
		}
	}

	public ResultData BonusCountMainCharaData
	{
		get
		{
			return GetResultData(DataType.BonusCount_MainChara);
		}
	}

	public ResultData BonusCountSubCharaData
	{
		get
		{
			return GetResultData(DataType.BonusCount_SubChara);
		}
	}

	public ResultData BonusCountCampaignData
	{
		get
		{
			return GetResultData(DataType.BonusCount_Campaign);
		}
	}

	public ResultData BonusCountRankData
	{
		get
		{
			return GetResultData(DataType.BonusCount_Rank);
		}
	}

	public ResultData FinalCountData
	{
		get
		{
			return GetResultData(DataType.FinalCount);
		}
	}

	public long ResultChaoBonusTotal
	{
		get
		{
			long num = 0L;
			num += GetResultBonusScore(DataType.BonusCount_MainChao);
			num += GetResultBonusScore(DataType.BonusCount_SubChao);
			return num + GetResultBonusScore(DataType.BonusCount_ChaoCount);
		}
	}

	public long ResultCampaignBonusTotal
	{
		get
		{
			long num = 0L;
			return num + GetResultBonusScore(DataType.BonusCount_Campaign);
		}
	}

	public long ResultPlayerBonusTotal
	{
		get
		{
			long num = 0L;
			num += GetResultBonusScore(DataType.BonusCount_MainChara);
			num += GetResultBonusScore(DataType.BonusCount_SubChara);
			return num + GetResultBonusScore(DataType.BonusCount_Rank);
		}
	}

	public long FinalScore
	{
		get
		{
			return m_final_score.Get();
		}
	}

	public long Score
	{
		get
		{
			return m_score.Get();
		}
	}

	public long Animal
	{
		get
		{
			return m_animal.Get();
		}
	}

	public long Ring
	{
		get
		{
			return m_ring.Get();
		}
	}

	public long RedRing
	{
		get
		{
			return m_red_ring.Get();
		}
	}

	public long SpecialCrystal
	{
		get
		{
			return m_special_crystal.Get();
		}
	}

	public long RaidBossRing
	{
		get
		{
			return m_raid_boss_ring.Get();
		}
	}

	public long CollectEventCount
	{
		get
		{
			return m_collectEventCount.Get();
		}
	}

	public int ContinueRing
	{
		get
		{
			return m_continueRing;
		}
	}

	public int ContinueRaidBossRing
	{
		get
		{
			return m_continueRaidBossRing;
		}
	}

	public StageScorePool ScorePool
	{
		get
		{
			return m_scorePool;
		}
		private set
		{
		}
	}

	public ResultData GetResultData(DataType type)
	{
		if (type < DataType.NUM && m_results != null && (int)type < m_results.Length)
		{
			return m_results[(int)type];
		}
		return new ResultData();
	}

	public long GetResultBonusScore(DataType type)
	{
		long num = 0L;
		num += m_results[(int)type].score * m_scoreCoefficient;
		num += m_results[(int)type].ring * m_stockRingCoefficient;
		num += m_results[(int)type].animal * m_animalCoefficient;
		return num + m_results[(int)type].distance * m_totaldistanceCoefficient;
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("PlayerInformation");
		if (gameObject != null)
		{
			m_information = gameObject.GetComponent<PlayerInformation>();
		}
		ResetScore();
		m_scorePool = new StageScorePool();
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
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Setup(bool bossStage)
	{
		m_bossStage = bossStage;
		if (StageModeManager.Instance != null)
		{
			m_quickMode = StageModeManager.Instance.IsQuickMode();
		}
		m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		if (m_scorePool != null)
		{
			m_scorePool.CheckHalfWay();
		}
		CPlusPlusLink cPlusPlusLink = CPlusPlusLink.Instance;
		if (cPlusPlusLink != null)
		{
			cPlusPlusLink.ResetNativeResultScore();
		}
	}

	public void ResetScore(MsgResetScore msg)
	{
		ResetScore(msg.m_score, msg.m_animal, msg.m_ring, msg.m_red_ring, msg.m_final_score);
	}

	public void AddScore(long score)
	{
		m_score.Set(m_score.Get() + score);
	}

	public void AddAnimal(long addCount)
	{
		m_animal.Set(m_animal.Get() + addCount);
	}

	public void AddRedRing()
	{
		m_red_ring.Set(m_red_ring.Get() + 1);
	}

	public void AddRecoveryRingCount(int addCount)
	{
		int transforDoubleRing = GetTransforDoubleRing(addCount);
		m_ring.Set(m_ring.Get() + transforDoubleRing);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(9, transforDoubleRing));
	}

	public void AddSpecialCrystal(long addCount)
	{
		m_special_crystal.Set(m_special_crystal.Get() + addCount);
	}

	public void AddScoreCheck(StageScoreData scoreData)
	{
		if (m_scorePool != null)
		{
			m_scorePool.AddScore(scoreData);
		}
	}

	private void ResetScore()
	{
		ResetScore(0L, 0L, 0L, 0L, 0L);
		m_special_crystal.Set(0L);
		m_raid_boss_ring.Set(0L);
		m_collectEventCount.Set(0L);
		m_realtime_score_back = 0L;
		m_animal_back = 0L;
		m_event_score_back = 0L;
		m_realtime_score_old = 0L;
		m_animal_old = 0L;
		m_event_score_old = 0L;
	}

	private void ResetScore(long score, long animal, long ring, long red_ring, long final_score)
	{
		m_score.Set(score);
		m_animal.Set(animal);
		m_ring.Set(ring);
		m_red_ring.Set(red_ring);
		m_final_score.Set(final_score);
	}

	private int GetTransforDoubleRing(int transferRingCount)
	{
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.TRANSFER_DOUBLE_RING))
		{
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.TRANSFER_DOUBLE_RING, false);
			return StageAbilityManager.Instance.GetChaoAbliltyValue(ChaoAbility.TRANSFER_DOUBLE_RING, transferRingCount);
		}
		return transferRingCount;
	}

	public void TransferRingForContinue(int ring)
	{
		if (m_information != null)
		{
			int transforDoubleRing = GetTransforDoubleRing(m_information.NumRings);
			m_ring.Set(m_ring.Get() + transforDoubleRing);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(9, transforDoubleRing));
			m_information.SetNumRings(ring);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnResetRingsForContinue", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void TransferRingForChaoAbility(int ring)
	{
		int transforDoubleRing = GetTransforDoubleRing(ring);
		m_ring.Set(m_ring.Get() + transforDoubleRing);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(9, transforDoubleRing));
	}

	public bool DefrayItemCostByRing(long costRing)
	{
		bool flag = false;
		if (m_ring.Get() > 0)
		{
			if (m_ring.Get() > costRing)
			{
				m_ring.Set(m_ring.Get() - costRing);
			}
			else
			{
				m_ring.Set(0L);
			}
			flag = true;
		}
		else if (m_information != null)
		{
			long num = m_information.NumRings;
			if (num > 0)
			{
				if (num > costRing)
				{
					m_information.SetNumRings((int)(num - costRing));
				}
				else
				{
					m_information.SetNumRings(0);
				}
				flag = true;
			}
		}
		if (flag)
		{
			GameObjectUtil.SendMessageToTagObjects("Player", "OnDefrayRing", null, SendMessageOptions.DontRequireReceiver);
		}
		return flag;
	}

	public void TransferRing()
	{
		if (m_information != null && m_levelInformation != null)
		{
			int transforDoubleRing = GetTransforDoubleRing(m_information.NumRings);
			m_ring.Set(m_ring.Get() + transforDoubleRing);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(9, transforDoubleRing));
			bool flag = true;
			if (m_information.NumRings > 0 && !m_levelInformation.DestroyRingMode)
			{
				flag = false;
			}
			if (!flag)
			{
				m_information.SetNumRings(1);
			}
			else
			{
				m_information.SetNumRings(0);
			}
			GameObjectUtil.SendMessageToTagObjects("Player", "OnResetRingsForCheckPoint", new MsgPlayerTransferRing(flag), SendMessageOptions.DontRequireReceiver);
		}
	}

	public void TransferRingCountToRaidBossRingCount()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			m_raid_boss_ring = m_ring;
			m_ring.Set(0L);
		}
	}

	public void SendMessageFinalScoreBeforeResult()
	{
		if (!m_isFinalScore)
		{
			m_isFinalScore = true;
		}
		OnCalcFinalScore();
	}

	public void OnCalcFinalScore()
	{
		m_results = null;
		m_results = new ResultData[12];
		for (int i = 0; i < 12; i++)
		{
			m_results[i] = new ResultData();
		}
		float num = (!(m_information != null)) ? 0f : m_information.TotalDistance;
		int num2 = (int)num;
		SetStageCount(num2);
		SetBonusCountType(num2, DataType.BonusCount_MainChao);
		SetBonusCountType(num2, DataType.BonusCount_SubChao);
		SetBonusCountType(num2, DataType.BonusCount_ChaoCount);
		SetBonusCountType(num2, DataType.BonusCount_MainChara);
		SetBonusCountType(num2, DataType.BonusCount_SubChara);
		SetBonusCountRank();
		SetBonusCountCampaign();
		SetBonusCount();
		SetFinalCount(num2);
		SetScore();
		SetMileageBonusScore();
		SetCollectEventCount();
		SetFinalScore();
	}

	private void SetStageCount(int distance)
	{
		int num = 0;
		m_results[num].score = m_score.Get();
		m_results[num].animal = m_animal.Get();
		m_results[num].ring = m_ring.Get();
		m_results[num].red_ring = m_red_ring.Get();
		m_results[num].distance = distance;
		m_results[num].sp_crystal = m_special_crystal.Get();
		m_results[num].raid_boss_ring = m_raid_boss_ring.Get();
	}

	private void SetBonusCountType(int distance, DataType type)
	{
		if (StageAbilityManager.Instance != null)
		{
			StageAbilityManager.BonusRate bonusRate;
			switch (type)
			{
			default:
				return;
			case DataType.BonusCount_MainChao:
				bonusRate = StageAbilityManager.Instance.MainChaoBonusValueRate;
				break;
			case DataType.BonusCount_SubChao:
				bonusRate = StageAbilityManager.Instance.SubChaoBonusValueRate;
				break;
			case DataType.BonusCount_ChaoCount:
				bonusRate = StageAbilityManager.Instance.CountChaoBonusValueRate;
				break;
			case DataType.BonusCount_MainChara:
				bonusRate = StageAbilityManager.Instance.MainCharaBonusValueRate;
				break;
			case DataType.BonusCount_SubChara:
				bonusRate = StageAbilityManager.Instance.SubCharaBonusValueRate;
				break;
			}
			double value = (double)m_score.Get() * (double)bonusRate.score;
			double value2 = (double)m_animal.Get() * (double)bonusRate.animal;
			double value3 = (double)m_ring.Get() * (double)bonusRate.ring;
			double value4 = (double)distance * (double)bonusRate.distance;
			if (m_isFinalScore)
			{
				ObjUtil.SendMessageScoreCheck(new StageScoreData(13, (int)(bonusRate.score * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(14, (int)(bonusRate.ring * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(15, (int)(bonusRate.animal * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(16, (int)(bonusRate.distance * 1000000f)));
			}
			m_results[(int)type].score = GetRoundUpValue(value);
			m_results[(int)type].animal = GetRoundUpValue(value2);
			m_results[(int)type].ring = GetRoundUpValue(value3);
			m_results[(int)type].red_ring = 0L;
			m_results[(int)type].distance = GetRoundUpValue(value4);
		}
	}

	private void SetBonusCountRank()
	{
		int num = 8;
		float num2 = 0f;
		if (m_quickMode)
		{
			m_results[num].score = 0L;
		}
		else
		{
			uint num3 = 1u;
			if (SaveDataManager.Instance != null)
			{
				num3 = SaveDataManager.Instance.PlayerData.DisplayRank;
				if (num3 < 1)
				{
					num3 = 1u;
				}
			}
			num2 = (float)num3 * 0.01f;
			m_results[num].score = GetRoundUpValue((float)m_score.Get() * num2);
		}
		if (m_isFinalScore)
		{
			ObjUtil.SendMessageScoreCheck(new StageScoreData(12, (int)(num2 * 1000000f)));
		}
	}

	private void SetBonusCountCampaign()
	{
		if (StageAbilityManager.Instance != null)
		{
			float num = 0f;
			float campaignValueRate = StageAbilityManager.Instance.CampaignValueRate;
			if (campaignValueRate > 0f)
			{
				num = (float)m_ring.Get() * campaignValueRate;
			}
			int num2 = 7;
			m_results[num2].ring += GetRoundUpValue(num);
			if (m_isFinalScore)
			{
				ObjUtil.SendMessageScoreCheck(new StageScoreData(14, (int)(campaignValueRate * 1000000f)));
			}
		}
	}

	private void SetBonusCount()
	{
		if (StageAbilityManager.Instance != null)
		{
			int[] array = new int[7]
			{
				2,
				3,
				4,
				5,
				6,
				7,
				8
			};
			int num = 1;
			for (int i = 0; i < array.Length; i++)
			{
				m_results[num].score += m_results[array[i]].score;
			}
			for (int j = 0; j < array.Length; j++)
			{
				m_results[num].animal += m_results[array[j]].animal;
			}
			for (int k = 0; k < array.Length; k++)
			{
				m_results[num].ring += m_results[array[k]].ring;
			}
			for (int l = 0; l < array.Length; l++)
			{
				m_results[num].distance += m_results[array[l]].distance;
			}
			for (int m = 0; m < array.Length; m++)
			{
				m_results[num].sp_crystal += m_results[array[m]].sp_crystal;
			}
			for (int n = 0; n < array.Length; n++)
			{
				m_results[num].raid_boss_ring += m_results[array[n]].raid_boss_ring;
			}
		}
	}

	private void SetFinalCount(int distance)
	{
		int num = 1;
		int num2 = 9;
		m_results[num2].score = m_score.Get() + m_results[num].score;
		m_results[num2].animal = m_animal.Get() + m_results[num].animal;
		m_results[num2].ring = m_ring.Get() + m_results[num].ring;
		m_results[num2].red_ring = m_red_ring.Get();
		m_results[num2].distance = distance + m_results[num].distance;
		m_results[num2].sp_crystal = m_special_crystal.Get() + m_results[num].sp_crystal;
		m_results[num2].raid_boss_ring = m_raid_boss_ring.Get() + m_results[num].raid_boss_ring;
	}

	private void SetScore()
	{
		int num = 10;
		int num2 = 9;
		m_results[num].score = m_results[num2].score * m_scoreCoefficient;
		m_results[num].ring = m_results[num2].ring * m_stockRingCoefficient;
		m_results[num].animal = m_results[num2].animal * m_animalCoefficient;
		m_results[num].distance = m_results[num2].distance * m_totaldistanceCoefficient;
	}

	private void SetMileageBonusScore()
	{
		if (StageAbilityManager.Instance != null)
		{
			StageAbilityManager.BonusRate mileageBonusScoreRate = StageAbilityManager.Instance.MileageBonusScoreRate;
			int num = 10;
			int num2 = 11;
			float num3 = (float)m_results[num].score * mileageBonusScoreRate.score;
			float num4 = (float)m_results[num].animal * mileageBonusScoreRate.animal;
			float num5 = (float)m_results[num].ring * mileageBonusScoreRate.ring;
			float num6 = (float)m_results[num].distance * mileageBonusScoreRate.distance;
			m_results[num2].score = GetRoundUpValue(num3);
			m_results[num2].animal = GetRoundUpValue(num4);
			m_results[num2].ring = GetRoundUpValue(num5);
			m_results[num2].distance = GetRoundUpValue(num6);
			if (m_isFinalScore)
			{
				ObjUtil.SendMessageScoreCheck(new StageScoreData(13, (int)(mileageBonusScoreRate.score * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(15, (int)(mileageBonusScoreRate.animal * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(14, (int)(mileageBonusScoreRate.ring * 1000000f)));
				ObjUtil.SendMessageScoreCheck(new StageScoreData(16, (int)(mileageBonusScoreRate.distance * 1000000f)));
			}
		}
	}

	private void SetFinalScore()
	{
		if (m_bossStage)
		{
			m_final_score.Set(0L);
			return;
		}
		m_final_score.Set(m_results[10].Sum() + m_results[11].Sum());
		if (StageAbilityManager.Instance != null)
		{
			StageAbilityManager.BonusRate mileageBonusScoreRate = StageAbilityManager.Instance.MileageBonusScoreRate;
			float final_score = mileageBonusScoreRate.final_score;
			if (final_score > 0f)
			{
				m_results[11].final_score = GetRoundUpValue((float)m_final_score.Get() * final_score);
				m_final_score.Set(m_final_score.Get() + (int)m_results[11].final_score);
			}
		}
		m_final_score.Set(GetTeamAbliltyResultScore(m_final_score.Get(), 1));
	}

	private void SetCollectEventCount()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsCollectEvent())
		{
			switch (EventManager.Instance.CollectType)
			{
			case EventManager.CollectEventType.GET_ANIMALS:
				m_collectEventCount.Set(GetResultData(DataType.FinalCount).animal);
				break;
			case EventManager.CollectEventType.GET_RING:
				m_collectEventCount.Set(GetResultData(DataType.FinalCount).ring);
				break;
			case EventManager.CollectEventType.RUN_DISTANCE:
				m_collectEventCount.Set(GetResultData(DataType.FinalCount).distance);
				break;
			}
		}
	}

	private long GetRoundUpValue(double value)
	{
		return (long)Math.Ceiling(value);
	}

	private long GetTeamAbliltyResultScore(long score, int coefficient)
	{
		StageAbilityManager stageAbilityManager = StageAbilityManager.Instance;
		if (stageAbilityManager != null)
		{
			return stageAbilityManager.GetTeamAbliltyResultScore(score, coefficient);
		}
		return score * coefficient;
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}

	public void SetupScoreRate()
	{
		m_rank_rate = 0f;
		if (!m_quickMode && SaveDataManager.Instance != null)
		{
			uint num = 1u;
			num = SaveDataManager.Instance.PlayerData.DisplayRank;
			if (num < 1)
			{
				num = 1u;
			}
			m_rank_rate = (float)num * 0.01f;
		}
	}

	private StageAbilityManager.BonusRate GetBonusRate(DataType type)
	{
		StageAbilityManager.BonusRate result = default(StageAbilityManager.BonusRate);
		switch (type)
		{
		case DataType.BonusCount_MainChao:
			return StageAbilityManager.Instance.MainChaoBonusValueRate;
		case DataType.BonusCount_SubChao:
			return StageAbilityManager.Instance.SubChaoBonusValueRate;
		case DataType.BonusCount_ChaoCount:
			return StageAbilityManager.Instance.CountChaoBonusValueRate;
		case DataType.BonusCount_MainChara:
			return StageAbilityManager.Instance.MainCharaBonusValueRate;
		case DataType.BonusCount_SubChara:
			return StageAbilityManager.Instance.SubCharaBonusValueRate;
		default:
			return result;
		}
	}

	public long GetRealtimeScore()
	{
		long num = m_score.Get();
		long num2 = m_animal.Get();
		long num3 = m_ring.Get();
		long num4 = (long)m_information.TotalDistance;
		long num5 = num + num2 + num3 + num4;
		if (m_realtime_score_back == num5)
		{
			return m_realtime_score_old;
		}
		m_realtime_score_back = num5;
		if (StageAbilityManager.Instance != null)
		{
			double num6 = m_score.Get();
			long num7 = num;
			StageAbilityManager.BonusRate bonusRate = GetBonusRate(DataType.BonusCount_MainChao);
			num = num7 + GetRoundUpValue(num6 * (double)bonusRate.score);
			long num8 = num;
			StageAbilityManager.BonusRate bonusRate2 = GetBonusRate(DataType.BonusCount_SubChao);
			num = num8 + GetRoundUpValue(num6 * (double)bonusRate2.score);
			long num9 = num;
			StageAbilityManager.BonusRate bonusRate3 = GetBonusRate(DataType.BonusCount_ChaoCount);
			num = num9 + GetRoundUpValue(num6 * (double)bonusRate3.score);
			long num10 = num;
			StageAbilityManager.BonusRate bonusRate4 = GetBonusRate(DataType.BonusCount_MainChara);
			num = num10 + GetRoundUpValue(num6 * (double)bonusRate4.score);
			long num11 = num;
			StageAbilityManager.BonusRate bonusRate5 = GetBonusRate(DataType.BonusCount_SubChara);
			num = num11 + GetRoundUpValue(num6 * (double)bonusRate5.score);
			num += GetRoundUpValue(num6 * (double)m_rank_rate);
			num *= m_scoreCoefficient;
			double num12 = m_animal.Get();
			long num13 = num2;
			StageAbilityManager.BonusRate bonusRate6 = GetBonusRate(DataType.BonusCount_MainChao);
			num2 = num13 + GetRoundUpValue(num12 * (double)bonusRate6.animal);
			long num14 = num2;
			StageAbilityManager.BonusRate bonusRate7 = GetBonusRate(DataType.BonusCount_SubChao);
			num2 = num14 + GetRoundUpValue(num12 * (double)bonusRate7.animal);
			long num15 = num2;
			StageAbilityManager.BonusRate bonusRate8 = GetBonusRate(DataType.BonusCount_MainChara);
			num2 = num15 + GetRoundUpValue(num12 * (double)bonusRate8.animal);
			long num16 = num2;
			StageAbilityManager.BonusRate bonusRate9 = GetBonusRate(DataType.BonusCount_SubChara);
			num2 = num16 + GetRoundUpValue(num12 * (double)bonusRate9.animal);
			num2 *= m_animalCoefficient;
			double num17 = m_ring.Get();
			long num18 = num3;
			StageAbilityManager.BonusRate bonusRate10 = GetBonusRate(DataType.BonusCount_MainChao);
			num3 = num18 + GetRoundUpValue(num17 * (double)bonusRate10.ring);
			long num19 = num3;
			StageAbilityManager.BonusRate bonusRate11 = GetBonusRate(DataType.BonusCount_SubChao);
			num3 = num19 + GetRoundUpValue(num17 * (double)bonusRate11.ring);
			long num20 = num3;
			StageAbilityManager.BonusRate bonusRate12 = GetBonusRate(DataType.BonusCount_MainChara);
			num3 = num20 + GetRoundUpValue(num17 * (double)bonusRate12.ring);
			long num21 = num3;
			StageAbilityManager.BonusRate bonusRate13 = GetBonusRate(DataType.BonusCount_SubChara);
			num3 = num21 + GetRoundUpValue(num17 * (double)bonusRate13.ring);
			num3 += GetRoundUpValue(num17 * (double)StageAbilityManager.Instance.CampaignValueRate);
			num3 *= m_stockRingCoefficient;
			double num22 = num4;
			long num23 = num4;
			StageAbilityManager.BonusRate bonusRate14 = GetBonusRate(DataType.BonusCount_MainChao);
			num4 = num23 + GetRoundUpValue(num22 * (double)bonusRate14.distance);
			long num24 = num4;
			StageAbilityManager.BonusRate bonusRate15 = GetBonusRate(DataType.BonusCount_SubChao);
			num4 = num24 + GetRoundUpValue(num22 * (double)bonusRate15.distance);
			long num25 = num4;
			StageAbilityManager.BonusRate bonusRate16 = GetBonusRate(DataType.BonusCount_MainChara);
			num4 = num25 + GetRoundUpValue(num22 * (double)bonusRate16.distance);
			long num26 = num4;
			StageAbilityManager.BonusRate bonusRate17 = GetBonusRate(DataType.BonusCount_SubChara);
			num4 = num26 + GetRoundUpValue(num22 * (double)bonusRate17.distance);
			num4 *= m_totaldistanceCoefficient;
		}
		long num27 = 0L;
		num27 += num;
		num27 += num2;
		num27 += num3;
		num27 += num4;
		m_realtime_score_old = GetTeamAbliltyResultScore(num27, 1);
		return m_realtime_score_old;
	}

	public long GetRealtimeEventScore()
	{
		long num = m_special_crystal.Get();
		if (m_event_score_back == num)
		{
			return m_event_score_old;
		}
		m_realtime_score_back = num;
		if (StageAbilityManager.Instance != null)
		{
			double num2 = m_special_crystal.Get();
			long num3 = num;
			StageAbilityManager.BonusRate bonusRate = GetBonusRate(DataType.BonusCount_MainChao);
			num = num3 + GetRoundUpValue(num2 * (double)bonusRate.sp_crystal);
			long num4 = num;
			StageAbilityManager.BonusRate bonusRate2 = GetBonusRate(DataType.BonusCount_SubChao);
			num = num4 + GetRoundUpValue(num2 * (double)bonusRate2.sp_crystal);
		}
		m_event_score_old = num;
		return num;
	}

	public long GetRealtimeEventAnimal()
	{
		long num = m_animal.Get();
		if (m_animal_back == num)
		{
			return m_animal_old;
		}
		m_animal_back = num;
		if (StageAbilityManager.Instance != null)
		{
			float num2 = m_animal.Get();
			long num3 = num;
			StageAbilityManager.BonusRate bonusRate = GetBonusRate(DataType.BonusCount_MainChao);
			num = num3 + GetRoundUpValue(num2 * bonusRate.animal);
			long num4 = num;
			StageAbilityManager.BonusRate bonusRate2 = GetBonusRate(DataType.BonusCount_SubChao);
			num = num4 + GetRoundUpValue(num2 * bonusRate2.animal);
			long num5 = num;
			StageAbilityManager.BonusRate bonusRate3 = GetBonusRate(DataType.BonusCount_MainChara);
			num = num5 + GetRoundUpValue(num2 * bonusRate3.animal);
			long num6 = num;
			StageAbilityManager.BonusRate bonusRate4 = GetBonusRate(DataType.BonusCount_SubChara);
			num = num6 + GetRoundUpValue(num2 * bonusRate4.animal);
		}
		m_animal_old = num;
		return num;
	}
}
