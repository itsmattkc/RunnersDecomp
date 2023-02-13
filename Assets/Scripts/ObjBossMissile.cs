using Message;
using UnityEngine;

public class ObjBossMissile : MonoBehaviour
{
	private class BossMissileTypeParam
	{
		public string m_modelName;

		public ResourceCategory m_resCategory;

		public string m_effectName;

		public string m_loopEffectName;

		public string m_seName;

		public BossMissileTypeParam(string modelName, ResourceCategory resCategory, string effectName, string loopEffectName, string seName)
		{
			m_modelName = modelName;
			m_resCategory = resCategory;
			m_effectName = effectName;
			m_loopEffectName = loopEffectName;
			m_seName = seName;
		}
	}

	private const float END_TIME = 5f;

	private const float MOVE_DISTANCE = 20f;

	private const float START_MOVE_DISTANCE = 20f;

	private float m_time;

	private BossType m_type = BossType.NONE;

	private BossMissileTypeParam m_typeParam = new BossMissileTypeParam("obj_cmn_movetrap", ResourceCategory.OBJECT_RESOURCE, "ef_com_explosion_m01", string.Empty, "obj_missile_shoot");

	private void Update()
	{
		m_time += Time.deltaTime;
		if (m_time > 5f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnMsgObjectDead(MsgObjectDead msg)
	{
		if (base.enabled)
		{
			SetBroken();
		}
	}

	private void SetBroken()
	{
		ObjUtil.PlayEffect(m_typeParam.m_effectName, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation, 1f);
		ObjUtil.PlaySE("obj_brk");
		Object.Destroy(base.gameObject);
	}

	public void Setup(GameObject obj, float speed, BossType type)
	{
		m_type = type;
		if (BossTypeUtil.GetBossCharaType(m_type) != 0)
		{
			m_typeParam = new BossMissileTypeParam(EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj1_ModelName), ResourceCategory.EVENT_RESOURCE, EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj1_EffectName), EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj1_LoopEffectName), EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj1_SetSeName));
		}
		string loopEffectName = m_typeParam.m_loopEffectName;
		if (loopEffectName != string.Empty)
		{
			ObjUtil.PlayEffectChild(base.gameObject, loopEffectName, ObjUtil.GetCollisionCenter(base.gameObject), Quaternion.identity);
		}
		CreateModel();
		ObjUtil.StartHudAlert(base.gameObject);
		MotorConstant component = GetComponent<MotorConstant>();
		if ((bool)component)
		{
			string se_category = "SE";
			if (BossTypeUtil.GetBossCharaType(m_type) != 0)
			{
				se_category = "SE_" + EventManager.GetEventTypeName(EventManager.EventType.RAID_BOSS);
			}
			component.SetParam(speed, 20f, 20f, -base.transform.right, se_category, m_typeParam.m_seName);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerColorPower);
				gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (msg.m_attackPower >= 4)
		{
			if ((bool)msg.m_sender)
			{
				GameObject gameObject = msg.m_sender.gameObject;
				if ((bool)gameObject)
				{
					MsgHitDamageSucceed value = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
					gameObject.SendMessage("OnDamageSucceed", value, SendMessageOptions.DontRequireReceiver);
					SetBroken();
				}
			}
		}
		else
		{
			SetBroken();
		}
	}

	private void CreateModel()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(m_typeParam.m_resCategory, m_typeParam.m_modelName);
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
		}
	}
}
