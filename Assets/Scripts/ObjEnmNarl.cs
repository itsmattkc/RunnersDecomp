using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmNarl")]
public class ObjEnmNarl : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmNarlData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmNarlData.GetEffectSize();
	}
}
