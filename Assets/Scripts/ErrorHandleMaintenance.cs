using Message;
using Text;
using UnityEngine;

public class ErrorHandleMaintenance : ErrorHandleBase
{
	private bool m_isEnd;

	private bool m_isExistMaintenancePage;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		m_isExistMaintenancePage = false;
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = (m_isExistMaintenancePage ? NetworkErrorWindow.ButtonType.HomePage : NetworkErrorWindow.ButtonType.Ok);
		info.anchor_path = string.Empty;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_maintenance").text;
		NetworkErrorWindow.Create(info);
	}

	public override void Update()
	{
		if (m_isExistMaintenancePage)
		{
			if (NetworkErrorWindow.IsYesButtonPressed)
			{
				NetworkErrorWindow.Close();
				HudMenuUtility.GoToTitleScene();
				string maintenancePageURL = ErrorHandleMaintenanceUtil.GetMaintenancePageURL();
				Application.OpenURL(maintenancePageURL);
				m_isEnd = true;
			}
			else if (NetworkErrorWindow.IsNoButtonPressed)
			{
				NetworkErrorWindow.Close();
				HudMenuUtility.GoToTitleScene();
				m_isEnd = true;
			}
		}
		else if (NetworkErrorWindow.IsOkButtonPressed)
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
