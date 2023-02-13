using Text;
using UnityEngine;

public class HudQuestionButton : MonoBehaviour
{
	private bool m_isQuickMode;

	public void Initialize(bool isQuickMode)
	{
		m_isQuickMode = isQuickMode;
		if (isQuickMode)
		{
			UIButtonMessage uIButtonMessage = null;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_5_MC");
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "1_Quick");
			if (!(gameObject2 == null))
			{
				uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject2, "Btn_0_help");
				if (!(uIButtonMessage == null))
				{
					uIButtonMessage.target = base.gameObject;
					uIButtonMessage.functionName = "QuickModeQuestionButtonClicked";
				}
			}
			return;
		}
		UIButtonMessage uIButtonMessage2 = null;
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_5_MC");
		if (gameObject3 == null)
		{
			return;
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject3, "0_Endless");
		if (!(gameObject4 == null))
		{
			uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject4, "Btn_0_help");
			if (!(uIButtonMessage2 == null))
			{
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "EndlessModeQuestionButtonClicked";
			}
		}
	}

	private void QuickModeQuestionButtonClicked()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "QuickModeQuestion";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "help_quick_mode_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "help_quick_mode_text").text;
		GeneralWindow.Create(info);
	}

	private void EndlessModeQuestionButtonClicked()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "EndlessModeQuestion";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "help_episode_mode_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "help_episode_mode_text").text;
		GeneralWindow.Create(info);
	}

	private void Start()
	{
	}

	private void Update()
	{
		bool flag = false;
		if (GeneralWindow.IsCreated("QuickModeQuestion"))
		{
			flag = true;
		}
		else if (GeneralWindow.IsCreated("QuickModeQuestion"))
		{
			flag = true;
		}
		if (flag && GeneralWindow.IsOkButtonPressed)
		{
			GeneralWindow.Close();
		}
	}
}
