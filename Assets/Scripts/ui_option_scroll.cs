using Text;
using UnityEngine;

public class ui_option_scroll : MonoBehaviour
{
	[SerializeField]
	private UISprite m_imgSprite;

	[SerializeField]
	private UILabel m_textLabel;

	private OptionUI.OptionInfo m_optionInfo;

	private OptionUI m_optionUI;

	public OptionUI.OptionInfo OptionInfo
	{
		get
		{
			return m_optionInfo;
		}
	}

	private void Start()
	{
		base.enabled = false;
	}

	public void UpdateView(OptionUI.OptionInfo info, OptionUI optionUI)
	{
		m_optionUI = optionUI;
		m_optionInfo = info;
		if (m_optionInfo == null)
		{
			return;
		}
		if (m_imgSprite != null)
		{
			m_imgSprite.spriteName = m_optionInfo.icon;
		}
		if (m_textLabel != null)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Option", m_optionInfo.label);
			if (text != null)
			{
				m_textLabel.text = text.text;
			}
		}
	}

	public void SetEnableImageButton(bool flag)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_option_top");
		if (gameObject != null)
		{
			UIImageButton component = gameObject.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = flag;
			}
		}
	}

	private void SetButtonTrigger(bool flag)
	{
		if (m_optionUI != null)
		{
			m_optionUI.SetButtonTrigger(flag);
		}
	}

	private void OnClickOptionScroll()
	{
		if (BackKeyManager.MenuSequenceTransitionFlag)
		{
			return;
		}
		SoundManager.SePlay("sys_menu_decide");
		SetButtonTrigger(true);
		if (m_optionInfo == null)
		{
			return;
		}
		switch (m_optionInfo.type)
		{
		case OptionType.USER_RESULT:
		{
			OptionUserResult optionUserResult = base.gameObject.GetComponent<OptionUserResult>();
			if (optionUserResult == null)
			{
				optionUserResult = base.gameObject.AddComponent<OptionUserResult>();
			}
			if (optionUserResult != null)
			{
				optionUserResult.Setup(this);
			}
			break;
		}
		case OptionType.PAST_RESULTS:
			AchievementManager.RequestShowAchievementsUI();
			SetButtonTrigger(false);
			break;
		case OptionType.BUYING_HISTORY:
		{
			BuyHistory buyHistory = base.gameObject.GetComponent<BuyHistory>();
			if (buyHistory == null)
			{
				buyHistory = base.gameObject.AddComponent<BuyHistory>();
			}
			if (buyHistory != null)
			{
				buyHistory.Setup(this);
			}
			break;
		}
		case OptionType.SOUND_CONFIG:
		{
			SoundSetting soundSetting = base.gameObject.GetComponent<SoundSetting>();
			if (soundSetting == null)
			{
				soundSetting = base.gameObject.AddComponent<SoundSetting>();
			}
			if (soundSetting != null)
			{
				soundSetting.Setup(this);
			}
			break;
		}
		case OptionType.PUSH_NOTIFICATION:
		{
			OptionPushNotification optionPushNotification = base.gameObject.GetComponent<OptionPushNotification>();
			if (optionPushNotification == null)
			{
				optionPushNotification = base.gameObject.AddComponent<OptionPushNotification>();
			}
			if (optionPushNotification != null)
			{
				optionPushNotification.Setup(this);
			}
			break;
		}
		case OptionType.USER_NAME:
		{
			OptionUserName optionUserName = base.gameObject.GetComponent<OptionUserName>();
			if (optionUserName == null)
			{
				optionUserName = base.gameObject.AddComponent<OptionUserName>();
			}
			if (optionUserName != null)
			{
				optionUserName.Setup(this);
			}
			break;
		}
		case OptionType.ID_CHECK:
		{
			CheckID checkID = base.gameObject.GetComponent<CheckID>();
			if (checkID == null)
			{
				checkID = base.gameObject.AddComponent<CheckID>();
			}
			if (checkID != null)
			{
				checkID.Setup(this);
			}
			break;
		}
		case OptionType.INVITE_FRIEND:
		{
			OptionFriendInvitation optionFriendInvitation = base.gameObject.GetComponent<OptionFriendInvitation>();
			if (optionFriendInvitation == null)
			{
				optionFriendInvitation = base.gameObject.AddComponent<OptionFriendInvitation>();
			}
			if (optionFriendInvitation != null)
			{
				optionFriendInvitation.Setup(this);
			}
			break;
		}
		case OptionType.ACCEPT_INVITE:
		{
			OptionAcceptInvite optionAcceptInvite = base.gameObject.GetComponent<OptionAcceptInvite>();
			if (optionAcceptInvite == null)
			{
				optionAcceptInvite = base.gameObject.AddComponent<OptionAcceptInvite>();
			}
			if (optionAcceptInvite != null)
			{
				optionAcceptInvite.Setup(this);
			}
			break;
		}
		case OptionType.WEIGHT_SAVING:
		{
			WeightSaving weightSaving = base.gameObject.GetComponent<WeightSaving>();
			if (weightSaving == null)
			{
				weightSaving = base.gameObject.AddComponent<WeightSaving>();
			}
			if (weightSaving != null)
			{
				weightSaving.Setup(this);
			}
			break;
		}
		case OptionType.FACEBOOK_ACCESS:
		{
			OptionFacebookAccess optionFacebookAccess = base.gameObject.GetComponent<OptionFacebookAccess>();
			if (optionFacebookAccess == null)
			{
				optionFacebookAccess = base.gameObject.AddComponent<OptionFacebookAccess>();
			}
			if (optionFacebookAccess != null)
			{
				optionFacebookAccess.Setup(this);
			}
			break;
		}
		case OptionType.CACHE_CLEAR:
		{
			OptionCacheClear optionCacheClear = base.gameObject.GetComponent<OptionCacheClear>();
			if (optionCacheClear == null)
			{
				optionCacheClear = base.gameObject.AddComponent<OptionCacheClear>();
			}
			if (optionCacheClear != null)
			{
				optionCacheClear.Setup(this);
			}
			break;
		}
		case OptionType.TUTORIAL:
		{
			OptionTutorial optionTutorial = base.gameObject.GetComponent<OptionTutorial>();
			if (optionTutorial == null)
			{
				optionTutorial = base.gameObject.AddComponent<OptionTutorial>();
			}
			if (optionTutorial != null)
			{
				optionTutorial.Setup(this);
			}
			break;
		}
		case OptionType.STAFF_CREDIT:
		{
			StaffRoll staffRoll2 = base.gameObject.GetComponent<StaffRoll>();
			if (staffRoll2 == null)
			{
				staffRoll2 = base.gameObject.AddComponent<StaffRoll>();
			}
			if (staffRoll2 != null)
			{
				staffRoll2.SetTextType(StaffRoll.TextType.STAFF_ROLL);
				staffRoll2.Setup(this);
			}
			break;
		}
		case OptionType.HELP:
		{
			OptionWebJump optionWebJump2 = base.gameObject.GetComponent<OptionWebJump>();
			if (optionWebJump2 == null)
			{
				optionWebJump2 = base.gameObject.AddComponent<OptionWebJump>();
			}
			if (optionWebJump2 != null)
			{
				optionWebJump2.Setup(this, OptionWebJump.WebType.HELP);
			}
			break;
		}
		case OptionType.TERMS_OF_SERVICE:
		{
			OptionWebJump optionWebJump = base.gameObject.GetComponent<OptionWebJump>();
			if (optionWebJump == null)
			{
				optionWebJump = base.gameObject.AddComponent<OptionWebJump>();
			}
			if (optionWebJump != null)
			{
				optionWebJump.Setup(this, OptionWebJump.WebType.TERMS_OF_SERVICE);
			}
			break;
		}
		case OptionType.COPYRIGHT:
		{
			StaffRoll staffRoll = base.gameObject.GetComponent<StaffRoll>();
			if (staffRoll == null)
			{
				staffRoll = base.gameObject.AddComponent<StaffRoll>();
			}
			if (staffRoll != null)
			{
				staffRoll.SetTextType(StaffRoll.TextType.COPYRIGHT);
				staffRoll.Setup(this);
			}
			break;
		}
		case OptionType.BACK_TITLE:
		{
			OptionBackTitle optionBackTitle = base.gameObject.GetComponent<OptionBackTitle>();
			if (optionBackTitle == null)
			{
				optionBackTitle = base.gameObject.AddComponent<OptionBackTitle>();
			}
			if (optionBackTitle != null)
			{
				optionBackTitle.Setup(this);
			}
			break;
		}
		}
	}

	public void OnEndChildPage()
	{
		SetButtonTrigger(false);
	}

	public void SetSystemSaveFlag()
	{
		if (m_optionUI != null)
		{
			m_optionUI.SystemSaveFlag = true;
		}
	}

	public void ResetSystemSaveFlag()
	{
		if (m_optionUI != null)
		{
			m_optionUI.SystemSaveFlag = false;
		}
	}
}
