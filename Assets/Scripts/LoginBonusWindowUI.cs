using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class LoginBonusWindowUI : WindowBase
{
	public enum LoginBonusType
	{
		NORMAL,
		FIRST,
		MAX
	}

	private bool m_isClickClose;

	private bool m_isEnd;

	private bool m_isOpened;

	private Animation m_animation;

	private List<LoginDayObject> m_days;

	private ServerLoginBonusData m_BonusData;

	private List<ServerLoginBonusReward> m_RewardList;

	private ServerLoginBonusReward m_TodayReward;

	private LoginBonusType m_type;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
		base.enabled = false;
		m_isEnd = false;
		m_isOpened = false;
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private IEnumerator Setup(LoginBonusType type)
	{
		m_type = type;
		ServerInterface serverInterface = ServerInterface.LoggedInServerInterface;
		if (!(serverInterface != null))
		{
			yield break;
		}
		m_days = new List<LoginDayObject>();
		GameObject loginBonusObj = GameObjectUtil.FindChildGameObject(base.gameObject, "login_bonus");
		m_BonusData = ServerInterface.LoginBonusData;
		if (m_BonusData == null || !(loginBonusObj != null))
		{
			yield break;
		}
		int nowBonusCount = 0;
		if (type == LoginBonusType.NORMAL)
		{
			m_RewardList = m_BonusData.m_loginBonusRewardList;
			nowBonusCount = m_BonusData.m_loginBonusState.m_numBonus;
		}
		else
		{
			m_RewardList = m_BonusData.m_firstLoginBonusRewardList;
			nowBonusCount = m_BonusData.m_loginBonusState.m_numLogin;
		}
		if (m_RewardList != null)
		{
			int dayCount = m_RewardList.Count;
			for (int j = 0; j < dayCount; j++)
			{
				string objName = "ui_login_day" + (j + 1);
				if (j == dayCount - 1)
				{
					objName = "ui_login_big";
				}
				GameObject loginDayObj = GameObjectUtil.FindChildGameObject(loginBonusObj, objName);
				if (!(loginDayObj != null))
				{
					continue;
				}
				LoginDayObject dayObj = new LoginDayObject(loginDayObj, j + 1);
				ServerLoginBonusReward reward = m_RewardList[j];
				if (reward != null)
				{
					if (reward.m_itemList != null)
					{
						dayObj.SetItem(reward.m_itemList[0].m_itemId);
						dayObj.count = reward.m_itemList[0].m_num;
					}
					dayObj.SetAlready(j < nowBonusCount);
				}
				m_days.Add(dayObj);
			}
			UILabel Label_days = GameObjectUtil.FindChildGameObjectComponent<UILabel>(loginBonusObj, "Lbl_days");
			if (Label_days != null)
			{
				int nowDayCount = m_BonusData.CalcTodayCount();
				int totalDays = m_BonusData.getTotalDays();
				if (m_type == LoginBonusType.FIRST)
				{
					nowDayCount = m_BonusData.m_loginBonusState.m_numLogin;
					totalDays = dayCount;
				}
				if (nowDayCount > -1)
				{
					int lastDayCount = totalDays - nowDayCount;
					if (lastDayCount < 0)
					{
						lastDayCount = 0;
					}
					if (m_type == LoginBonusType.FIRST)
					{
						Label_days.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "LoginBonus", "count").text, new Dictionary<string, string>
						{
							{
								"{COUNT}",
								lastDayCount.ToString()
							}
						});
					}
					else
					{
						Label_days.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "LoginBonus", "day").text, new Dictionary<string, string>
						{
							{
								"{DAY}",
								lastDayCount.ToString()
							}
						});
					}
				}
			}
			UILabel Label_BonusItems = GameObjectUtil.FindChildGameObjectComponent<UILabel>(loginBonusObj, "Lbl_loginbonus_feature");
			if (Label_BonusItems != null && m_BonusData.m_loginBonusState != null)
			{
				if (type == LoginBonusType.NORMAL)
				{
					m_TodayReward = m_BonusData.m_lastBonusReward;
				}
				else
				{
					m_TodayReward = m_BonusData.m_firstLastBonusReward;
				}
				string rewardText = string.Empty;
				string lineBreakText = "\n";
				if (m_TodayReward != null)
				{
					int itemCount = m_TodayReward.m_itemList.Count;
					for (int i = 0; i < itemCount; i++)
					{
						if (m_TodayReward.m_itemList[i] != null)
						{
							string itemName = new ServerItem((ServerItem.Id)m_TodayReward.m_itemList[i].m_itemId).serverItemName;
							int numItemCount = m_TodayReward.m_itemList[i].m_num;
							if (!string.IsNullOrEmpty(itemName))
							{
								string text = rewardText;
								rewardText = text + itemName + "×" + numItemCount + lineBreakText;
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(rewardText))
				{
					Label_BonusItems.text = rewardText;
				}
			}
		}
		yield return null;
	}

	public void PlayStart(LoginBonusType type)
	{
		base.enabled = true;
		m_TodayReward = null;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "login_window");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
			StartCoroutine(Setup(type));
			if (m_RewardList == null || m_TodayReward == null)
			{
				OnClosedWindowAnim();
				Debug.Log("LoginBonusWindowUI::PlayStart >> NotLoginBonusReward! = " + type);
				return;
			}
			gameObject.SetActive(true);
			m_animation = gameObject.GetComponent<Animation>();
			if (m_animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, OnOpenWindowAnim, true);
			}
			UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(gameObject, "Btn_next");
			if (uIPlayAnimation != null)
			{
				uIPlayAnimation.enabled = false;
			}
			UIPlayAnimation uIPlayAnimation2 = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(gameObject, "blinder");
			if (uIPlayAnimation2 != null)
			{
				uIPlayAnimation2.enabled = false;
			}
		}
		m_isEnd = false;
		m_isClickClose = false;
		m_isOpened = false;
	}

	private void Update()
	{
	}

	private void OnClickNextButton()
	{
		if (!m_isClickClose)
		{
			SoundManager.SePlay("sys_menu_decide");
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnClosedWindowAnim, true);
			}
			m_isClickClose = true;
		}
	}

	public void OnOpenWindowAnim()
	{
		if (m_isOpened)
		{
			return;
		}
		m_isOpened = true;
		if (m_days != null && m_BonusData != null && m_BonusData.m_loginBonusState != null)
		{
			int num = 0;
			int num2 = 0;
			if (m_type == LoginBonusType.NORMAL)
			{
				num = m_BonusData.m_loginBonusState.m_numBonus - 1;
				num2 = m_BonusData.m_loginBonusRewardList.Count;
			}
			else
			{
				num = m_BonusData.m_loginBonusState.m_numLogin - 1;
				num2 = m_BonusData.m_firstLoginBonusRewardList.Count;
			}
			if (num2 < num)
			{
				num = num2 - 1;
			}
			if (num > -1)
			{
				m_days[num].PlayGetAnimation();
			}
		}
	}

	public void OnClosedWindowAnim()
	{
		base.gameObject.SetActive(false);
		base.enabled = false;
		m_isEnd = true;
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isEnd)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (!m_isClickClose)
		{
			SoundManager.SePlay("sys_menu_decide");
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnClosedWindowAnim, true);
			}
			m_isClickClose = true;
		}
	}
}
