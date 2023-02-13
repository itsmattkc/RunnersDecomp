using System.Collections.Generic;
using UnityEngine;

public class StageTimeManager : MonoBehaviour
{
	public enum ExtendPattern
	{
		UNKNOWN,
		CONTINUE,
		BRONZE_WATCH,
		SILVER_WATCH,
		GOLD_WATCH
	}

	private Dictionary<ExtendPattern, float> m_extendParams = new Dictionary<ExtendPattern, float>();

	private bool m_playing;

	private bool m_phantomPause;

	private float m_time = 60f;

	private float m_extendedTime;

	private float m_cotinueTime;

	private float m_reservedExtendedTime;

	private float m_charaOverlapBonus = 6f;

	private float m_extendedLimit = 480f;

	private StageScoreManager.MaskedInt m_bronzeCount = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_silverCount = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_goldCount = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_continuCount = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_mainCharaExtend = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_subCharaExtend = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedInt m_totalTime = default(StageScoreManager.MaskedInt);

	private StageScoreManager.MaskedLong m_playTime = default(StageScoreManager.MaskedLong);

	private StageTimeTable m_stageTimeTable;

	[SerializeField]
	[Header("DebugFlag にチェックを入れると、下記項目にて設定した値が適応されます")]
	private bool m_debugFlag;

	[SerializeField]
	[Header("開始時の残り時間")]
	private float m_debugStartTime = 60f;

	[SerializeField]
	[Header("アイテムの効果")]
	private float m_debugBronzeWatch = 1f;

	[SerializeField]
	private float m_debugSilverWatch = 5f;

	[SerializeField]
	private float m_debugGoldWatch = 10f;

	[SerializeField]
	private float m_debugContinue = 60f;

	private static StageTimeManager s_instance;

	public float Time
	{
		get
		{
			return m_time;
		}
	}

	public static StageTimeManager Instance
	{
		get
		{
			return s_instance;
		}
	}

