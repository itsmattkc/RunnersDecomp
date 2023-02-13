public class ObjEnmMotoraData
{
	private static readonly string[] MODEL_FILES = new string[3]
	{
		"enm_motora",
		"enm_motora_m",
		"enm_motora_g"
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
