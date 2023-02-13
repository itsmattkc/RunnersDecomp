public class ObjBossZazz2 : ObjBossZazz1
{
	private const string ModelName = "enm_zazz_r";

	protected override string GetModelName()
	{
		return "enm_zazz_r";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.EVENT_RESOURCE;
	}

	protected override BossType GetBossType()
	{
		return BossType.EVENT2;
	}
}
