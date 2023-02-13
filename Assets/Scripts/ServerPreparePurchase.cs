using Message;
using System.Collections;
using UnityEngine;

public class ServerPreparePurchase
{
	public static IEnumerator Process(int itemId, GameObject callbackObject)
	{
		NetServerPreparePurchase net = new NetServerPreparePurchase(itemId);
		net.Request();
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerPreparePurchase_Succeeded", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			callbackObject.SendMessage("ServerPreparePurchase_Failed", msg, SendMessageOptions.DontRequireReceiver);
		}
	}
}
