using UnityEngine;

public class MenuPlayerSetGripR : MenuPlayerSetPartsBase
{
	public delegate void ButtonClickCallback();

	private ButtonClickCallback m_callback;

	public MenuPlayerSetGripR()
		: base("player_set_grip_R")
	{
	}

	public void SetCallback(ButtonClickCallback callback)
	{
		m_callback = callback;
	}

	public void SetDisplayFlag(bool flag)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_grip_R");
		if (gameObject != null)
		{
			if (!gameObject.activeSelf && flag)
			{
				gameObject.SetActive(true);
			}
			else if (gameObject.activeSelf && !flag)
			{
				gameObject.SetActive(false);
			}
		}
	}

	protected override void OnSetup()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_grip_R");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "GripRClickCallback";
		}
	}

	protected override void OnPlayStart()
	{
	}

	protected override void OnPlayEnd()
	{
	}

	protected override void OnUpdate(float deltaTime)
	{
	}

	private void GripRClickCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}
}
