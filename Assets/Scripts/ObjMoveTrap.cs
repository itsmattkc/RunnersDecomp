using GameScore;
using Tutorial;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjMoveTrap")]
public class ObjMoveTrap : ObjTrapBase
{
	private enum ModelType
	{
		Missile,
		Boo,
		NUM,
		NONE
	}

	private class ModelParam
	{
		public string m_modelName;

		public string m_seName;

		public string m_effectName;

		public ModelParam(string modelName, string seName, string effectName)
		{
			m_modelName = modelName;
			m_seName = seName;
			m_effectName = effectName;
		}
	}

	private static readonly ModelParam[] MODEL_PARAMS = new ModelParam[2]
	{
		new ModelParam("obj_cmn_movetrap", "obj_missile_shoot", "ef_com_explosion_m01"),
		new ModelParam("obj_cmn_boomboo", "obj_ghost_s", "ef_com_explosion_m01")
	};

	private ObjMoveTrapParameter m_param;

	private LevelInformation m_levelInformation;

	private ModelParam m_modelParam;

	protected override string GetModelName()
	{
		SetupParam();
		return m_modelParam.m_modelName;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override int GetScore()
	{
		return Data.MoveTrap;
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
		if (IsCreateCheck())
		{
			base.enabled = false;
			SetupParam();
			base.OnSpawned();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetObjMoveTrapParameter(ObjMoveTrapParameter param)
	{
		if (IsCreateCheck())
		{
			SetupParam();
			m_param = param;
			MotorConstant component = GetComponent<MotorConstant>();
			if ((bool)component)
			{
				component.SetParam(m_param.moveSpeed, m_param.moveDistance, m_param.startMoveDistance, -base.transform.right, "SE", m_modelParam.m_seName);
			}
		}
	}

	protected override void PlayEffect()
	{
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, m_modelParam.m_effectName, 1f);
	}

	protected override void TrapDamageHit()
	{
		SetBroken();
	}

	private bool IsTutorialCheck()
	{
		if (StageTutorialManager.Instance != null && !StageTutorialManager.Instance.IsCompletedTutorial())
		{
			return true;
		}
		return false;
	}

	private bool IsCreateCheck()
	{
		if (!ObjUtil.IsUseTemporarySet() && StageTutorialManager.Instance != null)
		{
			if (StageTutorialManager.Instance.IsCompletedTutorial())
			{
				return true;
			}
			EventID currentEventID = StageTutorialManager.Instance.CurrentEventID;
			if (currentEventID != EventID.DAMAGE)
			{
				return false;
			}
		}
		return true;
	}

	private void SetupParam()
	{
		if (m_levelInformation == null)
		{
			m_levelInformation = ObjUtil.GetLevelInformation();
		}
		if (m_modelParam != null)
		{
			return;
		}
		m_modelParam = MODEL_PARAMS[0];
		if (m_levelInformation != null && !IsTutorialCheck())
		{
			int randomRange = ObjUtil.GetRandomRange100();
			if (randomRange < m_levelInformation.MoveTrapBooRand)
			{
				m_modelParam = MODEL_PARAMS[1];
			}
		}
	}
}
