using System.Collections.Generic;
using UnityEngine;

public class HudHeaderPresentBox : MonoBehaviour
{
	private enum DataType
	{
		PRESEN_BOX,
		VOLUME_LABEL
	}

	private const string m_present_box_path = "Anchor_7_BL/Btn_2_presentbox";

	private GameObject m_present_box_badge;

	private UILabel m_volume_label;

	private BoxCollider m_collider;

	private UIImageButton m_image_button;

	private bool m_initEnd;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (mainMenuUIObject != null)
		{
			GameObject gameObject = mainMenuUIObject.transform.FindChild("Anchor_7_BL/Btn_2_presentbox").gameObject;
			if (gameObject != null)
			{
				m_present_box_badge = gameObject.transform.FindChild("badge").gameObject;
				if (m_present_box_badge != null)
				{
					GameObject gameObject2 = m_present_box_badge.transform.FindChild("Lbl_present_volume").gameObject;
					if (gameObject2 != null)
					{
						m_volume_label = gameObject2.GetComponent<UILabel>();
					}
				}
				m_collider = gameObject.GetComponent<BoxCollider>();
				m_image_button = gameObject.GetComponent<UIImageButton>();
			}
		}
		m_initEnd = true;
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
		int num = 0;
		if (ServerInterface.MessageList != null)
		{
			List<ServerMessageEntry> messageList = ServerInterface.MessageList;
			foreach (ServerMessageEntry item in messageList)
			{
				if (PresentBoxUtility.IsWithinTimeLimit(item.m_expireTiem))
				{
					num++;
				}
			}
		}
		if (ServerInterface.OperatorMessageList != null)
		{
			List<ServerOperatorMessageEntry> operatorMessageList = ServerInterface.OperatorMessageList;
			foreach (ServerOperatorMessageEntry item2 in operatorMessageList)
			{
				if (PresentBoxUtility.IsWithinTimeLimit(item2.m_expireTiem))
				{
					num++;
				}
			}
		}
		if (m_volume_label != null)
		{
			m_volume_label.text = num.ToString();
		}
		if (num == 0)
		{
			if (m_present_box_badge != null)
			{
				m_present_box_badge.SetActive(false);
			}
			if (m_collider != null)
			{
				m_collider.isTrigger = true;
			}
			if (m_image_button != null)
			{
				m_image_button.isEnabled = false;
			}
		}
		else
		{
			if (m_present_box_badge != null)
			{
				m_present_box_badge.SetActive(true);
			}
			if (m_collider != null)
			{
				m_collider.isTrigger = false;
			}
			if (m_image_button != null)
			{
				m_image_button.isEnabled = true;
			}
		}
	}
}
