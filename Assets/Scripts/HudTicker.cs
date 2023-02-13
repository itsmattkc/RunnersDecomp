using System.Collections.Generic;
using UnityEngine;

public class HudTicker : MonoBehaviour
{
	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	public void OnUpdateTickerDisplay()
	{
		TickerWindow tickerWindow = GetTickerWindow();
		if (tickerWindow == null)
		{
			return;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
			if (tickerInfo != null && tickerInfo.Data != null)
			{
				List<ServerTickerData> list = new List<ServerTickerData>();
				for (int i = 0; i < tickerInfo.Data.Count; i++)
				{
					ServerTickerData serverTickerData = tickerInfo.Data[i];
					if (serverTickerData != null && NetUtil.IsServerTimeWithinPeriod(serverTickerData.Start, serverTickerData.End))
					{
						list.Add(serverTickerData);
					}
				}
				if (list.Count > 0)
				{
					SetTickerData(list);
					return;
				}
			}
			List<ServerTickerData> list2 = new List<ServerTickerData>();
			ServerTickerData serverTickerData2 = new ServerTickerData();
			serverTickerData2.Init(0L, 0L, 0L, string.Empty);
			list2.Add(serverTickerData2);
			SetTickerData(list2);
		}
		else
		{
			List<ServerTickerData> list3 = new List<ServerTickerData>();
			ServerTickerData serverTickerData3 = new ServerTickerData();
			ServerTickerData serverTickerData4 = new ServerTickerData();
			serverTickerData3.Init(0L, 0L, 0L, "ログインしていません");
			serverTickerData4.Init(0L, 0L, 0L, "ログインしたらおしらせがひょうじされます");
			list3.Add(serverTickerData3);
			list3.Add(serverTickerData4);
			SetTickerData(list3);
		}
	}

	public void OnTickerReset()
	{
		TickerWindow tickerWindow = GetTickerWindow();
		if (tickerWindow != null)
		{
			tickerWindow.ResetWindow();
		}
	}

	private GameObject GetLabelObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "mainmenu_info_user");
		if (gameObject2 == null)
		{
			return null;
		}
		return GameObjectUtil.FindChildGameObject(gameObject2, "ticker");
	}

	private TickerWindow GetTickerWindow()
	{
		GameObject labelObject = GetLabelObject();
		if (labelObject == null)
		{
			return null;
		}
		return labelObject.GetComponent<TickerWindow>();
	}

	private void SetTickerData(List<ServerTickerData> tickerData)
	{
		TickerWindow tickerWindow = GetTickerWindow();
		if (tickerWindow != null)
		{
			TickerWindow.CInfo info = default(TickerWindow.CInfo);
			info.tickerList = tickerData;
			info.labelName = "Lbl_ticker";
			info.moveSpeed = 1f;
			info.moveSpeedUp = 20f;
			tickerWindow.Setup(info);
		}
	}
}
