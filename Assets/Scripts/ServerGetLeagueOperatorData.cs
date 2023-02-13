using Message;
using System.Collections;
using UnityEngine;

public class ServerGetLeagueOperatorData
{
	public static IEnumerator Process(int mode, GameObject callbackObject)
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
		NetServerGetLeagueOperatorData net = new NetServerGetLeagueOperatorData(mode);
		net.Request();
		monitor.StartMonitor(new ServerGetLeagueOperatorDataRetry(mode, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetLeagueOperatorDataSucceed msg2 = new MsgGetLeagueOperatorDataSucceed
			{
				m_leagueOperatorData = net.leagueOperatorData
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetLeagueOperatorData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetLeagueOperatorData_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetLeagueOperatorData_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
