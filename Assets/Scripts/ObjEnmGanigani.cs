using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Enemy/ObjEnmGanigani")]
public class ObjEnmGanigani : ObjEnemyConstant
{
	protected override ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected override string[] GetModelFiles()
	{
		return ObjEnmGaniganiData.GetModelFiles();
	}

	protected override ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnmGaniganiData.GetEffectSize();
	}

	protected override Vector3 GetConstantAngle()
	{
		return base.transform.right;
	}

	protected override bool IsNormalMotion(float speed)
	{
		if (speed < 0f)
		{
			return false;
		}
		return true;
	}
}
