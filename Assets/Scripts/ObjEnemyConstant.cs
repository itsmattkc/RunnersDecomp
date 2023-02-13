using UnityEngine;

public class ObjEnemyConstant : ObjEnemyBase
{
	public void SetObjEnemyConstantParameter(ObjEnemyConstantParameter param)
	{
		if (param != null)
		{
			SetupEnemy((uint)param.tblID, param.moveSpeed);
			MotorConstant component = GetComponent<MotorConstant>();
			if ((bool)component)
			{
				component.SetParam(param.moveSpeed, param.moveDistance, param.startMoveDistance, GetConstantAngle(), string.Empty, string.Empty);
			}
		}
	}

	protected virtual Vector3 GetConstantAngle()
	{
		return base.transform.forward;
	}
}
