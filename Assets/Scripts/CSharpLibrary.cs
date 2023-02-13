using System.Runtime.InteropServices;

public class CSharpLibrary
{
	[DllImport("CppLibrary")]
	private static extern float TestMultiply(float a, float b);

	[DllImport("CppLibrary")]
	private static extern float TestDivide(float a, float b);

	public static float CppMultiply(float a, float b)
	{
		return TestMultiply(a, b);
	}

	public static float CppDivide(float a, float b)
	{
		return TestDivide(a, b);
	}
}
