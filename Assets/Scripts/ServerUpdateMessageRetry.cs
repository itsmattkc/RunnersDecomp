using System.Collections.Generic;
using UnityEngine;

public class ServerUpdateMessageRetry : ServerRetryProcess
{
	public List<int> m_messageIdList;

	public List<int> m_operatorMessageIdList;

	public ServerUpdateMessageRetry(List<int> messageIdList, List<int> operatorMessageIdList, GameObject callbackObject)
		: base(callbackObject)
	{
		m_messageIdList = messageIdList;
		m_operatorMessageIdList = operatorMessageIdList;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUpdateMessage(m_messageIdList, m_operatorMessageIdList, m_callbackObject);
		}
	}
}
