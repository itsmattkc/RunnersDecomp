using Message;
using UnityEngine;

public class ObjBossBase : SpawnableObject
{
	protected override void OnSpawned()
	{
		if (ObjBossUtil.IsNowLastChance(ObjUtil.GetPlayerInformation()))
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SetOnlyOneObject();
		SetNotRageout(true);
	}

	private void OnMsgBossInfo(MsgBossInfo msg)
	{
		msg.m_boss = base.gameObject;
		msg.m_position = base.transform.position;
		msg.m_rotation = base.transform.rotation;
		msg.m_succeed = true;
	}
}
