using Text;
using UnityEngine;

public class HudCampaignBanner : MonoBehaviour
{
	private GameObject m_mainMenuObject;

	private GameObject m_textureObj;

	private UITexture m_replaceTex;

	private InformationWindow m_infoWindow;

	private long m_id = -1L;

	private bool m_quick;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		if (m_replaceTex != null)
		{
			m_replaceTex.mainTexture = null;
			m_replaceTex = null;
		}
	}

	private void Update()
	{
		if (m_infoWindow != null && m_infoWindow.IsEnd())
		{
			base.enabled = false;
		}
	}

	public void Initialize(GameObject mainMenuObject, bool quickMode)
	{
		m_quick = quickMode;
		if (mainMenuObject == null)
		{
			return;
		}
		m_mainMenuObject = mainMenuObject;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
		if (gameObject == null)
		{
			return;
		}
		string name = (!m_quick) ? "0_Endless" : "1_Quick";
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, name);
		if (gameObject2 == null)
		{
			return;
		}
		m_textureObj = GameObjectUtil.FindChildGameObject(gameObject2, "img_ad_tex");
		if (!(m_textureObj == null))
		{
			UIButtonMessage component = m_textureObj.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.enabled = true;
				component.trigger = UIButtonMessage.Trigger.OnClick;
				component.target = base.gameObject;
				component.functionName = "CampaignBannerClicked";
			}
			m_replaceTex = m_textureObj.GetComponent<UITexture>();
			m_textureObj.SetActive(false);
			UpdateView();
		}
	}

	public void UpdateView()
	{
		bool flag = false;
		if (EventManager.Instance != null)
		{
			if (EventManager.Instance.Type == EventManager.EventType.QUICK)
			{
				flag = m_quick;
			}
			else if (EventManager.Instance.Type == EventManager.EventType.BGM)
			{
				EventStageData stageData = EventManager.Instance.GetStageData();
				if (stageData != null)
				{
					flag = ((!m_quick) ? stageData.IsEndlessModeBGM() : stageData.IsQuickModeBGM());
				}
			}
		}
		if (flag)
		{
			if (ServerInterface.NoticeInfo == null || ServerInterface.NoticeInfo.m_eventItems == null)
			{
				return;
			}
			foreach (NetNoticeItem eventItem in ServerInterface.NoticeInfo.m_eventItems)
			{
				if (m_id != eventItem.Id)
				{
					m_id = eventItem.Id;
					if (InformationImageManager.Instance != null)
					{
						InformationImageManager.Instance.Load(eventItem.ImageId, true, OnLoadCallback);
					}
					if (m_textureObj != null)
					{
						m_textureObj.SetActive(true);
					}
					break;
				}
			}
		}
		else
		{
			if (m_textureObj != null)
			{
				m_textureObj.SetActive(false);
			}
			if (m_replaceTex != null && m_replaceTex.mainTexture != null)
			{
				m_replaceTex.mainTexture = null;
			}
		}
	}

	public void OnLoadCallback(Texture2D texture)
	{
		if (m_replaceTex != null && texture != null)
		{
			m_replaceTex.mainTexture = texture;
		}
	}

	private void CampaignBannerClicked()
	{
		if (m_mainMenuObject == null)
		{
			return;
		}
		m_infoWindow = base.gameObject.GetComponent<InformationWindow>();
		if (m_infoWindow == null)
		{
			m_infoWindow = base.gameObject.AddComponent<InformationWindow>();
		}
		if (!(m_infoWindow != null) || ServerInterface.NoticeInfo == null || ServerInterface.NoticeInfo.m_eventItems == null)
		{
			return;
		}
		foreach (NetNoticeItem eventItem in ServerInterface.NoticeInfo.m_eventItems)
		{
			if (m_id == eventItem.Id)
			{
				InformationWindow.Information info = default(InformationWindow.Information);
				info.pattern = InformationWindow.ButtonPattern.OK;
				info.imageId = eventItem.ImageId;
				info.caption = TextUtility.GetCommonText("Informaion", "announcement");
				GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
				if (cameraUIObject != null)
				{
					GameObject newsWindowObj = GameObjectUtil.FindChildGameObject(cameraUIObject, "NewsWindow");
					m_infoWindow.Create(info, newsWindowObj);
					base.enabled = true;
					SoundManager.SePlay("sys_menu_decide");
				}
				break;
			}
		}
	}
}
