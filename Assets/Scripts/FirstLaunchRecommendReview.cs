using Message;
using SaveData;
using Text;
using UnityEngine;

public class FirstLaunchRecommendReview : MonoBehaviour
{
	private enum EventSignal
	{
		PLAY_START = 100,
		REVIEW_END,
		INCENTIVE_CONNECT_END
	}

	private TinyFsmBehavior m_fsm;

	private bool m_isEndPlay;

	private string m_anchorPath;

	private SendApollo m_sendApollo;

	private IncentiveWindowQueue m_windowQueue = new IncentiveWindowQueue();

	public bool IsEndPlay
	{
		get
		{
			return m_isEndPlay;
		}
		private set
		{
		}
	}

	public void Setup(string anchorPath)
	{
		m_anchorPath = anchorPath;
	}

	public void PlayStart()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		m_isEndPlay = false;
	}

	private void Start()
	{
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateIdle);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
	}

	private void Update()
	{
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateSendApolloStart));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP8, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				m_fsm.ChangeState(new TinyFsmState(StateRecommendReview));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRecommendReview(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("FaceBook", "ui_Lbl_recommend_review_caption");
			string text = info.message = TextUtility.GetCommonText("FaceBook", "ui_Lbl_recommend_review_text", "{RED_STAR_RING_NUM}", "5");
			info.anchor_path = m_anchorPath;
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateUploadReview));
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUploadReview(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Application.OpenURL(NetBaseUtil.RedirectInstallPageUrl);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateEnd));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP8, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				m_fsm.ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.RECOMMEND_REVIEW_END);
			m_isEndPlay = true;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerGetFacebookIncentive_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		foreach (ServerPresentState item in msg.m_incentive)
		{
			if (item != null)
			{
				IncentiveWindow window = new IncentiveWindow(item.m_itemId, item.m_numItem, m_anchorPath);
				m_windowQueue.AddWindow(window);
			}
		}
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(102);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}
}
