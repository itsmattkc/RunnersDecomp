using Message;
using System.Collections;
using UnityEngine;

public class ServerLogin
{
	private static int m_passwordErrorCount;

	private static readonly int PasswordErrorCountMax = 3;

	public static IEnumerator Process(string userId, string password, GameObject callbackObject)
	{
		NetServerLogin net = new NetServerLogin(userId, password);
		net.Request();
		NetMonitor monitor = NetMonitor.Instance;
		if (monitor != null)
		{
			monitor.StartMonitor(new ServerLoginRetry(userId, password, callbackObject));
		}
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
			loginState.sessionTimeLimit = net.sessionTimeLimit;
			ServerSettingState settingState = ServerInterface.SettingState;
			settingState.m_energyRefreshTime = net.resultEnergyRefreshTime;
			settingState.m_energyRecoveryMax = net.energyRecoveryMax;
			settingState.m_invitBaseIncentive = net.resultInviteBaseIncentive;
			settingState.m_rentalBaseIncentive = net.resultRentalBaseIncentive;
			settingState.m_userName = net.userName;
			settingState.m_userId = net.userId;
			MsgLoginSucceed msg3 = new MsgLoginSucceed
			{
				m_userId = net.userId,
				m_password = net.password
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg3, callbackObject, "ServerLogin_Succeeded");
			}
			m_passwordErrorCount = 0;
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerLogin_Succeeded", msg3, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (net.resultStCd == ServerInterface.StatusCode.PassWordError && m_passwordErrorCount < PasswordErrorCountMax)
		{
			m_passwordErrorCount++;
			MsgServerPasswordError msg2 = new MsgServerPasswordError
			{
				m_key = net.key,
				m_userId = net.userId,
				m_password = net.password
			};
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerLogin_Failed", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			m_passwordErrorCount = 0;
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerLogin_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
