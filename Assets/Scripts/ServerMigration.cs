using Message;
using System.Collections;
using UnityEngine;

public class ServerMigration
{
	public static IEnumerator Process(string migrationId, string migrationPassword, GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		NetServerMigration net = new NetServerMigration(migrationId, migrationPassword);
		net.Request();
		monitor.StartMonitor(new ServerMigrationRetry(migrationId, migrationPassword, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerLoginState loginState = ServerInterface.LoginState;
			loginState.m_lineId = net.paramUserId;
			loginState.m_altLineId = string.Empty;
			loginState.m_lineAuthToken = string.Empty;
			loginState.sessionId = net.resultSessionId;
			ServerSettingState settingState = ServerInterface.SettingState;
			settingState.m_energyRefreshTime = net.resultEnergyRefreshTime;
			settingState.m_invitBaseIncentive = net.resultInviteBaseIncentive;
			settingState.m_rentalBaseIncentive = net.resultRentalBaseIncentive;
			settingState.m_userName = net.userName;
			settingState.m_userId = net.userId;
			MsgLoginSucceed msg2 = new MsgLoginSucceed
			{
				m_userId = net.userId,
				m_password = net.password,
				m_countryCode = net.countryCode
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerMigration_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerMigration_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (net.resultStCd == ServerInterface.StatusCode.PassWordError)
			{
				callbackObject.SendMessage("ServerMigration_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerMigration_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
