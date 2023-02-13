using AnimationOrTween;
using System.Collections;
using Text;
using UnityEngine;

public class HudNetworkConnect : MonoBehaviour
{
	public enum DisplayType
	{
		ALL,
		NO_BG,
		ONLY_ICON,
		LOADING
	}

	private GameObject m_object;

	private UISprite m_imgRing;

	private UISprite m_imgBg;

	private UILabel m_labelConnect;

	private UILabel m_labelConnectSdw;

	private bool m_isPlaying;

	private int m_refCount;

	private void Start()
	{
		Setup();
		base.enabled = false;
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		Debug.Log("HudNetworkConnct is Destroyed");
	}

	public GameObject Setup()
	{
		if (m_object != null)
		{
			return m_object;
		}
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			m_object = GameObjectUtil.FindChildGameObject(cameraUIObject, "ConnectAlertUI");
		}
		if (m_object != null)
		{
			m_imgRing = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_object, "img_ring");
			m_imgBg = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_object, "img_bg");
			m_labelConnect = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_object, "Lbl_conect_condition");
			if (m_imgRing != null)
			{
				m_imgRing.enabled = false;
			}
			if (m_imgBg != null)
			{
				m_imgBg.enabled = false;
			}
			if (m_labelConnect != null)
			{
				m_labelConnect.enabled = false;
				m_labelConnectSdw = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_labelConnect.gameObject, "Lbl_conect_condition_sh");
			}
			if (m_labelConnectSdw != null)
			{
				m_labelConnectSdw.enabled = false;
			}
		}
		return m_object;
	}

	public void PlayStart(DisplayType displayType)
	{
		if (m_refCount > 0)
		{
			return;
		}
		m_refCount++;
		base.enabled = true;
		if (m_object == null)
		{
			return;
		}
		m_object.SetActive(true);
		m_isPlaying = true;
		string empty = string.Empty;
		empty = ((displayType != DisplayType.LOADING) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "common", "ui_Lbl_connect").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "MainMenuLoading", "Lbl_conect_condition").text);
		if (m_labelConnect != null)
		{
			m_labelConnect.text = empty;
			if (m_labelConnectSdw != null)
			{
				m_labelConnectSdw.text = empty;
			}
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_object, "window_bg");
		BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(m_object, "window_bg_colider");
		Animation animation = null;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_object, "Anchor_9_BR");
		if (gameObject != null)
		{
			animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "Animation");
		}
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, Direction.Forward, true);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, InAnimFinishCallback, true);
			}
			else
			{
				InAnimFinishCallback();
			}
		}
		switch (displayType)
		{
		case DisplayType.ALL:
			if (uISprite != null)
			{
				uISprite.enabled = true;
			}
			if (boxCollider != null)
			{
				boxCollider.enabled = true;
			}
			break;
		case DisplayType.NO_BG:
			if (uISprite != null)
			{
				uISprite.enabled = false;
			}
			if (boxCollider != null)
			{
				boxCollider.enabled = true;
			}
			break;
		case DisplayType.ONLY_ICON:
			if (uISprite != null)
			{
				uISprite.enabled = false;
			}
			if (boxCollider != null)
			{
				boxCollider.enabled = false;
			}
			break;
		}
		StartCoroutine(OnWaitDisplayAnchor9());
	}

	private IEnumerator OnWaitDisplayAnchor9()
	{
		int count = 1;
		while (count > 0)
		{
			count--;
			yield return null;
		}
		if (m_imgRing != null)
		{
			m_imgRing.enabled = true;
		}
		if (m_imgBg != null)
		{
			m_imgBg.enabled = true;
		}
		if (m_labelConnectSdw != null)
		{
			m_labelConnectSdw.enabled = true;
		}
		if (m_labelConnect != null)
		{
			m_labelConnect.enabled = true;
		}
	}

	public void PlayEnd()
	{
		m_refCount--;
		if (m_refCount <= 0)
		{
			m_refCount = 0;
			StartCoroutine(OnPlayEnd());
		}
	}

	private IEnumerator OnPlayEnd()
	{
		while (m_isPlaying)
		{
			yield return null;
		}
		Animation animation2 = null;
		ActiveAnimation anim = null;
		if (m_object != null)
		{
			GameObject anchor9 = GameObjectUtil.FindChildGameObject(m_object, "Anchor_9_BR");
			if (anchor9 != null)
			{
				animation2 = GameObjectUtil.FindChildGameObjectComponent<Animation>(anchor9, "Animation");
				if (animation2 != null)
				{
					anim = ActiveAnimation.Play(animation2, Direction.Reverse, true);
				}
			}
		}
		if (anim != null)
		{
			EventDelegate.Add(anim.onFinished, OutAnimFinishCallback, true);
		}
		else
		{
			OutAnimFinishCallback();
		}
		base.enabled = false;
	}

	private void InAnimFinishCallback()
	{
		m_isPlaying = false;
	}

	private void OutAnimFinishCallback()
	{
		if (m_object != null)
		{
			m_object.SetActive(false);
			m_object = null;
		}
	}
}
