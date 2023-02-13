public class ObjEventCrystal10 : ObjEventCrystalBase
{
	protected override string GetModelName()
	{
		return EventSPStageObjectTable.GetItemData(EventSPStageObjectTableItem.SPCrystal10Model);
	}

	protected override int GetAddCount()
	{
		return 10;
	}

	protected override EventCtystalType GetOriginalType()
	{
		return EventCtystalType.BIG;
	}
}
