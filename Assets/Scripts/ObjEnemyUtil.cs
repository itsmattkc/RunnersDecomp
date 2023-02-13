using GameScore;

public class ObjEnemyUtil
{
	public enum EnemyEffectSize : uint
	{
		S,
		M,
		L,
		NUM
	}

	public enum EnemyType : uint
	{
		NORMAL,
		METAL,
		RARE,
		NUM
	}

	private static readonly string[] EFFECT_FILES = new string[3]
	{
		"ef_en_dead_s01",
		"ef_en_dead_m01",
		"ef_en_dead_l01"
	};

	private static readonly string[] SE_NAMETBL = new string[3]
	{
		"enm_dead",
		"enm_metal_dead",
		"enm_gold_dead"
	};

	private static readonly int[] SOCRE_TABLE = new int[3]
	{
		Data.EnemyNormal,
		Data.EnemyMetal,
		Data.EnemyRare
	};

	public static int[] GetDefaultScoreTable()
	{
		return SOCRE_TABLE;
	}

	public static string GetEffectName(EnemyEffectSize size)
	{
		if ((long)size < (long)EFFECT_FILES.Length)
		{
			return EFFECT_FILES[(uint)size];
		}
		return string.Empty;
	}

	public static string GetSEName(EnemyType type)
	{
		if ((long)type < (long)SE_NAMETBL.Length)
		{
			return SE_NAMETBL[(uint)type];
		}
		return string.Empty;
	}

	public static string GetModelName(EnemyType type, string[] model_files)
	{
		if (model_files != null && (long)type < (long)model_files.Length)
		{
			return model_files[(uint)type];
		}
		return string.Empty;
	}

	public static int GetScore(EnemyType type, int[] score_table)
	{
		if ((long)type < (long)score_table.Length)
		{
			return score_table[(uint)type];
		}
		return 0;
	}
}
