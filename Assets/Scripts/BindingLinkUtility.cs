public class BindingLinkUtility
{
	public const int MaxEquipItems = 6;

	public const int MaxFriends = 50;

	public const int MaxApolloParamCount = 100;

	public static void LongToIntArray(out int[] output, long input)
	{
		output = new int[2];
		int num = 32;
		int num2 = (int)(input >> num);
		int num3 = (int)(input << num >> num);
		output[0] = num2;
		output[1] = num3;
	}
}
