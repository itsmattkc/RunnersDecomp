using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmSpina")]
public class ObjEnmSpina : ObjEnemySwing
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmSpinaData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmSpinaData.GetEffectSize();
	}
}
