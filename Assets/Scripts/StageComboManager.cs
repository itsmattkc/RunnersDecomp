using App.Utility;
using Message;
using System;
using Tutorial;
using UnityEngine;

public class StageComboManager : MonoBehaviour
{
	public enum ChaoFlagStatus
	{
		NONE = -1,
		ENEMY_DEAD,
		DESTROY_TRAP,
		DESTROY_AIRTRAP
	}

	private const int COMBO_MAX = 999999;

	private const int COMBO_UP_CORRECTION_VALUE = 50;

	private const int TUTORIAL_CLEAR_COUNT = 3;

	private const int ITEM_DRILL_SCORE = 5;

	private const int COMBO_RESERVED_TIME = 7;

	private const int MAX_COMBO_BONUS = 2500;

	public bool m_debugInfo;

	[SerializeField]
	private float ENDLESS_COMBO_TIME = 1.5f;

	[SerializeField]
	private float QUICK_COMBO_TIME = 3f;

	private float COMBO_TIME = 1.5f;

	private static readonly ComboChaoAbilityData[] ChaoAbilityTbl = new ComboChaoAbilityData[30]
	{
		new ComboChaoAbilityData(ChaoAbility.COMBO_ITEM_BOX, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_SMALL_CRYSTAL_RED, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_SMALL_CRYSTAL_GREEN, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_BIG_CRYSTAL_RED, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_BIG_CRYSTAL_GREEN, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RARE_ENEMY, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_BARRIER, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RECOVERY_ANIMAL, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RECOVERY_ALL_OBJ, ComboChaoAbilityType.TIME, 0.5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_WIPE_OUT_ENEMY, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_DESTROY_TRAP, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_DESTROY_AIR_TRAP, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_EQUIP_ITEM, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_STEP_DESTROY_GET_10_RING, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_METAL_AND_METAL_SCORE, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_DESTROY_AND_RECOVERY, ComboChaoAbilityType.TIME, 0.5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RING_BANK, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.CHAO_RING_MAGNET, ComboChaoAbilityType.EXTRA, 0f),
		new ComboChaoAbilityData(ChaoAbility.CHAO_CRYSTAL_MAGNET, ComboChaoAbilityType.EXTRA, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_EQUIP_ITEM_EXTRA, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_COLOR_POWER_ASTEROID, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_COLOR_POWER_DRILL, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_COLOR_POWER_LASER, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_CHANGE_EQUIP_ITEM, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_COLOR_POWER, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_SUPER_RING, ComboChaoAbilityType.TIME, 5f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_ADD_COMBO_VALUE, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RANDOM_ITEM_MINUS_RING, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_RANDOM_ITEM, ComboChaoAbilityType.NORMAL, 0f),
		new ComboChaoAbilityData(ChaoAbility.COMBO_ALL_SPECIAL_CRYSTAL, ComboChaoAbilityType.TIME, 5f)
	};

	public static int COMBO_NUM = 7;

	public static int BONUS_NUM = 8;

	private int[] m_comboNumList = new int[COMBO_NUM];

	private int[] m_bonusNumList = new int[BONUS_NUM];

	private int m_comboCount;

	private int m_viewComboNum;

	private int m_viewBonusNum;

	private int m_chaoComboAbilityBonus;

	private bool m_viewBonusMax;

	private float m_time;

	private PlayerInformation m_playerInfo;

	private bool m_pauseCombo;

	private bool m_pauseTimer;

	private bool m_stopCombo;

	private bool m_comboItem;

	private bool m_reservedTimeFlag;

	private float m_reservedTime;

	private float m_receptionRate = 1f;

	private StageItemManager m_itemManager;

	private int m_maxComboCount;

	private int m_enemyBreak;

	private Bitset32 m_chaoFlag = new Bitset32(0u);

	public static float CHAO_OBJ_DEAD_TIME = 2f;

	private ComboChaoParam[] m_chaoParam = new ComboChaoParam[2];

	private ComboAcviteParam[] m_chaoCombActiveParam = new ComboAcviteParam[2];

	private static StageComboManager m_instance = null;

	public int MaxComboCount
	{
		get
		{
			return m_maxComboCount;
		}
	}

