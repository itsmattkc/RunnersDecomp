using Text;
using UnityEngine;

public class AgeVerification : MonoBehaviour
{
	private enum State
	{
		NONE = -1,
		IDLE,
		APOLLO_SEND_START,
		APOLLO_SEND_START_WAIT,
		CAUTION_AGE_VERIFICATION,
		CAUTION_AGE_VERIFICATION_WAIT,
		AGE_VERIFICATION,
		AGE_VERIFICATION_WAIT,
		APOLLO_SEND_END,
		APOLLO_SEND_END_WAIT,
		FINISHED_AGE_VERIFICATION,
		FINISHED_AGE_VERIFICATION_WAIT,
		END,
		COUNT
	}

	private SendApollo m_sendApollo;

	private string m_anchorPath;

	private AgeVerificationWindow m_ageVerification;

	private State m_state;

	public static bool IsAgeVerificated
	{
		get
		{
			ServerSettingState serverSettingState = null;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				serverSettingState = ServerInterface.SettingState;
			}
			if (!string.IsNullOrEmpty(serverSettingState.m_birthday))
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public bool IsEnd
	{
		get
		{
			if (m_state == State.END)
			{
				return true;
			}
			return false;
		}
	}

	public void Setup(string anchorPath)
	{
		m_anchorPath = anchorPath;
		m_ageVerification = base.gameObject.GetComponent<AgeVerificationWindow>();
		if (m_ageVerification == null)
		{
			m_ageVerification = base.gameObject.AddComponent<AgeVerificationWindow>();
		}
		m_ageVerification.Setup(anchorPath);
	}

	public void PlayStart()
	{
		base.gameObject.SetActive(true);
		m_state = State.APOLLO_SEND_START;
	}

	private void Start()
	{
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.END:
			break;
		case State.APOLLO_SEND_START:
		{
			string[] value2 = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP3, ref value2);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value2);
			m_state = State.APOLLO_SEND_START_WAIT;
			break;
		}
		case State.APOLLO_SEND_START_WAIT:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
				m_state = State.CAUTION_AGE_VERIFICATION;
			}
			break;
		case State.CAUTION_AGE_VERIFICATION:
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.caption = TextUtility.GetCommonText("Shop", "gw_age_verification_caption");
			info2.message = TextUtility.GetCommonText("Shop", "gw_age_verification_text");
			info2.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info2);
			m_state = State.CAUTION_AGE_VERIFICATION_WAIT;
			break;
		}
		case State.CAUTION_AGE_VERIFICATION_WAIT:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				m_state = State.AGE_VERIFICATION;
			}
			break;
		case State.AGE_VERIFICATION:
			if (m_ageVerification != null && m_ageVerification.IsReady)
			{
				m_ageVerification.PlayStart();
				m_state = State.AGE_VERIFICATION_WAIT;
			}
			break;
		case State.AGE_VERIFICATION_WAIT:
			if (m_ageVerification != null && m_ageVerification.IsEnd)
			{
				m_state = State.APOLLO_SEND_END;
			}
			break;
		case State.APOLLO_SEND_END:
		{
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP3, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
			m_state = State.APOLLO_SEND_END_WAIT;
			break;
		}
		case State.APOLLO_SEND_END_WAIT:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
				m_state = State.FINISHED_AGE_VERIFICATION;
			}
			break;
		case State.FINISHED_AGE_VERIFICATION:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("Shop", "gw_age_verification_success_caption");
			info.message = TextUtility.GetCommonText("Shop", "gw_age_verification_success_text");
			info.anchor_path = m_anchorPath;
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			m_state = State.FINISHED_AGE_VERIFICATION_WAIT;
			break;
		}
		case State.FINISHED_AGE_VERIFICATION_WAIT:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				base.gameObject.SetActive(false);
				m_state = State.END;
			}
			break;
		}
	}
}
