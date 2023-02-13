using AnimationOrTween;
using DataTable;
using System.Collections.Generic;
using UnityEngine;

public class AttentionChaoWindow : MonoBehaviour
{
	private enum BUTTON_ACT
	{
		CLOSE,
		NONE
	}

	private enum Mode
	{
		Idle,
		Wait,
		End
	}

	private const int MAX_CHAO = 3;

	private List<int> m_attentionChaoIds;

	private Mode m_mode;

	private Animation m_animation;

	private bool m_close;

	private List<ChaoData> m_chaoData;

	private GameObject[] m_chaoWindow;

	public void Setup(List<int> chaoIds)
	{
		m_animation = GetComponentInChildren<Animation>();
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "blinder");
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "blinder");
		if (uIPlayAnimation != null && uIButtonMessage != null)
		{
			uIPlayAnimation.enabled = false;
			uIButtonMessage.enabled = false;
		}
		m_chaoWindow = new GameObject[3];
		for (int i = 0; i < 3; i++)
		{
			m_chaoWindow[i] = GameObjectUtil.FindChildGameObject(base.gameObject, "chao_window_" + i);
			m_chaoWindow[i].SetActive(false);
		}
		if (uILabel != null)
		{
			uILabel.text = "今週の目玉";
			uILabel.text = ObjectUtility.SetColorString(uILabel.text, 0, 0, 0);
		}
		if (chaoIds != null)
		{
			m_attentionChaoIds = chaoIds;
			int count = m_attentionChaoIds.Count;
			if (count > 0)
			{
				m_chaoData = ChaoTable.GetChaoData(m_attentionChaoIds);
				SetChao(0);
			}
		}
		m_mode = Mode.Wait;
	}

	private bool SetChao(int offset)
	{
		bool result = false;
		if (m_chaoData != null && m_chaoData.Count > 0 && offset >= 0 && m_chaoWindow != null)
		{
			int chaoLevel = ChaoTable.ChaoMaxLevel();
			for (int i = 0; i < 3; i++)
			{
				int num = i + 3 * offset;
				if (num < m_chaoData.Count)
				{
					m_chaoWindow[i].SetActive(true);
					ChaoData chaoData = m_chaoData[num];
					if (chaoData == null)
					{
						continue;
					}
					UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_chaoWindow[i].gameObject, "Lbl_name");
					UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_chaoWindow[i].gameObject, "Lbl_text");
					UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_chaoWindow[i].gameObject, "Img_bg");
					UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_chaoWindow[i].gameObject, "Tex_chao");
					if (uILabel != null)
					{
						uILabel.text = chaoData.nameTwolines;
						uILabel.text = ObjectUtility.SetColorString(uILabel.text, 0, 0, 0);
					}
					if (uILabel2 != null)
					{
						uILabel2.text = chaoData.GetDetailsLevel(chaoLevel);
						uILabel2.text = ObjectUtility.SetColorString(uILabel2.text, 0, 0, 0);
					}
					if (uISprite != null)
					{
						switch (chaoData.rarity)
						{
						case ChaoData.Rarity.NORMAL:
							uISprite.spriteName = "ui_tex_chao_bg_0";
							break;
						case ChaoData.Rarity.RARE:
							uISprite.spriteName = "ui_tex_chao_bg_1";
							break;
						case ChaoData.Rarity.SRARE:
							uISprite.spriteName = "ui_tex_chao_bg_2";
							break;
						}
					}
					if (uITexture != null)
					{
						ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
						ChaoTextureManager.Instance.GetTexture(chaoData.id, info);
					}
				}
				else
				{
					m_chaoWindow[i].SetActive(false);
				}
			}
		}
		return result;
	}

	public bool IsEnd()
	{
		if (m_mode == Mode.Wait)
		{
			return false;
		}
		return true;
	}

	public void OnClickNoButton()
	{
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickNoBgButton()
	{
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			m_mode = Mode.End;
			Object.Destroy(base.gameObject);
		}
	}

	public static AttentionChaoWindow Create(List<int> chaoIds)
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "AttentionChaoWindowUI(Clone)");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			GameObject original = Resources.Load("Prefabs/UI/AttentionChaoWindowUI") as GameObject;
			GameObject gameObject2 = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.SetActive(true);
			gameObject2.transform.parent = cameraUIObject.transform;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			AttentionChaoWindow attentionChaoWindow = null;
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "attention_window");
			if (gameObject3 != null)
			{
				attentionChaoWindow = gameObject3.AddComponent<AttentionChaoWindow>();
				if (attentionChaoWindow != null)
				{
					attentionChaoWindow.Setup(chaoIds);
				}
			}
			return attentionChaoWindow;
		}
		return null;
	}
}
