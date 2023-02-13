using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidBosshelpRequestWindow : WindowBase
{
	private enum BUTTON_ACT
	{
		CLOSE,
		INFO,
		END,
		NONE
	}

	private const float UPDATE_TIME = 0.25f;

	[SerializeField]
	private UIPanel mainPanel;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIDraggablePanel m_listPanel;

	private bool m_close;

	private BUTTON_ACT m_btnAct = BUTTON_ACT.NONE;

	private UIRectItemStorage m_storage;

	private List<ServerEventRaidBossDesiredState> m_desiredList;

	private static RaidBosshelpRequestWindow s_instance;

	private static RaidBosshelpRequestWindow Instance
	{
		get
		{
			return s_instance;
		}
	}

	public bool isFinished()
	{
		return m_btnAct == BUTTON_ACT.END;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Setup(List<ServerEventRaidBossDesiredState> data)
	{
		StartCoroutine(OnSetup(data));
	}

	public IEnumerator OnSetup(List<ServerEventRaidBossDesiredState> data)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		mainPanel.alpha = 1f;
		m_close = false;
		m_btnAct = BUTTON_ACT.NONE;
		m_desiredList = data;
		GameObject ui_button = GameObjectUtil.FindChildGameObject(mainPanel.gameObject, "Btn_ok");
		if (ui_button != null)
		{
			UIButtonMessage button_msg = ui_button.GetComponent<UIButtonMessage>();
			if (button_msg != null)
			{
				button_msg.enabled = true;
				button_msg.trigger = UIButtonMessage.Trigger.OnClick;
				button_msg.target = base.gameObject;
				button_msg.functionName = "OnClickOkButton";
			}
		}
		if (m_desiredList != null)
		{
			m_listPanel.enabled = true;
			m_storage = m_listPanel.GetComponentInChildren<UIRectItemStorage>();
			if (m_storage != null && m_desiredList.Count > 0)
			{
				m_storage.maxItemCount = (m_storage.maxRows = m_desiredList.Count);
				m_storage.Restart();
				ui_ranking_scroll[] list = m_storage.GetComponentsInChildren<ui_ranking_scroll>();
				if (list != null && list.Length > 0)
				{
					for (int i = 0; i < m_desiredList.Count && list.Length > i; i++)
					{
						list[i].UpdateViewForRaidbossDesired(m_desiredList[i]);
					}
				}
			}
		}
		yield return null;
		if (m_animation != null)
		{
			ActiveAnimation activeAnim = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnim.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
	}

	public void OnClickOkButton()
	{
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_ok");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			component.Play(true);
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			switch (m_btnAct)
			{
			case BUTTON_ACT.INFO:
				base.gameObject.SetActive(false);
				break;
			case BUTTON_ACT.CLOSE:
				m_btnAct = BUTTON_ACT.END;
				base.gameObject.SetActive(false);
				break;
			default:
				base.gameObject.SetActive(false);
				break;
			}
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		OnClickOkButton();
	}

	public static RaidBosshelpRequestWindow Create(List<ServerEventRaidBossDesiredState> data)
	{
		if (s_instance != null)
		{
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(data);
			return s_instance;
		}
		return null;
	}

	private void Awake()
	{
		SetInstance();
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
