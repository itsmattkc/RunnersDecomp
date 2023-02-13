using System.Collections.Generic;
using System.Runtime.InteropServices;

public struct ActStartNativeParam
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
	public int[] modifire;

	public bool tutorial;

	public int eventId;

	public void Init(List<int> itemList, bool tutorial, int eventId)
	{
		this.modifire = itemList.ToArray();
		this.tutorial = tutorial;
		this.eventId = eventId;
	}
}
