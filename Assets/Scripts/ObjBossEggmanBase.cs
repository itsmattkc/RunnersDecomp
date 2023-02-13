using Message;
using UnityEngine;

public class ObjBossEggmanBase : ObjBossBase
{
	private const string ModelName = "enm_eggmobile";

	protected override string GetModelName()
	{
		return "enm_eggmobile";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.ENEMY_RESOURCE;
	}

	protected int GetMapBossLevel()
	{
		MsgGetMileageMapState msgGetMileageMapState = new MsgGetMileageMapState();
		GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnGetMileageMapState", msgGetMileageMapState, SendMessageOptions.DontRequireReceiver);
		if (msgGetMileageMapState.m_succeed && msgGetMileageMapState.m_mileageMapState != null)
		{
			int episode = msgGetMileageMapState.m_mileageMapState.m_episode;
			if (episode > 40)
			{
				return 5;
			}
			if (episode > 30)
			{
				return 4;
			}
			if (episode > 20)
			{
				return 3;
			}
			if (episode > 10)
			{
				return 2;
			}
		}
		return 1;
	}
}
