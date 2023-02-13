public class ObjEnemySwing : ObjEnemyBase
{
	protected override void OnSpawned()
	{
		base.OnSpawned();
	}

	public void SetObjEnemySwingParameter(ObjEnemySwingParameter param)
	{
		if (param != null)
		{
			SetupEnemy((uint)param.tblID, 0f);
			MotorSwing component = GetComponent<MotorSwing>();
			if ((bool)component)
			{
				component.SetParam(param.moveSpeed, param.moveDistanceX, param.moveDistanceY, base.transform.forward);
			}
		}
	}
}
