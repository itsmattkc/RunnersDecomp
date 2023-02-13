public class StageBlockUtil
{
	public static bool IsPastPosition(float pos, float basePos, float distanceOfPast)
	{
		float num = basePos - pos;
		if (num > distanceOfPast)
		{
			return true;
		}
		return false;
	}
}
