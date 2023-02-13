using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common")]
public class ObjRing : SpawnableObject
{
	private const int m_add_ring_value = 1;

	private bool m_end;

	public static string GetRingModelName()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return EventBossObjectTable.GetItemData(EventBossObjectTableItem.RingModel);
		}
		return "obj_cmn_ring";
	}

	protected override string GetModelName()
	{
		return GetRingModelName();
	}

	public static ResourceCategory GetRingModelCategory()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return ResourceCategory.EVENT_RESOURCE;
		}
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return GetRingModelCategory();
	}

	public static string GetRingEffect()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			return EventBossObjectTable.GetItemData(EventBossObjectTableItem.RingEffect);
		}
		return "ef_ob_get_ring01";
	}

	public static void SetPlayRingSE()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			ObjUtil.LightPlayEventSE(EventBossObjectTable.GetItemData(EventBossObjectTableItem.RingSE), EventManager.EventType.RAID_BOSS);
		}
		ObjUtil.LightPlaySE("obj_ring");
	}

	public static int GetScore()
	{
		return 10;
	}

	protected override void OnSpawned()
	{
		if (StageComboManager.Instance != null && (StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
		base.enabled = false;
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
				gameObject.SendMessage("OnAddRings", 1, SendMessageOptions.DontRequireReceiver);
				TakeRing();
			}
		}
		else if (a == "HitRing" && gameObject.tag == "Chao")
		{
			GameObjectUtil.SendMessageToTagObjects("Player", "OnAddRings", 1, SendMessageOptions.DontRequireReceiver);
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
			StageEffectManager.Instance.PlayEffect(EffectPlayType.RING, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
		SetPlayRingSE();
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
