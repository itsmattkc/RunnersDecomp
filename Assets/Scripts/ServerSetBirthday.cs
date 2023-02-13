using Message;
using System.Collections;
using UnityEngine;

public class ServerSetBirthday
{
	public static IEnumerator Process(string birthday, GameObject callbackObject)
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
		NetServerSetBirthday net = new NetServerSetBirthday(birthday);
		net.Request();
		monitor.StartMonitor(new ServerSetBirthdayRetry(birthday, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerSettingState settingState = ServerInterface.SettingState;
			if (settingState != null && string.IsNullOrEmpty(settingState.m_birthday))
			{
				settingState.m_birthday = birthday;
			}
			MsgSetBirthday msg2 = new MsgSetBirthday();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSetBirthday_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSetBirthday_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSetBirthday_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
