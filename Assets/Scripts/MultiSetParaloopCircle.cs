using Message;
using UnityEngine;

public class MultiSetParaloopCircle : MultiSetBase
{
	protected override void OnSpawned()
	{
		base.OnSpawned();
	}

	protected override void OnCreateSetup()
	{
		SetActiveParaloopComponent(false);
	}

	public void SucceedParaloop()
	{
		if (IsNowParaloop())
		{
			SetActiveParaloopComponent(true);
			SetStartMagnet();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if ((bool)gameObject)
		{
			string a = LayerMask.LayerToName(gameObject.layer);
			if (a == "Player")
			{
				SucceedParaloop();
				ObjUtil.PopCamera(CameraType.LOOP_TERRAIN, 2.5f);
			}
			else if (!(a == "Magnet"))
			{
			}
		}
	}

	private void SetActiveParaloopComponent(bool flag)
	{
		for (int i = 0; i < m_dataList.Count; i++)
		{
			GameObject obj = m_dataList[i].m_obj;
			if ((bool)obj)
			{
				MagnetControl component = obj.GetComponent<MagnetControl>();
				if ((bool)component)
				{
					component.enabled = flag;
				}
				SphereCollider component2 = obj.GetComponent<SphereCollider>();
				if ((bool)component2)
				{
					component2.enabled = flag;
				}
				BoxCollider component3 = obj.GetComponent<BoxCollider>();
				if ((bool)component3)
				{
					component3.enabled = flag;
				}
			}
		}
	}

	private void SetStartMagnet()
	{
		for (int i = 0; i < m_dataList.Count; i++)
		{
			GameObject obj = m_dataList[i].m_obj;
			if ((bool)obj)
			{
				MsgOnDrawingRings value = new MsgOnDrawingRings();
				obj.SendMessage("OnDrawingRings", value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static bool IsNowParaloop()
	{
		PlayerInformation playerInformation = ObjUtil.GetPlayerInformation();
		if (playerInformation != null && playerInformation.IsNowParaloop())
		{
			return true;
		}
		return false;
	}
}
