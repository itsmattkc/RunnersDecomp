using UnityEngine;

public class StageModeManager : MonoBehaviour
{
	public enum Mode
	{
		ENDLESS,
		QUICK,
		UNKNOWN
	}

	private static StageModeManager m_instance;

	[Header("debugFlag にチェックを入れると、Console にテキストが表示されます")]
	public bool m_debugFlag;

	public bool m_firstTutorial;

	[Header("モード設定パラメータ")]
	public Mode m_mode = Mode.UNKNOWN;

	private CharacterAttribute m_stageCharaAttribute;

	private int m_stageIndex = 1;

	public static StageModeManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public Mode StageMode
	{
		get
		{
			return m_mode;
		}
		set
		{
			m_mode = value;
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

	public CharacterAttribute QuickStageCharaAttribute
	{
		get
		{
			return m_stageCharaAttribute;
		}
		private set
		{
		}
	}

	public int QuickStageIndex
	{
		get
		{
			return m_stageIndex;
		}
		private set
		{
		}
	}

	public bool IsQuickMode()
	{
		return m_mode == Mode.QUICK;
	}

	public void DrawQuickStageIndex()
	{
		m_stageIndex = 1;
		if (EventManager.Instance != null && EventManager.Instance.IsQuickEvent())
		{
			EventStageData stageData = EventManager.Instance.GetStageData();
			if (stageData != null)
			{
				m_stageIndex = MileageMapUtility.GetStageIndex(stageData.stage_key);
			}
		}
		else
		{
			Random.seed = NetUtil.GetCurrentUnixTime();
			int num = 1;
			int num2 = 4;
			int num3 = Random.Range(num, num2);
			if (num3 >= num2)
			{
				num3 = num;
			}
			m_stageIndex = num3;
		}
		switch (m_stageIndex)
		{
		case 1:
			m_stageCharaAttribute = CharacterAttribute.SPEED;
			break;
		case 2:
			m_stageCharaAttribute = CharacterAttribute.FLY;
			break;
		case 3:
			m_stageCharaAttribute = CharacterAttribute.POWER;
			break;
		default:
			m_stageCharaAttribute = CharacterAttribute.UNKNOWN;
			break;
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			m_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void SetDebugDraw(string msg)
	{
	}
}
