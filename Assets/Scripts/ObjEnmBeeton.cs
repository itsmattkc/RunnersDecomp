using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmBeeton")]
public class ObjEnmBeeton : ObjEnemySwing
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmBeetonData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmBeetonData.GetEffectSize();
	}
}
