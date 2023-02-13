using UnityEngine;

public class PlayerSetWindowUIWithButton : PlayerSetWindowUI
{
	private GameObject m_btnMain;

	private GameObject m_btnSub;

	private bool m_isClickCancel;

	private bool m_isClickMain;

	private bool m_isClickSub;

	public bool isClickCancel
	{
		get
		{
			return m_isClickCancel;
		}
	}

	public bool isClickMain
	{
		get
		{
			return m_isClickMain;
		}
	}

	public bool isClickSub
	{
		get
		{
			return m_isClickSub;
		}
	}

	public void SetupWithButton(CharaType charaType)
	{
		Setup(charaType);
		m_btnMain = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_main");
		m_btnSub = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_sub");
		SetupButton(m_btnMain, base.gameObject, "OnClickMainButton");
		SetupButton(m_btnSub, base.gameObject, "OnClickSubButton");
		m_isClickCancel = false;
		m_isClickMain = false;
		m_isClickSub = false;
	}

	private void OnClickBtn()
	{
		m_isClickCancel = true;
	}

	private void OnClickMainButton()
	{
		m_isClickMain = true;
	}

	private void OnClickSubButton()
	{
		m_isClickSub = true;
	}

	public static PlayerSetWindowUIWithButton Create(CharaType charaType)
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "PlayerSetWindowUI");
			PlayerSetWindowUIWithButton playerSetWindowUIWithButton = null;
			if (gameObject != null)
			{
				playerSetWindowUIWithButton = gameObject.GetComponent<PlayerSetWindowUIWithButton>();
				if (playerSetWindowUIWithButton == null)
				{
					playerSetWindowUIWithButton = gameObject.AddComponent<PlayerSetWindowUIWithButton>();
				}
				if (playerSetWindowUIWithButton != null)
				{
					playerSetWindowUIWithButton.SetupWithButton(charaType);
				}
			}
			return playerSetWindowUIWithButton;
		}
		return null;
	}

	private static void SetupButton(GameObject buttonObject, GameObject targetObject, string functionName)
	{
		if (!(buttonObject == null))
		{
			buttonObject.SetActive(true);
			UIButtonMessage component = buttonObject.GetComponent<UIButtonMessage>();
			if (component == null)
			{
				component = buttonObject.AddComponent<UIButtonMessage>();
				component.target = targetObject;
				component.functionName = functionName;
			}
		}
	}
}
