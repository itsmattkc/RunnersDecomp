public class ObjBossZomom1 : ObjBossZazz1
{
	private const string ModelName = "enm_zazz";

	protected override string GetModelName()
	{
		return "enm_zazz";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.EVENT_RESOURCE;
	}
}
