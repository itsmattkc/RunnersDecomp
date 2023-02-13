using GameScore;

public class ObjEventCrystalData
{
	private static readonly CtystalParam[] PARAM_TBL = new CtystalParam[2]
	{
		new CtystalParam(false, "ObjEventCrystal", string.Empty, "ef_ob_get_crystal_rd_s01", "obj_crystal_red", Data.EventCrystal, false),
		new CtystalParam(true, "ObjEventCrystal10", string.Empty, "ef_ob_get_crystal_rd_l01", "obj_big_crystal", Data.EventCrystal10, false)
	};

	public static CtystalParam GetCtystalParam(EventCtystalType type)
	{
		if ((uint)type < (uint)PARAM_TBL.Length)
		{
			return PARAM_TBL[(int)type];
		}
		return PARAM_TBL[0];
	}
}
