using UnityEngine;

public class HudInformation : MonoBehaviour
{
	private GameObject m_badgeObj;

	private GameObject m_mileageObj;

	private UILabel m_volumeLabel;

	private bool m_initFlag;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		m_initFlag = true;
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (!(mainMenuUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(mainMenuUIObject, "Anchor_7_BL");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_0_info");
			if (gameObject2 != null)
			{
				m_badgeObj = GameObjectUtil.FindChildGameObject(gameObject2, "badge");
				m_volumeLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_present_volume");
			}
		}
	}

	private void UpdateInfoIcon()
	{
		int num = 0;
		if (ServerInterface.NoticeInfo != null && ServerInterface.NoticeInfo.m_noticeItems != null)
		{
			foreach (NetNoticeItem noticeItem in ServerInterface.NoticeInfo.m_noticeItems)
			{
				if (noticeItem != null && !ServerInterface.NoticeInfo.IsCheckedForMenuIcon(noticeItem) && ServerInterface.NoticeInfo.IsOnTime(noticeItem))
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			if (m_badgeObj != null && m_badgeObj.activeSelf)
			{
				m_badgeObj.SetActive(false);
			}
			return;
		}
		if (m_badgeObj != null && !m_badgeObj.activeSelf)
		{
			m_badgeObj.SetActive(true);
		}
		if (m_volumeLabel != null)
		{
			m_volumeLabel.text = num.ToString();
		}
	}

	public void OnUpdateInformationDisplay()
	{
		if (!m_initFlag)
		{
			Initialize();
		}
		UpdateInfoIcon();
	}

	public void OnUpdateSaveDataDisplay()
	{
		if (!m_initFlag)
		{
			Initialize();
		}
		if (EventManager.Instance != null && EventManager.Instance.IsInEvent())
		{
			GeneralUtil.SetEventBanner(m_mileageObj);
		}
		UpdateInfoIcon();
	}
}
