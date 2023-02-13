using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmBeetonMetal")]
public class ObjEnmBeetonMetal : ObjEnmBeeton
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
