using Message;
using System.Collections;
using UnityEngine;

public class ServerGetMigrationPassword
{
	public static IEnumerator Process(string userPassword, GameObject callbackObject)
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
		NetServerGetMigrationPassword net = new NetServerGetMigrationPassword(userPassword);
		net.Request();
		monitor.StartMonitor(new ServerGetMigrationPasswordRetry(userPassword, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerInterface.MigrationPassword = net.paramMigrationPassword;
			MsgGetMigrationPasswordSucceed msg2 = new MsgGetMigrationPasswordSucceed
			{
				m_migrationPassword = net.paramMigrationPassword
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetMigrationPassword_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetMigrationPassword_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetMigrationPassword_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
