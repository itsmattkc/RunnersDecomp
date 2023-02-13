using AnimationOrTween;
using System.Collections;
using Text;
using UnityEngine;

public class PlayerGetWindowUI : MonoBehaviour
{
	[SerializeField]
	public Animation m_openAnimation;

	[SerializeField]
	public UILabel m_nameLabel;

	[SerializeField]
	public UILabel m_levelLabel;

	[SerializeField]
	public UILabel m_detailsLabel;

	[SerializeField]
	public UISprite m_playerSprite;

	[SerializeField]
	public UISprite m_typeSprite;

	[SerializeField]
	public UISprite m_attrSprite;

	private GameObject m_calledGameObject;

	public static void OpenWindow(GameObject calledGameObject, ServerItem serverItem)
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "PlayerGetWindowUI");
		if (gameObject2 != null)
		{
			PlayerGetWindowUI component = gameObject2.GetComponent<PlayerGetWindowUI>();
			if (component != null)
			{
				component.OpenWindowSub(calledGameObject, serverItem);
			}
		}
	}

	private void OpenWindowSub(GameObject calledGameObject, ServerItem serverItem)
	{
		m_calledGameObject = calledGameObject;
		base.gameObject.SetActive(true);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_get_window");
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
		StartCoroutine(LateOpenWindow(serverItem));
	}

	private IEnumerator LateOpenWindow(ServerItem serverItem)
	{
		yield return null;
		SoundManager.SePlay("sys_window_open");
		UpdateView(serverItem);
		ActiveAnimation.Play(m_openAnimation, Direction.Forward);
	}

	private void UpdateView(ServerItem serverItem)
	{
		CharaType charaType = serverItem.charaType;
		m_nameLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)charaType]).text;
		m_levelLabel.text = TextUtility.GetTextLevel(MenuPlayerSetUtil.GetTotalLevel(charaType).ToString("D3"));
		m_detailsLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_attribute_" + CharaName.Name[(int)charaType]).text;
		m_playerSprite.spriteName = HudUtility.MakeCharaTextureName(charaType, HudUtility.TextureType.TYPE_L);
		CharacterAttribute characterAttribute = CharacterAttribute.UNKNOWN;
		TeamAttribute teamAttribute = TeamAttribute.UNKNOWN;
		if ((bool)CharacterDataNameInfo.Instance)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(charaType);
			if (dataByID != null)
			{
				characterAttribute = dataByID.m_attribute;
				teamAttribute = dataByID.m_teamAttribute;
			}
		}
		UISprite typeSprite = m_typeSprite;
		int num = (int)characterAttribute;
		typeSprite.spriteName = "ui_mm_player_species_" + num;
		UISprite attrSprite = m_attrSprite;
		int num2 = (int)teamAttribute;
		attrSprite.spriteName = "ui_mm_player_genus_" + num2;
	}

	private void OnClickOkButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	public void OnFinishedCloseAnim()
	{
		if (m_calledGameObject != null)
		{
			m_calledGameObject.SendMessage("OnClosedCharaGetWindow", SendMessageOptions.DontRequireReceiver);
		}
	}
}
