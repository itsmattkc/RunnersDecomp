using Message;
using System.Collections.Generic;
using UnityEngine;

internal class EasySnsFeedMonoBehaviour : MonoBehaviour
{
	public bool? m_isFeeded;

	public List<ServerPresentState> m_feedIncentiveList;

	public void Init()
	{
		m_isFeeded = null;
		m_feedIncentiveList = null;
	}

	private void FeedEndCallback(MsgSocialNormalResponse msg)
	{
		m_isFeeded = !msg.m_result.IsError;
	}

	private void ServerGetFacebookIncentive_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		m_feedIncentiveList = msg.m_incentive;
	}

	private void ServerGetFacebookIncentive_Failed(MsgServerConnctFailed mag)
	{
		m_feedIncentiveList = new List<ServerPresentState>();
	}
}
