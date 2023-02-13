using Message;
using System.Collections.Generic;
using UnityEngine;

public class ObjCrystalBase : SpawnableObject
{
	private PlayerInformation m_playerInfo;

	private CtystalType m_type = CtystalType.NONE;

	private bool m_end;

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override string GetModelName()
	{
		if (m_type == CtystalType.NONE)
		{
			m_type = GetCtystalModelType();
		}
		CtystalParam crystalParam = ObjCrystalData.GetCrystalParam(m_type);
		if (crystalParam != null)
		{
			return crystalParam.m_model;
		}
		return string.Empty;
	}

	protected override void OnSpawned()
	{
		base.enabled = false;
		m_playerInfo = ObjUtil.GetPlayerInformation();
		if (m_type == CtystalType.NONE)
		{
			m_type = GetCtystalModelType();
		}
		bool flag = ObjCrystalData.IsBig(GetOriginalType());
		bool flag2 = ObjCrystalData.IsBig(m_type);
		if (flag2 && flag != flag2)
		{
			SphereCollider component = GetComponent<SphereCollider>();
			if (component != null)
			{
				Vector3 center = component.center;
				float x = center.x;
				Vector3 center2 = component.center;
				float y = center2.y + 0.15f;
				Vector3 center3 = component.center;
				component.center = new Vector3(x, y, center3.z);
				component.radius += 0.1f;
			}
		}
		CheckActiveComboChaoAbility();
	}

	private CtystalType GetCtystalModelType()
	{
		return ObjCrystalUtil.GetCrystalModelType(GetOriginalType());
	}

	protected virtual CtystalType GetOriginalType()
	{
		return CtystalType.SMALL_A;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_end || !other)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if (!gameObject)
		{
			return;
		}
		string a = LayerMask.LayerToName(gameObject.layer);
		if (a == "Player")
		{
			if (gameObject.tag != "ChaoAttack")
			{
				TakeCrystal();
			}
		}
		else if (a == "HitCrystal" && gameObject.tag == "Chao")
		{
			TakeCrystal();
		}
	}

	private void OnDrawingRings(MsgOnDrawingRings msg)
	{
		ObjUtil.StartMagnetControl(base.gameObject);
	}

	private void OnDrawingRingsToChao(MsgOnDrawingRings msg)
	{
		if (msg != null)
		{
			ObjUtil.StartMagnetControl(base.gameObject, msg.m_target);
		}
	}

	private void OnDrawingRingsChaoAbility(MsgOnDrawingRings msg)
	{
		if (msg.m_chaoAbility == ChaoAbility.COMBO_RECOVERY_ALL_OBJ || msg.m_chaoAbility == ChaoAbility.COMBO_DESTROY_AND_RECOVERY)
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
	}

	private EffectPlayType GetEffectType()
	{
		switch (m_type)
		{
		case CtystalType.SMALL_A:
			return EffectPlayType.CRYSTAL_A;
		case CtystalType.SMALL_B:
			return EffectPlayType.CRYSTAL_B;
		case CtystalType.SMALL_C:
			return EffectPlayType.CRYSTAL_C;
		case CtystalType.BIG_A:
			return EffectPlayType.CRYSTAL_BIG_A;
		case CtystalType.BIG_B:
			return EffectPlayType.CRYSTAL_BIG_B;
		case CtystalType.BIG_C:
			return EffectPlayType.CRYSTAL_BIG_C;
		default:
			return EffectPlayType.CRYSTAL_A;
		}
	}

	private void TakeCrystal()
	{
		m_end = true;
		CtystalParam crystalParam = ObjCrystalData.GetCrystalParam(m_type);
		if (crystalParam != null)
		{
			SetChaoAbliltyScoreEffect(m_playerInfo, crystalParam, base.gameObject);
			if (StageEffectManager.Instance != null)
			{
				StageEffectManager.Instance.PlayEffect(GetEffectType(), ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
			}
			ObjUtil.LightPlaySE(crystalParam.m_se);
			HudTutorial.SendActionTutorial(HudTutorial.Id.ACTION_1);
			if (base.Share)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public static void SetChaoAbliltyScoreEffect(PlayerInformation playerInfo, CtystalParam ctystalParam, GameObject obj)
	{
		if (ctystalParam != null)
		{
			List<ChaoAbility> abilityList = new List<ChaoAbility>();
			ObjUtil.GetChaoAbliltyPhantomFlag(playerInfo, ref abilityList);
			int chaoAbliltyScore = ObjUtil.GetChaoAbliltyScore(abilityList, ctystalParam.m_score);
			ObjUtil.SendMessageAddScore(chaoAbliltyScore);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(2, chaoAbliltyScore));
			ObjUtil.AddCombo();
		}
	}

	public void CheckActiveComboChaoAbility()
	{
		if (StageComboManager.Instance != null && (StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
	}

	public override void OnRevival()
	{
		MagnetControl component = GetComponent<MagnetControl>();
		if (component != null)
		{
			component.Reset();
		}
		SphereCollider component2 = GetComponent<SphereCollider>();
		if ((bool)component2)
		{
			component2.enabled = true;
		}
		BoxCollider component3 = GetComponent<BoxCollider>();
		if ((bool)component3)
		{
			component3.enabled = true;
		}
		CheckActiveComboChaoAbility();
		m_end = false;
	}
}
