using UnityEngine;

public class StageInfo : MonoBehaviour
{
	public class MileageMapInfo
	{
		public MileageMapState m_mapState = new MileageMapState();

		public long[] m_pointScore = new long[6];
	}

	private string m_stageName;

	private BossType m_bossType;

	private int m_numBossAttack;

	private TenseType m_tenseType;

	private MileageMapInfo m_mapInfo;

	private bool m_notChangeTense;

	private bool m_existBoss;

	private bool m_bossStage;

	private bool m_fromTitle;

	private bool m_tutorialStage;

	private bool m_eventStage;

	private bool m_quickMode;

	private bool m_firstTutorial;

	private bool[] m_boostItemValid;

	private ItemType[] m_equippedItem;

	public string SelectedStageName
	{
		get
		{
			return m_stageName;
		}
		set
		{
			m_stageName = value;
		}
	}

	public BossType BossType
	{
		get
		{
			return m_bossType;
		}
		set
		{
			m_bossType = value;
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

	public TenseType TenseType
	{
		get
		{
			return m_tenseType;
		}
		set
		{
			m_tenseType = value;
		}
	}

	public MileageMapInfo MileageInfo
	{
		get
		{
			return m_mapInfo;
		}
		set
		{
			m_mapInfo = value;
		}
	}

	public bool NotChangeTense
	{
		get
		{
			return m_notChangeTense;
		}
		set
		{
			m_notChangeTense = value;
		}
	}

	public bool ExistBoss
	{
		get
		{
			return m_existBoss;
		}
		set
		{
			m_existBoss = value;
		}
	}

	public bool BossStage
	{
		get
		{
			return m_bossStage;
		}
		set
		{
			m_bossStage = value;
		}
	}

	public bool FromTitle
	{
		get
		{
			return m_fromTitle;
		}
		set
		{
			m_fromTitle = value;
		}
	}

	public bool FirstTutorial
	{
		get
		{
			return m_firstTutorial;
		}
		set
		{
			m_firstTutorial = value;
		}
	}

	public bool TutorialStage
	{
		get
		{
			return m_tutorialStage;
		}
		set
		{
			m_tutorialStage = value;
		}
	}

	public bool EventStage
	{
		get
		{
			return m_eventStage;
		}
		set
		{
			m_eventStage = value;
		}
	}

	public bool QuickMode
	{
		get
		{
			return m_quickMode;
		}
		set
		{
			m_quickMode = value;
		}
	}

	public bool[] BoostItemValid
	{
		get
		{
			return m_boostItemValid;
		}
		set
		{
			m_boostItemValid = value;
		}
	}

	public ItemType[] EquippedItems
	{
		get
		{
			return m_equippedItem;
		}
		set
		{
			m_equippedItem = new ItemType[value.Length];
			value.CopyTo(m_equippedItem, 0);
		}
	}

	public static string GetStageNameByIndex(int index)
	{
		return "w" + index.ToString("D2");
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		m_stageName = string.Empty;
		m_bossType = BossType.FEVER;
		m_tenseType = TenseType.AFTERNOON;
		m_numBossAttack = 0;
		m_mapInfo = new MileageMapInfo();
		m_boostItemValid = new bool[3];
		m_equippedItem = new ItemType[3];
		for (int i = 0; i < 3; i++)
		{
			m_equippedItem[i] = ItemType.UNKNOWN;
		}
		base.enabled = false;
	}
}
