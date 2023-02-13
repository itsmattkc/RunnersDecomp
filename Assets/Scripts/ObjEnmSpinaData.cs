public class ObjEnmSpinaData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_spina",
		"enm_spina_m",
		"enm_spina_g"
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
