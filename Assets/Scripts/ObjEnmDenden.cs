using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmDenden")]
public class ObjEnmDenden : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmDendenData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmDendenData.GetEffectSize();
	}
}
