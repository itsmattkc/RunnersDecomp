public class ObjEnmValkyneData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_valkyne",
		"enm_valkyne_m",
		"enm_valkyne_g"
	};

	public static string[] GetModelFiles()
	{
		return MODEL_FILES;
	}

	public static ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnemyUtil.EnemyEffectSize.S;
	}
}
