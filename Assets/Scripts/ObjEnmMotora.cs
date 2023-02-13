using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmMotora")]
public class ObjEnmMotora : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmMotoraData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmMotoraData.GetEffectSize();
	}
}
