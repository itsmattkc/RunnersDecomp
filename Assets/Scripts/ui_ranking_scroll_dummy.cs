using UnityEngine;

public class ui_ranking_scroll_dummy : MonoBehaviour
{
	[SerializeField]
	private UIDragPanelContents dragPanelContent;

	[SerializeField]
	private UISprite dummySprite;

	[SerializeField]
	private UIButtonMessage button;

	[SerializeField]
	private UILabel label;

	private GameObject m_rankerObject;

	private RankingUtil.Ranker m_rankerData;

	public RankingUtil.Ranker myRankerData;

	public UIRectItemStorageRanking storage;

	public SpecialStageWindow spWindow;

	public RankingUI rankingUI;

	public RankingUtil.RankingScoreType scoreType;

	public RankingUtil.RankingRankerType rankerType;

	public bool end;

	public bool top;

	public int slot;

	private BoxCollider m_boxCollider;

	public RankingUtil.Ranker rankerData
	{
		get
		{
			return m_rankerData;
		}
		set
		{
			if (value != null && m_rankerData == null)
			{
				m_rankerData = value;
			}
			else if (value == null)
			{
				m_rankerData = null;
			}
		}
	}

	public bool isMyData
	{
		get
		{
			bool result = false;
			if (myRankerData != null && m_rankerData != null && m_rankerData.id == myRankerData.id)
			{
				result = true;
			}
			return result;
		}
	}

	public bool isMask
	{
		get
		{
			bool result = false;
			if (dummySprite != null)
			{
				result = dummySprite.gameObject.activeSelf;
			}
			return result;
		}
	}

	public void SetActiveObject(bool act, float delay = 0f)
	{
		if (dragPanelContent != null)
		{
			dragPanelContent.draggablePanel = storage.parentPanel;
		}
		if (m_boxCollider == null)
		{
			m_boxCollider = base.gameObject.GetComponentInChildren<BoxCollider>();
		}
		if (act)
		{
			if (m_rankerObject == null && storage != null)
			{
				m_rankerObject = storage.GetFreeObject();
				if (m_rankerObject != null)
				{
					UpdateRanker(delay);
				}
				else if (m_rankerData != null && spWindow != null)
				{
					RankingUtil.Ranker currentRanker = spWindow.GetCurrentRanker(slot);
					if (currentRanker != null && !m_rankerData.CheckRankerIdentity(currentRanker))
					{
						m_rankerData = currentRanker;
						UpdateRanker(delay);
					}
				}
				else if (m_rankerData != null && rankingUI != null)
				{
					RankingUtil.Ranker currentRanker2 = rankingUI.GetCurrentRanker(slot);
					if (currentRanker2 != null && !m_rankerData.CheckRankerIdentity(currentRanker2))
					{
						m_rankerData = currentRanker2;
						UpdateRanker(delay);
					}
				}
			}
		}
		else
		{
			if (button != null)
			{
				button.target = null;
			}
			if (label != null)
			{
				label.text = string.Empty;
			}
			SetMask(1f);
			if (m_rankerObject != null)
			{
				DrawClear();
				m_rankerObject.SetActive(false);
				m_rankerObject = null;
			}
			top = false;
		}
		if (m_boxCollider != null)
		{
			m_boxCollider.enabled = act;
		}
		base.gameObject.SetActive(act);
	}

	public void DrawClear()
	{
		if (m_rankerObject != null)
		{
			ui_ranking_scroll componentInChildren = m_rankerObject.GetComponentInChildren<ui_ranking_scroll>();
			if (componentInChildren != null)
			{
				componentInChildren.DrawClear();
			}
		}
	}

	public void SetMask(float alpha = 0f)
	{
		if (dummySprite != null)
		{
			if (alpha <= 0f)
			{
				dummySprite.gameObject.SetActive(false);
				dummySprite.alpha = 0f;
				base.enabled = false;
			}
			else
			{
				dummySprite.gameObject.SetActive(true);
				dummySprite.alpha = alpha;
				base.enabled = true;
			}
		}
	}

	public bool IsCreating(float line = 0f)
	{
		if (storage != null && storage.IsCreating(line))
		{
			return true;
		}
		return false;
	}

	public void CheckCreate()
	{
		if (storage != null)
		{
			storage.CheckCreate();
		}
	}

	private void UpdateRanker(float delay)
	{
		if (!(m_rankerObject != null))
		{
			return;
		}
		m_rankerObject.transform.localPosition = base.transform.localPosition;
		if (m_rankerData == null)
		{
			return;
		}
		m_rankerObject.SetActive(true);
		ui_ranking_scroll componentInChildren = m_rankerObject.GetComponentInChildren<ui_ranking_scroll>();
		if (componentInChildren != null)
		{
			bool myCell = false;
			if (myRankerData != null && m_rankerData.id == myRankerData.id)
			{
				myCell = true;
			}
			componentInChildren.UpdateViewAsync(scoreType, rankerType, m_rankerData, end, myCell, delay, this);
			if (button != null)
			{
				button.target = null;
			}
			if (label != null)
			{
				label.text = string.Empty;
			}
			end = false;
		}
		UIRectItemStorageSlot component = m_rankerObject.GetComponent<UIRectItemStorageSlot>();
		if (component != null)
		{
			component.storage = null;
			component.storageRanking = storage;
			component.slot = m_rankerData.rankIndex;
		}
	}

	private void Update()
	{
		if (dummySprite != null)
		{
			if ((double)dummySprite.alpha < 1.0)
			{
				SetMask(dummySprite.alpha - Time.deltaTime * 10f);
			}
		}
		else
		{
			base.enabled = false;
		}
	}
}
