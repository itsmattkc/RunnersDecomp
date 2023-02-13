using System;

public class ServerCampaignData
{
	public Constants.Campaign.emType campaignType;

	public int id;

	public long beginDate;

	public long endDate;

	public int iContent;

	public int iSubContent;

	public float fContent
	{
		get
		{
			return (float)iContent / fContentBasis;
		}
	}

	public static float fContentBasis
	{
		get
		{
			return 1000f;
		}
	}

	public float fSubContent
	{
		get
		{
			return (float)iSubContent / 1000f;
		}
	}

	public ServerCampaignData()
	{
		campaignType = Constants.Campaign.emType.BankedRingBonus;
		id = 0;
		beginDate = 0L;
		endDate = 0L;
		iContent = 0;
	}

	public void CopyTo(ServerCampaignData to)
	{
		to.campaignType = campaignType;
		to.id = id;
		to.beginDate = beginDate;
		to.endDate = endDate;
		to.iContent = iContent;
	}

	public bool InSession()
	{
		DateTime t = (beginDate == 0L) ? DateTime.MinValue : NetUtil.GetLocalDateTime(beginDate);
		DateTime t2 = (endDate == 0L) ? DateTime.MaxValue : NetUtil.GetLocalDateTime(endDate);
		DateTime currentTime = NetUtil.GetCurrentTime();
		return currentTime >= t && currentTime <= t2;
	}
}
