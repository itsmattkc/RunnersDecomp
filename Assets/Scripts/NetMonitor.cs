using Message;
using System.Collections;
using Text;
using UnityEngine;

public class NetMonitor : MonoBehaviour
{
	private enum State
	{
		IDLE,
		PREPARE,
		SESSION_TIMEOUT,
		WAIT_CONNECTING,
		CONNECTING,
		ERROR_HANDLING
	}

	private static NetMonitor m_instance;

	private ErrorHandleBase m_errorHandler;

	private State m_state;

	private ServerRetryProcess m_retryProcess;

	private float m_hudDisplayWait;

	private float m_currentWait;

	private HudNetworkConnect.DisplayType m_ConnectDisplayType;

	private ServerSessionWatcher.ValidateType m_validateType;

	private int m_connectFailedCount;

	private bool m_isEndPrepare;

	private bool m_isSuccessPrepare;

	private static readonly int CountToAskGiveUp = 3;

	private bool m_isServerBusy;

	public static NetMonitor Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsIdle()
	{
		return m_state == State.IDLE;
	}

	private void Start()
	{
		if (m_instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			m_instance = this;
			base.gameObject.AddComponent<HudNetworkConnect>();
			base.gameObject.AddComponent<ServerSessionWatcher>();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void LateUpdate()
	{
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.PREPARE:
			break;
		case State.CONNECTING:
			break;
		case State.SESSION_TIMEOUT:
			if (NetworkErrorWindow.IsButtonPressed)
			{
				HudMenuUtility.GoToTitleScene();
				m_state = State.IDLE;
			}
			break;
		case State.WAIT_CONNECTING:
			if (m_hudDisplayWait < 0f)
			{
				m_state = State.CONNECTING;
				break;
			}
			m_currentWait += Time.deltaTime;
			if (m_currentWait >= m_hudDisplayWait)
			{
				HudNetworkConnect component = base.gameObject.GetComponent<HudNetworkConnect>();
				if (component != null)
				{
					component.Setup();
					component.PlayStart(m_ConnectDisplayType);
				}
				m_state = State.CONNECTING;
			}
			break;
		case State.ERROR_HANDLING:
			if (m_errorHandler != null)
			{
				m_errorHandler.Update();
				if (m_errorHandler.IsEnd())
				{
					m_state = State.IDLE;
					m_errorHandler.EndErrorHandle();
					m_errorHandler = null;
				}
			}
			break;
		}
	}

	private void OnDestroy()
	{
	}

	public void PrepareConnect()
	{
		PrepareConnect(ServerSessionWatcher.ValidateType.LOGIN_OR_RELOGIN);
	}

	public void PrepareConnect(ServerSessionWatcher.ValidateType validateType)
	{
		m_isEndPrepare = false;
		m_isSuccessPrepare = false;
		m_validateType = validateType;
		StartCoroutine(PrepareConnectCoroutine(validateType));
	}

	public IEnumerator PrepareConnectCoroutine(ServerSessionWatcher.ValidateType validateType)
	{
		if (m_isServerBusy)
		{
			m_isServerBusy = false;
			HudNetworkConnect connect = base.gameObject.GetComponent<HudNetworkConnect>();
			if (connect != null)
			{
				connect.PlayStart(HudNetworkConnect.DisplayType.ALL);
			}
			float currentTime = 0f;
			while (currentTime >= 3f)
			{
				currentTime += Time.unscaledDeltaTime;
				yield return null;
			}
			if (connect != null)
			{
				connect.PlayEnd();
			}
		}
		ServerSessionWatcher watcher = base.gameObject.GetComponent<ServerSessionWatcher>();
		if (watcher != null)
		{
			watcher.ValidateSession(m_validateType, ValidateSessionCallback);
		}
	}

	public bool IsEndPrepare()
	{
		return m_isEndPrepare;
	}

	public bool IsSuccessPrepare()
	{
		return m_isSuccessPrepare;
	}

	public void StartMonitor(ServerRetryProcess process)
	{
		StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
	}

	public void StartMonitor(ServerRetryProcess process, float hudDisplayWait, HudNetworkConnect.DisplayType displayType)
	{
		m_retryProcess = process;
		m_hudDisplayWait = hudDisplayWait;
		m_currentWait = 0f;
		m_ConnectDisplayType = displayType;
		m_state = State.WAIT_CONNECTING;
	}

	public void EndMonitorForward(MessageBase msg, GameObject callbackObject, string callbackFuncName)
	{
		if (msg == null)
		{
			return;
		}
		if (msg.ID == 61517)
		{
			m_connectFailedCount++;
			MsgServerConnctFailed msgServerConnctFailed = msg as MsgServerConnctFailed;
			if (msgServerConnctFailed != null)
			{
				switch (msgServerConnctFailed.m_status)
				{
				case ServerInterface.StatusCode.ServerMaintenance:
					m_errorHandler = new ErrorHandleMaintenance();
					break;
				case ServerInterface.StatusCode.NotReachability:
				case ServerInterface.StatusCode.TimeOut:
					if (m_connectFailedCount < CountToAskGiveUp)
					{
						ErrorHandleRetry errorHandleRetry = new ErrorHandleRetry();
						errorHandleRetry.SetRetryProcess(m_retryProcess);
						m_errorHandler = errorHandleRetry;
					}
					else
					{
						ErrorHandleAskGiveUpRetry errorHandleAskGiveUpRetry = new ErrorHandleAskGiveUpRetry();
						errorHandleAskGiveUpRetry.SetRetryProcess(m_retryProcess);
						m_errorHandler = errorHandleAskGiveUpRetry;
					}
					break;
				case ServerInterface.StatusCode.ExpirationSession:
				{
					ErrorHandleExpirationSession errorHandleExpirationSession = new ErrorHandleExpirationSession();
					errorHandleExpirationSession.SetRetryProcess(m_retryProcess);
					errorHandleExpirationSession.SetSessionValidateType(m_validateType);
					m_errorHandler = errorHandleExpirationSession;
					break;
				}
				case ServerInterface.StatusCode.MissingPlayer:
					m_errorHandler = new ErrorHandleMissingPlayer();
					break;
				case ServerInterface.StatusCode.VersionDifference:
					m_errorHandler = new ErrorHandleVersionDifference();
					break;
				case ServerInterface.StatusCode.InvalidReceiptData:
					m_errorHandler = new ErrorHandleInvalidReciept();
					break;
				case ServerInterface.StatusCode.ServerBusy:
					m_errorHandler = new ErrorHandleUnExpectedError();
					m_isServerBusy = true;
					break;
				case ServerInterface.StatusCode.ServerSecurityError:
				{
					ErrorHandleSecurityError errorHandleSecurityError = new ErrorHandleSecurityError();
					errorHandleSecurityError.SetRetryProcess(m_retryProcess);
					m_errorHandler = errorHandleSecurityError;
					break;
				}
				case ServerInterface.StatusCode.VersionForApplication:
				{
					ErrorHandleVersionForApplication errorHandleVersionForApplication = new ErrorHandleVersionForApplication();
					errorHandleVersionForApplication.SetRetryProcess(m_retryProcess);
					m_errorHandler = errorHandleVersionForApplication;
					break;
				}
				case ServerInterface.StatusCode.AlreadyInvitedFriend:
					m_errorHandler = new ErrorHandleAlreadyInvited();
					break;
				case ServerInterface.StatusCode.ServerNextVersion:
					m_errorHandler = new ErrorHandleServerNextVersion();
					break;
				default:
					m_errorHandler = new ErrorHandleUnExpectedError();
					break;
				}
			}
		}
		else if (msg.ID == 61520)
		{
			m_connectFailedCount++;
			if (m_connectFailedCount < CountToAskGiveUp)
			{
				ErrorHandleRetry errorHandleRetry2 = new ErrorHandleRetry();
				errorHandleRetry2.SetRetryProcess(m_retryProcess);
				m_errorHandler = errorHandleRetry2;
			}
			else
			{
				ErrorHandleAskGiveUpRetry errorHandleAskGiveUpRetry2 = new ErrorHandleAskGiveUpRetry();
				errorHandleAskGiveUpRetry2.SetRetryProcess(m_retryProcess);
				m_errorHandler = errorHandleAskGiveUpRetry2;
			}
		}
		else
		{
			OnResetConnectFailedCount();
		}
		if (m_errorHandler != null)
		{
			m_errorHandler.Setup(callbackObject, callbackFuncName, msg);
		}
	}

	public void EndMonitorBackward()
	{
		HudNetworkConnect component = base.gameObject.GetComponent<HudNetworkConnect>();
		if (component != null)
		{
			component.PlayEnd();
		}
		if (m_errorHandler != null)
		{
			m_errorHandler.StartErrorHandle();
			m_state = State.ERROR_HANDLING;
		}
		else
		{
			m_state = State.IDLE;
		}
	}

	public void ServerReLogin_Succeeded()
	{
		m_isEndPrepare = true;
		m_isSuccessPrepare = true;
	}

	private void ValidateSessionCallback(bool isSuccess)
	{
		m_isEndPrepare = true;
		m_isSuccessPrepare = isSuccess;
		if (!m_isSuccessPrepare)
		{
			NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
			info.buttonType = NetworkErrorWindow.ButtonType.Ok;
			info.anchor_path = string.Empty;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_session_timeout_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_session_timeout_text").text;
			NetworkErrorWindow.Create(info);
			m_state = State.SESSION_TIMEOUT;
		}
	}

	private void OnResetConnectFailedCount()
	{
		m_connectFailedCount = 0;
	}
}
