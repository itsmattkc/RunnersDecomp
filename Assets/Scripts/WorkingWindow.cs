using Message;
using UnityEngine;

public class WorkingWindow : MonoBehaviour
{
	private GameObject m_window_obj;

	private bool m_update_flag;

	private void Start()
	{
	}

	public void CreateWindow(string caption)
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = caption;
		info.message = "         CLOSED \n DUE TO CONSTRUCTION";
		info.anchor_path = "Camera/menu_Anim/MainMenuUI4/Anchor_5_MC";
		m_window_obj = GeneralWindow.Create(info);
		m_update_flag = true;
	}

	public void Update()
	{
		if (m_update_flag && m_window_obj != null && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			m_window_obj = null;
			m_update_flag = false;
			SendMessage();
		}
	}

	public void SendMessage()
	{
		MsgMenuSequence msgMenuSequence = new MsgMenuSequence(MsgMenuSequence.SequeneceType.MAIN);
		if (msgMenuSequence != null)
		{
			GameObjectUtil.SendMessageFindGameObject("MainMenu", "OnMsgReceive", msgMenuSequence, SendMessageOptions.DontRequireReceiver);
		}
	}
}
