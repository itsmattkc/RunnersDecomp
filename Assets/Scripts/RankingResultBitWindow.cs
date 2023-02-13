using AnimationOrTween;
using System.Collections.Generic;
using UnityEngine;

public class RankingResultBitWindow : MonoBehaviour
{
	private const float RANK_CHANGE_EFFECT_START_TIME = 2f;

	private const float RANK_CHANGE_EFFECT_MOVE_TIME = 0.9f;

	private const int RANKER_DATA_SIZE_H = 94;

	private const int RANKER_LIST_INIT_X = 0;

	private const int RANKER_LIST_INIT_Y = 153;

	private const int RANKER_LIST_ADD_Y = -94;

	[SerializeField]
	private GameObject m_orgRankingbit;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIPanel m_panel;

	[SerializeField]
	private UIDraggablePanel m_draggable;

	[SerializeField]
	private GameObject m_frontCollider;

	private List<ui_rankingbit_scroll> m_rankerList;

	private RankingUtil.RankingMode m_rankingMode;

	private RankingUtil.RankChange m_change;

	private int m_currentRank = -1;

	private int m_oldRank = -1;

	private float m_openTime;

	private float m_autoMoveSpeed;

	private float m_autoMoveTime;

	private bool m_isMove;

	private bool m_isEnd;

	private Vector3 m_autoMoveTargetPos;

	private static RankingResultBitWindow s_instance;

