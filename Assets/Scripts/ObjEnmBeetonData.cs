public class ObjEnmBeetonData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_beeton",
		"enm_beeton_m",
		"enm_beeton_g"
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
