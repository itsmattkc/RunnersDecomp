public class ObjCrystalUtil
{
	public static CtystalType GetCrystalModelType(CtystalType originalType)
	{
		StageComboManager instance = StageComboManager.Instance;
		if (instance != null)
		{
			bool flag = ObjCrystalData.IsBig(originalType);
			if (instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_SMALL_CRYSTAL_RED) && !flag)
			{
				return CtystalType.SMALL_C;
			}
			if (instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_BIG_CRYSTAL_RED) && flag)
			{
				return CtystalType.BIG_C;
			}
			if (instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_SMALL_CRYSTAL_GREEN) && !flag && originalType == CtystalType.SMALL_A)
			{
				return CtystalType.SMALL_B;
			}
			if (instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_BIG_CRYSTAL_GREEN) && flag && originalType == CtystalType.BIG_A)
			{
				return CtystalType.BIG_B;
			}
		}
		return originalType;
	}
}
