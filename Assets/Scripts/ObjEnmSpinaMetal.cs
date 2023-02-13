using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmSpinaMetal")]
public class ObjEnmSpinaMetal : ObjEnmSpina
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
