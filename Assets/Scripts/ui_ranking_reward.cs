using UnityEngine;

public class ui_ranking_reward : MonoBehaviour
{
	public const int ADD_HEGHT = 48;

	public const int INIT_LINE = 3;

	public const float OPEN_SPEED = 0.5f;

	public const float CLOSE_SPEED = 0.25f;

	[SerializeField]
	private UISprite m_bg;

	[SerializeField]
	private BoxCollider m_collider;

	[SerializeField]
	private UILabel m_label;

	[SerializeField]
	private UISprite m_icon;

	[SerializeField]
	private UIDragPanelContents m_dragPanelContents;

	private float m_move;

	private UIDraggablePanel m_parent;

	private UITable m_table;

	private void Start()
	{
		base.enabled = false;
	}

	private void Update()
	{
		if (m_parent == null)
		{
			m_parent = base.gameObject.transform.parent.GetComponent<UIDraggablePanel>();
			if (m_parent == null)
			{
				m_parent = base.gameObject.transform.parent.transform.parent.GetComponent<UIDraggablePanel>();
				m_dragPanelContents.draggablePanel = m_parent;
			}
			else
			{
				m_dragPanelContents.draggablePanel = m_parent;
			}
		}
		if (m_table == null)
		{
			m_table = base.gameObject.transform.parent.GetComponent<UITable>();
			if (m_table == null)
			{
				m_table = base.gameObject.transform.parent.transform.parent.GetComponent<UITable>();
			}
		}
		if (m_move > 0f)
		{
			m_move -= Time.deltaTime;
			if (m_move <= 0f)
			{
				if (m_table != null)
				{
					m_table.repositionNow = true;
				}
				m_move = 0f;
			}
		}
		Vector3 size = m_collider.size;
		if (size.y - 48f != (float)m_label.height)
		{
			float num = (float)m_label.height * 1.2f + 48f;
			BoxCollider collider = m_collider;
			Vector3 size2 = m_collider.size;
			float x = size2.x;
			Vector3 size3 = m_collider.size;
			collider.size = new Vector3(x, num, size3.z);
			m_bg.height = (int)num;
		}
	}

	private void OnClickBg()
	{
		Debug.Log("OnClickBg m_icon:" + (m_icon != null));
	}
}
