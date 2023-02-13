public class ObjEnmGaniganiData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_ganigani",
		"enm_ganigani_m",
		"enm_ganigani_g"
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