	public UIDraggablePanel draggable
	{
		get
		{
			return m_draggable;
		}
	}

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public static RankingResultBitWindow Instance
	{
		get
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				RankingResultBitWindow rankingResultBitWindow = GameObjectUtil.FindChildGameObjectComponent<RankingResultBitWindow>(gameObject, "RankingResultBitWindow");
				if (rankingResultBitWindow != null)
				{
					rankingResultBitWindow.gameObject.SetActive(true);
				}
			}
			return s_instance;
		}
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		m_openTime += Time.deltaTime;
		if (m_openTime >= 2f && !m_isMove && m_rankerList != null && m_rankerList.Count > 0)
		{
			foreach (ui_rankingbit_scroll ranker in m_rankerList)
			{
				ranker.MoveStart(0.9f + (float)Mathf.Abs(m_oldRank - m_currentRank) * 0.03f);
			}
			m_isMove = true;
		}
		if (m_autoMoveTime > 0f)
		{
			m_autoMoveTime -= Time.deltaTime;
			if (m_rankerList != null)
			{
				AutoMove();
			}
		}
	}

	public void AutoMove()
	{
		if (m_autoMoveSpeed > 0f && m_draggable != null && m_autoMoveTime > 0f)
		{
			Vector3 localPosition = m_draggable.panel.transform.localPosition;
			float num = localPosition.y * -1f;
			Vector4 clipRange = m_draggable.panel.clipRange;
			float num2 = num + clipRange.w * 0.5f + -47f;
			Vector3 localPosition2 = m_draggable.panel.transform.localPosition;
			float num3 = localPosition2.y * -1f;
			Vector4 clipRange2 = m_draggable.panel.clipRange;
			float num4 = num3 + clipRange2.w * -0.5f + 47f;
			float num5 = 0f;
			if (m_autoMoveTargetPos.y < num4)
			{
				num5 = m_autoMoveTargetPos.y - num4;
				if (num5 <= m_autoMoveSpeed * -1f)
				{
					num5 = m_autoMoveSpeed * -1f;
					m_autoMoveSpeed = 0f;
				}
			}
			else if (m_autoMoveTargetPos.y > num2)
			{
				num5 = m_autoMoveTargetPos.y - num2;
				if (num5 >= m_autoMoveSpeed)
				{
					num5 = m_autoMoveSpeed;
					m_autoMoveSpeed = 0f;
				}
			}
			Vector3 localPosition3 = m_draggable.panel.transform.localPosition;
			float num6 = localPosition3.y * -1f + num5;
			if (num6 >= -0.75f)
			{
				num6 = -0.75f;
			}
			else if (num6 <= -0.75f + (float)(m_rankerList.Count * -94))
			{
				num6 = -0.75f + (float)(m_rankerList.Count * -94);
			}
			UIPanel panel = m_draggable.panel;
			float y = num6;
			Vector4 clipRange3 = m_draggable.panel.clipRange;
			float z = clipRange3.z;
			Vector4 clipRange4 = m_draggable.panel.clipRange;
			panel.clipRange = new Vector4(0f, y, z, clipRange4.w);
			Transform transform = m_draggable.panel.transform;
			Vector4 clipRange5 = m_draggable.panel.clipRange;
			float y2 = clipRange5.y * -1f;
			Vector3 localPosition4 = m_draggable.panel.transform.localPosition;
			transform.localPosition = new Vector3(0f, y2, localPosition4.z);
		}
		else
		{
			m_autoMoveSpeed = 0f;
			m_autoMoveTime = 0f;
		}
	}

	public void AutoMoveScrollEnd()
	{
		SoundManager.SePlay("sys_rank_kettei");
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(false);
		}
	}

	public void AutoMoveScroll(Vector3 position, bool up)
	{
		if (up)
		{
			m_autoMoveTargetPos = new Vector3(position.x, position.y + 188f, 0f);
		}
		else
		{
			m_autoMoveTargetPos = new Vector3(position.x, position.y, 0f);
		}
		m_autoMoveSpeed = 10000f;
		m_autoMoveTime = 0.2f;
		if (m_rankerList != null)
		{
			AutoMove();
		}
	}

	private void OnClickNoButton()
	{
		SingletonGameObject<RankingManager>.Instance.ResetRankingRankChange(m_rankingMode);
		SoundManager.SePlay("sys_window_close");
	}

	public void Close()
	{
		ResetRankerList();
		m_openTime = 0f;
		m_isMove = false;
		m_isEnd = true;
		if (m_panel != null)
		{
			m_panel.alpha = 0f;
		}
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(false);
		}
		RemoveBackKeyCallBack();
		base.gameObject.SetActive(false);
	}

	public void Init()
	{
		if (m_panel != null)
		{
			m_panel.alpha = 0f;
		}
		m_openTime = 0f;
		m_autoMoveSpeed = 0f;
		m_isMove = false;
		m_isEnd = false;
	}

	public bool Open(RankingUtil.RankingMode rankingMode)
	{
		m_rankingMode = rankingMode;
		if (m_panel != null)
		{
			m_panel.alpha = 0f;
		}
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(true);
		}
		SoundManager.SePlay("sys_window_open");
		m_autoMoveSpeed = 0f;
		base.gameObject.SetActive(true);
		RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
		RankingUtil.RankingScoreType endlessRivalRankingScoreType = RankingManager.EndlessRivalRankingScoreType;
		m_change = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(m_rankingMode, endlessRivalRankingScoreType, rankerType, out m_currentRank, out m_oldRank);
		m_isEnd = false;
		if (m_change != 0)
		{
			SingletonGameObject<RankingManager>.Instance.GetRanking(m_rankingMode, endlessRivalRankingScoreType, rankerType, 0, CallbackRanking);
			return true;
		}
		base.gameObject.SetActive(false);
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(false);
		}
		return false;
	}

	private ui_rankingbit_scroll CreateRankerData(int index, bool mydata, RankingUtil.Ranker ranker, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type)
	{
		m_openTime = 0f;
		m_isMove = false;
		GameObject gameObject = Object.Instantiate(m_orgRankingbit) as GameObject;
		gameObject.transform.parent = m_draggable.transform;
		int rankIndex = ranker.rankIndex;
		Vector3 vector = new Vector3(0f, 153 + ranker.rankIndex * -94, 0f);
		Vector3 movePos = new Vector3(0f, 153 + ranker.rankIndex * -94, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		ranker.userDataType = RankingUtil.UserDataType.RANK_UP;
		ui_rankingbit_scroll componentInChildren = gameObject.GetComponentInChildren<ui_rankingbit_scroll>();
		if (componentInChildren != null)
		{
			if (mydata)
			{
				componentInChildren.UpdateView(basePos: new Vector3(0f, 153 + (rankIndex + (m_oldRank - m_currentRank)) * -94, 0f), parent: this, change: m_change, scoreType: score, rankerType: type, ranker: ranker, addRank: m_oldRank - m_currentRank, movePos: movePos);
			}
			else
			{
				if (ranker.isFriend)
				{
					ranker.isSentEnergy = true;
				}
				if (ranker.rankIndex + 1 > m_currentRank && ranker.rankIndex + 1 <= m_oldRank)
				{
					vector = new Vector3(0f, 153 + (rankIndex - 1) * -94, 0f);
					componentInChildren.UpdateView(this, RankingUtil.RankChange.DOWN, score, type, ranker, -1, movePos, vector);
				}
				else
				{
					componentInChildren.UpdateView(this, RankingUtil.RankChange.NONE, score, type, ranker, 0, vector, vector);
				}
			}
			if (m_rankerList == null)
			{
				m_rankerList = new List<ui_rankingbit_scroll>();
			}
			m_rankerList.Add(componentInChildren);
		}
		return componentInChildren;
	}

	private void CallbackRanking(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		if (rankerList != null && rankerList.Count > 1 && rankerList[0] != null && m_orgRankingbit != null)
		{
			if (m_frontCollider != null)
			{
				m_frontCollider.SetActive(true);
			}
			if (m_oldRank > rankerList.Count - 1)
			{
				m_oldRank = rankerList.Count - 1;
			}
			ui_rankingbit_scroll ui_rankingbit_scroll = null;
			for (int i = 1; i < rankerList.Count; i++)
			{
				if (rankerList[i].id == rankerList[0].id)
				{
					ui_rankingbit_scroll = CreateRankerData(i - 1, true, rankerList[i], score, type);
				}
				else
				{
					CreateRankerData(i - 1, false, rankerList[i], score, type);
				}
			}
			if (m_panel != null)
			{
				m_panel.alpha = 1f;
			}
			m_draggable.ResetPosition();
			if (ui_rankingbit_scroll != null)
			{
				AutoMoveScroll(ui_rankingbit_scroll.transform.localPosition, false);
			}
			if (m_animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
			}
		}
		else
		{
			m_change = RankingUtil.RankChange.NONE;
			if (m_panel != null)
			{
				m_panel.alpha = 0f;
			}
			base.gameObject.SetActive(false);
		}
	}

	private void ResetRankerList()
	{
		if (m_rankerList == null)
		{
			return;
		}
		if (m_rankerList.Count > 0)
		{
			foreach (ui_rankingbit_scroll ranker in m_rankerList)
			{
				ranker.Remove();
			}
			m_rankerList.Clear();
		}
		m_rankerList = null;
	}

	public void AddRanker(ui_rankingbit_scroll ranker)
	{
	}

	private void AnimationFinishCallback()
	{
	}

	private void Awake()
	{
		SetInstance();
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			RemoveBackKeyCallBack();
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			s_instance = this;
			EntryBackKeyCallBack();
			s_instance.Init();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
