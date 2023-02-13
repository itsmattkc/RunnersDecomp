using GameScore;
using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjAirTrap")]
public class ObjAirTrap : ObjTrapBase
{
	private const string ModelName = "obj_cmn_airtrap";

	private ObjAirTrapParameter m_param;

	protected override string GetModelName()
	{
		return "obj_cmn_airtrap";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override int GetScore()
	{
		return Data.AirTrap;
	}

	public override bool IsValid()
	{
		if (StageModeManager.Instance != null)
		{
			return !StageModeManager.Instance.IsQuickMode();
		}
		return true;
	}

	protected override void OnSpawned()
	{
		base.OnSpawned();
		if (StageComboManager.Instance != null && StageComboManager.Instance.IsChaoFlagStatus(StageComboManager.ChaoFlagStatus.DESTROY_AIRTRAP))
		{
			SetBroken();
		}
		base.enabled = false;
	}

	public void OnMsgObjectDeadChaoCombo(MsgObjectDead msg)
	{
		if (msg.m_chaoAbility == ChaoAbility.COMBO_DESTROY_AIR_TRAP)
		{
			SetBroken();
		}
	}

	public void SetObjAirTrapParameter(ObjAirTrapParameter param)
	{
		m_param = param;
		MotorSwing component = GetComponent<MotorSwing>();
		if ((bool)component)
		{
			component.SetParam(m_param.moveSpeed, m_param.moveDistanceX, m_param.moveDistanceY, base.transform.right);
		}
	}

	protected override void PlayEffect()
	{
		if (StageEffectManager.Instance != null)
		{
			StageEffectManager.Instance.PlayEffect(EffectPlayType.AIR_TRAP, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
	}

	public override void OnRevival()
	{
		SphereCollider component = GetComponent<SphereCollider>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		base.enabled = true;
		m_end = false;
		OnSpawned();
	}
}
