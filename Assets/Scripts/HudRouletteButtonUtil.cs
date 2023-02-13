using UnityEngine;

public class HudRouletteButtonUtil
{
	public static void SetSpecialEggIcon(GameObject eggObj)
	{
		if (eggObj != null)
		{
			bool active = false;
			if (RouletteManager.Instance != null && RouletteManager.Instance.specialEgg >= 10)
			{
				active = true;
			}
			eggObj.SetActive(active);
		}
	}

	public static void SetFreeSpin(GameObject badgeSpinObj, UILabel uiLable, bool counterStop = false)
	{
		int num = 0;
		bool active = false;
		ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
		if (wheelOptions != null)
		{
			num = wheelOptions.m_numRemaining;
			if (num > 0)
			{
				active = true;
			}
			if (counterStop && num > 999)
			{
				num = 999;
			}
		}
		if (badgeSpinObj != null)
		{
			badgeSpinObj.SetActive(active);
		}
		if (uiLable != null)
		{
			uiLable.text = num.ToString();
		}
	}

	public static void SetChaoFreeSpin(GameObject badgeSpinObj, UILabel uiLable, bool counterStop = false)
	{
		int num = 0;
		bool active = false;
		ServerChaoWheelOptions chaoWheelOptions = ServerInterface.ChaoWheelOptions;
		if (chaoWheelOptions != null)
		{
			num = chaoWheelOptions.NumRouletteToken;
			if (num > 0)
			{
				active = true;
			}
			if (counterStop && num > 999)
			{
				num = 999;
			}
		}
		if (badgeSpinObj != null)
		{
			badgeSpinObj.SetActive(active);
		}
		if (uiLable != null)
		{
			uiLable.text = num.ToString();
		}
	}

	public static void SetSaleIcon(GameObject iconObj)
	{
		if (iconObj != null)
		{
			bool active = HudMenuUtility.IsSale(Constants.Campaign.emType.ChaoRouletteCost);
			iconObj.SetActive(active);
		}
	}

	public static void SetEventIcon(GameObject eventObj)
	{
		if (eventObj != null)
		{
			bool active = EventUtility.IsEnableRouletteUI();
			eventObj.SetActive(active);
		}
	}
}
