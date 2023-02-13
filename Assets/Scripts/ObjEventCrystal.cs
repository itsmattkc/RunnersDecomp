public class ObjEventCrystal : ObjEventCrystalBase
{
	protected override string GetModelName()
	{
		return EventSPStageObjectTable.GetSPCrystalModel();
	}

	protected override int GetAddCount()
	{
		return 1;
	}

	protected override EventCtystalType GetOriginalType()
	{
		return EventCtystalType.SMALL;
	}
}
