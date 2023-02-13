using Message;
using Text;
using UnityEngine;

public class HudContinue : MonoBehaviour
{
	private enum State
	{
		IDLE,
		SERVER_CONNECT_WAIT,
		ASK_CONTINUE_START,
		ASK_CONTINUE,
		BUY_RED_STAR_RING_START,
		BUY_RED_STAR_RING,
		PURCHASE_COMPLETED,
		WAIT_VIDEO_RESPONSE,
		NUM
	}

	private HudContinueWindow m_continueWindow;

	private HudContinueBuyRsRing m_buyRsRingWindow;

	private int m_cmWatchingMaxCount;

	private int m_watchingCount;

	private int m_beforeRsRingCount;

	private State m_state;

	private void Start()
	{
		if (ServerInterface.SettingState != null)
		{
			m_cmWatchingMaxCount = ServerInterface.SettingState.m_onePlayCmCount;
		}
	}

	public void SetTimeUp(bool timeUp)
	{
		if (m_continueWindow != null)
		{
			m_continueWindow.SetTimeUpObj(timeUp);
		}
	}

	public void PlayStart()
	{
		m_state = State.ASK_CONTINUE_START;
		m_beforeRsRingCount = GetCurrentRsRingCount();
	}

	public void PushBackKey()
	{
		if (m_state == State.ASK_CONTINUE)
		{
			if (m_continueWindow != null)
			{
				m_continueWindow.OnPushBackKey();
			}
		}
		else if (m_state == State.BUY_RED_STAR_RING && m_buyRsRingWindow != null)
		{
			m_buyRsRingWindow.OnPushBackKey();
		}
	}

	public void Setup(bool bossStage)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Continue_window");
		if (gameObject != null)
		{
			m_continueWindow = gameObject.AddComponent<HudContinueWindow>();
			m_continueWindow.Setup(bossStage);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "simple_shop_window");
		if (gameObject2 != null)
		{
			m_buyRsRingWindow = gameObject2.AddComponent<HudContinueBuyRsRing>();
			m_buyRsRingWindow.Setup();
		}
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.SERVER_CONNECT_WAIT:
			break;
		case State.WAIT_VIDEO_RESPONSE:
			break;
		case State.ASK_CONTINUE_START:
			if (m_continueWindow != null)
			{
				m_continueWindow.SetVideoButton(m_watchingCount < m_cmWatchingMaxCount);
				m_continueWindow.PlayStart();
			}
			m_state = State.ASK_CONTINUE;
			break;
		case State.ASK_CONTINUE:
		{
			if (!(m_continueWindow != null))
			{
				break;
			}
			int currentRsRingCount = GetCurrentRsRingCount();
			if (m_continueWindow.IsYesButtonPressed)
			{
				int redStarRingCount = HudContinueUtility.GetRedStarRingCount();
				int continueCost = HudContinueUtility.GetContinueCost();
				if (redStarRingCount >= continueCost)
				{
					ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
					if (loggedInServerInterface != null)
					{
						loggedInServerInterface.RequestServerActRetry(base.gameObject);
						m_state = State.SERVER_CONNECT_WAIT;
					}
					else
					{
						ServerActRetry_Succeeded(null);
					}
				}
				else
				{
					m_state = State.BUY_RED_STAR_RING_START;
				}
			}
			else if (m_continueWindow.IsNoButtonPressed)
			{
				MsgContinueResult value = new MsgContinueResult(false);
				GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
				m_state = State.IDLE;
			}
			else if (m_continueWindow.IsVideoButtonPressed)
			{
				m_state = State.WAIT_VIDEO_RESPONSE;
			}
			else if (m_beforeRsRingCount < currentRsRingCount)
			{
				GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
				info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_item_caption").text;
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Item", "red_star_ring").text;
				text += " ";
				TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", "gw_purchase_success_text");
				text2.ReplaceTag("{COUNT}", (currentRsRingCount - m_beforeRsRingCount).ToString());
				text = (info2.message = text + text2.text);
				info2.anchor_path = "Camera/Anchor_5_MC";
				info2.buttonType = GeneralWindow.ButtonType.Ok;
				info2.name = "PurchaseCompleted";
				GeneralWindow.Create(info2);
				m_beforeRsRingCount = currentRsRingCount;
				m_state = State.PURCHASE_COMPLETED;
			}
			break;
		}
		case State.BUY_RED_STAR_RING_START:
			if (ServerInterface.IsRSREnable())
			{
				if (m_buyRsRingWindow != null)
				{
					m_buyRsRingWindow.PlayStart();
				}
			}
			else
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.name = "ErrorRSRing";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption");
				info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption_text_2");
				info.isPlayErrorSe = true;
				GeneralWindow.Create(info);
			}
			m_state = State.BUY_RED_STAR_RING;
			break;
		case State.BUY_RED_STAR_RING:
			if (ServerInterface.IsRSREnable())
			{
				if (m_buyRsRingWindow != null && m_buyRsRingWindow.IsEndPlay)
				{
					if (m_buyRsRingWindow.IsSuccess || m_buyRsRingWindow.IsCanceled)
					{
						m_state = State.ASK_CONTINUE_START;
					}
					else if (m_buyRsRingWindow.IsFailed)
					{
						m_state = State.BUY_RED_STAR_RING_START;
					}
				}
			}
			else if (GeneralWindow.IsCreated("ErrorRSRing") && GeneralWindow.IsButtonPressed)
			{
				m_state = State.ASK_CONTINUE_START;
				GeneralWindow.Close();
			}
			break;
		case State.PURCHASE_COMPLETED:
			if (GeneralWindow.IsCreated("PurchaseCompleted") && GeneralWindow.IsButtonPressed)
			{
				m_state = State.ASK_CONTINUE_START;
			}
			break;
		}
	}

	private void SendContinuResult(bool continueFlag)
	{
		MsgContinueResult value = new MsgContinueResult(continueFlag);
		GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
	}

	private void ServerActRetry_Succeeded(MsgActRetrySucceed msg)
	{
		int continueCost = HudContinueUtility.GetContinueCost();
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerInterface.PlayerState.m_numRedRings -= continueCost;
			SaveDataManager.Instance.ItemData.RedRingCount = (uint)ServerInterface.PlayerState.m_numRedRings;
		}
		SendContinuResult(true);
		m_state = State.IDLE;
	}

	private void ServerActRetryFree_Succeeded(MsgActRetryFreeSucceed msg)
	{
		SendContinuResult(true);
		m_state = State.IDLE;
	}

	private static int GetCurrentRsRingCount()
	{
		int result = 0;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			result = ServerInterface.PlayerState.m_numRedRings;
		}
		else
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				result = (int)instance.ItemData.RedRingCount;
			}
		}
		return result;
	}
}
