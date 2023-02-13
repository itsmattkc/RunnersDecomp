using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjRedStarRing")]
public class ObjRedStarRing : SpawnableObject
{
	private const float m_rotate_ratio = 60f;

	public static string ModelName = "obj_cmn_redsterring";

	public static string EffectName = "ef_ob_get_redring01";

	public static string SEName = "obj_redring";

	private bool m_end;

	protected override string GetModelName()
	{
		return ModelName;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
		if (StageComboManager.Instance != null && (StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
	}

	private void Update()
	{
		UpdateModelPose();
	}

	private void UpdateModelPose()
	{
		float y = 60f * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(0f, y, 0f) * base.transform.rotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_end || !other)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if ((bool)gameObject)
		{
			string a = LayerMask.LayerToName(gameObject.layer);
			if (a == "Player" && gameObject.tag != "ChaoAttack")
			{
				ObjUtil.SendMessageAddRedRing();
				ObjUtil.SendMessageScoreCheck(new StageScoreData(8, 1));
				TakeRing();
			}
		}
	}

	private void OnDrawingRings(MsgOnDrawingRings msg)
	{
		ObjUtil.StartMagnetControl(base.gameObject);
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
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, EffectName, 1f);
		ObjUtil.PlaySE(SEName);
		Object.Destroy(base.gameObject);
	}
}
