using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmGaniganiMetal")]
public class ObjEnmGaniganiMetal : ObjEnmGanigani
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.METAL;
	}
}
