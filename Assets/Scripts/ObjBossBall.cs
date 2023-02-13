using Message;
using UnityEngine;

public class ObjBossBall : MonoBehaviour
{
	public struct SetData
	{
		public GameObject obj;

		public float bound_param;

		public BossBallType type;

		public Quaternion shot_rot;

		public float shot_speed;

		public float attack_speed;

		public float firstSpeed;

		public float outOfcontrol;

		public float ballSpeed;

		public bool bossAppear;
	}

	private enum State
	{
		Idle,
		Start,
		Down,
		Bound,
		Attack
	}

	private const ResourceCategory MODEL_CATEGORY = ResourceCategory.OBJECT_RESOURCE;

	private const float END_TIME = 5f;

	private const float BALL_GRAVITY = -6.1f;

	private const float ATTACK_ROT_SPEED = 10f;

	private static readonly string[] MODEL_FILES = new string[3]
	{
		"obj_boss_ironball",
		"obj_boss_thornball",
		"obj_boss_bumper"
	};

	private State m_state;

	private float m_time;

	private float m_bound_param;

	private BossBallType m_type;

	private GameObject m_boss_obj;

	private GameObject m_model_obj;

	private MotorShot m_motor_cmp;

	private Quaternion m_shot_rotation = Quaternion.identity;

	private float m_shot_speed;

	private float m_attack_speed;

	private float m_add_speed = 1f;

	private float m_firstSpeed;

	private float m_outOfcontrol;

	private bool m_playerDead;

	private float m_ballSpeed;

	private bool m_bossAppear;

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
					MotorShot();
					m_time = 0f;
					m_state = State.Bound;
				}
			}
			else
			{
				MotorShot();
				m_time = 0f;
				m_state = State.Bound;
			}
			break;
		case State.Bound:
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
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, "ef_com_explosion_s01", 1f);
		ObjUtil.PlaySE("obj_brk");
		Object.Destroy(base.gameObject);
	}

	public void Setup(SetData setData)
	{
		CreateModel(setData.type);
		m_state = State.Start;
		m_boss_obj = setData.obj;
		m_bound_param = setData.bound_param;
		m_type = setData.type;
		m_shot_rotation = setData.shot_rot;
		m_shot_speed = setData.shot_speed;
		m_attack_speed = setData.attack_speed;
		m_firstSpeed = setData.firstSpeed;
		m_outOfcontrol = setData.outOfcontrol;
		m_ballSpeed = setData.ballSpeed;
		m_bossAppear = setData.bossAppear;
		if (m_bossAppear)
		{
			ObjUtil.SetModelVisible(base.gameObject, false);
		}
	}

	public void MotorShot()
	{
		m_motor_cmp = GetComponent<MotorShot>();
		if ((bool)m_motor_cmp)
		{
			MotorShot.ShotParam shotParam = new MotorShot.ShotParam();
			shotParam.m_obj = base.gameObject;
			shotParam.m_gravity = -6.1f;
			shotParam.m_rot_speed = 0f;
			shotParam.m_rot_downspeed = 0f;
			shotParam.m_rot_angle = Vector3.zero;
			shotParam.m_shot_rotation = ObjBossUtil.GetShotRotation(m_shot_rotation, m_playerDead);
			shotParam.m_shot_time = 1f;
			shotParam.m_shot_speed = m_shot_speed;
			shotParam.m_shot_downspeed = 0f;
			shotParam.m_bound = true;
			SphereCollider component = GetComponent<SphereCollider>();
			if ((bool)component)
			{
				shotParam.m_bound_pos_y = component.radius;
			}
			shotParam.m_bound_add_y = Mathf.Max(m_bound_param, 0f);
			shotParam.m_bound_down_x = 0f;
			shotParam.m_bound_down_y = 0.01f;
			shotParam.m_after_speed = m_ballSpeed;
			shotParam.m_after_add_x = 0f;
			shotParam.m_after_up = base.transform.up;
			shotParam.m_after_forward = base.transform.right;
			m_motor_cmp.Setup(shotParam);
		}
	}

	private void StartAttack()
	{
		m_time = 0f;
		m_state = State.Attack;
		ObjUtil.PlaySE("boss_counterattack");
		if ((bool)m_motor_cmp)
		{
			m_motor_cmp.SetEnd();
		}
	}

	private void HitAttack(GameObject obj)
	{
		if ((bool)obj)
		{
			AttackPower myPower = GetMyPower(m_type);
			MsgHitDamage value = new MsgHitDamage(base.gameObject, myPower);
			obj.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_type == BossBallType.BUMPER)
		{
			MsgOnSpringImpulse msgOnSpringImpulse = new MsgOnSpringImpulse(base.transform.position, base.transform.rotation, m_firstSpeed, m_outOfcontrol);
			other.gameObject.SendMessage("OnSpringImpulse", msgOnSpringImpulse, SendMessageOptions.DontRequireReceiver);
			if (m_boss_obj != null)
			{
				m_boss_obj.SendMessage("OnHitBumper", SendMessageOptions.DontRequireReceiver);
			}
			if (!msgOnSpringImpulse.m_succeed)
			{
				return;
			}
			if (m_model_obj != null)
			{
				Animation componentInChildren = m_model_obj.GetComponentInChildren<Animation>();
				if ((bool)componentInChildren)
				{
					componentInChildren.wrapMode = WrapMode.Once;
					componentInChildren.Play("obj_boss_bumper_bounce");
				}
			}
			ObjUtil.PlaySE("obj_spring");
		}
		else if ((bool)other)
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
		if (m_type == BossBallType.BUMPER || !IsAttackPower(m_type, msg.m_attackPower) || !msg.m_sender)
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
			else
			{
				StartAttack();
			}
		}
	}

	private static bool IsAttackPower(BossBallType type, int plPower)
	{
		AttackPower attackPower = AttackPower.PlayerSpin;
		if (type == BossBallType.TRAP)
		{
			attackPower = AttackPower.PlayerColorPower;
		}
		if (plPower >= (int)attackPower)
		{
			return true;
		}
		return false;
	}

	private static bool IsBrokenPower(BossBallType type, int plPower)
	{
		if (type == BossBallType.ATTACK && plPower == 5)
		{
			return false;
		}
		if (plPower >= 4)
		{
			return true;
		}
		return false;
	}

	private static AttackPower GetMyPower(BossBallType type)
	{
		AttackPower result = AttackPower.PlayerSpin;
		if (type == BossBallType.TRAP)
		{
			result = AttackPower.PlayerColorPower;
		}
		return result;
	}

	private void CreateModel(BossBallType type)
	{
		if ((uint)type >= (uint)MODEL_FILES.Length)
		{
			return;
		}
		string name = MODEL_FILES[(int)type];
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_RESOURCE, name);
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				ObjUtil.StopAnimation(gameObject2);
				gameObject2.gameObject.SetActive(true);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.localRotation = Quaternion.Euler(Vector3.zero);
				m_model_obj = gameObject2;
			}
		}
	}

	public void OnMsgNotifyDead(MsgNotifyDead msg)
	{
		m_playerDead = true;
	}
}
