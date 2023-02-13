using AnimationOrTween;
using System.Collections;
using UnityEngine;

public class ButtonEventAnimation : MonoBehaviour
{
	public delegate void AnimationEndCallback();

	private GameObject m_menu_anim_obj;

	private ButtonInfoTable m_info_table = new ButtonInfoTable();

	private AnimationEndCallback m_inAnimEndCallback;

	private AnimationEndCallback m_outAnimEndCallback;

	private ButtonInfoTable.PageType m_currentPageType;

	public void Initialize()
	{
		m_menu_anim_obj = HudMenuUtility.GetMenuAnimUIObject();
	}

	public void PageOutAnimation(ButtonInfoTable.PageType currentPageType, ButtonInfoTable.PageType nextPageType, AnimationEndCallback animEndCallback)
	{
		m_currentPageType = currentPageType;
		m_outAnimEndCallback = animEndCallback;
		ButtonInfoTable.AnimInfo pageAnimInfo = m_info_table.GetPageAnimInfo(currentPageType);
		if (pageAnimInfo == null)
		{
			OnFinishedOutAnimationCallback();
		}
		else if (nextPageType == ButtonInfoTable.PageType.STAGE)
		{
			SetOutAnimation(new ButtonInfoTable.AnimInfo("ItemSet_3_UI", "ui_itemset_3_outro_Anim"), false);
		}
		else
		{
			SetOutAnimation(pageAnimInfo, true);
		}
	}

	public void PageInAnimation(ButtonInfoTable.PageType nextPageType, AnimationEndCallback animEndCallback)
	{
		m_inAnimEndCallback = animEndCallback;
		ButtonInfoTable.AnimInfo pageAnimInfo = m_info_table.GetPageAnimInfo(nextPageType);
		StartCoroutine(SetInAnimationCoroutine(pageAnimInfo, false));
	}

	private void SetOutAnimation(ButtonInfoTable.AnimInfo animInfo, bool reverseFlag)
	{
		if (animInfo != null && animInfo.animName != null)
		{
			Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_menu_anim_obj, animInfo.targetName);
			if (animation != null)
			{
				if (animInfo.animName == "ui_mm_Anim")
				{
					reverseFlag = !reverseFlag;
				}
				Direction playDirection = (!reverseFlag) ? Direction.Forward : Direction.Reverse;
				ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, animInfo.animName, playDirection);
				if (activeAnimation != null)
				{
					EventDelegate.Add(activeAnimation.onFinished, OnFinishedOutAnimationCallback, true);
				}
			}
			else
			{
				OnFinishedOutAnimationCallback();
			}
		}
		else if (animInfo.targetName == "RouletteTopUI")
		{
			StartCoroutine(WaitRouletteClose());
		}
		else
		{
			OnFinishedOutAnimationCallback();
		}
	}

	private IEnumerator SetInAnimationCoroutine(ButtonInfoTable.AnimInfo animInfo, bool reverseFlag)
	{
		InitInAnimation(animInfo);
		yield return null;
		yield return null;
		if (animInfo != null)
		{
			yield return StartCoroutine(DelayPlayAnimation(animInfo, reverseFlag));
		}
		else
		{
			OnFinishedInAnimationCallback();
		}
	}

	private void InitInAnimation(ButtonInfoTable.AnimInfo animInfo)
	{
		if (animInfo == null)
		{
			return;
		}
		bool flag = false;
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_menu_anim_obj, animInfo.targetName);
		if (animation != null)
		{
			if (animInfo.animName == "ui_mm_Anim")
			{
				flag = !flag;
			}
			Direction playDirection = (!flag) ? Direction.Forward : Direction.Reverse;
			ActiveAnimation.Play(animation, animInfo.animName, playDirection);
			animation.Stop(animInfo.animName);
		}
	}

	public IEnumerator DelayPlayAnimation(ButtonInfoTable.AnimInfo animInfo, bool reverseFlag)
	{
		int waite_frame = 2;
		while (waite_frame > 0)
		{
			waite_frame--;
			yield return null;
		}
		if (animInfo == null)
		{
			yield break;
		}
		GameObject obj = GameObjectUtil.FindChildGameObject(m_menu_anim_obj, animInfo.targetName);
		if (!(obj != null))
		{
			yield break;
		}
		ShopUI shop = obj.GetComponent<ShopUI>();
		if (shop != null)
		{
			while (!shop.IsInitShop)
			{
				yield return null;
			}
		}
		if (obj.name == "RouletteTopUI")
		{
			RouletteManager.Instance.gameObject.SetActive(true);
			while (!RouletteManager.IsRouletteEnabled())
			{
				yield return null;
			}
		}
		if (obj.name == "DailyChallengeInformationUI")
		{
			daily_challenge dailyChallenge = GameObjectUtil.FindChildGameObjectComponent<daily_challenge>(obj, "daily_challenge");
			if (dailyChallenge != null)
			{
				while (!dailyChallenge.IsEndSetup)
				{
					yield return null;
				}
			}
		}
		ChaoSetUI chao = obj.GetComponent<ChaoSetUI>();
		if (chao != null)
		{
			while (!chao.IsEndSetup)
			{
				yield return null;
			}
		}
		ItemSetMenu item = obj.GetComponent<ItemSetMenu>();
		if (item != null)
		{
			while (!item.IsEndSetup)
			{
				yield return null;
			}
		}
		MenuPlayerSet player = obj.GetComponent<MenuPlayerSet>();
		if (player != null)
		{
			while (!player.SetUpped)
			{
				yield return null;
			}
		}
		OptionUI option = obj.GetComponent<OptionUI>();
		if (option != null)
		{
			while (!option.IsEndSetup)
			{
				yield return null;
			}
		}
		PresentBoxUI presentBox = obj.GetComponent<PresentBoxUI>();
		if (presentBox != null)
		{
			while (!presentBox.IsEndSetup)
			{
				yield return null;
			}
		}
		DailyInfo dailyInfo = obj.GetComponent<DailyInfo>();
		if (dailyInfo != null)
		{
			yield return null;
		}
		if (animInfo.animName != null)
		{
			Animation anim = obj.GetComponent<Animation>();
			if (anim != null)
			{
				if (animInfo.animName == "ui_mm_Anim")
				{
					reverseFlag = !reverseFlag;
				}
				Direction dire = (!reverseFlag) ? Direction.Forward : Direction.Reverse;
				ActiveAnimation acviteAnim = ActiveAnimation.Play(anim, animInfo.animName, dire);
				if (acviteAnim != null)
				{
					EventDelegate.Add(acviteAnim.onFinished, OnFinishedInAnimationCallback, true);
				}
			}
		}
		else
		{
			OnFinishedInAnimationCallback();
		}
	}

	public IEnumerator WaitRouletteClose()
	{
		while (!RouletteManager.IsRouletteClose())
		{
			yield return null;
		}
		OnFinishedOutAnimationCallback();
	}

	private void OnFinishedOutAnimationCallback()
	{
		if (m_outAnimEndCallback != null)
		{
			m_outAnimEndCallback();
			m_outAnimEndCallback = null;
		}
	}

	private void OnFinishedInAnimationCallback()
	{
		if (m_inAnimEndCallback != null)
		{
			m_inAnimEndCallback();
			m_inAnimEndCallback = null;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
