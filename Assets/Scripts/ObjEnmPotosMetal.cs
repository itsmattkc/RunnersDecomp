using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmPotosMetal")]
public class ObjEnmPotosMetal : ObjEnmPotos
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
