using Message;
using Text;
using UnityEngine;

public class OptionCacheClear : MonoBehaviour
{
	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		CreateCacheClearWindow();
	}

	private void CreateCacheClearWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "cache_clear";
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_bar");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_explanation");
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
	}

	public void Update()
	{
		bool flag = false;
		if (GeneralWindow.IsCreated("cache_clear") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralUtil.CleanAllCache();
				GeneralWindow.Close();
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.name = "cache_clear_end";
				info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_confirmation_bar");
				info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_confirmation");
				info.anchor_path = "Camera/Anchor_5_MC";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				GeneralWindow.Create(info);
			}
			else
			{
				flag = true;
				GeneralWindow.Close();
			}
		}
		if (GeneralWindow.IsCreated("cache_clear_end") && GeneralWindow.IsOkButtonPressed)
		{
			GeneralWindow.Close();
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.TITLE);
		}
		if (flag)
		{
			if (m_ui_option_scroll != null)
			{
				m_ui_option_scroll.OnEndChildPage();
			}
			base.enabled = false;
		}
	}
}
