using Message;
using UnityEngine;

public class SendApollo : MonoBehaviour
{
	public enum State
	{
		Idle,
		Request,
		Succeeded,
		Failed
	}

	private bool m_debugDraw = true;

	private State m_state;

	private bool IsDebugDraw()
	{
		return false;
	}

	public State GetState()
	{
		return m_state;
	}

	public bool IsEnd()
	{
		if (m_state != State.Request)
		{
			return true;
		}
		return false;
	}

	public void RequestServer(ApolloType type, string[] value)
	{
		if (IsDebugDraw())
		{
			Debug.Log("SendApollo RequestServer type=" + type);
			if (value != null)
			{
				foreach (string str in value)
				{
					Debug.Log("SendApollo RequestServer value=" + str);
				}
			}
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSendApollo((int)type, value, base.gameObject);
			m_state = State.Request;
		}
	}

	private void ServerSendApollo_Succeeded(MsgSendApolloSucceed msg)
	{
		if (IsDebugDraw())
		{
			Debug.Log("SendApollo ServerSendApollo_Succeeded");
		}
		m_state = State.Succeeded;
	}

	private void ServerSendApollo_Failed(MsgServerConnctFailed msg)
	{
		if (IsDebugDraw())
		{
			Debug.Log("SendApollo ServerSendApollo_Failed");
		}
		m_state = State.Failed;
	}

	public static SendApollo Create()
	{
		GameObject gameObject = new GameObject("SendApollo");
		return gameObject.AddComponent<SendApollo>();
	}

	public static SendApollo CreateRequest(ApolloType type, string[] value)
	{
		SendApollo sendApollo = Create();
		if (sendApollo != null)
		{
			sendApollo.RequestServer(type, value);
		}
		return sendApollo;
	}

	public static void GetTutorialValue(ApolloTutorialIndex index, ref string[] value)
	{
		if (value != null)
		{
			string[] obj = value;
			int num = (int)index;
			obj[0] = num.ToString();
		}
	}
}
