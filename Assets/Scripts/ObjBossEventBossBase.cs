public class ObjBossEventBossBase : ObjBossBase
{
	protected virtual BossType GetBossType()
	{
		return BossType.NONE;
	}

	protected int GetEventBossLevel()
	{
		int num = 0;
		if (RaidBossInfo.currentRaidData != null)
		{
			num = RaidBossInfo.currentRaidData.lv;
		}
		if (num > (int)EventBossParamTable.GetItemData(EventBossParamTableItem.Level4))
		{
			return 5;
		}
		if (num > (int)EventBossParamTable.GetItemData(EventBossParamTableItem.Level3))
		{
			return 4;
		}
		if (num > (int)EventBossParamTable.GetItemData(EventBossParamTableItem.Level2))
		{
			return 3;
		}
		if (num > (int)EventBossParamTable.GetItemData(EventBossParamTableItem.Level1))
		{
			return 2;
		}
		return 1;
	}

	protected int GetEventBossHpMax()
	{
		int result = 10;
		if (RaidBossInfo.currentRaidData != null)
		{
			result = (int)RaidBossInfo.currentRaidData.hpMax;
		}
		return result;
	}
}
