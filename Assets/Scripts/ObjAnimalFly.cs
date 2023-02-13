public class ObjAnimalFly : ObjAnimalBase
{
	private const float FLY_SPEED = 0.5f;

	private const float FLY_DISTANCE = 1f;

	private const float FLY_ADD_X = 1f;

	private const float GROUND_DISTANCE = 3f;

	private const float HIT_CHECK_DISTANCE = 2f;

	protected override float GetCheckGroundHitLength()
	{
		return 2f;
	}

	protected override void StartNextComponent()
	{
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.enabled = true;
			component.SetupParam(0.5f, 1f, 1f + GetMoveSpeed(), base.transform.right, 3f, true);
		}
	}

	protected override void EndNextComponent()
	{
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.enabled = false;
			component.SetEnd();
		}
	}
}
