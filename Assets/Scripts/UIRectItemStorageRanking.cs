using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Item/UI Rect Item Storage")]
public class UIRectItemStorageRanking : MonoBehaviour
{
	public enum ActiveType
	{
		ACTIVE,
		NOT_ACTTIVE,
		DEFAULT
	}

	public delegate void CallbackCreated(ui_ranking_scroll_dummy obj, UIRectItemStorageRanking storage);

	public delegate bool CallbackTopOrLast(bool isTop);

	public const float NEXT_BAR_SIZE = 0.8f;

	public const int NEXT_LOAD_LINE = 25;

	public const int MAX_ORG_ITEM_LIMIT = 80;

	public const float ADD_CREATE_DELAY = 0.02f;

	public RankingUtil.RankingRankerType rankingType = RankingUtil.RankingRankerType.RIVAL;

	public CallbackCreated callback;

	public CallbackTopOrLast callbackTopOrLast;

	public UIDraggablePanel parentPanel;

	private float drawAreaScale = 5f;

	private Vector3 drawArea = default(Vector3);

	private int m_drawLastIndex;

	private int m_drawStartIndex;

	[SerializeField]
	private bool isDirectionVertical;

	public int maxItemCount = 8;

	public GameObject template;

	public GameObject templateDummy;

	public int spacing = 128;

	public int padding = 10;

	private float m_currentBarSize;

	private List<UIInvGameItem> mItems = new List<UIInvGameItem>();

	private List<PlaceObjectData> m_placeObjects;

	private float m_placeDelay;

	private float m_placeCurrentTime;

	private float m_createLastTime;

	private float m_addItemLastTime;

	private float m_drawAllLastPoint;

	public ActiveType m_activeType = ActiveType.DEFAULT;

	private bool m_initCountainer;

	private List<ui_ranking_scroll_dummy> m_childs = new List<ui_ranking_scroll_dummy>();

	private List<ui_ranking_scroll> m_childsOrg = new List<ui_ranking_scroll>();

	public List<UIInvGameItem> items
	{
		get
		{
			while (mItems.Count < maxItemCount)
			{
				mItems.Add(null);
			}
			return mItems;
		}
	}

	public int childCount
	{
		get
		{
			return m_childs.Count;
		}
	}

	public UIInvGameItem GetItem(int slot)
	{
		return (slot >= items.Count) ? null : mItems[slot];
	}

	public UIInvGameItem Replace(int slot, UIInvGameItem item)
	{
		if (slot < maxItemCount)
		{
			UIInvGameItem result = items[slot];
			mItems[slot] = item;
			return result;
		}
		return item;
	}

	public bool IsCreating(float line = 0f)
	{
		if (m_placeObjects != null && (float)m_placeObjects.Count > line)
		{
			return true;
		}
		if (m_createLastTime != 0f && Time.realtimeSinceStartup > m_createLastTime + 0.1f)
		{
			m_createLastTime = 0f;
			return true;
		}
		return false;
	}

	public void CheckCreate()
	{
		m_createLastTime = Time.deltaTime;
	}