	public static StageComboManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private StageItemManager GetItemManager()
	{
		if (StageItemManager.Instance != null)
		{
			return StageItemManager.Instance;
		}
		return null;
	}

	public void SetComboTime(bool quick)
	{
		if (quick)
		{
			COMBO_TIME = QUICK_COMBO_TIME;
		}
		else
		{
			COMBO_TIME = ENDLESS_COMBO_TIME;
		}
	}

	public void Setup()
	{
		m_comboCount = 0;
		m_viewComboNum = 0;
		m_viewBonusNum = 0;
		m_viewBonusMax = false;
		m_pauseCombo = false;
		m_pauseTimer = false;
		m_stopCombo = false;
		m_comboItem = false;
		m_reservedTimeFlag = false;
		m_playerInfo = ObjUtil.GetPlayerInformation();
		GameObject gameObject = GameObject.Find("GameModeStage");
		if (gameObject != null)
		{
			GameModeStage component = gameObject.GetComponent<GameModeStage>();
			if (component != null)
			{
				ObjectPartTable objectPartTable = component.GetObjectPartTable();
				if (objectPartTable != null)
				{
					for (int i = 0; i < COMBO_NUM; i++)
					{
						m_comboNumList[i] = objectPartTable.GetComboBonusComboNum(i);
					}
					for (int j = 0; j < BONUS_NUM; j++)
					{
						m_bonusNumList[j] = objectPartTable.GetComboBonusBonusNum(j);
					}
				}
			}
		}
		bool changeChara = false;
		SetupChao(changeChara);
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (m_reservedTime > 0f)
		{
			if (!m_pauseCombo && !m_pauseTimer)
			{
				m_reservedTime -= deltaTime;
			}
			if (m_time < COMBO_TIME)
			{
				m_time = COMBO_TIME;
			}
		}
		UpdateChao(deltaTime);
		bool flag = IsComboItem();
		if (flag && m_time < COMBO_TIME)
		{
			m_time = COMBO_TIME;
		}
		if (!m_pauseCombo && !m_pauseTimer && !flag && m_time > 0f)
		{
			m_time -= deltaTime;
			if (!(m_time > 0f))
			{
				if (m_maxComboCount < m_comboCount)
				{
					m_maxComboCount = m_comboCount;
				}
				m_time = 0f;
				m_comboCount = 0;
				ResetChao();
			}
		}
		if (m_viewComboNum != 0)
		{
			MsgCaution caution = new MsgCaution(HudCaution.Type.COMBO_N, m_viewComboNum, flag || m_reservedTime > 0f);
			HudCaution.Instance.SetCaution(caution);
		}
		else if ((m_reservedTimeFlag || m_comboItem) && m_comboCount > 0 && !flag && m_reservedTime <= 0f)
		{
			MsgCaution caution2 = new MsgCaution(HudCaution.Type.COMBO_N, m_comboCount, false);
			HudCaution.Instance.SetCaution(caution2);
		}
		if (m_viewBonusNum != 0)
		{
			if (flag)
			{
				MsgCaution caution3 = new MsgCaution(HudCaution.Type.COMBOITEM_BONUS_N, m_viewBonusNum, m_viewBonusMax);
				HudCaution.Instance.SetCaution(caution3);
			}
			else
			{
				MsgCaution caution4 = new MsgCaution(HudCaution.Type.BONUS_N, m_viewBonusNum, m_viewBonusMax);
				HudCaution.Instance.SetCaution(caution4);
			}
		}
		if (m_stopCombo)
		{
			MsgCaution invisibleCaution = new MsgCaution(HudCaution.Type.COMBO_N);
			MsgCaution invisibleCaution2 = new MsgCaution(HudCaution.Type.BONUS_N);
			MsgCaution invisibleCaution3 = new MsgCaution(HudCaution.Type.COMBOITEM_BONUS_N);
			HudCaution.Instance.SetInvisibleCaution(invisibleCaution);
			HudCaution.Instance.SetInvisibleCaution(invisibleCaution2);
			HudCaution.Instance.SetInvisibleCaution(invisibleCaution3);
		}
		m_viewComboNum = 0;
		m_viewBonusNum = 0;
		m_viewBonusMax = false;
		m_stopCombo = false;
		m_comboItem = flag;
		m_reservedTimeFlag = (m_reservedTime > 0f);
	}

