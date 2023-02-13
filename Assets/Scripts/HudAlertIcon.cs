using Message;
using UnityEngine;

public class HudAlertIcon : MonoBehaviour
{
	private const float X_OFFSET = 50f;

	private static GameObject m_prefabObject;

	private GameObject m_iconObject;

	private Camera m_camera;

	private GameObject m_chaseObject;

	private TinyFsmBehavior m_fsm;

	private float m_displayTime;

	private float m_currentTime;

	private bool m_isEnd;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public void Setup(Camera camera, GameObject chaseObject, float displayTime)
	{
		m_camera = camera;
		m_chaseObject = chaseObject;
		m_displayTime = displayTime;
		m_currentTime = 0f;
		m_isEnd = false;
	}

	public bool IsChasingObject(GameObject gameObject)
	{
		if (m_chaseObject == gameObject)
		{
			return true;
		}
		return false;
	}

	private void Start()
	{
		if (m_prefabObject == null)
		{
			m_prefabObject = (Resources.Load("Prefabs/UI/ui_gp_icon_alert") as GameObject);
			if (m_prefabObject == null)
			{
				return;
			}
		}
		m_iconObject = (Object.Instantiate(m_prefabObject, Vector3.zero, Quaternion.identity) as GameObject);
		m_iconObject.SetActive(false);
		GameObject gameObject = GameObject.Find("UI Root (2D)/Camera/Anchor_6_MR/");
		if (gameObject == null)
		{
			gameObject = new GameObject("Anchor_6_MR");
			GameObject gameObject2 = GameObject.Find("UI Root (2D)/Camera");
			if (gameObject2 != null)
			{
				gameObject.transform.parent = gameObject2.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			UIAnchor uIAnchor = gameObject.AddComponent<UIAnchor>();
			uIAnchor.side = UIAnchor.Side.Right;
			uIAnchor.halfPixelOffset = false;
			if (gameObject2 != null)
			{
				uIAnchor.uiCamera = gameObject2.GetComponent<Camera>();
			}
		}
		m_iconObject.transform.parent = gameObject.transform;
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (!(m_fsm == null))
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StatePlay);
			description.onFixedUpdate = true;
			m_fsm.SetUp(description);
		}
	}

	private void Update()
	{
	}

	private TinyFsmState StatePlay(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			m_iconObject.SetActive(true);
			Vector2 screenPosition = HudUtility.GetScreenPosition(m_camera, m_chaseObject);
			screenPosition.x = -50f;
			screenPosition.y -= (float)Screen.height * 0.5f;
			Transform component = m_iconObject.GetComponent<Transform>();
			component.localPosition = new Vector3(screenPosition.x, screenPosition.y, 0f);
			m_currentTime += Time.deltaTime;
			if (m_currentTime >= m_displayTime)
			{
				m_isEnd = true;
				m_iconObject.SetActive(false);
				Object.Destroy(m_iconObject);
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}
}
