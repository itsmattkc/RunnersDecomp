using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjSuperRing")]
public class ObjSuperRing : SpawnableObject
{
	private bool m_end;

	public static string ModelName = "obj_cmn_superring10";

	public static string SEName = "obj_superring";

	public static string GetSuperRingModelName()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return EventBossObjectTable.GetItemData(EventBossObjectTableItem.Ring10Model);
		}
		return ModelName;
	}

	protected override string GetModelName()
	{
		return GetSuperRingModelName();
	}

	public static ResourceCategory GetSuperRingModelCategory()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return ResourceCategory.EVENT_RESOURCE;
		}
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return GetSuperRingModelCategory();
	}

	public static string GetSuperRingEffect()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return EventBossObjectTable.GetItemData(EventBossObjectTableItem.Ring10Effect);
		}
		return "ef_ob_get_superring01";
	}

	public static void SetPlaySuperRingSE()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			ObjUtil.PlayEventSE(EventBossObjectTable.GetItemData(EventBossObjectTableItem.Ring10SE), EventManager.EventType.RAID_BOSS);
		}
		ObjUtil.PlaySE(SEName);
	}

	protected override void OnSpawned()
	{
		if (StageComboManager.Instance != null && (StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
		base.enabled = true;
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
		m_end = false;
		OnSpawned();
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
				gameObject.SendMessage("OnAddRings", GetRingCount(), SendMessageOptions.DontRequireReceiver);
				TakeRing();
			}
		}
		else if (a == "HitRing" && gameObject.tag == "Chao")
		{
			GameObjectUtil.SendMessageToTagObjects("Player", "OnAddRings", GetRingCount(), SendMessageOptions.DontRequireReceiver);
			TakeRing();
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

	private void TakeRing()
	{
		m_end = true;
		if (StageEffectManager.Instance != null)
		{
			StageEffectManager.Instance.PlayEffect(EffectPlayType.SUPER_RING, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
		SetPlaySuperRingSE();
		if (base.Share)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public static int GetRingCount()
	{
		int num = 10;
		if (StageAbilityManager.Instance != null)
		{
			num += (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.SUPER_RING_UP);
		}
		return num;
	}

	public static void AddSuperRing(GameObject obj)
	{
		if ((bool)obj)
		{
			obj.SendMessage("OnAddRings", GetRingCount(), SendMessageOptions.DontRequireReceiver);
		}
	}
}
