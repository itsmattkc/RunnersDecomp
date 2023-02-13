using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmMotoraMetal")]
public class ObjEnmMotoraMetal : ObjEnmMotora
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
