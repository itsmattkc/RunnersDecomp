using GameScore;
using Message;
using UnityEngine;

public class ObjDashRing : SpawnableObject
{
	private float m_firstSpeed = 8f;

	private float m_outOfcontrol = 0.5f;

	protected override string GetModelName()
	{
		return "obj_cmn_dashring";
	}

	protected virtual string GetEffectName()
	{
		return "ef_ob_pass_dashring01";
	}

	protected virtual string GetSEName()
	{
		return "obj_dashring";
	}

	protected virtual int GetScore()
	{
		return Data.DashRing;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
	}

	public void SetObjDashRingParameter(ObjDashRingParameter param)
	{
		m_firstSpeed = param.firstSpeed;
		m_outOfcontrol = param.outOfcontrol;
	}

	private void OnTriggerEnter(Collider other)
	{
		Quaternion rot = Quaternion.FromToRotation(base.transform.up, base.transform.right) * base.transform.rotation;
		MsgOnDashRingImpulse msgOnDashRingImpulse = new MsgOnDashRingImpulse(base.transform.position, rot, m_firstSpeed, m_outOfcontrol);
		other.gameObject.SendMessage("OnDashRingImpulse", msgOnDashRingImpulse, SendMessageOptions.DontRequireReceiver);
		if (msgOnDashRingImpulse.m_succeed)
		{
			ObjUtil.SendMessageAddScore(GetScore());
			ObjUtil.SendMessageScoreCheck(new StageScoreData(4, GetScore()));
			ObjUtil.PlayEffect(GetEffectName(), base.transform.position, base.transform.rotation, 1f);
			ObjUtil.PlaySE(GetSEName());
		}
	}
}
