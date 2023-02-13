using Message;
using System.Collections;
using UnityEngine;

public class ServerPurchase
{
	public static IEnumerator Process(bool purchaseResult, GameObject callbackObject)
	{
		NetServerPurchase net = new NetServerPurchase(purchaseResult);
		net.Request();
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerPurchase_Succeeded", new MsgGetPlayerStateSucceed
				{
					m_playerState = playerState
				}, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (callbackObject != null)
		{
			callbackObject.SendMessage("ServerPurchase_Failed", new MsgServerConnctFailed(net.resultStCd)
			{
				m_status = net.resultStCd
			}, SendMessageOptions.DontRequireReceiver);
		}
	}
}
