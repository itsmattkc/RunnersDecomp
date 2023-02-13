using Message;
using Tutorial;
using UnityEngine;

public class StageTutorialManager : MonoBehaviour
{
	private enum Mode
	{
		Idle,
		PlayStart,
		PlayEnd
	}

	private enum EventState : uint
	{
		Clear = 1u,
		Damage = 2u,
		Miss = 4u
	}

	public bool m_debugDraw;

	private bool m_debugSkip;

	private static float RetryOffsetPos = 10f;

	private Mode m_mode;

	private uint m_eventState;

	private int m_currentEventID;

	private bool m_complete;

	private Vector3 m_startPos = Vector3.zero;

	private TempTutorialScore m_tempScore;

	private static StageTutorialManager instance;

	public EventID CurrentEventID
	{
		get
		{
			return (EventID)m_currentEventID;
		}
	}

	public static StageTutorialManager Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	public void SetupTutorial()
	{
		DebugDraw("SetupTutorial");
		m_tempScore = new TempTutorialScore();
		EventID id = EventID.JUMP;
		StartTutorial(id);
		m_mode = Mode.Idle;
	}

	private void StartTutorial(EventID id)
	{
		DebugDraw("StartTutorial id=" + id);
		m_currentEventID = (int)id;
		m_eventState = 0u;
		m_complete = false;
		if (m_currentEventID == 9)
		{
			CompleteTutorial();
		}
	}

	private void CompleteTutorial()
	{
		DebugDraw("CompleteTutorial");
		m_currentEventID = 9;
		m_complete = true;
	}

	public void OnMsgTutorialStart(MsgTutorialStart msg)
	{
		if (!IsCompletedTutorial() && m_mode == Mode.Idle)
		{
			m_startPos = msg.m_pos;
			m_eventState = 0u;
			GetTempScore();
			MsgTutorialPlayStart value = new MsgTutorialPlayStart((EventID)m_currentEventID);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialPlayStart", value, SendMessageOptions.DontRequireReceiver);
			DebugDraw("OnMsgTutorialStart m_startPos=" + m_startPos);
			m_mode = Mode.PlayStart;
		}
	}

	public void OnMsgTutorialEnd(MsgTutorialEnd msg)
	{
		if (!IsCompletedTutorial() && m_mode == Mode.PlayStart)
		{
			bool flag = CheckNextTutorial();
			Vector3 vector = (!flag) ? GetNextStartCollisionPosition() : m_startPos;
			vector -= Vector3.right * RetryOffsetPos;
			Quaternion identity = Quaternion.identity;
			MsgTutorialPlayEnd value = new MsgTutorialPlayEnd(IsCompletedTutorial(), flag, (EventID)m_currentEventID, vector, identity);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialPlayEnd", value, SendMessageOptions.DontRequireReceiver);
			DebugDraw("OnMsgTutorialEnd nextStartPos=" + vector);
			m_mode = Mode.PlayEnd;
		}
	}

	public void OnMsgTutorialDebugEnd(MsgTutorialEnd msg)
	{
	}

	public void OnMsgTutorialNext(MsgTutorialNext msg)
	{
		if (m_mode == Mode.PlayEnd)
		{
			if (IsCompletedTutorial())
			{
				GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnBossEnd", new MsgBossEnd(true), SendMessageOptions.DontRequireReceiver);
			}
			DebugDraw("OnMsgTutorialNext");
			m_mode = Mode.Idle;
		}
	}

	public void OnMsgTutorialClear(MsgTutorialClear msg)
	{
		if (IsCompletedTutorial() && !IsPlayStart())
		{
			return;
		}
		foreach (MsgTutorialClear.Data datum in msg.m_data)
		{
			if (datum.eventid == (EventID)m_currentEventID)
			{
				DebugDraw("OnMsgTutorialClear id=" + datum.eventid);
				SetEventState(EventState.Clear);
			}
		}
	}

	public void OnMsgTutorialDamage(MsgTutorialDamage msg)
	{
		if (!IsCompletedTutorial() || IsPlayStart())
		{
			SetEventState(EventState.Damage);
			EventClearType eventClearType = Tutorial.EventData.GetEventClearType((EventID)m_currentEventID);
			if (eventClearType == EventClearType.NO_DAMAGE)
			{
				OnMsgTutorialEnd(new MsgTutorialEnd());
			}
		}
	}

	public void OnMsgTutorialMiss(MsgTutorialMiss msg)
	{
		if (!IsCompletedTutorial() || IsPlayStart())
		{
			SetEventState(EventState.Miss);
			OnMsgTutorialEnd(new MsgTutorialEnd());
		}
	}

