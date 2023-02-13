using Message;
using UnityEngine;

public class ObjAnimalBase : MonoBehaviour
{
	private enum State
	{
		Jump,
		Wait,
		Drawing,
		End
	}

	private const float JUMP_END_TIME = 0.3f;

	private const float WAIT_END_TIME = 7f;

	private const float ANIMAL_SPEED = 6f;

	private const float ANIMAL_GRAVITY = -6.1f;

	private const float ADD_SPEED = 0.12f;

	private const float ADD_X = 4.2f;

	private const float ADD_Y = 3f;

	public static string EFFECT_NAME = "ef_ob_get_animal01";

	private State m_state;

	private float m_time;

	private float m_move_speed;

	private float m_hit_length;

	private int m_addCount = 1;

	private StageComboManager m_stageComboManager;

	private bool m_end;

	private bool m_share;

	private bool m_sleep;

	private AnimalType m_animalType = AnimalType.ERROR;

	public bool Sleep
	{
		set
		{
			m_sleep = value;
		}
	}

	public bool IsSleep()
	{
		return m_share && m_sleep;
	}

	private void Start()
	{
		m_move_speed = 0.12f * ObjUtil.GetPlayerAddSpeed();
		m_hit_length = GetCheckGroundHitLength();
		SetMotorThrowComponent();
		m_stageComboManager = StageComboManager.Instance;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		m_time += deltaTime;
		switch (m_state)
		{
		case State.Jump:
		{
			if (!(m_time > 0.3f))
			{
				break;
			}
			if (UpdateCheckComboChaoAbility())
			{
				m_time = 0f;
				m_state = State.Drawing;
				break;
			}
			Vector3 hit_pos = Vector3.zero;
			if (ObjUtil.CheckGroundHit(base.transform.position, base.transform.up, 1f, m_hit_length, out hit_pos))
			{
				SetCollider(true);
				EndThrowComponent();
				StartNextComponent();
				m_time = 0f;
				m_state = State.Wait;
			}
			else if (m_time > 7f)
			{
				m_state = State.End;
				SleepOrDestroy();
			}
			break;
		}
		case State.Drawing:
			if (m_time > 7f)
			{
				m_state = State.End;
				SleepOrDestroy();
			}
			break;
		case State.Wait:
			if (UpdateCheckComboChaoAbility())
			{
				m_time = 0f;
				m_state = State.Drawing;
			}
			else if (m_time > 7f)
			{
				m_state = State.End;
				SleepOrDestroy();
			}
			break;
		}
	}

	protected virtual float GetCheckGroundHitLength()
	{
		return 1f;
	}

	protected virtual void StartNextComponent()
	{
	}

	protected virtual void EndNextComponent()
	{
	}

	protected float GetMoveSpeed()
	{
		return m_move_speed;
	}

	public void SetMotorThrowComponent()
	{
		MotorThrow component = GetComponent<MotorThrow>();
		if ((bool)component)
		{
			component.enabled = true;
			component.SetEnd();
			MotorThrow.ThrowParam throwParam = new MotorThrow.ThrowParam();
			throwParam.m_obj = base.gameObject;
			throwParam.m_speed = 6f;
			throwParam.m_gravity = -6.1f;
			throwParam.m_add_x = 4.2f + m_move_speed;
			throwParam.m_add_y = 3f + m_move_speed;
			throwParam.m_rot_speed = 0f;
			throwParam.m_up = base.transform.up;
			throwParam.m_forward = base.transform.right;
			throwParam.m_rot_angle = Vector3.zero;
			component.Setup(throwParam);
		}
	}

	public void OnRevival()
	{
		base.enabled = true;
		m_end = false;
		m_state = State.Jump;
		m_time = 0f;
		m_move_speed = 0.12f * ObjUtil.GetPlayerAddSpeed();
		SetMotorThrowComponent();
	}

	public void SetShareState(AnimalType animalType)
	{
		m_share = true;
		m_sleep = true;
		m_animalType = animalType;
	}

	private void SleepOrDestroy()
	{
		if (m_share)
		{
			base.gameObject.SetActive(false);
			if (AnimalResourceManager.Instance != null)
			{
				AnimalResourceManager.Instance.SetSleep(m_animalType, base.gameObject);
			}
			EndThrowComponent();
			EndNextComponent();
			MagnetControl component = base.gameObject.GetComponent<MagnetControl>();
			if (component != null)
			{
				component.Reset();
			}
			SetCollider(false);
			m_state = State.End;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
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
			if (a == "Player" || a == "Chao")
			{
				TakeAnimal();
			}
			else if (!(a == "Magnet"))
			{
			}
		}
	}

	private void OnDrawingRings(MsgOnDrawingRings msg)
	{
		if (m_state != State.Drawing && m_state != State.End)
		{
			ObjUtil.StartMagnetControl(base.gameObject);
			SetCollider(true);
			EndThrowComponent();
			EndNextComponent();
			m_time = 0f;
			m_state = State.Drawing;
		}
	}

	public void OnDestroyAnimal()
	{
		Object.Destroy(base.gameObject);
	}

	private void TakeAnimal()
	{
		m_end = true;
		if (StageEffectManager.Instance != null)
		{
			StageEffectManager.Instance.PlayEffect(EffectPlayType.ANIMAL, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
		ObjUtil.SendMessageAddAnimal(m_addCount);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(7, m_addCount));
		ObjUtil.LightPlaySE("obj_animal_get");
		ObjUtil.AddCombo();
		SleepOrDestroy();
	}

	private void EndThrowComponent()
	{
		MotorThrow component = GetComponent<MotorThrow>();
		if ((bool)component)
		{
			component.enabled = false;
			component.SetEnd();
		}
	}

	public static void DestroyAnimalEffect()
	{
		string name = EFFECT_NAME + "(Clone)";
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	private bool UpdateCheckComboChaoAbility()
	{
		if (m_stageComboManager != null && (m_stageComboManager.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ANIMAL) || m_stageComboManager.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || m_stageComboManager.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			OnDrawingRings(new MsgOnDrawingRings());
			return true;
		}
		return false;
	}

	public void SetAnimalAddCount(int addCount)
	{
		m_addCount = addCount;
	}

	private void SetCollider(bool on)
	{
		SphereCollider component = GetComponent<SphereCollider>();
		if (component != null)
		{
			component.enabled = on;
		}
	}
}
