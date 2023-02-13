public class ObjEnmEggpawnData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_eggpawn",
		"enm_eggpawn_m",
		"enm_eggpawn_g"
	};

	public static string[] GetModelFiles()
	{
		return MODEL_FILES;
	}

	public static ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnemyUtil.EnemyEffectSize.M;
	}
}
