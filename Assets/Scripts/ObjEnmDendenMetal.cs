using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmDendenMetal")]
public class ObjEnmDendenMetal : ObjEnmDenden
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
