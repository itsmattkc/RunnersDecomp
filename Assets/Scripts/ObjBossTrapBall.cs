using Message;
using UnityEngine;

public class ObjBossTrapBall : MonoBehaviour
{
	private class BossTrapBallModelTypeParam
	{
		public string m_modelName1;

		public string m_modelName2;

		public ResourceCategory m_resCategory;

		public string m_effectName;

		public string m_modelNameLeft;

		public string m_modelNameRight;

		public string m_modelNameTop;

		public string m_modelNameUnder;

		public BossTrapBallModelTypeParam(string model1, string model2, ResourceCategory resCategory, string effect, string modelL, string modelR, string modelT, string modelU)
		{
			m_modelName1 = model1;
			m_modelName2 = model2;
			m_resCategory = resCategory;
			m_effectName = effect;
			m_modelNameLeft = modelL;
			m_modelNameRight = modelR;
			m_modelNameTop = modelT;
			m_modelNameUnder = modelU;
		}
	}

	private enum State
	{
		Idle,
		Start,
		Down,
		Wait,
		Attack
	}

	private const float END_TIME = 5f;

	private const float ATTACK_ROT_SPEED = 25f;

	private const int COLLI_NUM = 2;

	private static Vector3 TRAP_TYPE_BALLROT = new Vector3(0f, 0f, 1f);

	private State m_state;

	private float m_time;

	private GameObject m_boss_obj;

	private GameObject m_model_obj;

	private GameObject[] m_colli_obj;

	private GameObject[] m_colli_model_obj;

	private Vector3 m_colli = Vector3.zero;

	private float m_rot_speed;

	private float m_attack_speed;

	private float m_add_speed = 3f;

	private BossTrapBallType m_type;

	private bool m_bossAppear;

