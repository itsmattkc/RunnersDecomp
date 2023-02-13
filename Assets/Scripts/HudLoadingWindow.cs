using AnimationOrTween;
using UnityEngine;

public class HudLoadingWindow : MonoBehaviour
{
	private enum EventSignal
	{
		SIG_PLAYSTART = 100,
		SIG_PLAYEND
	}

	public const float TEXTURE_IMAGE_SCALE = 55f / 64f;

	private TinyFsmBehavior m_fsm;

	private float m_lastClick;

	private float m_parcentage;

	[SerializeField]
	private float m_charaDisplayTime = 10f;

	private float m_currentDisplayTime;

	private HudLoadingCharaInfo m_charaInfo;

	[SerializeField]
	private Texture2D defultImage;

	public void PlayStart()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			UIPanel component = base.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				component.alpha = 1f;
			}
			m_fsm.Dispatch(signal);
		}
		m_charaInfo.enabled = true;
	}

	public void SetLoadingPercentage(float parcentage)
	{
		if (parcentage > 100f)
		{
			parcentage = 100f;
		}
		m_parcentage = parcentage;
	}

	public void PlayEnd()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void Start()
	{
		m_lastClick = 0f;
		m_fsm = base.gameObject.AddComponent<TinyFsmBehavior>();
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateSetup);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
		m_charaInfo = base.gameObject.AddComponent<HudLoadingCharaInfo>();
		m_charaInfo.enabled = false;
	}

	private void OnDestroy()
	{
		if (m_charaInfo != null)
		{
			Object.Destroy(m_charaInfo.gameObject);
			m_charaInfo = null;
		}
	}

	private void Update()
	{
	}

	private TinyFsmState StateSetup(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_lastClick = 0f;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			base.gameObject.SetActive(false);
			SetCharaExplainActive(false);
			SetLoadingBarParcentage(m_parcentage);
			m_fsm.ChangeState(new TinyFsmState(StateIdle));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
		{
			base.gameObject.SetActive(true);
			Animation component = base.gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation.Play(component, Direction.Forward);
			}
			m_currentDisplayTime = m_charaDisplayTime;
			m_fsm.ChangeState(new TinyFsmState(StateLoading));
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoading(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			SetLoadingBarParcentage(m_parcentage);
			m_currentDisplayTime += Time.deltaTime;
			if (m_currentDisplayTime >= m_charaDisplayTime)
			{
				m_currentDisplayTime = m_charaDisplayTime;
				if (ChangeCharaExplain())
				{
					m_currentDisplayTime = 0f;
				}
			}
			return TinyFsmState.End();
		case 101:
		{
			SetLoadingBarParcentage(100f);
			Animation component = base.gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation.Play(component, Direction.Reverse);
			}
			m_fsm.ChangeState(new TinyFsmState(StateIdle));
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private void SetLoadingBarParcentage(float parcentage)
	{
		UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Progress_Bar");
		if (uISlider != null)
		{
			uISlider.value = parcentage / 100f;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_progress");
		if (uILabel != null)
		{
			uILabel.text = (int)parcentage + "%";
		}
	}

	private bool ChangeCharaExplain()
	{
		if (m_charaInfo == null)
		{
			return false;
		}
		if (!m_charaInfo.IsReady())
		{
			return false;
		}
		SetCharaExplainActive(true);
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_event_tex");
		if (uITexture != null)
		{
			Texture2D charaPicture = m_charaInfo.GetCharaPicture();
			if (charaPicture != null)
			{
				bool flag = false;
				if (charaPicture.width < 150 || charaPicture.height < 150)
				{
					flag = true;
				}
				if (!flag)
				{
					uITexture.mainTexture = charaPicture;
					uITexture.width = (int)((float)uITexture.mainTexture.width * (55f / 64f));
					uITexture.height = (int)((float)uITexture.mainTexture.height * (55f / 64f));
				}
				else
				{
					uITexture.mainTexture = defultImage;
					uITexture.width = 220;
					uITexture.height = 220;
				}
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_tex_flame");
				if (uITexture != null)
				{
					uISprite.width = uITexture.width + 4;
					uISprite.height = uITexture.height + 6;
				}
			}
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_name");
		if (uILabel != null)
		{
			uILabel.text = m_charaInfo.GetCharaName();
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_help");
		if (uILabel2 != null)
		{
			uILabel2.text = m_charaInfo.GetCharaExplain();
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption_sub");
		if (uILabel3 != null)
		{
			uILabel3.text = m_charaInfo.GetCharaExplainCaption();
		}
		m_charaInfo.GoNext();
		return true;
	}

	private void SetCharaExplainActive(bool isActive)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_event_tex");
		if (gameObject != null)
		{
			gameObject.SetActive(isActive);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_name");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(isActive);
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_help");
		if (gameObject3 != null)
		{
			gameObject3.SetActive(isActive);
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_caption_sub");
		if (gameObject4 != null)
		{
			gameObject4.SetActive(isActive);
		}
	}

	public void OnPressBg()
	{
		if (Time.realtimeSinceStartup >= m_lastClick + 0.05f && m_currentDisplayTime > 0.5f)
		{
			m_currentDisplayTime = m_charaDisplayTime;
			m_lastClick = Time.realtimeSinceStartup;
		}
	}
}
