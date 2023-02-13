using Message;
using System.Collections;
using UnityEngine;

public class ServerGetCampaignList
{
	public static IEnumerator Process(GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		monitor.PrepareConnect();
		while (!monitor.IsEndPrepare())
		{
			yield return null;
		}
		if (!monitor.IsSuccessPrepare())
		{
			yield break;
		}
		NetServerGetCampaignList net = new NetServerGetCampaignList();
		net.Request();
		monitor.StartMonitor(new ServerGetCampaignListRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetCampaignListSucceed msg2 = new MsgGetCampaignListSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "GetCampaignList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("GetCampaignList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "GetCampaignList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
