using System.Collections.Generic;
using System.Runtime.InteropServices;

public struct QuickModeActStartNativeParam
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
	public int[] modifire;

	public int m_tutorial;

	public void Init(List<int> itemList, int tutorial)
	{
		modifire = itemList.ToArray();
		m_tutorial = tutorial;
	}
}
