public class ObjBossEggmanParameter : ObjBossParameter
{
	protected override void OnSetup()
	{
		base.BossAttackPower = 1;
		if (base.TypeBoss == 0)
		{
			base.BossHP = base.BossHPMax;
			return;
		}
		int num = 0;
		LevelInformation levelInformation = ObjUtil.GetLevelInformation();
		if (levelInformation != null)
		{
			levelInformation.NumBossHpMax = base.BossHPMax;
			num = base.BossHPMax - levelInformation.NumBossAttack;
		}
		if (num < 1)
		{
			num = 1;
		}
		base.BossHP = num;
	}
}
