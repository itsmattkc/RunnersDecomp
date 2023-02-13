using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetChaoRentalStates
{
	private const string SuccessEvent = "ServerGetChaoRentalStates_Succeeded";

	private const string FailEvent = "ServerGetChaoRentalStates_Failed";

	public static IEnumerator Process(string[] friendId, GameObject callbackObject)
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
		NetServerGetRentalState net = new NetServerGetRentalState(friendId);
		net.Request();
		monitor.StartMonitor(new ServerGetChaoRentalStatesRetry(friendId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			int resultChaoRentalStates = net.resultStates;
			List<ServerChaoRentalState> resultChaoRentalStateList = new List<ServerChaoRentalState>(resultChaoRentalStates);
			for (int i = 0; i < resultChaoRentalStates; i++)
			{
				ServerChaoRentalState chaoRentalState = net.GetResultChaoRentalState(i);
				resultChaoRentalStateList.Add(chaoRentalState);
			}
			MsgGetFriendChaoStateSucceed msg2 = new MsgGetFriendChaoStateSucceed
			{
				m_chaoRentalStates = resultChaoRentalStateList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetChaoRentalStates_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetChaoRentalStates_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetChaoRentalStates_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
