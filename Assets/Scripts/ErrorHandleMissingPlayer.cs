using Message;
using Text;
using UnityEngine;

public class ErrorHandleMissingPlayer : ErrorHandleBase
{
	private bool m_isEnd;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_missing_player").text;
		NetworkErrorWindow.Create(info);
	}

	public override void Update()
	{
		if (NetworkErrorWindow.IsOkButtonPressed)
		{
			NetworkErrorWindow.Close();
			HudMenuUtility.GoToTitleScene();
			m_isEnd = true;
		}
	}

	public override bool IsEnd()
	{
		return m_isEnd;
	}

	public override void EndErrorHandle()
	{
	}
}
