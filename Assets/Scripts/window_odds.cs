using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class window_odds : WindowBase
{
	[SerializeField]
	private UIRectItemStorage m_oddsItemStorage;

	[SerializeField]
	private UILabel m_noteLabel;

	private List<string[]> m_oddsList;

	private string m_note;

	public void Init()
	{
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	public void Open(List<string[]> oddsList, string note)
	{
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, false);
		}
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(true);
			Animation component = gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation.Play(component, Direction.Forward);
			}
		}
		m_oddsList = oddsList;
		m_note = note;
		StartCoroutine(OpenCoroutine());
	}

	public void Open(ServerPrizeState prize, ServerWheelOptionsData data)
	{
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, false);
		}
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(true);
			Animation component = gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation.Play(component, Direction.Forward);
			}
		}
		m_oddsList = prize.GetItemOdds(data);
		m_note = prize.GetPrizeText(data);
		StartCoroutine(OpenCoroutine());
		RouletteManager.OpenRouletteWindow();
	}

	private IEnumerator OpenCoroutine()
	{
		while (!base.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		yield return null;
		SoundManager.SePlay("sys_window_open");
		if (m_oddsList != null)
		{
			m_oddsItemStorage.maxItemCount = (m_oddsItemStorage.maxRows = m_oddsList.Count);
		}
		else
		{
			m_oddsItemStorage.maxItemCount = (m_oddsItemStorage.maxRows = 0);
		}
		m_oddsItemStorage.Restart();
		ui_roulette_window_odds_scroll[] ui_roulette_window_odds_scrolls = m_oddsItemStorage.GetComponentsInChildren<ui_roulette_window_odds_scroll>(true);
		if (m_oddsList != null)
		{
			for (int i = 0; i < m_oddsItemStorage.maxItemCount; i++)
			{
				ui_roulette_window_odds_scrolls[i].UpdateView(m_oddsList[i][0], m_oddsList[i][1]);
			}
		}
		m_noteLabel.text = m_note;
	}

	private void OnClickCloseButton()
	{
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, true);
		}
		SoundManager.SePlay("sys_window_close");
		RouletteManager.CloseRouletteWindow();
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (base.gameObject.activeSelf)
		{
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_close");
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
	}
}
