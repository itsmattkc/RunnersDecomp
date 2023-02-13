using Message;
using System.Collections;
using UnityEngine;

public class ServerGetCountry
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
		NetServerGetCountry net = new NetServerGetCountry();
		net.Request();
		monitor.StartMonitor(new ServerGetCountryRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerInterface.SettingState.m_countryId = net.resultCountryId;
			ServerInterface.SettingState.m_countryCode = net.resultCountryCode;
			MsgGetCountrySucceed msg2 = new MsgGetCountrySucceed
			{
				m_countryId = net.resultCountryId,
				m_countryCode = net.resultCountryCode
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetCountry_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetCountry_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetCountry_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
