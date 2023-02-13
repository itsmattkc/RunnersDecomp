using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmValkyne")]
public class ObjEnmValkyne : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmValkyneData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmValkyneData.GetEffectSize();
	}

	protected override void OnSpawned()
	{
		base.OnSpawned();
	}
}
