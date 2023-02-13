using AnimationOrTween;
using System.Collections;
using UnityEngine;

public class DailyWindowUI : WindowBase
{
	[SerializeField]
	public bool m_isDebug;

	public bool m_isClickClose;

	public bool m_isEnd;

	private bool m_isDisplay;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
		base.enabled = false;
		m_isDisplay = false;
	}

	private void OnDestroy()
	{
		Destroy();
	}

	public void PlayStart()
	{
		base.gameObject.SetActive(true);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "daily_window");
		if (gameObject != null)
		{
			gameObject.SetActive(true);
			Animation component = gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation.Play(component, "ui_cmn_window_Anim", Direction.Forward);
			}
			StartCoroutine(DisplayProgressBar());
		}
		m_isEnd = false;
		m_isClickClose = false;
	}

	private IEnumerator DisplayProgressBar()
	{
		yield return null;
		GameObject dailyChallengeObj = GameObjectUtil.FindChildGameObject(base.gameObject, "daily_challenge");
		if (!(dailyChallengeObj != null))
		{
			yield break;
		}
		long progress = -1L;
		if (SaveDataManager.Instance != null && !m_isDisplay)
		{
			DailyMissionData nowData = SaveDataManager.Instance.PlayerData.DailyMission;
			DailyMissionData beforeData = SaveDataManager.Instance.PlayerData.BeforeDailyMissionData;
			if (nowData.date == beforeData.date && nowData.id == beforeData.id)
			{
				progress = beforeData.progress;
				m_isDisplay = true;
			}
		}
		dailyChallengeObj.SendMessage("OnStartDailyMissionInMileageMap", progress, SendMessageOptions.DontRequireReceiver);
	}

	private void OnClickNextButton()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "daily_challenge");
		if (gameObject != null)
		{
			SoundManager.SePlay("sys_menu_decide");
			gameObject.SendMessage("OnClickNextButton", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		m_isClickClose = true;
	}

	public void OnClosedWindowAnim()
	{
		SoundManager.SeStop("sys_gauge");
		base.gameObject.SetActive(false);
		m_isEnd = true;
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isEnd)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (m_isClickClose || daily_challenge.isUpdateEffect)
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_next");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
