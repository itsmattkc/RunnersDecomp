using Message;
using System.Collections.Generic;
using UnityEngine;

public class HudAlert : MonoBehaviour
{
	private const float IconDisplayTime = 1f;

	private List<HudAlertIcon> m_iconList;

	private Camera m_camera;

	public void StartAlert(GameObject chaseObject)
	{
		HudAlertIcon hudAlertIcon = base.gameObject.AddComponent<HudAlertIcon>();
		hudAlertIcon.Setup(m_camera, chaseObject, 1f);
		m_iconList.Add(hudAlertIcon);
	}

	public void EndAlert(GameObject chaseObject)
	{
		HudAlertIcon hudAlertIcon = null;
		foreach (HudAlertIcon icon in m_iconList)
		{
			if (!(icon == null) && icon.IsChasingObject(chaseObject))
			{
				hudAlertIcon = icon;
			}
		}
		if (hudAlertIcon != null)
		{
			m_iconList.Remove(hudAlertIcon);
			Object.Destroy(hudAlertIcon);
		}
	}

	private void Start()
	{
		m_iconList = new List<HudAlertIcon>();
		GameObject gameObject = GameObject.Find("GameMainCamera");
		if (gameObject != null)
		{
			m_camera = gameObject.GetComponent<Camera>();
			if (!(m_camera == null))
			{
			}
		}
	}

	private void Update()
	{
		if (m_iconList.Count <= 0)
		{
			return;
		}
		List<HudAlertIcon> list = new List<HudAlertIcon>();
		foreach (HudAlertIcon icon in m_iconList)
		{
			if (!(icon == null) && icon.IsEnd)
			{
				list.Add(icon);
			}
		}
		foreach (HudAlertIcon item in list)
		{
			if (!(item == null))
			{
				m_iconList.Remove(item);
				Object.Destroy(item);
			}
		}
		list.Clear();
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}
}
