using UnityEngine;

public class MenuPlayerSetGripL : MenuPlayerSetPartsBase
{
	public delegate void ButtonClickCallback();

	private ButtonClickCallback m_callback;

	public MenuPlayerSetGripL()
		: base("player_set_grip_L")
	{
	}

	public void SetCallback(ButtonClickCallback callback)
	{
		m_callback = callback;
	}

	public void SetDisplayFlag(bool flag)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_grip_L");
		if (gameObject != null)
		{
			gameObject.SetActive(flag);
		}
	}

	protected override void OnSetup()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_grip_L");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "GripLClickCallback";
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

	private void GripLClickCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}
}