	private BossTrapBallModelTypeParam m_typeParam = new BossTrapBallModelTypeParam("obj_boss_movetrap", string.Empty, ResourceCategory.OBJECT_RESOURCE, "ef_com_explosion_m01", string.Empty, string.Empty, string.Empty, string.Empty);

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		switch (m_state)
		{
		case State.Start:
			if (m_bossAppear)
			{
				ObjBossUtil.SetupBallAppear(m_boss_obj, base.gameObject);
			}
			m_state = State.Down;
			break;
		case State.Down:
			if (!m_boss_obj)
			{
				break;
			}
			if (m_bossAppear)
			{
				if (ObjBossUtil.UpdateBallAppear(deltaTime, m_boss_obj, base.gameObject, m_add_speed))
				{
					ObjBossUtil.PlayShotEffect(m_boss_obj);
					ObjBossUtil.PlayShotSE();
					Shot(m_colli);
					m_time = 0f;
					m_state = State.Wait;
				}
			}
			else
			{
				Shot(m_colli);
				m_time = 0f;
				m_state = State.Wait;
			}
			break;
		case State.Wait:
			if (m_colli_model_obj != null)
			{
				for (int i = 0; i < m_colli_model_obj.Length; i++)
				{
					if (m_colli_model_obj[i] != null)
					{
						ObjBossUtil.UpdateBallRot(deltaTime, m_colli_model_obj[i], TRAP_TYPE_BALLROT, m_rot_speed);
					}
				}
			}
			m_time += deltaTime;
			if (m_time > 5f)
			{
				Object.Destroy(base.gameObject);
			}
			break;
		case State.Attack:
			if ((bool)m_boss_obj)
			{
				m_time += deltaTime;
				ObjBossUtil.UpdateBallAttack(m_boss_obj, base.gameObject, m_time, m_attack_speed);
				float num = Mathf.Abs(Vector3.Distance(base.transform.position, m_boss_obj.transform.position));
				if (num < 0.1f)
				{
					HitAttack(m_boss_obj);
					SetBroken();
				}
			}
			break;
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
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, m_typeParam.m_effectName, 1f);
		ObjUtil.PlaySE("obj_brk");
		Object.Destroy(base.gameObject);
	}

	public void Setup(GameObject obj, Vector3 colli, float rot_speed, float attack_speed, BossTrapBallType type, BossType bossType, bool bossAppear)
	{
		m_boss_obj = obj;
		m_colli = colli;
		m_rot_speed = rot_speed;
		m_attack_speed = attack_speed;
		m_type = type;
		m_bossAppear = bossAppear;
		if (BossTypeUtil.GetBossCharaType(bossType) != 0)
		{
			m_typeParam.m_modelName1 = EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj2_ModelName);
			m_typeParam.m_resCategory = ResourceCategory.EVENT_RESOURCE;
			m_typeParam.m_effectName = EventBossObjectTable.GetItemData(EventBossObjectTableItem.Obj2_EffectName);
		}
		m_typeParam.m_modelName2 = m_typeParam.m_modelName1 + "brk";
		m_typeParam.m_modelNameLeft = m_typeParam.m_modelName1 + "_left";
		m_typeParam.m_modelNameRight = m_typeParam.m_modelName1 + "_right";
		m_typeParam.m_modelNameTop = m_typeParam.m_modelName1 + "_top";
		m_typeParam.m_modelNameUnder = m_typeParam.m_modelName1 + "_under";
		CreateModel(m_type);
		if (m_bossAppear)
		{
			ObjUtil.SetModelVisible(base.gameObject, false);
		}
		m_time = 0f;
		m_state = State.Start;
	}

	public void Shot(Vector3 colli)
	{
		if ((bool)m_model_obj)
		{
			Animator componentInChildren = GetComponentInChildren<Animator>();
			if (componentInChildren != null)
			{
				if (colli.x > 0f)
				{
					componentInChildren.Play(m_typeParam.m_modelName1 + "_width");
				}
				else
				{
					componentInChildren.Play(m_typeParam.m_modelName1 + "_length");
				}
			}
		}
		if (m_colli_obj == null)
		{
			m_colli_obj = new GameObject[2];
		}
		if (m_colli_model_obj == null)
		{
			m_colli_model_obj = new GameObject[2];
		}
		for (int i = 0; i < 2; i++)
		{
			if (colli.x > 0f)
			{
				string name = (i != 0) ? m_typeParam.m_modelNameRight : m_typeParam.m_modelNameLeft;
				m_colli_obj[i] = CreateCollision(name);
				m_colli_model_obj[i] = GameObjectUtil.FindChildGameObject(base.gameObject, name);
			}
			else
			{
				string name2 = (i != 0) ? m_typeParam.m_modelNameUnder : m_typeParam.m_modelNameTop;
				m_colli_obj[i] = CreateCollision(name2);
				m_colli_model_obj[i] = GameObjectUtil.FindChildGameObject(base.gameObject, name2);
			}
		}
	}

	private void StartAttack()
	{
		if (m_colli_obj != null)
		{
			for (int i = 0; i < m_colli_obj.Length; i++)
			{
				if (m_colli_obj[i] != null)
				{
					Object.Destroy(m_colli_obj[i].gameObject);
				}
			}
		}
		ObjUtil.PlaySE("boss_counterattack");
		m_time = 0f;
		m_rot_speed = 25f;
		m_state = State.Attack;
	}

	private void HitAttack(GameObject obj)
	{
		if ((bool)obj)
		{
			MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerSpin);
			obj.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				HitAttack(gameObject);
			}
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (msg.m_attackPower <= 0 || !msg.m_sender)
		{
			return;
		}
		GameObject gameObject = msg.m_sender.gameObject;
		if ((bool)gameObject)
		{
			MsgHitDamageSucceed value = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
			gameObject.SendMessage("OnDamageSucceed", value, SendMessageOptions.DontRequireReceiver);
			if (IsBrokenPower(m_type, msg.m_attackPower) || gameObject.name == "ChaoPartsAttackEnemy")
			{
				SetBroken();
			}
			else if (m_type == BossTrapBallType.ATTACK)
			{
				StartAttack();
			}
		}
	}

	private static bool IsBrokenPower(BossTrapBallType type, int plPower)
	{
		switch (type)
		{
		case BossTrapBallType.ATTACK:
			if (plPower == 5)
			{
				return false;
			}
			if (plPower >= 4)
			{
				return true;
			}
			break;
		case BossTrapBallType.BREAK:
			return true;
		}
		return false;
	}

	private void CreateModel(BossTrapBallType type)
	{
		string name = (type != 0) ? m_typeParam.m_modelName2 : m_typeParam.m_modelName1;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(m_typeParam.m_resCategory, name);
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.localRotation = Quaternion.Euler(Vector3.zero);
				m_model_obj = gameObject2;
			}
		}
	}

	private GameObject CreateCollision(string name)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name);
		if ((bool)gameObject)
		{
			GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossTrapBallCollision");
			if ((bool)gameObject2)
			{
				GameObject gameObject3 = Object.Instantiate(gameObject2, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
				if ((bool)gameObject3)
				{
					gameObject3.gameObject.SetActive(true);
					gameObject3.transform.parent = gameObject.transform;
					return gameObject3;
				}
			}
		}
		return null;
	}
}