	public bool IsCompletedTutorial()
	{
		return m_complete;
	}

	private bool IsClearTutorial()
	{
		if (IsCompletedTutorial())
		{
			return true;
		}
		if (m_currentEventID == 9)
		{
			return true;
		}
		switch (Tutorial.EventData.GetEventClearType((EventID)m_currentEventID))
		{
		case EventClearType.CLEAR:
			if (IsEventState(EventState.Clear))
			{
				return true;
			}
			break;
		case EventClearType.NO_DAMAGE:
			if (!IsEventState(EventState.Miss) && !IsEventState(EventState.Damage))
			{
				return true;
			}
			break;
		case EventClearType.NO_MISS:
			if (!IsEventState(EventState.Miss))
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool CheckNextTutorial()
	{
		bool result = false;
		int currentEventID = m_currentEventID;
		if (IsClearTutorial())
		{
			currentEventID++;
			if (currentEventID >= 8)
			{
				DebugDraw("CheckNextTutorial Complete!");
				CompleteTutorial();
			}
			else
			{
				DebugDraw("CheckNextTutorial Clear!");
				StartTutorial((EventID)currentEventID);
			}
		}
		else
		{
			DebugDraw("CheckNextTutorial Miss!");
			StartTutorial((EventID)currentEventID);
			result = true;
		}
		return result;
	}

	private void SetEventState(EventState state)
	{
		m_eventState |= (uint)state;
	}

	private bool IsEventState(EventState state)
	{
		return ((int)m_eventState & (int)state) != 0;
	}

	private bool IsPlayStart()
	{
		return m_mode == Mode.PlayStart;
	}

	private Vector3 GetNextStartCollisionPosition()
	{
		Vector3 vector = Vector3.zero;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Gimmick");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!gameObject.GetComponent<ObjTutorialStartCollision>())
			{
				continue;
			}
			float num = m_startPos.x + 1f;
			Vector3 position = gameObject.transform.position;
			if (!(num < position.x))
			{
				continue;
			}
			if (vector == Vector3.zero)
			{
				vector = gameObject.transform.position;
				continue;
			}
			float x = vector.x;
			Vector3 position2 = gameObject.transform.position;
			if (x > position2.x)
			{
				vector = gameObject.transform.position;
			}
		}
		if (vector == Vector3.zero)
		{
			vector = m_startPos;
		}
		return vector;
	}

	private void GetTempScore()
	{
		if (m_tempScore != null)
		{
			MsgTutorialGetRingNum msgTutorialGetRingNum = new MsgTutorialGetRingNum();
			GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgTutorialGetRingNum", msgTutorialGetRingNum, SendMessageOptions.DontRequireReceiver);
			m_tempScore.m_ring = msgTutorialGetRingNum.m_ring;
			StageScoreManager stageScoreManager = StageScoreManager.Instance;
			if (stageScoreManager != null)
			{
				m_tempScore.m_stkRing = (int)stageScoreManager.Ring;
				m_tempScore.m_score = (int)stageScoreManager.Score;
				m_tempScore.m_animal = (int)stageScoreManager.Animal;
			}
			PlayerInformation playerInformation = ObjUtil.GetPlayerInformation();
			if (playerInformation != null)
			{
				m_tempScore.m_distance = playerInformation.TotalDistance;
			}
			DebugDraw("GetTempScore score=" + m_tempScore.m_score + " ring=" + m_tempScore.m_ring + " animal=" + m_tempScore.m_animal + "distance=" + m_tempScore.m_distance);
		}
	}

	public void OnMsgTutorialResetForRetry(MsgTutorialResetForRetry msg)
	{
		if (m_tempScore != null)
		{
			MsgTutorialResetForRetry value = new MsgTutorialResetForRetry(m_tempScore.m_ring, msg.m_blink);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgTutorialResetForRetry", value, SendMessageOptions.DontRequireReceiver);
			MsgResetScore msg2 = new MsgResetScore(m_tempScore.m_score, m_tempScore.m_animal, m_tempScore.m_stkRing);
			if (StageScoreManager.Instance != null)
			{
				StageScoreManager.Instance.ResetScore(msg2);
			}
			PlayerInformation playerInformation = ObjUtil.GetPlayerInformation();
			if (playerInformation != null)
			{
				playerInformation.TotalDistance = m_tempScore.m_distance - RetryOffsetPos - 1.05f;
			}
			DebugDraw("SetTempScore score=" + m_tempScore.m_score + " ring=" + m_tempScore.m_ring + " animal=" + m_tempScore.m_animal + "distance=" + m_tempScore.m_distance);
		}
	}

	private void DebugDraw(string msg)
	{
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
