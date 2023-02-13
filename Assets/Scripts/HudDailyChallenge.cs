using DataTable;
using Text;
using UnityEngine;

public class HudDailyChallenge : MonoBehaviour
{
	public void OnUpdateSaveDataDisplay()
	{
		UpdateChallengePanel();
	}

	private void UpdateChallengePanel()
	{
		int num = 0;
		if (SaveDataManager.Instance != null)
		{
			int id = SaveDataManager.Instance.PlayerData.DailyMission.id;
			MissionData missionData = MissionTable.GetMissionData(id);
			if (missionData != null && missionData.quota > 0)
			{
				double num2 = (double)SaveDataManager.Instance.PlayerData.DailyMission.progress / (double)missionData.quota * 100.0;
				if (num2 > 100.0)
				{
					num2 = 100.0;
				}
				num = (int)num2;
			}
		}
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (mainMenuUIObject == null)
		{
			return;
		}
		Transform transform = mainMenuUIObject.transform.FindChild("Anchor_9_BR/Btn_1_challenge");
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		if (gameObject != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_player_main_lv");
			if (uILabel != null)
			{
				uILabel.text = TextUtility.GetCommonText("ChaoSet", "bonus_percent", "{BONUS}", num.ToString());
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
