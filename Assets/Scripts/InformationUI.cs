using AnimationOrTween;
using UnityEngine;

public class InformationUI : MonoBehaviour
{
	[SerializeField]
	public GameObject m_newsSet;

	private void Start()
	{
		base.enabled = false;
	}

	private void PlayAnimation(bool inAnim)
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (!(component != null))
		{
			return;
		}
		Direction playDirection = inAnim ? Direction.Forward : Direction.Reverse;
		ActiveAnimation activeAnimation = ActiveAnimation.Play(component, "ui_daily_challenge_infomation_intro_Anim", playDirection);
		if (activeAnimation != null)
		{
			if (inAnim)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedInAnimationCallback, true);
			}
			else
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedOutAnimationCallback, true);
			}
		}
	}

	private void OnFinishedInAnimationCallback()
	{
		HudMenuUtility.SendUIPageStart();
	}

	private void OnFinishedOutAnimationCallback()
	{
		HudMenuUtility.SendUIPageEnd();
	}

	private void OnClickBackButton()
	{
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.INFOMATION_BACK);
	}

	private void OnStartInformation()
	{
		if (m_newsSet != null)
		{
			ui_mm_news_page component = m_newsSet.GetComponent<ui_mm_news_page>();
			if (component != null)
			{
				component.StartInformation();
			}
		}
	}

	private void OnEndInformation()
	{
		HudMenuUtility.SendMsgInformationDisplay();
		ServerInterface.NoticeInfo.SaveInformation();
		if (InformationImageManager.Instance != null)
		{
			InformationImageManager.Instance.ClearWinowImage();
		}
	}
}
