using Message;
using UnityEngine;

public class ObjPointMarker : SpawnableObject
{
	private const string ModelName = "obj_cmn_pointmarker";

	private const float ChaoItemTime = 0.3f;

	private static Vector3 EffectLocalPosition = new Vector3(0f, 0.8f, 0f);

	private int m_type;

	private bool m_end;

	private float m_chaoItemTime;

	protected override string GetModelName()
	{
		return "obj_cmn_pointmarker";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
		ObjUtil.StopAnimation(base.gameObject);
	}

	public override void OnCreate()
	{
		SetActivePointMarker(false);
	}

	private void Update()
	{
		if (m_end && m_chaoItemTime > 0f)
		{
			m_chaoItemTime -= Time.deltaTime;
			if (m_chaoItemTime <= 0f)
			{
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.CHECK_POINT_ITEM_BOX, false);
				m_chaoItemTime = 0f;
			}
		}
	}

	public void SetType(PointMarkerType type)
	{
		m_type = (int)type;
	}

	public void OnActivePointMarker(MsgActivePointMarker msg)
	{
		if (msg.m_type == (PointMarkerType)m_type)
		{
			SetActivePointMarker(true);
			msg.m_activated = true;
		}
	}

	private void SetActivePointMarker(bool flag)
	{
		base.enabled = flag;
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			component.enabled = flag;
		}
		Component[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer meshRenderer = (MeshRenderer)array[i];
			if ((bool)meshRenderer)
			{
				meshRenderer.enabled = flag;
			}
		}
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
		if (!(a == "Player"))
		{
			return;
		}
		PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		if (playerInformation != null)
		{
			int numRings = playerInformation.NumRings;
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnAddStockRing", new MsgAddStockRing(numRings), SendMessageOptions.DontRequireReceiver);
		}
		ObjUtil.SendMessageTransferRing();
		PassPointMarker();
		GameObjectUtil.SendMessageFindGameObject("StageComboManager", "OnPassPointMarker", null, SendMessageOptions.DontRequireReceiver);
		if (!(StageAbilityManager.Instance != null) || !StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.CHECK_POINT_ITEM_BOX))
		{
			return;
		}
		int num = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.CHECK_POINT_ITEM_BOX);
		if (num >= ObjUtil.GetRandomRange100())
		{
			if (m_type == 0)
			{
				m_chaoItemTime = 0.3f;
			}
			else
			{
				m_chaoItemTime = 0.001f;
			}
		}
	}

	private void PassPointMarker()
	{
		ObjUtil.PlaySE("obj_checkpoint");
		Component[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer meshRenderer = (MeshRenderer)array[i];
			if (meshRenderer.name == "obj_cmn_pointmarkerbar")
			{
				ObjUtil.PlayEffectChild(meshRenderer.gameObject, "ef_ob_pass_pointmarker01", EffectLocalPosition, Quaternion.identity, 1f);
				break;
			}
		}
		Animation componentInChildren = GetComponentInChildren<Animation>();
		if ((bool)componentInChildren)
		{
			componentInChildren.wrapMode = WrapMode.Once;
			componentInChildren.Play("obj_pointmarker_bar");
		}
		m_end = true;
	}
}
