using Message;
using Text;
using UnityEngine;

public class OptionBackTitle : MonoBehaviour
{
	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		CreateBackTitleWindow();
	}

	private void CreateBackTitleWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "BackTitle";
		info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
		info.message = TextUtility.GetCommonText("MainMenu", "back_title_text");
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
	}

	public void Update()
	{
		bool flag = false;
		if (GeneralWindow.IsCreated("BackTitle") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.TITLE);
			}
			else
			{
				flag = true;
				GeneralWindow.Close();
			}
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
