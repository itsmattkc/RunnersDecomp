using System.Collections.Generic;

public class HudContinueUtility
{
	public static int GetContinueCost()
	{
		int result = 1;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerCampaignData continueCostCampaignData = GetContinueCostCampaignData();
			if (continueCostCampaignData != null)
			{
				result = continueCostCampaignData.iContent;
			}
			else
			{
				List<ServerConsumedCostData> costList = ServerInterface.CostList;
				if (costList != null)
				{
					foreach (ServerConsumedCostData item in costList)
					{
						if (item != null && item.consumedItemId == 950000)
						{
							return item.numItem;
						}
					}
					return result;
				}
			}
		}
		return result;
	}

	public static bool IsInContinueCostCampaign()
	{
		ServerCampaignData continueCostCampaignData = GetContinueCostCampaignData();
		if (continueCostCampaignData != null)
		{
			return true;
		}
		return false;
	}

	public static ServerCampaignData GetContinueCostCampaignData()
	{
		if (ServerInterface.CampaignState != null)
		{
			return ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.ContinueCost);
		}
		return null;
	}

	public static string GetContinueCostString()
	{
		return GetContinueCost().ToString();
	}

	public static int GetRedStarRingCount()
	{
		return (int)SaveDataManager.Instance.ItemData.RedRingCount;
	}

	public static string GetRedStarRingCountString()
	{
		return GetRedStarRingCount().ToString();
	}
}
