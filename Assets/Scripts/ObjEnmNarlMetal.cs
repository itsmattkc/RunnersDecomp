using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmNarlMetal")]
public class ObjEnmNarlMetal : ObjEnmNarl
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
