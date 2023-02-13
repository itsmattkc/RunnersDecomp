public class CameraTypeData
{
	private static readonly int[] GIMMICK_CMAERA_TBL = new int[6]
	{
		0,
		1,
		1,
		1,
		1,
		1
	};

	public static bool IsGimmickCamera(CameraType type)
	{
		if ((uint)type < 6u)
		{
			return GIMMICK_CMAERA_TBL[(int)type] == 1;
		}
		return false;
	}
}
