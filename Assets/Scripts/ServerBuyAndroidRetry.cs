using UnityEngine;

public class ServerBuyAndroidRetry : ServerRetryProcess
{
	public string m_receiptData;

	public string m_signature;

	public ServerBuyAndroidRetry(string receiptData, string signature, GameObject callbackObject)
		: base(callbackObject)
	{
		m_receiptData = receiptData;
		m_signature = signature;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerBuyAndroid(m_receiptData, m_signature, m_callbackObject);
		}
	}
}
