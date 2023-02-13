using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmPotosMoveMetal")]
public class ObjEnmPotosMoveMetal : ObjEnmPotosMove
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
