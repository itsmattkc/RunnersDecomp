using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetMileageData
{
	public static IEnumerator Process(string[] distanceFriendList, GameObject callbackObject)
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
		NetServerGetMileageData net = new NetServerGetMileageData(distanceFriendList);
		net.Request();
		monitor.StartMonitor(new ServerGetMileageDataRetry(distanceFriendList, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
			ServerMileageMapState resultMileageMapState = MakeResultMileageState(net);
			if (resultMileageMapState != null && mileageMapState != null)
			{
				resultMileageMapState.CopyTo(mileageMapState);
			}
			List<ServerMileageFriendEntry> mileageFriendList = ServerInterface.MileageFriendList;
			List<ServerMileageFriendEntry> resultMileageFriendList = net.m_resultMileageFriendList;
			mileageFriendList.Clear();
			for (int index = 0; index < resultMileageFriendList.Count; index++)
			{
				ServerMileageFriendEntry e = new ServerMileageFriendEntry();
				resultMileageFriendList[index].CopyTo(e);
				mileageFriendList.Add(e);
			}
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetMileageData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetMileageData_Succeeded", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetMileageData_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}

	private static ServerMileageMapState MakeResultMileageState(NetServerGetMileageData net)
	{
		ServerMileageMapState serverMileageMapState = new ServerMileageMapState();
		net.resultMileageMapState.CopyTo(serverMileageMapState);
		return serverMileageMapState;
	}
}
