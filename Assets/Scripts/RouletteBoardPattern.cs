using System.Collections.Generic;
using UnityEngine;

public class RouletteBoardPattern : MonoBehaviour
{
	private const int CELL_MAX = 8;

	[SerializeField]
	[Header("各枠情報 x:開始角度 y:終了角度 z:針速度")]
	private List<Vector3> m_cells;

	private RouletteBoard m_parent;

	private List<GameObject> m_itemPosList;

	private GameObject m_activeEffBase;

	private GameObject m_rankEffBase;

	private GameObject m_cellEffBase;

	private List<UISprite> m_activeEffList;

	private List<UISprite> m_rankEffList;

	private List<UISprite> m_cellEffList;

	private List<RouletteItem> m_itemList;

	public void Setup(RouletteBoard parent, RouletteItem orgItem, int initCell = 0)
	{
		base.gameObject.SetActive(true);
		m_activeEffBase = null;
		m_rankEffBase = null;
		m_cellEffBase = null;
		m_parent = parent;
		GameObject gameObject = null;
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_bg_LT");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_bg_RT");
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "bg_LT");
		UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "bg_RT");
		if (m_parent != null && uISprite != null && uISprite2 != null && uISprite3 != null && uISprite4 != null)
		{
			uISprite.spriteName = m_parent.wheel.GetRouletteBgSprite();
			uISprite2.spriteName = m_parent.wheel.GetRouletteBgSprite();
			uISprite3.spriteName = m_parent.wheel.GetRouletteBoardSprite();
			uISprite4.spriteName = m_parent.wheel.GetRouletteBoardSprite();
		}
		if (m_itemPosList == null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "item");
			if (gameObject2 != null)
			{
				m_itemPosList = new List<GameObject>();
				for (int i = 0; i < 8; i++)
				{
					gameObject = GameObjectUtil.FindChildGameObject(gameObject2, "pos_" + i);
					if (gameObject != null)
					{
						gameObject.SetActive(true);
						m_itemPosList.Add(gameObject);
						continue;
					}
					break;
				}
			}
		}
		if (m_cellEffList == null)
		{
			m_cellEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "cell_eff");
			if (m_cellEffBase != null)
			{
				m_cellEffList = new List<UISprite>();
				for (int j = 0; j < 8; j++)
				{
					UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_cellEffBase, "cell_eff_" + j);
					if (uISprite5 != null)
					{
						uISprite5.enabled = true;
						uISprite5.gameObject.SetActive(true);
						GeneralUtil.SetGameObjectOutMoveEnabled(uISprite5.gameObject, j == initCell);
						m_cellEffList.Add(uISprite5);
						continue;
					}
					break;
				}
				m_cellEffBase.SetActive(m_parent.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BOARD));
			}
		}
		if (m_rankEffList == null)
		{
			m_rankEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "rank_eff");
			if (m_rankEffBase != null)
			{
				m_rankEffList = new List<UISprite>();
				for (int k = 0; k < 8; k++)
				{
					UISprite uISprite6 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rankEffBase, "rank_eff_" + k);
					if (uISprite6 != null)
					{
						uISprite6.enabled = false;
						uISprite6.gameObject.SetActive(false);
						m_rankEffList.Add(uISprite6);
						continue;
					}
					break;
				}
				m_rankEffBase.SetActive(m_parent.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BOARD));
			}
		}
		if (m_activeEffList == null)
		{
			m_activeEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "active_eff");
			if (m_activeEffBase != null)
			{
				m_activeEffList = new List<UISprite>();
				for (int l = 0; l < 8; l++)
				{
					UISprite uISprite7 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_activeEffBase, "active_eff_" + l);
					if (uISprite7 != null)
					{
						uISprite7.enabled = false;
						uISprite7.gameObject.SetActive(false);
						m_activeEffList.Add(uISprite7);
						continue;
					}
					break;
				}
				m_activeEffBase.SetActive(false);
			}
		}
		SetupItem(orgItem);
	}

	private void SetupItem(RouletteItem orgItem)
	{
		if (m_itemList == null && m_itemPosList != null && m_itemPosList.Count > 0)
		{
			m_itemList = new List<RouletteItem>();
			for (int i = 0; i < m_itemPosList.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(orgItem.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.gameObject.transform.parent = m_itemPosList[i].transform;
				gameObject.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				RouletteItem componentInChildren = gameObject.GetComponentInChildren<RouletteItem>();
				if (componentInChildren != null)
				{
					componentInChildren.Setup(m_parent, i);
					m_itemList.Add(componentInChildren);
					if (m_rankEffList != null && m_rankEffList.Count >= 0 && m_rankEffList.Count > i)
					{
						m_rankEffList[i].enabled = componentInChildren.isRank;
					}
				}
			}
		}
		else if (m_itemList != null)
		{
			for (int j = 0; j < m_itemList.Count; j++)
			{
				m_itemList[j].Setup(m_parent, j);
			}
		}
	}

	public void SetCurrentCell(int currentCell)
	{
		if (m_cellEffList != null && m_cellEffList.Count > currentCell)
		{
			for (int i = 0; i < m_cellEffList.Count; i++)
			{
				GeneralUtil.SetGameObjectOutMoveEnabled(m_cellEffList[i].gameObject, currentCell == i);
			}
		}
	}

	public void Reset()
	{
		if (m_itemList != null)
		{
			for (int i = 0; i < m_itemList.Count; i++)
			{
				Object.Destroy(m_itemList[i].gameObject);
			}
			m_itemList.Clear();
			m_itemList = null;
		}
		base.gameObject.SetActive(false);
	}

	public int GetCellIndex(float degree)
	{
		int result = -1;
		float num = 0f;
		float num2 = 0f;
		if (degree >= 360f)
		{
			int num3 = (int)(degree / 360f);
			degree -= 360f * (float)num3;
		}
		degree += 360f;
		if (m_cells != null && m_cells.Count > 0)
		{
			for (int i = 0; i < m_cells.Count; i++)
			{
				Vector3 vector = m_cells[i];
				num = vector.x + 360f;
				Vector3 vector2 = m_cells[i];
				num2 = vector2.y + 360f;
				if (num > num2)
				{
					num -= 360f;
				}
				if (num <= degree && num2 >= degree)
				{
					result = i;
					break;
				}
				if (num + 360f <= degree && num2 + 360f >= degree)
				{
					result = i;
					break;
				}
			}
		}
		else
		{
			float num4 = 45f;
			for (int j = 0; j < 8; j++)
			{
				num = num4 * -0.5f + num4 * (float)j + 360f;
				num2 = num4 * 0.5f + num4 * (float)j + 360f;
				if (num <= degree && num2 >= degree)
				{
					result = j;
					break;
				}
				if (num + 360f <= degree && num2 + 360f >= degree)
				{
					result = j;
					break;
				}
			}
		}
		return result;
	}

	public float GetCelllastSpeed(int cellIndex)
	{
		float result = 1f;
		if (m_cells != null && m_cells.Count > cellIndex)
		{
			cellIndex = (cellIndex + m_cells.Count) % m_cells.Count;
			Vector3 vector = m_cells[cellIndex];
			result = vector.z;
		}
		return result;
	}

	public void GetCellData(int cellIndex, out float startRot, out float endRot, out float lastSpeedRate)
	{
		startRot = 0f;
		endRot = 0f;
		lastSpeedRate = 1f;
		if (m_cells != null && m_cells.Count > cellIndex)
		{
			cellIndex = (cellIndex + m_cells.Count) % m_cells.Count;
			Vector3 vector = m_cells[cellIndex];
			startRot = vector.x;
			endRot = vector.y;
			if (startRot > endRot)
			{
				endRot += 360f;
			}
			else
			{
				startRot += 360f;
				endRot += 360f;
			}
			lastSpeedRate = vector.z;
		}
		else
		{
			cellIndex = (cellIndex + 8) % 8;
			startRot = -18 + 45 * cellIndex + 360;
			endRot = 18 + 45 * cellIndex + 360;
			lastSpeedRate = 1f;
		}
	}

	public void UpdateEffectSetting()
	{
		if (m_parent != null && m_parent.parent != null)
		{
			m_activeEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "active_eff");
			m_cellEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "cell_eff");
			m_rankEffBase = GameObjectUtil.FindChildGameObject(base.gameObject, "rank_eff");
			if (m_activeEffBase != null)
			{
				m_activeEffBase.SetActive(false);
			}
			if (m_cellEffBase != null)
			{
				m_cellEffBase.SetActive(m_parent.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BOARD));
			}
			if (m_rankEffBase != null)
			{
				m_rankEffBase.SetActive(m_parent.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BOARD));
			}
		}
	}

	private void Start()
	{
		base.enabled = false;
	}

	private void Update()
	{
		base.enabled = false;
	}
}
