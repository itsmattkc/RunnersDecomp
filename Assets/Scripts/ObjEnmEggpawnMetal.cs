using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmEggpawnMetal")]
public class ObjEnmEggpawnMetal : ObjEnmEggpawn
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
