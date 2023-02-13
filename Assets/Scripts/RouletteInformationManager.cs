using System.Collections.Generic;
using Text;
using UnityEngine;

public class RouletteInformationManager : MonoBehaviour
{
	public class InfoBannerRequest
	{
		private UITexture m_texture;

		public InfoBannerRequest(UITexture texture)
		{
			m_texture = texture;
		}

		public void LoadDone(Texture2D loadedTex)
		{
			if (!(loadedTex == null) && !(m_texture == null))
			{
				m_texture.mainTexture = loadedTex;
			}
		}
	}

	private bool m_isSetuped;

	private Dictionary<RouletteCategory, InformationWindow.Information> m_rouletteInfo = new Dictionary<RouletteCategory, InformationWindow.Information>();

	private static RouletteInformationManager m_instance;

	public bool IsSetuped
	{
		get
		{
			return m_isSetuped;
		}
	}

	public static RouletteInformationManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool GetCurrentInfoParam(out Dictionary<RouletteCategory, InformationWindow.Information> infoParam)
	{
		if (m_isSetuped)
		{
			infoParam = new Dictionary<RouletteCategory, InformationWindow.Information>(m_rouletteInfo);
			return true;
		}
		infoParam = null;
		return false;
	}

	public void SetUp()
	{
		m_rouletteInfo.Clear();
		ServerNoticeInfo noticeInfo = ServerInterface.NoticeInfo;
		if (noticeInfo == null)
		{
			return;
		}
		List<NetNoticeItem> list = null;
		list = noticeInfo.m_rouletteItems;
		if (list == null)
		{
			return;
		}
		string commonText = TextUtility.GetCommonText("Informaion", "announcement");
		using (List<NetNoticeItem>.Enumerator enumerator = list.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				NetNoticeItem current = enumerator.Current;
				InformationWindow.Information value = default(InformationWindow.Information);
				value.pattern = (InformationWindow.ButtonPattern)current.WindowType;
				value.bodyText = current.Message;
				value.imageId = current.ImageId;
				value.texture = null;
				value.caption = commonText;
				value.url = current.Adress;
				m_rouletteInfo.Add(RouletteCategory.PREMIUM, value);
			}
		}
		EventManager.EventType typeInTime = EventManager.Instance.TypeInTime;
		if (typeInTime == EventManager.EventType.RAID_BOSS)
		{
			m_rouletteInfo.Add(RouletteCategory.RAID, default(InformationWindow.Information));
		}
		m_isSetuped = true;
	}

	public void LoadInfoBaner(InfoBannerRequest bannerRequest, RouletteCategory category = RouletteCategory.PREMIUM)
	{
		if (!m_isSetuped || m_rouletteInfo == null || !m_rouletteInfo.ContainsKey(category))
		{
			return;
		}
		InformationWindow.Information information = m_rouletteInfo[category];
		InformationImageManager instance = InformationImageManager.Instance;
		if (instance != null)
		{
			bool bannerFlag = true;
			instance.Load(information.imageId, bannerFlag, delegate(Texture2D tex)
			{
				bannerRequest.LoadDone(tex);
			});
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
