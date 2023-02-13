using System.Collections.Generic;
using Text;
using UnityEngine;

public class ui_mm_news_page : MonoBehaviour
{
	private const int DISPLAY_MAX_ITEM_COUNT = 10;

	[SerializeField]
	private UIRectItemStorage m_itemStorage;

	[SerializeField]
	private UIScrollBar m_scrollBar;

	private List<ui_mm_new_page_ad_banner.BannerInfo> m_bannerInfoList = new List<ui_mm_new_page_ad_banner.BannerInfo>();

	private List<NetNoticeItem> m_noticeItems;

	private void Start()
	{
		base.enabled = false;
	}

	private void SetInfomation()
	{
		string commonText = TextUtility.GetCommonText("Informaion", "announcement");
		m_bannerInfoList.Clear();
		ServerNoticeInfo noticeInfo = ServerInterface.NoticeInfo;
		if (noticeInfo == null)
		{
			return;
		}
		m_noticeItems = noticeInfo.m_noticeItems;
		if (m_noticeItems == null)
		{
			return;
		}
		NetNoticeItem netNoticeItem = null;
		foreach (NetNoticeItem noticeItem in m_noticeItems)
		{
			bool flag = true;
			ui_mm_new_page_ad_banner.BannerInfo bannerInfo = new ui_mm_new_page_ad_banner.BannerInfo();
			bannerInfo.info = default(InformationWindow.Information);
			bannerInfo.info.pattern = (InformationWindow.ButtonPattern)noticeItem.WindowType;
			bannerInfo.info.bodyText = noticeItem.Message;
			bannerInfo.info.imageId = noticeItem.ImageId;
			bannerInfo.info.texture = null;
			bannerInfo.info.url = noticeItem.Adress;
			bannerInfo.item = noticeItem;
			if (noticeItem.Id == NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID)
			{
				bannerInfo.info.rankingType = InformationWindow.RankingType.LEAGUE;
			}
			else if (noticeItem.Id == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID)
			{
				bannerInfo.info.rankingType = InformationWindow.RankingType.QUICK_LEAGUE;
			}
			else if (noticeItem.Id == NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID)
			{
				bannerInfo.info.rankingType = InformationWindow.RankingType.EVENT;
			}
			else
			{
				bannerInfo.info.caption = commonText;
				bannerInfo.info.rankingType = InformationWindow.RankingType.NON;
				flag = noticeInfo.IsOnTime(noticeItem);
			}
			if (flag)
			{
				m_bannerInfoList.Add(bannerInfo);
			}
		}
	}

	private void UpdatePage()
	{
		UpdateRectItemStorage();
		if (m_scrollBar != null)
		{
			m_scrollBar.value = 0f;
		}
	}

	private void UpdateRectItemStorage()
	{
		if (!(m_itemStorage != null))
		{
			return;
		}
		int count = m_bannerInfoList.Count;
		m_itemStorage.maxItemCount = count;
		m_itemStorage.maxRows = count;
		m_itemStorage.Restart();
		ui_mm_new_page_ad_banner[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_mm_new_page_ad_banner>(true);
		int num = componentsInChildren.Length;
		for (int i = 0; i < count; i++)
		{
			if (i < num)
			{
				componentsInChildren[i].UpdateView(m_bannerInfoList[i]);
			}
		}
	}

	public void StartInformation()
	{
		SetInfomation();
		UpdatePage();
	}
}
