using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmEggpawn")]
public class ObjEnmEggpawn : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmEggpawnData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmEggpawnData.GetEffectSize();
	}
}
