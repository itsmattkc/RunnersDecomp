using App.Utility;
using UnityEngine;

public class LevelInformation : MonoBehaviour
{
	private enum Status
	{
		FEVER_BOSS,
		BOSS,
		BOSS_DESTROY,
		TUTORIAL,
		BOSS_STAGE,
		MISSED,
		REQEST_PAUSE,
		REQEST_CHARA_CHANGE,
		REQEST_EQUIP_ITEM
	}

	private float m_distanceToBossOnStart;

	private float m_distanceOnStage;

	private float m_distanceToBoss;

	private float m_bossEndTime;

	private int m_numBossAttack;

	private int m_numBossHpMax;

	private Bitset32 m_status;

	private int m_playerRank;

	private bool m_lightMode;

	private Rect m_window;

	public bool m_showLevelInfo;

	private int m_feverBossCount;

	private int m_moveTrapBooRand;

	private bool m_extreme;

	private bool m_invalidExtreme;

	public float DistanceToBoss
	{
		get
		{
			return m_distanceToBoss;
		}
	}

	public bool NowFeverBoss
	{
		get
		{
			return m_status.Test(0);
		}
		set
		{
			m_status.Set(0, value);
		}
	}

	public bool NowBoss
	{
		get
		{
			return m_status.Test(1);
		}
		set
		{
			m_status.Set(1, value);
		}
	}

	public bool BossDestroy
	{
		get
		{
			return m_status.Test(2);
		}
		set
		{
			m_status.Set(2, value);
		}
	}

	public bool NowTutorial
	{
		get
		{
			return m_status.Test(3);
		}
		set
		{
			m_status.Set(3, value);
		}
	}

	public bool BossStage
	{
		get
		{
			return m_status.Test(4);
		}
		set
		{
			m_status.Set(4, value);
		}
	}

	public bool Missed
	{
		get
		{
			return m_status.Test(5);
		}
		set
		{
			m_status.Set(5, value);
		}
	}

	public bool RequestPause
	{
		get
		{
			return m_status.Test(6);
		}
		set
		{
			m_status.Set(6, value);
		}
	}

	public bool RequestCharaChange
	{
		get
		{
			return m_status.Test(6);
		}
		set
		{
			m_status.Set(6, value);
		}
	}

	public bool RequestEqitpItem
	{
		get
		{
			return m_status.Test(8);
		}
		set
		{
			m_status.Set(8, value);
		}
	}

	public float DistanceToBossOnStart
	{
		get
		{
			return m_distanceToBossOnStart;
		}
		set
		{
			m_distanceToBossOnStart = value;
		}
	}

	public float DistanceOnStage
	{
		get
		{
			return m_distanceOnStage;
		}
		set
		{
			m_distanceOnStage = value;
		}
	}

	public float BossEndTime
	{
		get
		{
			return m_bossEndTime;
		}
		set
		{
			m_bossEndTime = value;
		}
	}

	public int NumBossAttack
	{
		get
		{
			return m_numBossAttack;
		}
		set
		{
			m_numBossAttack = value;
		}
	}

	public int NumBossHpMax
	{
		get
		{
			return m_numBossHpMax;
		}
		set
		{
			m_numBossHpMax = value;
		}
	}

	public int PlayerRank
	{
		get
		{
			return m_playerRank;
		}
		set
		{
			m_playerRank = value;
		}
	}

	public bool LightMode
	{
		get
		{
			return m_lightMode;
		}
		set
		{
			m_lightMode = value;
		}
	}

	public int FeverBossCount
	{
		get
		{
			return m_feverBossCount;
		}
		set
		{
			m_feverBossCount = value;
		}
	}

	public bool Extreme
	{
		get
		{
			return m_extreme;
		}
		set
		{
			m_extreme = value;
		}
	}

	public bool InvalidExtreme
	{
		get
		{
			return m_invalidExtreme;
		}
		set
		{
			m_invalidExtreme = value;
		}
	}

	public bool DestroyRingMode
	{
		get
		{
			return m_extreme && !m_invalidExtreme;
		}
	}

	public int MoveTrapBooRand
	{
		get
		{
			return m_moveTrapBooRand;
		}
		set
		{
			m_moveTrapBooRand = value;
		}
	}

	private void Start()
	{
		m_showLevelInfo = false;
	}

	private void Update()
	{
		if (NowFeverBoss || NowBoss)
		{
			m_distanceToBoss = 0f;
		}
		else
		{
			m_distanceToBoss = Mathf.Max(0f, m_distanceToBossOnStart - m_distanceOnStage);
		}
	}

	public void AddNumBossAttack(int count)
	{
		m_numBossAttack += count;
	}
}
