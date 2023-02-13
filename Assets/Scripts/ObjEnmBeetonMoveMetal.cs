using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmBeetonMoveMetal")]
public class ObjEnmBeetonMoveMetal : ObjEnmBeetonMove
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
