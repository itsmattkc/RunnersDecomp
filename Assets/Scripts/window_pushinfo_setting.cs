using UnityEngine;

public class window_pushinfo_setting : WindowBase
{
	private void Start()
	{
		base.enabled = false;
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}

	private void OnDestroy()
	{
		Destroy();
	}
}