	public void AddComboForChaoAbilityValue(int addCombo)
	{
		m_comboCount += addCombo;
		if (m_comboCount > 999999)
		{
			m_comboCount = 999999;
		}
		CheckChao(m_comboCount);
		int level = GetLevel(m_comboCount);
		int num = GetBonus(level);
		if (num > 2500)
		{
			num = 2500;
		}
		ObjUtil.SendMessageAddScore(num);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(0, num));
		if (m_time < COMBO_TIME)
		{
			m_time = COMBO_TIME;
		}
		m_viewComboNum = m_comboCount;
		m_viewBonusNum = num;
		if (level == COMBO_NUM)
		{
			m_viewBonusMax = true;
		}
		else
		{
			m_viewBonusMax = false;
		}
	}

	public void AddCombo()
	{
		if (!m_pauseCombo)
		{
			AddCombo(1);
		}
	}

	public void DebugAddCombo(int val)
	{
	}

	private void AddCombo(int add)
	{
		m_comboCount += add;
		if (m_comboCount > 999999)
		{
			m_comboCount = 999999;
		}
		CheckChao(m_comboCount);
		int level = GetLevel(m_comboCount);
		int bonus = GetBonus(level);
		int num = GetComboScore(bonus);
		if (num > 2500)
		{
			num = 2500;
		}
		ObjUtil.SendMessageAddScore(num);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(0, num));
		if (m_comboCount >= 3)
		{
			ObjUtil.SendMessageTutorialClear(EventID.RING_BONUS);
		}
		if (m_receptionRate > 0f)
		{
			m_reservedTime = m_receptionRate * COMBO_TIME;
		}
		if (m_time < COMBO_TIME)
		{
			m_time = COMBO_TIME;
		}
		m_viewComboNum = m_comboCount;
		m_viewBonusNum = num;
		if (level == COMBO_NUM)
		{
			m_viewBonusMax = true;
		}
		else
		{
			m_viewBonusMax = false;
		}
	}

	public int GetLevel(int comboCount)
	{
		if (comboCount > m_comboNumList[COMBO_NUM - 1])
		{
			return COMBO_NUM;
		}
		for (int i = 0; i < COMBO_NUM; i++)
		{
			int num = 0;
			int num2 = m_comboNumList[i];
			if (i > 0)
			{
				num = m_comboNumList[i - 1];
			}
			if (num < comboCount && comboCount <= num2)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetBonus(int level)
	{
		if ((uint)level < (uint)BONUS_NUM)
		{
			return m_bonusNumList[level] + m_chaoComboAbilityBonus;
		}
		return 0;
	}

	private int GetComboScore(int score)
	{
		if (IsDrillItem())
		{
			return score * 5;
		}
		if (IsComboItem())
		{
			StageItemManager itemManager = GetItemManager();
			if (itemManager != null)
			{
				return score * itemManager.GetComboScoreRate();
			}
		}
		return score;
	}

	private bool IsComboItem()
	{
		if (m_playerInfo != null && m_playerInfo.IsNowCombo())
		{
			return true;
		}
		if (IsDrillItem())
		{
			return true;
		}
		return false;
	}

	private bool IsDrillItem()
	{
		if (m_playerInfo != null && m_playerInfo.PhantomType == PhantomType.DRILL)
		{
			return true;
		}
		return false;
	}

	private void OnMsgStopCombo(MsgStopCombo msg)
	{
		if (!IsComboItem())
		{
			if (m_maxComboCount < m_comboCount)
			{
				m_maxComboCount = m_comboCount;
			}
			m_time = 0f;
			m_reservedTime = 0f;
			m_comboCount = 0;
			m_stopCombo = true;
			ResetChao();
		}
	}

	private void OnMsgPauseComboTimer(MsgPauseComboTimer msg)
	{
		switch (msg.m_value)
		{
		case MsgPauseComboTimer.State.PAUSE:
			m_pauseCombo = true;
			break;
		case MsgPauseComboTimer.State.PAUSE_TIMER:
			m_pauseTimer = true;
			break;
		case MsgPauseComboTimer.State.PLAY:
			m_pauseCombo = false;
			m_pauseTimer = false;
			break;
		case MsgPauseComboTimer.State.PLAY_SET:
			m_pauseCombo = false;
			m_pauseTimer = false;
			m_reservedTime = 0f;
			if (m_comboCount > 0 && msg.m_time > 0f)
			{
				if (msg.m_time > COMBO_TIME)
				{
					m_reservedTime = msg.m_time - COMBO_TIME;
					m_time = COMBO_TIME;
				}
				else if (m_time < msg.m_time)
				{
					m_time = msg.m_time;
				}
			}
			break;
		case MsgPauseComboTimer.State.PLAY_RESET:
			m_pauseCombo = false;
			m_pauseTimer = false;
			m_reservedTime = 0f;
			if (m_comboCount > 0 && m_time > 0f)
			{
				m_time = COMBO_TIME;
			}
			break;
		}
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}

	private void SetupChao(bool changeChara)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			m_chaoComboAbilityBonus = (int)instance.GetChaoAbilityValue(ChaoAbility.COMBO_BONUS_UP);
			if (!changeChara)
			{
				for (int i = 0; i < m_chaoCombActiveParam.Length; i++)
				{
					if (m_chaoCombActiveParam[i] != null)
					{
						continue;
					}
					m_chaoCombActiveParam[i] = new ComboAcviteParam();
					for (int j = 0; j < ChaoAbilityTbl.Length; j++)
					{
						ChaoAbility chaoAbility = ChaoAbilityTbl[j].m_chaoAbility;
						if (instance.HasChaoAbility(chaoAbility, (ChaoType)i) && ChaoAbilityTbl[j].m_type == ComboChaoAbilityType.TIME)
						{
							m_chaoCombActiveParam[i].m_chaoAbility = ChaoAbilityTbl[j].m_chaoAbility;
							m_chaoCombActiveParam[i].m_timeMax = ChaoAbilityTbl[j].m_timeMax;
						}
					}
				}
			}
			for (int k = 0; k < m_chaoParam.Length; k++)
			{
				int continuationNum = 0;
				if (m_chaoParam[k] != null)
				{
					continuationNum = m_chaoParam[k].m_continuationNum;
				}
				m_chaoParam[k] = new ComboChaoParam(ChaoAbility.UNKNOWN, ComboChaoAbilityType.NORMAL, 0f, 0, 0);
				for (int l = 0; l < ChaoAbilityTbl.Length; l++)
				{
					ChaoAbility chaoAbility2 = ChaoAbilityTbl[l].m_chaoAbility;
					if (instance.HasChaoAbility(chaoAbility2, (ChaoType)k))
					{
						m_chaoParam[k].m_chaoAbility = chaoAbility2;
						m_chaoParam[k].m_extraValue = instance.GetChaoAbilityExtraValue(chaoAbility2, (ChaoType)k);
						m_chaoParam[k].m_continuationNum = continuationNum;
						if (ChaoAbilityTbl[l].m_type == ComboChaoAbilityType.EXTRA)
						{
							m_chaoParam[k].m_comboNum = (int)m_chaoParam[k].m_extraValue;
						}
						else
						{
							m_chaoParam[k].m_comboNum = (int)instance.GetChaoAbilityValue(chaoAbility2, (ChaoType)k);
						}
						if (chaoAbility2 == ChaoAbility.COMBO_ADD_COMBO_VALUE && m_chaoParam[k].m_extraValue > (float)m_chaoParam[k].m_comboNum)
						{
							m_chaoParam[k].m_extraValue = Math.Max(m_chaoParam[k].m_comboNum - 10, 1);
						}
					}
				}
			}
		}
		SetReceptionTime();
		if (changeChara)
		{
			ResetChaoForChangeChara();
		}
		else
		{
			ResetChao();
		}
		for (int m = 0; m < m_chaoParam.Length; m++)
		{
			SetDebugDraw("SetupChao ChaoAbility=" + m_chaoParam[m].m_chaoAbility.ToString() + " index=" + m + " m_comboNum=" + m_chaoParam[m].m_comboNum + " m_nextCombo=" + m_chaoParam[m].m_nextCombo);
		}
	}

	private void UpdateChao(float delta)
	{
		for (int i = 0; i < m_chaoParam.Length; i++)
		{
			if (m_chaoParam[i] != null)
			{
				m_chaoParam[i].m_movement = false;
			}
		}
		for (int j = 0; j < m_chaoCombActiveParam.Length; j++)
		{
			if (m_chaoCombActiveParam[j] != null && m_chaoCombActiveParam[j].m_chaoAbility != ChaoAbility.UNKNOWN && m_chaoCombActiveParam[j].m_time > 0f)
			{
				m_chaoCombActiveParam[j].m_time -= delta;
				if (m_chaoCombActiveParam[j].m_time < 0f)
				{
					m_chaoCombActiveParam[j].m_time = 0f;
				}
				SetDebugDraw("UpdateChao ChaoAbility=" + m_chaoCombActiveParam[j].m_chaoAbility.ToString() + " index=" + j + " time=" + m_chaoCombActiveParam[j].m_time);
			}
		}
	}

	private void CheckChao(int comboCount)
	{
		for (int i = 0; i < m_chaoParam.Length; i++)
		{
			if (m_chaoParam[i] != null && m_chaoParam[i].m_comboNum != 0 && comboCount >= m_chaoParam[i].m_nextCombo)
			{
				m_chaoParam[i].m_nextCombo += m_chaoParam[i].m_comboNum;
				m_chaoParam[i].m_continuationNum++;
				if (comboCount >= m_chaoParam[i].m_nextCombo)
				{
					CheckChao(comboCount);
				}
				if (!m_chaoParam[i].m_movement)
				{
					SetDebugDraw("CheckChao ChaoAbility=" + m_chaoParam[i].m_chaoAbility.ToString() + " index=" + i + " m_comboNum=" + m_chaoParam[i].m_comboNum + " m_nextCombo=" + m_chaoParam[i].m_nextCombo);
					SetActiveComboChaoAbility((ChaoType)i);
					m_chaoParam[i].m_movement = true;
				}
			}
		}
	}

	private void SetReceptionTime()
	{
		m_receptionRate = 0f;
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.COMBO_RECEPTION_TIME))
		{
			m_receptionRate = StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.COMBO_RECEPTION_TIME) * 0.01f;
		}
	}

	private void ResetChao()
	{
		for (int i = 0; i < m_chaoParam.Length; i++)
		{
			if (m_chaoParam[i] != null && m_chaoParam[i].m_comboNum != 0)
			{
				m_chaoParam[i].m_nextCombo = m_chaoParam[i].m_comboNum;
				m_chaoParam[i].m_continuationNum = 0;
			}
		}
	}

	private void ResetChaoForChangeChara()
	{
		for (int i = 0; i < m_chaoParam.Length; i++)
		{
			if (m_chaoParam[i] != null && m_chaoParam[i].m_comboNum != 0)
			{
				m_chaoParam[i].m_nextCombo = m_chaoParam[i].m_comboNum;
				m_chaoParam[i].m_nextCombo += m_chaoParam[i].m_continuationNum * m_chaoParam[i].m_comboNum;
			}
		}
	}

	public void SetChaoFlagStatus(ChaoFlagStatus status, bool flag)
	{
		m_chaoFlag.Set((int)status, flag);
	}

	public bool IsChaoFlagStatus(ChaoFlagStatus status)
	{
		return m_chaoFlag.Test((int)status);
	}

	public bool IsActiveComboChaoAbility(ChaoAbility chaoAbility)
	{
		for (int i = 0; i < m_chaoCombActiveParam.Length; i++)
		{
			if (m_chaoCombActiveParam[i] != null && m_chaoCombActiveParam[i].m_chaoAbility == chaoAbility && m_chaoCombActiveParam[i].m_time > 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void SetActiveComboChaoAbility(ChaoType chaoType)
	{
		if ((uint)chaoType < m_chaoParam.Length && m_chaoParam[(int)chaoType] != null)
		{
			if (m_chaoCombActiveParam[(int)chaoType] != null && m_chaoCombActiveParam[(int)chaoType].m_chaoAbility != ChaoAbility.UNKNOWN)
			{
				m_chaoCombActiveParam[(int)chaoType].m_time = m_chaoCombActiveParam[(int)chaoType].m_timeMax;
			}
			bool withEffect = true;
			switch (m_chaoParam[(int)chaoType].m_chaoAbility)
			{
			case ChaoAbility.COMBO_ITEM_BOX:
			case ChaoAbility.COMBO_BARRIER:
			case ChaoAbility.COMBO_WIPE_OUT_ENEMY:
			case ChaoAbility.COMBO_COLOR_POWER:
			case ChaoAbility.COMBO_DESTROY_AIR_TRAP:
			case ChaoAbility.COMBO_STEP_DESTROY_GET_10_RING:
			case ChaoAbility.COMBO_EQUIP_ITEM:
			case ChaoAbility.CHAO_RING_MAGNET:
			case ChaoAbility.CHAO_CRYSTAL_MAGNET:
			case ChaoAbility.COMBO_RING_BANK:
			case ChaoAbility.COMBO_METAL_AND_METAL_SCORE:
			case ChaoAbility.COMBO_EQUIP_ITEM_EXTRA:
			case ChaoAbility.COMBO_CHANGE_EQUIP_ITEM:
			case ChaoAbility.COMBO_COLOR_POWER_DRILL:
			case ChaoAbility.COMBO_COLOR_POWER_LASER:
			case ChaoAbility.COMBO_COLOR_POWER_ASTEROID:
			case ChaoAbility.COMBO_RANDOM_ITEM_MINUS_RING:
			case ChaoAbility.COMBO_RANDOM_ITEM:
				withEffect = false;
				break;
			case ChaoAbility.COMBO_ADD_COMBO_VALUE:
			{
				int add = (int)m_chaoParam[(int)chaoType].m_extraValue;
				AddCombo(add);
				break;
			}
			case ChaoAbility.COMBO_RECOVERY_ALL_OBJ:
			case ChaoAbility.COMBO_DESTROY_AND_RECOVERY:
				withEffect = false;
				GameObjectUtil.SendDelayedMessageToTagObjects("Ring", "OnDrawingRingsChaoAbility", new MsgOnDrawingRings(m_chaoParam[(int)chaoType].m_chaoAbility));
				GameObjectUtil.SendDelayedMessageToTagObjects("Crystal", "OnDrawingRingsChaoAbility", new MsgOnDrawingRings(m_chaoParam[(int)chaoType].m_chaoAbility));
				GameObjectUtil.SendDelayedMessageToTagObjects("Timer", "OnDrawingRingsChaoAbility", new MsgOnDrawingRings(m_chaoParam[(int)chaoType].m_chaoAbility));
				m_reservedTime = 7f;
				break;
			}
			ObjUtil.RequestStartAbilityToChao(m_chaoParam[(int)chaoType].m_chaoAbility, withEffect);
			SetDebugDraw("SetActiveComboChaoAbility ChaoAbility=" + m_chaoParam[(int)chaoType].m_chaoAbility.ToString() + " index=" + (int)chaoType);
		}
	}

	private void OnPassPointMarker()
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (!(instance == null) && instance.HasChaoAbility(ChaoAbility.CHECK_POINT_COMBO_UP))
		{
			int num = (int)instance.GetChaoAbilityValue(ChaoAbility.CHECK_POINT_COMBO_UP);
			if (num > 0)
			{
				AddCombo(num);
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.CHECK_POINT_COMBO_UP, true);
			}
			SetDebugDraw("OnPassPointMarker upComboCount=" + num);
		}
	}

	public void OnChangeCharaSucceed(MsgChangeCharaSucceed msg)
	{
		SetDebugDraw("OnChangeCharaSucceed");
		bool changeChara = true;
		SetupChao(changeChara);
	}

	public void OnChaoAbilityEnemyBreak()
	{
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.ENEMY_COUNT_BOMB))
		{
			m_enemyBreak++;
			Debug.LogWarning("OnChaoAbilityEnemyBreak m_enemyBreak = " + m_enemyBreak);
			int num = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.ENEMY_COUNT_BOMB);
			if (m_enemyBreak >= num)
			{
				m_enemyBreak = 0;
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.ENEMY_COUNT_BOMB, false);
			}
		}
	}

	private void SetDebugDraw(string msg)
	{
	}
}
