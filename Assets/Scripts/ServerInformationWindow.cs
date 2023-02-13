using System.Collections.Generic;
using Text;
using UnityEngine;

public class ServerInformationWindow : MonoBehaviour
{
	private enum State
	{
		IDLE,
		INIT,
		SETUP,
		SETUP_END,
		PLAYING,
		WAIT_PLAY,
		END
	}

	private InformationWindow m_window;

	private GameObject m_windowObj;

	private List<NetNoticeItem> m_infos = new List<NetNoticeItem>();

	private int m_current_info;

	private bool m_playStartCue;

	private bool m_saveFlag;

	private State m_state;

	public bool IsReady
	{
		get
		{
			if (m_state != 0 && m_state != State.INIT && m_state != State.SETUP)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public bool IsEnd()
	{
		return m_state == State.END;
	}

	private void Start()
	{
		if (ServerInterface.NoticeInfo == null)
		{
			return;
		}
		List<NetNoticeItem> noticeItems = ServerInterface.NoticeInfo.m_noticeItems;
		if (noticeItems == null)
		{
			return;
		}
		m_infos = new List<NetNoticeItem>();
		if (m_infos == null)
		{
			return;
		}
		foreach (NetNoticeItem item in noticeItems)
		{
			if (item != null && item.Id != NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID && item.Id != NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID && !item.IsOnlyInformationPage() && !ServerInterface.NoticeInfo.IsChecked(item) && ServerInterface.NoticeInfo.IsOnTime(item))
			{
				m_infos.Add(item);
			}
		}
		if (m_infos.Count > 0)
		{
			m_state = State.INIT;
		}
	}

	private void SetWindowData()
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			m_windowObj = GameObjectUtil.FindChildGameObject(cameraUIObject, "NewsWindow");
			if (m_windowObj != null)
			{
				m_windowObj.SetActive(false);
			}
		}
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.SETUP:
			break;
		case State.SETUP_END:
			break;
		case State.INIT:
			SetWindowData();
			if (m_playStartCue)
			{
				m_playStartCue = false;
				CreateInformationWindow();
				m_state = State.PLAYING;
			}
			break;
		case State.PLAYING:
			if (m_window != null)
			{
				if (m_window.IsButtonPress(InformationWindow.ButtonType.LEFT))
				{
					m_state = State.WAIT_PLAY;
				}
				else if (m_window.IsButtonPress(InformationWindow.ButtonType.RIGHT))
				{
					m_state = State.WAIT_PLAY;
				}
				else if (m_window.IsButtonPress(InformationWindow.ButtonType.CLOSE))
				{
					m_state = State.WAIT_PLAY;
				}
			}
			break;
		case State.WAIT_PLAY:
			if (m_window != null && m_window.IsEnd())
			{
				UpdateInformaitionSaveData();
				if (HasNext())
				{
					PlayNext();
					break;
				}
				DestroyInformationWindow();
				m_state = State.END;
			}
			break;
		}
	}

	public void Clear()
	{
		m_current_info = 0;
	}

	public bool HasNext()
	{
		if (m_infos != null)
		{
			int num = m_current_info + 1;
			if (num < m_infos.Count)
			{
				return true;
			}
		}
		return false;
	}

	public void PlayStart()
	{
		if (m_infos != null && m_infos.Count > 0)
		{
			if (m_state != State.INIT)
			{
				CreateInformationWindow();
				m_state = State.PLAYING;
			}
			else
			{
				m_playStartCue = true;
			}
		}
		else
		{
			m_state = State.END;
		}
	}

	public void PlayNext()
	{
		if (HasNext())
		{
			m_current_info++;
			CreateInformationWindow();
			m_state = State.PLAYING;
		}
	}

	public void SetSaveFlag()
	{
		m_saveFlag = true;
	}

	private void CreateInformationWindow()
	{
		m_window = base.gameObject.GetComponent<InformationWindow>();
		if (m_window == null)
		{
			m_window = base.gameObject.AddComponent<InformationWindow>();
		}
		if (m_window != null)
		{
			NetNoticeItem netNoticeItem = m_infos[m_current_info];
			InformationWindow.Information info = default(InformationWindow.Information);
			if (netNoticeItem.WindowType == 0 || netNoticeItem.WindowType == 16)
			{
				info.pattern = InformationWindow.ButtonPattern.TEXT;
				info.imageId = "-1";
			}
			else
			{
				info.pattern = InformationWindow.ButtonPattern.OK;
				info.imageId = netNoticeItem.ImageId;
			}
			info.caption = TextUtility.GetCommonText("Informaion", "announcement");
			info.bodyText = netNoticeItem.Message;
			m_window.Create(info, m_windowObj);
		}
	}

	private void UpdateInformaitionSaveData()
	{
		ServerNoticeInfo noticeInfo = ServerInterface.NoticeInfo;
		if (noticeInfo != null)
		{
			noticeInfo.UpdateChecked(m_infos[m_current_info]);
			if (m_saveFlag)
			{
				noticeInfo.SaveInformation();
			}
		}
	}

	private void DestroyInformationWindow()
	{
		if (m_window != null)
		{
			Object.Destroy(m_window);
			m_window = null;
		}
	}
}
