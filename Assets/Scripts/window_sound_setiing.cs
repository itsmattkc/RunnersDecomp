using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class window_sound_setiing : WindowBase
{
	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UISlider m_BGMSlider;

	[SerializeField]
	private UISlider m_SESlider;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_headerSubTextLabel;

	[SerializeField]
	private UILabel m_BGMTextLabel;

	[SerializeField]
	private UILabel m_SETextLabel;

	[SerializeField]
	private int m_tickMarkNum = 6;

	private List<float> m_tickMarkValue = new List<float>();

	private UIPlayAnimation m_uiAnimation;

	private int m_preBgmVolume;

	private int m_preSeVolume;

	private SoundManager.PlayId m_playID;

	private bool m_isEnd;

	private bool m_isOverwrite;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public bool IsOverwrite
	{
		get
		{
			return m_isOverwrite;
		}
	}

	private void Start()
	{
		OptionMenuUtility.TranObj(base.gameObject);
		if (m_BGMSlider != null)
		{
			m_BGMSlider.value = SoundManager.BgmVolume;
			EventDelegate.Add(m_BGMSlider.onChange, OnChangeBGMSlider);
		}
		if (m_SESlider != null)
		{
			m_SESlider.value = SoundManager.SeVolume;
			EventDelegate.Add(m_SESlider.onChange, OnChangeSESlider);
		}
		m_preBgmVolume = Mathf.Clamp((int)(SoundManager.BgmVolume * 100f), 0, 100);
		m_preSeVolume = Mathf.Clamp((int)(SoundManager.SeVolume * 100f), 0, 100);
		float num = 1f / (float)m_tickMarkNum;
		if (num > 0f)
		{
			for (int i = 0; i < m_tickMarkNum; i++)
			{
				if (i != m_tickMarkNum - 1)
				{
					float item = num * (float)(i + 1);
					m_tickMarkValue.Add(item);
				}
				else
				{
					m_tickMarkValue.Add(1f);
				}
			}
		}
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "sound_config");
		TextUtility.SetCommonText(m_headerSubTextLabel, "Option", "sound_config_info");
		TextUtility.SetCommonText(m_BGMTextLabel, "Option", "sound_bgm");
		TextUtility.SetCommonText(m_SETextLabel, "Option", "sound_se");
		if (m_closeBtn != null)
		{
			UIPlayAnimation component = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedAnimationCallback, false);
			}
			UIButtonMessage component2 = m_closeBtn.GetComponent<UIButtonMessage>();
			if (component2 == null)
			{
				m_closeBtn.AddComponent<UIButtonMessage>();
				component2 = m_closeBtn.GetComponent<UIButtonMessage>();
			}
			if (component2 != null)
			{
				component2.enabled = true;
				component2.trigger = UIButtonMessage.Trigger.OnClick;
				component2.target = base.gameObject;
				component2.functionName = "OnClickCloseButton";
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component3 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component3;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		SoundManager.SePlay("sys_window_open");
	}

	private void Update()
	{
	}

	private void OnChangeBGMSlider()
	{
		SoundManager.BgmVolume = m_BGMSlider.value;
	}

	private void OnChangeSESlider()
	{
		float seVolume = SoundManager.SeVolume;
		SoundManager.SeVolume = m_SESlider.value;
		CheckSEPlay(seVolume, m_SESlider.value);
	}

	private void CheckSEPlay(float preValue, float currentValue)
	{
		int num = -1;
		for (int i = 0; i < m_tickMarkNum; i++)
		{
			if (currentValue >= m_tickMarkValue[i])
			{
				num = i;
			}
		}
		int num2 = -1;
		for (int j = 0; j < m_tickMarkNum; j++)
		{
			if (preValue >= m_tickMarkValue[j])
			{
				num2 = j;
			}
		}
		if (num != num2 && (preValue != 1f || num2 - num != 1))
		{
			if (m_playID != 0)
			{
				SoundManager.SeStop(m_playID);
			}
			m_playID = SoundManager.SePlay("obj_ring");
		}
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
		int num = Mathf.Clamp((int)(SoundManager.BgmVolume * 100f), 0, 100);
		int num2 = Mathf.Clamp((int)(SoundManager.SeVolume * 100f), 0, 100);
		if (num != m_preBgmVolume || num2 != m_preSeVolume)
		{
			m_isOverwrite = true;
		}
		if (!m_isOverwrite)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.bgmVolume = num;
				systemdata.seVolume = num2;
			}
		}
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		m_isOverwrite = false;
		m_preBgmVolume = Mathf.Clamp((int)(SoundManager.BgmVolume * 100f), 0, 100);
		m_preSeVolume = Mathf.Clamp((int)(SoundManager.SeVolume * 100f), 0, 100);
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.SendMessage("OnClick");
		}
	}
}
