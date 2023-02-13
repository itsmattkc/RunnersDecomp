using UnityEngine;

public class RouletteInformationUtility
{
	public static void ShowNewsWindow(InformationWindow.Information informationParam)
	{
		if (string.IsNullOrEmpty(informationParam.imageId))
		{
			return;
		}
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (!(cameraUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "NewsWindow");
		if (gameObject != null)
		{
			SoundManager.SePlay("sys_menu_decide");
			InformationWindow informationWindow = gameObject.GetComponent<InformationWindow>();
			if (informationWindow == null)
			{
				informationWindow = gameObject.AddComponent<InformationWindow>();
			}
			if (informationWindow != null)
			{
				informationWindow.Create(informationParam, gameObject);
			}
		}
	}
}
