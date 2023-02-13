using Message;
using UnityEngine;

public class ObjTimerBase : SpawnableObject
{
	public enum MoveType
	{
		Thorw,
		Bound,
		Still
	}

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

	private const float FLY_SPEED = 0.5f;

	private const float FLY_DISTANCE = 1f;

	private const float FLY_ADD_X = 1f;

	private const float GROUND_DISTANCE = 3f;

	private const float HIT_CHECK_DISTANCE = 2f;

	private float m_time;

	private float m_move_speed;

	private float m_hit_length;

	private bool m_end;

	private TimerType m_timerType = TimerType.ERROR;

	private float m_startPosY;

	private MoveType m_moveType;

	private State m_state;

	public void SetMoveType(MoveType type)
	{
		m_moveType = type;
	}

	protected virtual TimerType GetTimerType()
	{
		return TimerType.ERROR;
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	private void Start()
	{
		m_timerType = GetTimerType();
		if (StageTimeManager.Instance != null)
		{
			StageTimeManager.Instance.ReserveExtendTime(GetExtendPattern());
		}
		MotorAnimalFly component = base.gameObject.GetComponent<MotorAnimalFly>();
		MotorThrow component2 = GetComponent<MotorThrow>();
		switch (m_moveType)
		{
		case MoveType.Thorw:
		{
			Vector3 position = base.transform.position;
			m_startPosY = position.y;
			m_move_speed = 0.12f * ObjUtil.GetPlayerAddSpeed();
			m_hit_length = GetCheckGroundHitLength();
			SetMotorThrowComponent();
			break;
		}
		case MoveType.Bound:
			if (component != null)
			{
				component.enabled = false;
			}
			break;
		case MoveType.Still:
			if (component != null)
			{
				component.enabled = false;
			}
			if (component2 != null)
			{
				component2.enabled = false;
			}
			break;
		}
	}

	protected override void OnSpawned()
	{
	}

	protected override void OnDestroyed()
	{
		if (StageTimeManager.Instance != null)
		{
			StageTimeManager.Instance.CancelReservedExtendTime(GetExtendPattern());
		}
	}

	private void Update()
	{
		switch (m_moveType)
		{
		case MoveType.Bound:
			break;
		case MoveType.Still:
			break;
		case MoveType.Thorw:
			UpdateThorwType();
			break;
		}
	}

	private void UpdateThorwType()
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
			if (CheckComboChaoAbility())
			{
				m_time = 0f;
				m_state = State.Drawing;
				OnDrawingRings(new MsgOnDrawingRings());
				break;
			}
			Vector3 hit_pos = Vector3.zero;
			if (!ObjUtil.CheckGroundHit(base.transform.position, base.transform.up, 1f, m_hit_length, out hit_pos))
			{
				float startPosY = m_startPosY;
				Vector3 position = base.transform.position;
				if (!(startPosY > position.y))
				{
					if (m_time > 7f)
					{
						m_state = State.End;
						Object.Destroy(base.gameObject);
					}
					break;
				}
			}
			SetCollider(true);
			EndThrowComponent();
			StartNextComponent();
			m_time = 0f;
			m_state = State.Wait;
			break;
		}
		case State.Drawing:
			if (m_time > 7f)
			{
				m_state = State.End;
				Object.Destroy(base.gameObject);
			}
			break;
		case State.Wait:
			if (CheckComboChaoAbility())
			{
				m_time = 0f;
				m_state = State.Drawing;
				OnDrawingRings(new MsgOnDrawingRings());
			}
			else if (m_time > 7f)
			{
				m_state = State.End;
				Object.Destroy(base.gameObject);
			}
			break;
		}
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

	private bool CheckComboChaoAbility()
	{
		if (StageComboManager.Instance != null && (StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RECOVERY_ALL_OBJ) || StageComboManager.Instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_DESTROY_AND_RECOVERY)))
		{
			return true;
		}
		return false;
	}

	protected float GetCheckGroundHitLength()
	{
		return 2f;
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
				TakeTimer();
			}
		}
	}

	private StageTimeManager.ExtendPattern GetExtendPattern()
	{
		StageTimeManager.ExtendPattern result = StageTimeManager.ExtendPattern.UNKNOWN;
		switch (m_timerType)
		{
		case TimerType.BRONZE:
			result = StageTimeManager.ExtendPattern.BRONZE_WATCH;
			break;
		case TimerType.SILVER:
			result = StageTimeManager.ExtendPattern.SILVER_WATCH;
			break;
		case TimerType.GOLD:
			result = StageTimeManager.ExtendPattern.GOLD_WATCH;
			break;
		}
		return result;
	}

	private int GetAtlasIndex()
	{
		int result = 0;
		switch (m_timerType)
		{
		case TimerType.BRONZE:
			result = 0;
			break;
		case TimerType.SILVER:
			result = 1;
			break;
		case TimerType.GOLD:
			result = 2;
			break;
		}
		return result;
	}

	private int GetAddTimer()
	{
		int result = 0;
		if (StageTimeManager.Instance != null)
		{
			result = (int)StageTimeManager.Instance.GetExtendTime(GetExtendPattern());
		}
		return result;
	}

	private void TakeTimer()
	{
		m_end = true;
		if (StageTimeManager.Instance != null)
		{
			StageTimeManager.Instance.ExtendTime(GetExtendPattern());
		}
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, ObjTimerUtil.GetEffectName(m_timerType), 1f);
		ObjUtil.PlaySE(ObjTimerUtil.GetSEName(m_timerType));
		ObjUtil.SendGetTimerIcon(GetAtlasIndex(), GetAddTimer());
		ObjUtil.AddCombo();
		Object.Destroy(base.gameObject);
	}

	protected void StartNextComponent()
	{
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.enabled = true;
			component.SetupParam(0.5f, 1f, 1f + m_move_speed, base.transform.right, 3f, true);
		}
	}

	protected void EndNextComponent()
	{
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.enabled = false;
			component.SetEnd();
		}
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

	private void OnDrawingRingsChaoAbility(MsgOnDrawingRings msg)
	{
		if (msg.m_chaoAbility == ChaoAbility.COMBO_RECOVERY_ALL_OBJ || msg.m_chaoAbility == ChaoAbility.COMBO_DESTROY_AND_RECOVERY)
		{
			OnDrawingRings(new MsgOnDrawingRings());
		}
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
