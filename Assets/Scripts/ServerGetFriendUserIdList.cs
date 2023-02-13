using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetFriendUserIdList
{
	public static IEnumerator Process(List<string> friendFBIdList, GameObject callbackObject)
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
		NetServerGetFriendUserIdList net = new NetServerGetFriendUserIdList(friendFBIdList);
		net.Request();
		monitor.StartMonitor(new ServerGetFriendUserIdListRetry(friendFBIdList, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerUserTransformData> transformDataList = ServerInterface.UserTransformDataList;
			transformDataList.Clear();
			if (net.resultTransformDataList != null)
			{
				foreach (ServerUserTransformData data in net.resultTransformDataList)
				{
					transformDataList.Add(data);
				}
			}
			MsgGetFriendUserIdListSucceed msg2 = new MsgGetFriendUserIdListSucceed
			{
				m_transformDataList = net.resultTransformDataList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetFriendUserIdList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetFriendUserIdList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetFriendUserIdList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
