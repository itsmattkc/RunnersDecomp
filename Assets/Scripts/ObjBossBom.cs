using Message;
using UnityEngine;

public class ObjBossBom : MonoBehaviour
{
	private enum State
	{
		Idle,
		Start,
		Down,
		Bom,
		Wait
	}

	private const string MODEL_NAME = "obj_boss_bomb";

	private const ResourceCategory MODEL_CATEGORY = ResourceCategory.OBJECT_RESOURCE;

	private const float BOM_END_TIME = 0.1f;

	private const float WAIT_END_TIME = 5f;

	private const float BALL_GRAVITY = -6.1f;

	private State m_state;

	private float m_time;

	private bool m_hit;

	private GameObject m_boss_obj;

	private Quaternion m_shot_rotation = Quaternion.identity;

	private float m_shot_speed;

	private float m_bom_pos_y;

	private float m_wait_time;

	private string m_blast_effect_name = string.Empty;

	private float m_blast_destroy_time;

	private float m_add_speed = 1f;

	private bool m_shot;

	private bool m_playerDead;

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		switch (m_state)
		{
		case State.Start:
			ObjBossUtil.SetupBallAppear(m_boss_obj, base.gameObject);
			m_state = State.Down;
			break;
		case State.Down:
			if ((bool)m_boss_obj && ObjBossUtil.UpdateBallAppear(deltaTime, m_boss_obj, base.gameObject, m_add_speed) && m_shot)
			{
				ObjBossUtil.PlayShotEffect(m_boss_obj);
				ObjBossUtil.PlayShotSE();
				MotorShot();
				m_wait_time = 5f;
				m_state = State.Wait;
			}
			break;
		case State.Bom:
			ObjUtil.PlayEffectCollisionCenter(base.gameObject, m_blast_effect_name, m_blast_destroy_time, true);
			ObjUtil.PlaySE("obj_common_exp");
			m_time = 0f;
			m_wait_time = 0.1f;
			m_state = State.Wait;
			break;
		case State.Wait:
			m_time += deltaTime;
			if (m_time > m_wait_time)
			{
				Object.Destroy(base.gameObject);
			}
			break;
		}
	}

	public void OnMsgObjectDead(MsgObjectDead msg)
	{
		if (base.enabled && m_hit)
		{
			SetBroken();
		}
	}

	private void SetBroken()
	{
		Blast("ef_bo_em_bom01", 2f);
	}

	public void Setup(GameObject obj, bool colli, Quaternion shot_rot, float shot_speed, float add_speed, bool shot)
	{
		CreateModel();
		ObjUtil.SetModelVisible(base.gameObject, false);
		m_hit = colli;
		m_boss_obj = obj;
		m_shot_rotation = shot_rot;
		m_shot_speed = shot_speed;
		m_add_speed = add_speed;
		m_shot = shot;
		m_state = State.Start;
	}

	public void Blast(string name, float destroy_time)
	{
		m_blast_effect_name = name;
		m_blast_destroy_time = destroy_time;
		m_state = State.Bom;
	}

	public void SetShot(bool shot)
	{
		m_shot = shot;
	}

	public void MotorShot()
	{
		MotorShot component = GetComponent<MotorShot>();
		if ((bool)component)
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
			SphereCollider component2 = GetComponent<SphereCollider>();
			if ((bool)component2)
			{
				m_bom_pos_y = component2.radius;
			}
			shotParam.m_bound_pos_y = m_bom_pos_y;
			shotParam.m_bound_add_y = 0f;
			component.Setup(shotParam);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other && m_hit)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerColorPower);
				gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
				SetBroken();
			}
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (m_hit && msg.m_attackPower >= 4 && (bool)msg.m_sender)
		{
			GameObject gameObject = msg.m_sender.gameObject;
			if ((bool)gameObject)
			{
				SetBroken();
			}
		}
	}

	private void CreateModel()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_RESOURCE, "obj_boss_bomb");
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

	public void OnMsgNotifyDead(MsgNotifyDead msg)
	{
		m_playerDead = true;
	}
}
