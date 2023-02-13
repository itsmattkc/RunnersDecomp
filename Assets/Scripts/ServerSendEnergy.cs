using Message;
using System.Collections;
using UnityEngine;

public class ServerSendEnergy
{
	private const string SuccessEvent = "ServerSendEnergy_Succeeded";

	private const string FailEvent = "ServerSendEnergy_Failed";

	public static IEnumerator Process(string friendId, GameObject callbackObject)
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
		NetServerSendEnergy net = new NetServerSendEnergy(friendId);
		net.Request();
		monitor.StartMonitor(new ServerSendEnergyRetry(friendId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSendEnergySucceed msg2 = new MsgSendEnergySucceed
			{
				m_expireTime = net.resultExpireTime
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSendEnergy_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSendEnergy_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSendEnergy_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
