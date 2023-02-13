using Message;
using System.Collections;
using UnityEngine;

public class ServerAtomSerial
{
	public static IEnumerator Process(string campaignId, string serial, bool new_user, GameObject callbackObject)
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
		NetServerAtomSerial net = new NetServerAtomSerial(campaignId, serial, new_user);
		net.Request();
		monitor.StartMonitor(new ServerAtomSerialRetry(campaignId, serial, new_user, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSendAtomSerialSucceed msg2 = new MsgSendAtomSerialSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerAtomSerial_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerAtomSerial_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (msg.m_status == ServerInterface.StatusCode.InvalidSerialCode || msg.m_status == ServerInterface.StatusCode.UsedSerialCode)
			{
				if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerAtomSerial_Failed", msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerAtomSerial_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}

	public static bool GetSerialFromScheme(string scheme, ref string campaign, ref string serial)
	{
		if (string.IsNullOrEmpty(scheme))
		{
			return false;
		}
		int num = scheme.IndexOf("cid=");
		if (num > 0)
		{
			campaign = scheme.Substring(num + "cid=".Length);
			int num2 = campaign.IndexOf("&");
			if (num2 > 0)
			{
				campaign = campaign.Remove(num2);
			}
		}
		int num3 = scheme.IndexOf("serial=");
		if (num3 > 0)
		{
			serial = scheme.Substring(num3 + "serial=".Length);
		}
		if (string.IsNullOrEmpty(serial))
		{
			int num4 = scheme.IndexOf("start_code");
			if (num4 > 0)
			{
				serial = scheme.Substring(num4 + "start_code".Length);
			}
		}
		if (string.IsNullOrEmpty(campaign) || string.IsNullOrEmpty(serial))
		{
			return false;
		}
		return true;
	}
}
