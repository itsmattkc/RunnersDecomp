using Message;
using System.Collections.Generic;
using UnityEngine;

public class ObjEventCrystalBase : SpawnableObject
{
	private PlayerInformation m_playerInfo;

	private bool m_end;

	private int DRILL_UP_COUNT = 3;

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.EVENT_RESOURCE;
	}

	protected override void OnSpawned()
	{
		base.enabled = false;
		m_playerInfo = ObjUtil.GetPlayerInformation();
		CheckActiveComboChaoAbility();
	}

	private EventCtystalType GetCtystalType()
	{
		return GetOriginalType();
	}

	protected virtual int GetAddCount()
	{
		return 0;
	}

	protected virtual EventCtystalType GetOriginalType()
	{
		return EventCtystalType.SMALL;
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

	private void TakeCrystal()
	{
		m_end = true;
		CtystalParam ctystalParam = ObjEventCrystalData.GetCtystalParam(GetCtystalType());
		if (ctystalParam != null)
		{
			ObjUtil.SendMessageAddSpecialCrystal(GetSrytalCount());
			SetChaoAbliltyScoreEffect(m_playerInfo, ctystalParam, base.gameObject);
			if (StageEffectManager.Instance != null)
			{
				StageEffectManager.Instance.PlayEffect((!ctystalParam.m_big) ? EffectPlayType.CRYSTAL_C : EffectPlayType.CRYSTAL_BIG_C, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
			}
			ObjUtil.LightPlaySE(ctystalParam.m_se);
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

	private int GetSrytalCount()
	{
		if (m_playerInfo != null && m_playerInfo.PhantomType == PhantomType.DRILL)
		{
			return GetAddCount() * DRILL_UP_COUNT;
		}
		return GetAddCount();
	}

	public static void SetChaoAbliltyScoreEffect(PlayerInformation playerInfo, CtystalParam ctystalParam, GameObject obj)
	{
		if (ctystalParam != null)
		{
			List<ChaoAbility> abilityList = new List<ChaoAbility>();
			ObjUtil.GetChaoAbliltyPhantomFlag(playerInfo, ref abilityList);
			int chaoAbliltyScore = ObjUtil.GetChaoAbliltyScore(abilityList, ctystalParam.m_score);
			ObjUtil.SendMessageAddScore(chaoAbliltyScore);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(3, chaoAbliltyScore));
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