	public bool CheckBothEnd(out bool isNext)
	{
		bool result = false;
		isNext = false;
		if (parentPanel != null)
		{
			if (isDirectionVertical)
			{
				if (parentPanel.verticalScrollBar != null)
				{
					isNext = ((double)parentPanel.verticalScrollBar.value >= 1.0);
					if (parentPanel.verticalScrollBar.value <= 0.01f || parentPanel.verticalScrollBar.value >= 0.99f)
					{
						result = true;
					}
				}
			}
			else if (parentPanel.horizontalScrollBar != null)
			{
				isNext = ((double)parentPanel.horizontalScrollBar.value >= 1.0);
				if (parentPanel.horizontalScrollBar.value <= 0.01f || parentPanel.horizontalScrollBar.value >= 0.99f)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void Start()
	{
		if (!m_initCountainer)
		{
			InitContainer();
		}
		if (parentPanel != null)
		{
			Vector4 clipRange = parentPanel.panel.clipRange;
			drawArea = new Vector3(clipRange.z * drawAreaScale, clipRange.w * drawAreaScale, 0f);
		}
	}

	private void Update()
	{
		if (m_placeObjects != null && m_placeObjects.Count > 0)
		{
			if (m_placeCurrentTime <= 0f)
			{
				PlaceObjectData placeObjectData = m_placeObjects[0];
				Place(placeObjectData.x, placeObjectData.y, placeObjectData.count, placeObjectData.bound);
				m_placeObjects.RemoveAt(0);
				m_placeCurrentTime = m_placeDelay;
			}
			else
			{
				m_placeCurrentTime -= Time.deltaTime;
			}
		}
		if (!(parentPanel != null))
		{
			return;
		}
		if (CheckItemUpdate())
		{
			if (isDirectionVertical)
			{
				if (!(parentPanel.verticalScrollBar != null))
				{
					return;
				}
				if ((parentPanel.verticalScrollBar.value < 1f && parentPanel.verticalScrollBar.value > 0f) || m_currentBarSize <= 0f)
				{
					m_currentBarSize = parentPanel.verticalScrollBar.barSize;
				}
				else
				{
					if (!(m_currentBarSize > 0f))
					{
						return;
					}
					float barSize = parentPanel.verticalScrollBar.barSize;
					if (!(barSize / m_currentBarSize <= 0.8f) || m_childs == null)
					{
						return;
					}
					bool flag = true;
					bool flag2 = (double)parentPanel.verticalScrollBar.value >= 1.0;
					if (flag2 && callbackTopOrLast != null)
					{
						int count = m_childs.Count;
						if (m_drawLastIndex + 25 >= count && (m_placeObjects == null || m_placeObjects.Count <= 0) && (m_addItemLastTime == 0f || Mathf.Abs(m_addItemLastTime - Time.realtimeSinceStartup) >= 0.199999988f))
						{
							flag = !callbackTopOrLast(!flag2);
						}
					}
					if (flag)
					{
						CheckItemDrawAll(flag2);
					}
				}
			}
			else
			{
				if (!(parentPanel.horizontalScrollBar != null))
				{
					return;
				}
				if (parentPanel.horizontalScrollBar.value < 1f && parentPanel.horizontalScrollBar.value > 0f)
				{
					m_currentBarSize = parentPanel.horizontalScrollBar.barSize;
				}
				else if (m_currentBarSize > 0f)
				{
					float barSize2 = parentPanel.horizontalScrollBar.barSize;
					if (barSize2 / m_currentBarSize <= 0.8f)
					{
						CheckItemDrawAll(parentPanel.verticalScrollBar.value <= 0f);
						m_currentBarSize = 0f;
					}
				}
			}
		}
		else if (isDirectionVertical)
		{
			if (parentPanel.verticalScrollBar.value < 1f && parentPanel.verticalScrollBar.value > 0f)
			{
				m_currentBarSize = parentPanel.verticalScrollBar.barSize;
			}
		}
		else if (parentPanel.horizontalScrollBar.value < 1f && parentPanel.horizontalScrollBar.value > 0f)
		{
			m_currentBarSize = parentPanel.horizontalScrollBar.barSize;
		}
	}

	private float GetPosX(int param)
	{
		return (float)padding + ((float)param + 0.5f) * (float)spacing;
	}

	private float GetPosY(int param)
	{
		return (float)(-padding) - ((float)param + 0.5f) * (float)spacing;
	}

	private void PlaceAdd(int x, int y, int count, Bounds b)
	{
		m_placeDelay = 0f;
		Place(x, y, count, b);
	}

	private void Place(int x, int y, int count, Bounds b)
	{
		if (m_childsOrg != null && m_childsOrg.Count < 80)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, template);
			if (gameObject != null)
			{
				ui_ranking_scroll component = gameObject.GetComponent<ui_ranking_scroll>();
				gameObject.SetActive(false);
				if (component != null)
				{
					m_childsOrg.Add(component);
				}
			}
		}
		GameObject gameObject2 = NGUITools.AddChild(base.gameObject, templateDummy);
		if (gameObject2 != null)
		{
			ui_ranking_scroll_dummy component2 = gameObject2.GetComponent<ui_ranking_scroll_dummy>();
			if (component2 != null)
			{
				m_childs.Add(component2);
				Transform transform = component2.transform;
				if (isDirectionVertical)
				{
					transform.localPosition = new Vector3(0f, GetPosY(y), 0f);
				}
				else
				{
					transform.localPosition = new Vector3(GetPosX(x), 0f, 0f);
				}
				component2.storage = this;
				component2.end = false;
				component2.slot = count;
				if (callback != null)
				{
					callback(component2, this);
				}
			}
		}
		b.Encapsulate(new Vector3((float)padding * 2f + (float)((x + 1) * spacing), (float)(-padding) * 2f - (float)((y + 1) * spacing), 0f));
	}

	public GameObject GetFreeObject()
	{
		GameObject result = null;
		if (m_childsOrg != null && m_childsOrg.Count > 0)
		{
			foreach (ui_ranking_scroll item in m_childsOrg)
			{
				if (!item.gameObject.activeSelf)
				{
					return item.gameObject;
				}
			}
			return result;
		}
		return result;
	}

	public int GetFreeObjectNum()
	{
		int num = 0;
		if (m_childsOrg != null && m_childsOrg.Count > 0)
		{
			foreach (ui_ranking_scroll item in m_childsOrg)
			{
				if (!item.gameObject.activeSelf)
				{
					num++;
				}
			}
		}
		num -= 5;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public void Reset()
	{
		m_drawAllLastPoint = 0f;
		m_addItemLastTime = 0f;
		m_createLastTime = 0f;
		m_placeObjects = new List<PlaceObjectData>();
		m_drawLastIndex = 0;
		m_drawStartIndex = 0;
		if (parentPanel != null && parentPanel.gameObject.activeSelf && parentPanel.panel != null)
		{
			UIPanel panel = parentPanel.panel;
			Vector4 clipRange = parentPanel.panel.clipRange;
			float z = clipRange.z;
			Vector4 clipRange2 = parentPanel.panel.clipRange;
			panel.clipRange = new Vector4(0f, 0f, z, clipRange2.w);
			Transform transform = parentPanel.transform;
			Vector3 localPosition = parentPanel.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = parentPanel.transform.localPosition;
			transform.localPosition = new Vector3(x, 0f, localPosition2.z);
			Vector4 clipRange3 = parentPanel.panel.clipRange;
			drawArea = new Vector3(clipRange3.z * drawAreaScale, clipRange3.w * drawAreaScale, 0f);
		}
		maxItemCount = 0;
		if (m_childs != null && m_childs.Count > 0)
		{
			foreach (ui_ranking_scroll_dummy child in m_childs)
			{
				if (child != null)
				{
					child.DrawClear();
					child.transform.parent = null;
					child.gameObject.SetActive(false);
					Object.Destroy(child.gameObject);
				}
			}
			Resources.UnloadUnusedAssets();
			m_childs = new List<ui_ranking_scroll_dummy>();
		}
		if (m_childsOrg == null || m_childsOrg.Count <= 0)
		{
			return;
		}
		foreach (ui_ranking_scroll item in m_childsOrg)
		{
			if (item != null)
			{
				item.transform.parent = null;
				item.gameObject.SetActive(false);
				Object.Destroy(item.gameObject);
			}
		}
		m_childsOrg = new List<ui_ranking_scroll>();
	}

	public void Restart()
	{
		InitContainer();
	}

	public GameObject GetMyDataGameObject()
	{
		GameObject result = null;
		if (m_childs != null && m_childs.Count > 0)
		{
			foreach (ui_ranking_scroll_dummy child in m_childs)
			{
				if (child != null && child.isMyData)
				{
					return child.gameObject;
				}
			}
			return result;
		}
		return result;
	}

	public bool AddItem(int addItem, float delay = 0.02f)
	{
		m_drawAllLastPoint = 0f;
		m_addItemLastTime = Time.realtimeSinceStartup;
		CheckCreate();
		m_placeObjects = new List<PlaceObjectData>();
		if (template != null && templateDummy != null && addItem > 0)
		{
			m_placeDelay = delay;
			m_placeCurrentTime = 0f;
			if (m_childs.Count < maxItemCount)
			{
				return false;
			}
			maxItemCount += addItem;
			int num = 0;
			int num2 = 0;
			Bounds b = default(Bounds);
			if (callback != null)
			{
				for (int i = 0; i < m_childs.Count; i++)
				{
					ui_ranking_scroll_dummy ui_ranking_scroll_dummy = m_childs[i];
					if (ui_ranking_scroll_dummy != null)
					{
						callback(ui_ranking_scroll_dummy, this);
					}
				}
			}
			int count = m_childs.Count;
			if (isDirectionVertical)
			{
				for (int j = count; j < maxItemCount; j++)
				{
					if (delay > 0f)
					{
						PlaceAdd(0, j, num + count, b);
					}
					else
					{
						Place(0, j, num + count, b);
					}
					num2++;
					num++;
				}
			}
			else
			{
				for (int k = count; k < maxItemCount; k++)
				{
					if (delay > 0f)
					{
						PlaceAdd(0, k, num + count, b);
					}
					else
					{
						Place(k, 0, num + count, b);
					}
					num2++;
					num++;
				}
			}
			m_initCountainer = true;
			return true;
		}
		if (template != null && templateDummy != null && callback != null)
		{
			for (int l = 0; l < m_childs.Count; l++)
			{
				ui_ranking_scroll_dummy ui_ranking_scroll_dummy2 = m_childs[l];
				if (ui_ranking_scroll_dummy2 != null)
				{
					callback(ui_ranking_scroll_dummy2, this);
				}
			}
			return true;
		}
		return false;
	}

	private void InitContainer()
	{
		m_drawAllLastPoint = 0f;
		m_drawLastIndex = 0;
		m_drawStartIndex = 0;
		if (parentPanel != null)
		{
			UIPanel panel = parentPanel.panel;
			Vector4 clipRange = parentPanel.panel.clipRange;
			float z = clipRange.z;
			Vector4 clipRange2 = parentPanel.panel.clipRange;
			panel.clipRange = new Vector4(0f, 0f, z, clipRange2.w);
			Transform transform = parentPanel.transform;
			Vector3 localPosition = parentPanel.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = parentPanel.transform.localPosition;
			transform.localPosition = new Vector3(x, 0f, localPosition2.z);
			Vector4 clipRange3 = parentPanel.panel.clipRange;
			drawArea = new Vector3(clipRange3.z * drawAreaScale, clipRange3.w * drawAreaScale, 0f);
		}
		if (!(template != null) || !(templateDummy != null) || maxItemCount <= 0)
		{
			return;
		}
		int num = 0;
		Bounds b = default(Bounds);
		if (isDirectionVertical)
		{
			for (int i = 0; i < maxItemCount; i++)
			{
				Place(0, i, num, b);
				num++;
			}
		}
		else
		{
			for (int j = 0; j < maxItemCount; j++)
			{
				Place(j, 0, num, b);
				num++;
			}
		}
		m_initCountainer = true;
	}

	private bool CheckItemUpdate()
	{
		if (m_childs == null)
		{
			return true;
		}
		bool result = true;
		int num = m_drawLastIndex - 320;
		if (num < 0)
		{
			num = 0;
		}
		for (int i = num; i < m_childs.Count; i++)
		{
			ui_ranking_scroll_dummy ui_ranking_scroll_dummy = m_childs[i];
			if (ui_ranking_scroll_dummy != null && ui_ranking_scroll_dummy.gameObject.activeSelf && ui_ranking_scroll_dummy.enabled)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public void CheckItemDrawAllAuto(bool isNext, int num = 20)
	{
		if (IsCreating(0f))
		{
			return;
		}
		if (GetFreeObjectNum() < 15)
		{
			CheckItemDrawAll(isNext);
			return;
		}
		Debug.Log(" + CheckItemDrawAllAuto  isNext:" + isNext);
		int num2 = 0;
		int num3 = m_drawStartIndex;
		int num4 = m_drawLastIndex;
		if (isNext)
		{
			num4 += num;
		}
		else
		{
			num3 -= num;
		}
		if (num4 > m_childs.Count)
		{
			num4 = m_childs.Count;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		m_drawStartIndex = m_drawLastIndex;
		m_drawLastIndex = num3;
		if (!isNext)
		{
			m_drawStartIndex = m_drawLastIndex;
			m_drawLastIndex = 0;
		}
		for (int i = num3; i < num4; i++)
		{
			int num5 = i;
			ui_ranking_scroll_dummy ui_ranking_scroll_dummy = m_childs[num5];
			if (!(ui_ranking_scroll_dummy != null))
			{
				continue;
			}
			bool flag = false;
			flag = CheckItemDraw(ui_ranking_scroll_dummy.gameObject);
			if (flag && !ui_ranking_scroll_dummy.gameObject.activeSelf)
			{
				num2++;
				if (num2 == 1 && num5 > 0)
				{
					ui_ranking_scroll_dummy.top = true;
				}
				else
				{
					ui_ranking_scroll_dummy.top = false;
				}
				ui_ranking_scroll_dummy.SetActiveObject(flag, 0.05f * (float)num2);
			}
			else if (!flag && ui_ranking_scroll_dummy.gameObject.activeSelf)
			{
				ui_ranking_scroll_dummy.SetActiveObject(flag, 0f);
			}
			if (flag)
			{
				if (m_drawLastIndex < num5)
				{
					m_drawLastIndex = num5;
				}
				if (m_drawStartIndex > num5)
				{
					m_drawStartIndex = num5;
				}
			}
		}
		if (parentPanel != null && parentPanel.panel != null)
		{
			if (isDirectionVertical)
			{
				Vector4 clipRange = parentPanel.panel.clipRange;
				m_drawAllLastPoint = clipRange.y;
			}
			else
			{
				Vector4 clipRange2 = parentPanel.panel.clipRange;
				m_drawAllLastPoint = clipRange2.x;
			}
		}
	}

	public void CheckItemDrawAll(bool isNext)
	{
		if (IsCreating(0f))
		{
			return;
		}
		int num = m_drawStartIndex - 80;
		if (!isNext)
		{
			num = m_drawStartIndex + 80;
			if (num >= m_childs.Count)
			{
				num = m_childs.Count - 1;
			}
		}
		if (num < 0)
		{
			num = 0;
		}
		int num2 = 0;
		int num3 = m_childs.Count - num;
		if (!isNext)
		{
			num3 = num + 1;
			if (num3 > 240)
			{
				num3 = 240;
			}
		}
		else if (num3 > 240)
		{
			num3 = 240;
		}
		m_drawStartIndex = m_drawLastIndex;
		m_drawLastIndex = num;
		if (!isNext)
		{
			m_drawStartIndex = num;
			m_drawLastIndex = 0;
		}
		for (int i = 0; i < num3; i++)
		{
			int num4 = i + num;
			if (!isNext)
			{
				num4 = num - i;
			}
			ui_ranking_scroll_dummy ui_ranking_scroll_dummy = null;
			if (num4 < 0 || num4 >= m_childs.Count)
			{
				break;
			}
			ui_ranking_scroll_dummy = m_childs[num4];
			if (!(ui_ranking_scroll_dummy != null))
			{
				continue;
			}
			bool flag = CheckItemDraw(ui_ranking_scroll_dummy.gameObject);
			if (flag && !ui_ranking_scroll_dummy.gameObject.activeSelf)
			{
				num2++;
			}
			if (num2 == 1 && num4 > 0)
			{
				ui_ranking_scroll_dummy.top = true;
			}
			else
			{
				ui_ranking_scroll_dummy.top = false;
			}
			ui_ranking_scroll_dummy.SetActiveObject(flag, 0.05f * (float)num2);
			if (flag)
			{
				if (m_drawLastIndex < num4)
				{
					m_drawLastIndex = num4;
				}
				if (m_drawStartIndex > num4)
				{
					m_drawStartIndex = num4;
				}
			}
		}
		if (parentPanel != null && parentPanel.panel != null)
		{
			if (isDirectionVertical)
			{
				Vector4 clipRange = parentPanel.panel.clipRange;
				m_drawAllLastPoint = clipRange.y;
			}
			else
			{
				Vector4 clipRange2 = parentPanel.panel.clipRange;
				m_drawAllLastPoint = clipRange2.x;
			}
		}
	}

	private bool CheckMove(out bool isNext, float move = 400f)
	{
		bool flag = false;
		isNext = false;
		if (parentPanel != null && parentPanel.panel != null)
		{
			if (isDirectionVertical)
			{
				Vector4 clipRange = parentPanel.panel.clipRange;
				if (Mathf.Abs(clipRange.y - m_drawAllLastPoint) >= move)
				{
					Vector4 clipRange2 = parentPanel.panel.clipRange;
					isNext = (clipRange2.y - m_drawAllLastPoint < 0f);
					flag = true;
				}
			}
			else
			{
				Vector4 clipRange3 = parentPanel.panel.clipRange;
				if (Mathf.Abs(clipRange3.x - m_drawAllLastPoint) >= move)
				{
					Vector4 clipRange4 = parentPanel.panel.clipRange;
					isNext = (clipRange4.x - m_drawAllLastPoint < 0f);
					flag = true;
				}
			}
			if (flag && m_addItemLastTime != 0f && Mathf.Abs(m_addItemLastTime - Time.realtimeSinceStartup) <= 2f)
			{
				flag = false;
			}
		}
		return flag;
	}

	public bool CheckItemDraw(int slot)
	{
		bool flag = false;
		if (rankingType == RankingUtil.RankingRankerType.RIVAL)
		{
			return true;
		}
		if (parentPanel != null)
		{
			Vector3 localPosition = parentPanel.gameObject.transform.localPosition;
			float f = localPosition.x + GetPosX(slot);
			float f2 = localPosition.y + GetPosY(slot);
			if (isDirectionVertical)
			{
				if (Mathf.Abs(f2) < drawArea.y + (float)(spacing * 2))
				{
					flag = true;
				}
			}
			else if (Mathf.Abs(f) < drawArea.x + (float)(spacing * 2))
			{
				flag = true;
			}
		}
		if (m_drawLastIndex < slot && flag)
		{
			m_drawLastIndex = slot;
		}
		return flag;
	}

	public bool CheckItemDraw(GameObject go)
	{
		bool result = false;
		if (rankingType == RankingUtil.RankingRankerType.RIVAL)
		{
			return true;
		}
		if (parentPanel != null)
		{
			Vector3 localPosition = parentPanel.gameObject.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = go.transform.localPosition;
			float f = x + localPosition2.x;
			float y = localPosition.y;
			Vector3 localPosition3 = go.transform.localPosition;
			float f2 = y + localPosition3.y;
			if (isDirectionVertical)
			{
				if (Mathf.Abs(f2) < drawArea.y + (float)(spacing * 2))
				{
					result = true;
				}
			}
			else if (Mathf.Abs(f) < drawArea.x + (float)(spacing * 2))
			{
				result = true;
			}
		}
		return result;
	}

	public void Strip()
	{
		while (m_childs.Count > maxItemCount)
		{
			if (m_childs.Count > 0)
			{
				GameObject gameObject = m_childs[m_childs.Count - 1].gameObject;
				m_childs.RemoveAt(m_childs.Count - 1);
				gameObject.transform.parent = null;
				Object.Destroy(gameObject);
			}
		}
	}
}
