using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmSpinaMoveMetal")]
public class ObjEnmSpinaMoveMetal : ObjEnmSpinaMove
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
