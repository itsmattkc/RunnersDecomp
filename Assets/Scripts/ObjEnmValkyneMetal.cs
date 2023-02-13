using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmValkyneMetal")]
public class ObjEnmValkyneMetal : ObjEnmValkyne
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
