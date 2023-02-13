using UnityEngine;

public class ServerBuyIosRetry : ServerRetryProcess
{
	public string m_receiptData;

	public ServerBuyIosRetry(string receiptData, GameObject callbackObject)
		: base(callbackObject)
	{
		m_receiptData = receiptData;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerBuyIos(m_receiptData, m_callbackObject);
		}
	}
}
