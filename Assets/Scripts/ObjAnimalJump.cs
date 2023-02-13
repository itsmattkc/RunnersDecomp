public class ObjAnimalJump : ObjAnimalBase
{
	private const float JUMP_SPEED = 6f;

	private const float JUMP_GRAVITY = -6.1f;

	private const float JUMP_ADD_X = 1f;

	private const float BOUND_ADD_X = 3f;

	private const float BOUND_DOWN_X = 0.2f;

	private const float BOUND_DOWN_Y = 0f;

	private const float HIT_CHECK_DISTANCE = 1f;

	protected override float GetCheckGroundHitLength()
	{
		return 1f;
	}

	protected override void StartNextComponent()
	{
		MotorAnimalJump component = GetComponent<MotorAnimalJump>();
		if ((bool)component)
		{
			component.enabled = true;
			MotorAnimalJump.JumpParam param = default(MotorAnimalJump.JumpParam);
			param.m_obj = base.gameObject;
			param.m_speed = 6f;
			param.m_gravity = -6.1f;
			param.m_add_x = 1f + GetMoveSpeed();
			param.m_up = base.transform.up;
			param.m_forward = base.transform.right;
			param.m_bound = true;
			param.m_bound_add_y = 3f;
			param.m_bound_down_x = 0.2f;
			param.m_bound_down_y = 0f;
			component.Setup(ref param);
		}
	}

	protected override void EndNextComponent()
	{
		MotorAnimalJump component = GetComponent<MotorAnimalJump>();
		if ((bool)component)
		{
			component.enabled = false;
			component.SetEnd();
		}
	}
}
