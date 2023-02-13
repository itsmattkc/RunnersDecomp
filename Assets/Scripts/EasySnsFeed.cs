using System.Diagnostics;
using Text;
using UnityEngine;

public class EasySnsFeed
{
	private enum Phase
	{
		START,
		WAIT_PRELOGIN,
		LOGIN,
		WAIT_LOGIN,
		FEED,
		WAIT_FEED,
		GET_INCENTIVE,
		WAIT_GET_INCENTIVE,
		VIEW_INCENTIVE,
		WAIT_VIEW_INCENTIVE,
		COMPLETED,
		FAILED
	}

	public enum Result
	{
		NONE,
		COMPLETED,
		FAILED
	}

	private GameObject m_snsLoginGameObject;

	private SettingPartsSnsLogin m_snsLogin;

	private Phase m_phase;

	private Result m_result;

	private GameObject m_gameObject;

	private string m_caption;

	private string m_text;

	private ui_mm_mileage_page m_mileage_page;

	private EasySnsFeedMonoBehaviour m_easySnsFeedMonoBehaviour;

	private bool m_isLoginOnly;

	public EasySnsFeed(GameObject gameObject, string anchorPath, string caption, string text, ui_mm_mileage_page mileage_page = null)
	{
		Init(gameObject, anchorPath, caption, text, mileage_page);
	}

	public EasySnsFeed(GameObject gameObject, string anchorPath)
	{
		m_isLoginOnly = true;
		Init(gameObject, anchorPath);
	}

	private void Init(GameObject gameObject, string anchorPath, string caption = null, string text = null, ui_mm_mileage_page mileage_page = null)
	{
		m_gameObject = gameObject;
		m_caption = caption;
		m_text = text;
		m_mileage_page = mileage_page;
		m_easySnsFeedMonoBehaviour = gameObject.GetComponent<EasySnsFeedMonoBehaviour>();
		if (m_easySnsFeedMonoBehaviour == null)
		{
			m_easySnsFeedMonoBehaviour = gameObject.AddComponent<EasySnsFeedMonoBehaviour>();
		}
		m_easySnsFeedMonoBehaviour.Init();
		m_snsLoginGameObject = GameObject.Find("EasySnsFeed.SnsLogin");
		if (m_snsLoginGameObject == null)
		{
			m_snsLoginGameObject = new GameObject("EasySnsFeed.SnsLogin");
		}
		if (m_snsLoginGameObject != null)
		{
			m_snsLogin = m_snsLoginGameObject.GetComponent<SettingPartsSnsLogin>();
			if (m_snsLogin == null)
			{
				m_snsLogin = m_snsLoginGameObject.AddComponent<SettingPartsSnsLogin>();
			}
		}
		if (m_snsLogin != null)
		{
			m_snsLogin.Setup(anchorPath);
			HudMenuUtility.SetConnectAlertSimpleUI(true);
		}
		else
		{
			m_result = Result.FAILED;
		}
	}

	public Result Update()
	{
		if (m_result == Result.NONE)
		{
			ServerInterface serverInterface = null;
			switch (m_phase)
			{
			case Phase.START:
			case Phase.WAIT_PRELOGIN:
				m_phase = Phase.LOGIN;
				break;
			case Phase.LOGIN:
				HudMenuUtility.SetConnectAlertSimpleUI(false);
				m_snsLogin.SetCancelWindowUseFlag(!m_isLoginOnly);
				m_snsLogin.PlayStart();
				m_phase = Phase.WAIT_LOGIN;
				break;
			case Phase.WAIT_LOGIN:
				if (m_snsLogin.IsEnd)
				{
					m_phase = (m_snsLogin.IsCalceled ? Phase.FAILED : ((!m_isLoginOnly) ? Phase.FEED : Phase.COMPLETED));
				}
				break;
			case Phase.FEED:
			{
				SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
				if (socialInterface != null)
				{
					socialInterface.Feed(m_caption, m_text, m_gameObject);
				}
				m_phase = ((!(socialInterface != null)) ? Phase.FAILED : Phase.WAIT_FEED);
				break;
			}
			case Phase.WAIT_FEED:
			{
				bool? isFeeded = m_easySnsFeedMonoBehaviour.m_isFeeded;
				if (isFeeded.HasValue)
				{
					m_phase = ((m_easySnsFeedMonoBehaviour.m_isFeeded != true) ? Phase.FAILED : Phase.COMPLETED);
				}
				break;
			}
			case Phase.GET_INCENTIVE:
				serverInterface = ServerInterface.LoggedInServerInterface;
				if (serverInterface != null)
				{
					serverInterface.RequestServerGetFacebookIncentive(2, 0, m_gameObject);
				}
				m_phase = ((!(serverInterface != null)) ? Phase.FAILED : Phase.WAIT_GET_INCENTIVE);
				break;
			case Phase.WAIT_GET_INCENTIVE:
				if (m_easySnsFeedMonoBehaviour.m_feedIncentiveList != null)
				{
					m_phase = Phase.VIEW_INCENTIVE;
				}
				break;
			case Phase.VIEW_INCENTIVE:
				if (m_easySnsFeedMonoBehaviour.m_feedIncentiveList.Count > 0)
				{
					ServerPresentState serverPresentState = m_easySnsFeedMonoBehaviour.m_feedIncentiveList[0];
					m_easySnsFeedMonoBehaviour.m_feedIncentiveList.RemoveAt(0);
					ItemGetWindow itemGetWindow2 = ItemGetWindowUtil.GetItemGetWindow();
					if (itemGetWindow2 != null)
					{
						itemGetWindow2.Create(new ItemGetWindow.CInfo
						{
							caption = TextUtility.GetCommonText("SnsFeed", "gw_feed_incentive_caption"),
							serverItemId = serverPresentState.m_itemId,
							imageCount = TextUtility.GetCommonText("SnsFeed", "gw_feed_incentive_text", "{COUNT}", HudUtility.GetFormatNumString(serverPresentState.m_numItem))
						});
					}
					HudMenuUtility.SendMsgUpdateSaveDataDisplay();
					m_phase = Phase.WAIT_VIEW_INCENTIVE;
				}
				else
				{
					HudMenuUtility.SendMsgUpdateSaveDataDisplay();
					m_phase = Phase.COMPLETED;
				}
				break;
			case Phase.WAIT_VIEW_INCENTIVE:
			{
				ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
				if (itemGetWindow != null && itemGetWindow.IsEnd)
				{
					itemGetWindow.Reset();
					m_phase = Phase.VIEW_INCENTIVE;
				}
				break;
			}
			case Phase.COMPLETED:
				m_result = Result.COMPLETED;
				break;
			case Phase.FAILED:
				m_result = Result.FAILED;
				break;
			}
		}
		return m_result;
	}

	[Conditional("DEBUG_INFO")]
	public static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
