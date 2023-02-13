using Text;
using UnityEngine;

public class HudEventResult : MonoBehaviour
{
	public enum EventType
	{
		SP_STAGE,
		RAID_BOSS,
		ANIMAL,
		NUM
	}

	public enum AnimType
	{
		IDLE,
		IN,
		IN_BONUS,
		WAIT_ADD_COLLECT_OBJECT,
		ADD_COLLECT_OBJECT,
		SHOW_QUOTA_LIST,
		OUT_WAIT,
		OUT,
		NUM
	}

	private enum State
	{
		STATE_IDLE,
		STATE_TIME_UP_WINDOW_START,
		STATE_TIME_UP_WINDOW,
		STATE_RESULT_START,
		STATE_RESULT,
		NUM
	}

	public delegate void AnimationEndCallback(AnimType animType);

	private HudEventResultParts m_parts;

	private AnimType m_currentAnim;

	private bool m_isEndResult;

	private bool m_isEndOutAnim;

	private bool m_eventTimeup;

	private long m_beforeTotalPoint;

	private State m_state;

	public bool IsEndResult
	{
		get
		{
			return m_isEndResult;
		}
		private set
		{
		}
	}

	public bool IsEndOutAnim
	{
		get
		{
			return m_isEndOutAnim;
		}
		private set
		{
		}
	}

	public bool IsBackkeyEnable()
	{
		bool result = true;
		if (m_parts != null)
		{
			result = m_parts.IsBackkeyEnable();
		}
		return result;
	}

	public void Setup(bool eventTimeup)
	{
		if (m_parts != null)
		{
			Object.Destroy(m_parts.gameObject);
			m_parts = null;
		}
		m_eventTimeup = eventTimeup;
		EventManager instance = EventManager.Instance;
		if (instance != null)
		{
			instance.SetEventInfo();
		}
		HudEventResultParts component = base.gameObject.GetComponent<HudEventResultParts>();
		if (component != null)
		{
			m_parts = component;
			m_parts.Init(base.gameObject, m_beforeTotalPoint, EventResultAnimEndCallback);
		}
	}

	public void PlayStart()
	{
		m_isEndResult = false;
		base.gameObject.SetActive(true);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "EventResult_Anim");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		if (m_eventTimeup)
		{
			m_state = State.STATE_TIME_UP_WINDOW_START;
		}
		else
		{
			m_state = State.STATE_RESULT_START;
		}
	}

	public void PlayOutAnimation()
	{
		if (m_eventTimeup)
		{
			m_isEndOutAnim = true;
			return;
		}
		m_isEndOutAnim = false;
		if (m_parts != null)
		{
			m_currentAnim = AnimType.OUT;
			m_parts.PlayAnimation(m_currentAnim);
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Result");
			if (gameObject2 != null)
			{
				Vector3 localPosition = base.gameObject.transform.localPosition;
				Vector3 localScale = base.gameObject.transform.localScale;
				base.gameObject.transform.parent = gameObject2.transform;
				base.gameObject.transform.localPosition = localPosition;
				base.gameObject.transform.localScale = localScale;
			}
		}
		base.gameObject.SetActive(false);
		EventManager instance = EventManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		switch (instance.Type)
		{
		case EventManager.EventType.SPECIAL_STAGE:
			base.gameObject.AddComponent<HudEventResultSpStage>();
			if (instance.SpecialStageInfo != null)
			{
				m_beforeTotalPoint = instance.SpecialStageInfo.totalPoint;
			}
			break;
		case EventManager.EventType.RAID_BOSS:
			base.gameObject.AddComponent<HudEventResultRaidBoss>();
			if (instance.RaidBossInfo != null)
			{
				m_beforeTotalPoint = instance.RaidBossInfo.totalPoint;
			}
			break;
		case EventManager.EventType.COLLECT_OBJECT:
			base.gameObject.AddComponent<HudEventResultCollect>();
			if (instance.EtcEventInfo != null)
			{
				m_beforeTotalPoint = instance.EtcEventInfo.totalPoint;
			}
			break;
		}
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.STATE_IDLE:
			break;
		case State.STATE_RESULT:
			break;
		case State.STATE_TIME_UP_WINDOW_START:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "EventTimeupResult";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "event_finished_game_result_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "event_finished_game_result").text;
			GeneralWindow.Create(info);
			m_state = State.STATE_TIME_UP_WINDOW;
			break;
		}
		case State.STATE_TIME_UP_WINDOW:
			if (GeneralWindow.IsCreated("EventTimeupResult") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				m_isEndOutAnim = true;
				m_state = State.STATE_RESULT;
			}
			break;
		case State.STATE_RESULT_START:
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "EventResult_Anim");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			if (m_parts != null)
			{
				m_currentAnim = AnimType.IN;
				m_parts.PlayAnimation(m_currentAnim);
			}
			m_state = State.STATE_RESULT;
			break;
		}
		}
	}

	private void EventResultAnimEndCallback(AnimType animType)
	{
		m_currentAnim = animType + 1;
		if (m_currentAnim == AnimType.OUT_WAIT)
		{
			m_isEndResult = true;
		}
		else if (m_currentAnim >= AnimType.NUM)
		{
			m_isEndOutAnim = true;
			m_currentAnim = AnimType.IDLE;
		}
		else if (m_parts != null)
		{
			m_parts.PlayAnimation(m_currentAnim);
		}
	}
}
