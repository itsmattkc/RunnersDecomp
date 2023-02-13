using Message;
using System.Collections;
using UnityEngine;

public class ServerGetVariousParameter
{
	public static IEnumerator Process(GameObject callbackObject)
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
		NetServerGetVariousParameter net = new NetServerGetVariousParameter();
		net.Request();
		monitor.StartMonitor(new ServerGetVariousParameterRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerInterface.SettingState.m_energyRefreshTime = net.resultEnergyRecveryTime;
			ServerInterface.SettingState.m_energyRecoveryMax = net.resultEnergyRecoveryMax;
			ServerInterface.SettingState.m_onePlayCmCount = net.resultOnePlayCmCount;
			ServerInterface.SettingState.m_onePlayContinueCount = net.resultOnePlayContinueCount;
			ServerInterface.SettingState.m_cmSkipCount = net.resultCmSkipCount;
			ServerInterface.SettingState.m_isPurchased = net.resultIsPurchased;
			MsgGetVariousParameterSucceed msg2 = new MsgGetVariousParameterSucceed
			{
				m_energyRefreshTime = net.resultEnergyRecveryTime,
				m_energyRecoveryMax = net.resultEnergyRecoveryMax,
				m_onePlayCmCount = net.resultOnePlayCmCount,
				m_onePlayContinueCount = net.resultOnePlayContinueCount,
				m_cmSkipCount = net.resultCmSkipCount,
				m_isPurchased = net.resultIsPurchased
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetVariousParameter_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetVariousParameter_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetVariousParameter_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
