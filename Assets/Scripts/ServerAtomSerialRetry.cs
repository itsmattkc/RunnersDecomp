using UnityEngine;

public class ServerAtomSerialRetry : ServerRetryProcess
{
	public string m_campaignId;

	public string m_serial;

	public bool m_new_user;

	public ServerAtomSerialRetry(string campaignId, string serial, bool new_user, GameObject callbackObject)
		: base(callbackObject)
	{
		m_campaignId = campaignId;
		m_serial = serial;
		m_new_user = new_user;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerAtomSerial(m_campaignId, m_serial, m_new_user, m_callbackObject);
		}
	}
}
