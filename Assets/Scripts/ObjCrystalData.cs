using GameScore;

public class ObjCrystalData
{
	private static readonly CtystalParam[] PARAM_TBL = new CtystalParam[6]
	{
		new CtystalParam(false, "ObjCrystal_A", "obj_cmn_crystalA", "ef_ob_get_crystal_s01", "obj_crystal", Data.CrystalA, false),
		new CtystalParam(false, "ObjCrystal_B", "obj_cmn_crystalB", "ef_ob_get_crystal_gr_s01", "obj_crystal_green", Data.CrystalB, false),
		new CtystalParam(false, "ObjCrystal_C", "obj_cmn_crystalC", "ef_ob_get_crystal_rd_s01", "obj_crystal_red", Data.CrystalC, false),
		new CtystalParam(true, "ObjCrystal10_A", "obj_cmn_crystalA10", "ef_ob_get_crystal_l01", "obj_big_crystal", Data.Crystal10A, true),
		new CtystalParam(true, "ObjCrystal10_B", "obj_cmn_crystalB10", "ef_ob_get_crystal_gr_l01", "obj_big_crystal", Data.Crystal10B, true),
		new CtystalParam(true, "ObjCrystal10_C", "obj_cmn_crystalC10", "ef_ob_get_crystal_rd_l01", "obj_big_crystal", Data.Crystal10C, true)
	};

	public static CtystalParam GetCrystalParam(CtystalType type)
	{
		if ((uint)type < (uint)PARAM_TBL.Length)
		{
			return PARAM_TBL[(int)type];
		}
		return PARAM_TBL[0];
	}

	public static bool IsBig(CtystalType type)
	{
		if ((uint)type < (uint)PARAM_TBL.Length)
		{
			return PARAM_TBL[(int)type].m_big;
		}
		return PARAM_TBL[0].m_big;
	}

	public static string GetCrystalObjectName(CtystalType type)
	{
		if ((uint)type < (uint)PARAM_TBL.Length)
		{
			return PARAM_TBL[(int)type].m_objName;
		}
		return PARAM_TBL[0].m_objName;
	}
}
