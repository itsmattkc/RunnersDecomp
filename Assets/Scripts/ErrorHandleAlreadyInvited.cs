using Message;
using Text;
using UnityEngine;

public class ErrorHandleAlreadyInvited : ErrorHandleBase
{
	private bool m_isEnd;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Option", "accepted_invite_caption").text;
		string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Option", "accepted_invite_text").text;
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = text;
		info.message = text2;
		NetworkErrorWindow.Create(info);
		m_isEnd = false;
	}

	public override void Update()
	{
		if (NetworkErrorWindow.IsOkButtonPressed)
		{
			NetworkErrorWindow.Close();
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
