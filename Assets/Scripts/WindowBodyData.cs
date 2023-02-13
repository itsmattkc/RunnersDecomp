public class WindowBodyData
{
	public int face_count;

	public WindowProductData[] product;

	public ArrowType arrow;

	public string bgm;

	public string se;

	public string text_cell_id;

	public bool IsBgmStop()
	{
		return IsBgmStop(bgm);
	}

	public static bool IsBgmStop(string bgm)
	{
		if (bgm != null)
		{
			return bgm == "@stop";
		}
		return false;
	}
}
