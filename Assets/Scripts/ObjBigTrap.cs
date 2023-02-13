using GameScore;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjBigTrap")]
public class ObjBigTrap : ObjTrapBase
{
	private const string ModelName = "obj_cmn_boomboo_L";

	private ObjBigTrapParameter m_param;

	private PlayerInformation m_playerInfo;

	private bool m_start;

	private uint m_move_SEID;

	protected override string GetModelName()
	{
		return "obj_cmn_boomboo_L";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override int GetScore()
	{
		return Data.BigTrap;
	}

	private void StopSE()
	{
		if (m_move_SEID != 0)
		{
			ObjUtil.StopSE((SoundManager.PlayId)m_move_SEID);
			m_move_SEID = 0u;
		}
	}

	private void Update()
	{
		if (m_start)
		{
			return;
		}
		if (m_playerInfo == null)
		{
			m_playerInfo = ObjUtil.GetPlayerInformation();
		}
		if (!(m_playerInfo != null) || m_param == null)
		{
			return;
		}
		float playerDistance = GetPlayerDistance();
		if (!(playerDistance < m_param.startMoveDistance))
		{
			return;
		}
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.SetupParam(m_param.moveSpeedY, m_param.moveDistanceY, m_param.moveSpeedX, base.transform.right, 0f, false);
			if (m_move_SEID == 0)
			{
				m_move_SEID = (uint)ObjUtil.LightPlaySE("obj_ghost_l");
			}
			m_start = true;
		}
	}

	private float GetPlayerDistance()
	{
		if ((bool)m_playerInfo)
		{
			Vector3 position = base.transform.position;
			return Mathf.Abs(Vector3.Distance(position, m_playerInfo.Position));
		}
		return 0f;
	}

	public void SetObjBigTrapParameter(ObjBigTrapParameter param)
	{
		m_param = param;
	}

	protected override void PlayEffect()
	{
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, "ef_com_explosion_l01", 1f);
	}

	protected override void TrapDamageHit()
	{
		StopSE();
		SetBroken();
	}
}
