using System.Collections.Generic;
using UnityEngine;

public class ui_rankingbit_scroll : MonoBehaviour
{
	[SerializeField]
	private GameObject m_orgRankingUser;

	[SerializeField]
	private List<GameObject> m_pattern;

	[SerializeField]
	private UIDragPanelContents m_dragContents;

	private ui_ranking_scroll m_child;

	private RankingResultBitWindow m_parent;

	private RankingUtil.Ranker m_ranker;

	private UILabel m_differencevalue;

	private RankingUtil.RankChange m_change;

	private int m_addRank;

	private int m_currentUpRank;

	private float m_frac;

	private bool m_isMydata;

	private float m_moveTime;

	private float m_moveTimeMax;

	private Vector3 m_movePosition;

	private Vector3 m_basePosition;

	private Vector3 m_vector;

	public bool isMydata
	{
		get
		{
			return m_isMydata;
		}
	}

	private void Update()
	{
		if (!(m_moveTimeMax > 0f) || !(m_moveTime > 0f))
		{
			return;
		}
		m_moveTime -= Time.deltaTime;
		if (m_moveTime <= 0f)
		{
			m_moveTime = 0f;
			m_moveTimeMax = 0f;
			base.transform.localPosition = m_movePosition;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			m_child.UpdateViewRank(-1);
			if (m_isMydata)
			{
				m_parent.AutoMoveScrollEnd();
			}
		}
		else
		{
			float num = 1f;
			if (m_moveTimeMax - m_moveTime < 0.2f)
			{
				num = (m_moveTimeMax - m_moveTime) / 0.2f * 2f;
				if (num > 1f)
				{
					num = 1f - (num - 1f) * 0.5f;
				}
				if (m_isMydata)
				{
					base.transform.localScale = new Vector3(1f + num * 0.2f, 1f + num * 0.2f, 1f);
				}
				else
				{
					base.transform.localScale = new Vector3(1f - num * 0.2f, 1f - num * 0.2f, 1f);
				}
			}
			else if (m_moveTime < 0.1f)
			{
				Vector3 localScale = base.transform.localScale;
				num = (localScale.x - 1f) * 0.2f;
				Transform transform = base.transform;
				Vector3 localScale2 = base.transform.localScale;
				float x = localScale2.x - num;
				Vector3 localScale3 = base.transform.localScale;
				transform.localScale = new Vector3(x, localScale3.y - num, 1f);
			}
			float num2 = m_moveTime / m_moveTimeMax;
			num2 = ((m_moveTimeMax - m_moveTime < 0.25f) ? 1f : ((!(m_moveTime < 0.25f)) ? ((m_moveTime - 0.25f) / (m_moveTimeMax - 0.5f)) : 0f));
			float num3 = 1f - num2 * num2;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			UpdateRank(num3);
			base.transform.localPosition = new Vector3(m_basePosition.x + m_vector.x * num3, m_basePosition.y + m_vector.y * num3, m_basePosition.z + m_vector.z * num3);
		}
		if (m_isMydata)
		{
			m_parent.AutoMoveScroll(base.transform.localPosition, true);
		}
	}

	public void MoveStart(float time)
	{
		if (m_vector.x != 0f || m_vector.y != 0f)
		{
			m_moveTime = time;
			m_moveTimeMax = time;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void UpdateRank(float rate)
	{
		if (m_addRank == 0 || !(m_frac > 0f))
		{
			return;
		}
		bool flag = false;
		int num = 0;
		for (float num2 = 0f; num2 < 1f; num2 += m_frac)
		{
			if (num2 >= rate)
			{
				flag = true;
				break;
			}
			num++;
		}
		if (!flag)
		{
			m_child.UpdateViewRank(-1);
			m_frac = 0f;
			return;
		}
		int num3 = 0;
		num3 = ((m_addRank <= 0) ? (m_addRank + num) : (m_addRank - num));
		if (m_differencevalue != null && m_currentUpRank != m_addRank - num3)
		{
			m_currentUpRank = m_addRank - num3;
			if (m_currentUpRank >= m_addRank)
			{
				m_currentUpRank = m_addRank;
			}
			m_differencevalue.text = "+" + m_currentUpRank;
		}
		m_child.UpdateViewRank(m_ranker.rankIndex + 1 + num3);
	}

	public void UpdateView(RankingResultBitWindow parent, RankingUtil.RankChange change, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, int addRank, Vector3 movePos, Vector3 basePos)
	{
		m_currentUpRank = 0;
		m_frac = 0f;
		m_moveTime = 0f;
		m_moveTimeMax = 0f;
		m_change = change;
		m_ranker = ranker;
		m_movePosition = movePos;
		m_basePosition = basePos;
		m_vector = m_movePosition - m_basePosition;
		base.transform.localPosition = m_basePosition;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		m_addRank = addRank;
		m_parent = parent;
		m_dragContents.draggablePanel = m_parent.draggable;
		SetPattern(m_change);
		SetUser(scoreType, rankerType, ranker);
		if (m_addRank != 0)
		{
			m_frac = 1f / (float)(Mathf.Abs(m_addRank) + 1);
		}
		if (m_child != null && m_change == RankingUtil.RankChange.UP)
		{
			m_isMydata = true;
			m_child.SetMyRanker(m_isMydata);
			if (m_differencevalue != null)
			{
				m_differencevalue.text = "+" + m_currentUpRank;
			}
			UISprite[] componentsInChildren = m_child.gameObject.GetComponentsInChildren<UISprite>();
			UILabel[] componentsInChildren2 = m_child.gameObject.GetComponentsInChildren<UILabel>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				UISprite[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].depth += 50;
				}
			}
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				UILabel[] array2 = componentsInChildren2;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].depth += 50;
				}
			}
		}
		if (m_addRank != 0 && m_child != null)
		{
			m_child.UpdateViewRank(ranker.rankIndex + 1 + m_addRank);
		}
	}

	private void SetUser(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker)
	{
		if (m_child == null && m_orgRankingUser != null)
		{
			GameObject gameObject = Object.Instantiate(m_orgRankingUser) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = default(Vector3);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			m_child = gameObject.GetComponentInChildren<ui_ranking_scroll>();
		}
		if (m_child != null)
		{
			m_child.UpdateView(scoreType, rankerType, ranker, false);
		}
	}

	private void SetPattern(RankingUtil.RankChange change)
	{
		if (m_pattern == null || m_pattern.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_pattern.Count; i++)
		{
			switch (change)
			{
			case RankingUtil.RankChange.UP:
				if (m_pattern[i].name.IndexOf("up") != -1)
				{
					m_pattern[i].SetActive(true);
					m_differencevalue = m_pattern[i].GetComponentInChildren<UILabel>();
				}
				else
				{
					m_pattern[i].SetActive(false);
				}
				break;
			case RankingUtil.RankChange.DOWN:
				if (m_pattern[i].name.IndexOf("down") != -1)
				{
					m_pattern[i].SetActive(true);
				}
				else
				{
					m_pattern[i].SetActive(false);
				}
				break;
			default:
				m_pattern[i].SetActive(false);
				break;
			}
		}
	}

	public void Remove()
	{
		if (m_child != null)
		{
			Object.Destroy(m_child.gameObject);
		}
		Object.Destroy(base.gameObject);
	}
}
