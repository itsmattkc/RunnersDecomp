using Message;
using System.Collections;
using UnityEngine;

public class ServerGetCharacterState
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
		NetServerGetCharacterState net = new NetServerGetCharacterState();
		net.Request();
		monitor.StartMonitor(new ServerGetCharacterStateRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerCharacterState[] resultCharacterState = net.resultCharacterState;
			playerState.ClearCharacterState();
			ServerCharacterState[] array = resultCharacterState;
			foreach (ServerCharacterState characterState in array)
			{
				if (characterState != null)
				{
					playerState.SetCharacterState(characterState);
				}
			}
			MsgGetCharacterStateSucceed msg2 = new MsgGetCharacterStateSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetCharacterState_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetCharacterState_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetCharacterState_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