	private void Awake()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_playing && !m_phantomPause && m_time > 0f)
		{
			m_time -= UnityEngine.Time.deltaTime;
			if (m_time < 0f)
			{
				m_time = 0f;
			}
			m_playTime.Set(m_playTime.Get() + (long)(UnityEngine.Time.deltaTime * 1000f));
		}
	}

	public void SetTable()
	{
		StageTimeTable stageTimeTable = GameObjectUtil.FindGameObjectComponent<StageTimeTable>("StageTimeTable");
		if (stageTimeTable != null)
		{
			m_stageTimeTable = stageTimeTable;
		}
		SetTime();
	}

	private void SetTime()
	{
		float num = 0f;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null && instance.PlayerData != null)
			{
				ServerCharacterState serverCharacterState = playerState.CharacterState(instance.PlayerData.MainChara);
				if (serverCharacterState != null)
				{
					float quickModeTimeExtension = serverCharacterState.QuickModeTimeExtension;
					num += quickModeTimeExtension;
					m_mainCharaExtend.Set((int)quickModeTimeExtension);
				}
				if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.BoostItemValidFlag[2])
				{
					ServerCharacterState serverCharacterState2 = playerState.CharacterState(instance.PlayerData.SubChara);
					if (serverCharacterState2 != null)
					{
						float quickModeTimeExtension2 = serverCharacterState2.QuickModeTimeExtension;
						num += quickModeTimeExtension2;
						m_subCharaExtend.Set((int)quickModeTimeExtension2);
					}
				}
			}
		}
		m_extendParams.Clear();
		if (!m_debugFlag && m_stageTimeTable != null && m_stageTimeTable.IsSetupEnd())
		{
			m_extendedLimit = m_stageTimeTable.GetTableItemData(StageTimeTableItem.ItemExtendedLimit);
			m_charaOverlapBonus = m_stageTimeTable.GetTableItemData(StageTimeTableItem.OverlapBonus);
			m_extendParams.Add(ExtendPattern.CONTINUE, m_stageTimeTable.GetTableItemData(StageTimeTableItem.Continue));
			m_extendParams.Add(ExtendPattern.BRONZE_WATCH, m_stageTimeTable.GetTableItemData(StageTimeTableItem.BronzeWatch));
			m_extendParams.Add(ExtendPattern.SILVER_WATCH, m_stageTimeTable.GetTableItemData(StageTimeTableItem.SilverWatch));
			m_extendParams.Add(ExtendPattern.GOLD_WATCH, m_stageTimeTable.GetTableItemData(StageTimeTableItem.GoldWatch));
			m_time = (float)m_stageTimeTable.GetTableItemData(StageTimeTableItem.StartTime) + num;
		}
		else if (m_debugFlag)
		{
			m_extendParams.Add(ExtendPattern.CONTINUE, m_debugContinue);
			m_extendParams.Add(ExtendPattern.BRONZE_WATCH, m_debugBronzeWatch);
			m_extendParams.Add(ExtendPattern.SILVER_WATCH, m_debugSilverWatch);
			m_extendParams.Add(ExtendPattern.GOLD_WATCH, m_debugGoldWatch);
			m_time = m_debugStartTime + num;
		}
		else
		{
			m_extendParams.Add(ExtendPattern.CONTINUE, 60f);
			m_extendParams.Add(ExtendPattern.BRONZE_WATCH, 1f);
			m_extendParams.Add(ExtendPattern.SILVER_WATCH, 5f);
			m_extendParams.Add(ExtendPattern.GOLD_WATCH, 10f);
			m_time = m_debugStartTime + num;
		}
		m_totalTime.Set((int)m_time);
		m_mainCharaExtend = default(StageScoreManager.MaskedInt);
		m_subCharaExtend = default(StageScoreManager.MaskedInt);
	}

	public void PlayStart()
	{
		m_playing = true;
		m_phantomPause = false;
	}

	public void PlayEnd()
	{
		m_playing = false;
	}

	public void Pause()
	{
		m_playing = false;
	}

	public void Resume()
	{
		m_playing = true;
	}

	public void PhantomPause(bool pause)
	{
		m_phantomPause = pause;
	}

	public int GetTakeTimerCount(ExtendPattern pattern)
	{
		switch (pattern)
		{
		case ExtendPattern.BRONZE_WATCH:
			return m_bronzeCount.Get();
		case ExtendPattern.SILVER_WATCH:
			return m_silverCount.Get();
		case ExtendPattern.GOLD_WATCH:
			return m_goldCount.Get();
		default:
			return 0;
		}
	}

	public void ExtendTime(ExtendPattern pattern)
	{
		if (m_extendParams.ContainsKey(pattern))
		{
			m_time += m_extendParams[pattern];
			switch (pattern)
			{
			case ExtendPattern.BRONZE_WATCH:
				m_bronzeCount.Set(m_bronzeCount.Get() + 1);
				m_extendedTime += m_extendParams[pattern];
				break;
			case ExtendPattern.SILVER_WATCH:
				m_silverCount.Set(m_silverCount.Get() + 1);
				m_extendedTime += m_extendParams[pattern];
				break;
			case ExtendPattern.GOLD_WATCH:
				m_goldCount.Set(m_goldCount.Get() + 1);
				m_extendedTime += m_extendParams[pattern];
				break;
			case ExtendPattern.CONTINUE:
				m_continuCount.Set(m_continuCount.Get() + 1);
				break;
			}
			m_totalTime.Set(m_totalTime.Get() + (int)m_extendParams[pattern]);
		}
	}

	public void ReserveExtendTime(ExtendPattern pattern)
	{
		if (m_extendParams.ContainsKey(pattern))
		{
			switch (pattern)
			{
			case ExtendPattern.BRONZE_WATCH:
			case ExtendPattern.SILVER_WATCH:
			case ExtendPattern.GOLD_WATCH:
				m_reservedExtendedTime += m_extendParams[pattern];
				break;
			}
		}
	}

	public void CancelReservedExtendTime(ExtendPattern pattern)
	{
		if (m_extendParams.ContainsKey(pattern))
		{
			switch (pattern)
			{
			case ExtendPattern.BRONZE_WATCH:
			case ExtendPattern.SILVER_WATCH:
			case ExtendPattern.GOLD_WATCH:
				m_reservedExtendedTime -= m_extendParams[pattern];
				break;
			}
		}
	}

	public float GetExtendTime(ExtendPattern pattern)
	{
		if (m_extendParams.ContainsKey(pattern))
		{
			return m_extendParams[pattern];
		}
		return 0f;
	}

	public void CheckResultTimer()
	{
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			instance.CheckNativeQuickModeResultTimer(m_goldCount.Get(), m_silverCount.Get(), m_bronzeCount.Get(), m_continuCount.Get(), m_mainCharaExtend.Get(), m_subCharaExtend.Get(), m_totalTime.Get(), m_playTime.Get());
		}
	}

	public bool IsTimeUp()
	{
		return m_time <= 0f;
	}

	public bool IsReachedExtendedLimit()
	{
		return m_extendedTime + m_reservedExtendedTime >= m_extendedLimit;
	}
}
