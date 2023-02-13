using DataTable;
using Text;
using UnityEngine;

public class ui_chao_set_cell : MonoBehaviour
{
	private const float IMAGE_LOAD_DELAY = 0.02f;

	private static float s_lastLoadTime = -1f;

	private static bool s_loadLock = true;

	[SerializeField]
	private UISprite m_setSprite;

	[SerializeField]
	private UISprite m_chaoRankSprite;

	[SerializeField]
	private UILabel m_chaoLevelLabel;

	[SerializeField]
	private UISprite m_chaoTypeSprite;

	[SerializeField]
	private UISprite m_bonusTypeSprite;

	[SerializeField]
	private UILabel m_bonusLabel;

	[SerializeField]
	private GameObject m_disabledSprite;

	[SerializeField]
	private UIButtonScale m_buttonScale;

	private ChaoData m_chaoData;

	private bool m_isLoad;

	private bool m_isLoadCmp;

	private bool m_isDraw;

	private bool m_isDrawChao;

	private float m_loadingTime;

	private float m_drawDelay = -1f;

	private float m_checkDelay;

	private int m_chaoId = -1;

	private UITexture m_chaoTex;

	private Texture m_chaoTextureData;

	private UISprite m_chaoDefault;

	private UIPanel m_parentPanel;

	public static void ResetLastLoadTime()
	{
		s_lastLoadTime = 0f;
		s_loadLock = true;
	}

	private void Start()
	{
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.transform.root.gameObject, "chao_set_window");
		UIPlayAnimation component = base.gameObject.GetComponent<UIPlayAnimation>();
		if (animation != null && component != null)
		{
			component.target = animation;
		}
	}

	private void Update()
	{
		if (m_chaoId >= 0 && !s_loadLock)
		{
			if (!m_isLoadCmp)
			{
				if (m_chaoDefault != null && !m_chaoDefault.gameObject.activeSelf && m_chaoTex != null && m_chaoTex.alpha <= 0f)
				{
					m_chaoDefault.gameObject.SetActive(true);
				}
				if (m_isLoad)
				{
					m_loadingTime += Time.deltaTime;
					if (m_loadingTime > 1f)
					{
						ChaoTextureManager.CallbackInfo.LoadFinishCallback callback = ChaoLoadFinishCallback;
						ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(null, callback);
						ChaoTextureManager.Instance.GetTexture(m_chaoId, info);
						m_loadingTime = 0.001f;
					}
				}
			}
			if (!m_isDrawChao)
			{
				if (m_isLoad && m_chaoTextureData != null)
				{
					if (IsDisp())
					{
						m_drawDelay = 0.02f;
					}
					else
					{
						m_drawDelay = 0.04f;
					}
					m_isDrawChao = true;
					if (m_chaoDefault != null && !m_chaoDefault.gameObject.activeSelf)
					{
						m_chaoDefault.gameObject.SetActive(true);
					}
				}
				else if (!m_isLoad && IsDraw())
				{
					float num = 0.05f;
					if (!IsDisp())
					{
						num = 0.3f;
					}
					if (s_lastLoadTime <= 0f)
					{
						if (s_lastLoadTime < 0f)
						{
							s_lastLoadTime = Time.realtimeSinceStartup + 1f;
						}
						else
						{
							s_lastLoadTime = Time.realtimeSinceStartup + 0.5f;
						}
					}
					else if (s_lastLoadTime + num < Time.realtimeSinceStartup)
					{
						ChaoTextureManager.CallbackInfo.LoadFinishCallback callback2 = ChaoLoadFinishCallback;
						ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(null, callback2);
						ChaoTextureManager.Instance.GetTexture(m_chaoId, info2);
						m_loadingTime = 0f;
						m_isLoad = true;
						m_isLoadCmp = false;
						s_lastLoadTime = Time.realtimeSinceStartup;
					}
				}
			}
			else if (m_drawDelay > 0f)
			{
				m_drawDelay -= Time.deltaTime;
				if (m_drawDelay <= 0f)
				{
					if (GeneralUtil.CheckChaoTexture(m_chaoTextureData, m_chaoData.id))
					{
						m_drawDelay = -1f;
						m_loadingTime = -1f;
						m_isLoadCmp = true;
						m_checkDelay = 1f;
						m_chaoTex.mainTexture = m_chaoTextureData;
					}
					else
					{
						ChaoTextureManager.CallbackInfo.LoadFinishCallback callback3 = ChaoReloadFinishCallback;
						ChaoTextureManager.CallbackInfo info3 = new ChaoTextureManager.CallbackInfo(null, callback3);
						ChaoTextureManager.Instance.GetTexture(m_chaoData.id, info3);
					}
				}
			}
		}
		if (!m_isLoadCmp || !(m_checkDelay > 0f))
		{
			return;
		}
		m_checkDelay -= Time.deltaTime;
		if (!(m_checkDelay <= 0f))
		{
			return;
		}
		m_checkDelay = 0f;
		if (!GeneralUtil.CheckChaoTexture(m_chaoTex, m_chaoData.id))
		{
			ChaoTextureManager.CallbackInfo.LoadFinishCallback callback4 = ChaoReloadFinishCallback;
			ChaoTextureManager.CallbackInfo info4 = new ChaoTextureManager.CallbackInfo(null, callback4);
			ChaoTextureManager.Instance.GetTexture(m_chaoData.id, info4);
			return;
		}
		m_chaoTex.alpha = 1f;
		if (m_chaoDefault != null)
		{
			m_chaoDefault.gameObject.SetActive(false);
		}
	}

	private bool IsDraw()
	{
		bool result = false;
		if (m_parentPanel != null)
		{
			if (!m_isDraw)
			{
				Vector3 localPosition = m_parentPanel.transform.localPosition;
				float num = localPosition.y * -1f;
				Vector4 clipRange = m_parentPanel.clipRange;
				float w = clipRange.w;
				float num2 = num - w;
				Vector3 localPosition2 = base.gameObject.transform.localPosition;
				float y = localPosition2.y;
				if (y > num2)
				{
					m_isDraw = true;
				}
			}
			result = m_isDraw;
		}
		return result;
	}

	private bool IsDisp()
	{
		bool result = false;
		if (m_parentPanel != null && m_isDraw)
		{
			Vector3 localPosition = m_parentPanel.transform.localPosition;
			float num = localPosition.y * -1f;
			Vector4 clipRange = m_parentPanel.clipRange;
			float num2 = clipRange.w * 1.2f;
			float num3 = num - num2;
			Vector3 localPosition2 = base.gameObject.transform.localPosition;
			float y = localPosition2.y;
			if (y > num3 && y < num3 + num2)
			{
				result = true;
			}
		}
		return result;
	}

	private void UpdateView(ChaoData chaoData, int mainChaoId, int subChaoId, UIPanel parentPanel)
	{
		s_loadLock = false;
		m_isDraw = false;
		m_drawDelay = -1f;
		m_isDrawChao = false;
		m_chaoId = -1;
		m_loadingTime = 0f;
		m_isLoad = false;
		m_isLoadCmp = false;
		m_chaoTextureData = null;
		m_parentPanel = parentPanel;
		if (chaoData != null)
		{
			m_chaoId = chaoData.id;
		}
		ChaoTextureManager instance = ChaoTextureManager.Instance;
		Texture texture = null;
		if (instance != null)
		{
			texture = instance.GetLoadedTexture(chaoData.id);
		}
		if (m_chaoTex == null)
		{
			m_chaoTex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_tex");
		}
		m_chaoDefault = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_chao_default");
		if (m_chaoDefault != null)
		{
			m_chaoDefault.gameObject.SetActive(true);
		}
		if (texture != null)
		{
			m_drawDelay = 0.005f;
			m_chaoTextureData = texture;
			m_isLoad = true;
			m_isDraw = true;
			m_isDrawChao = true;
		}
		if (chaoData != null)
		{
			m_chaoRankSprite.spriteName = "ui_chao_set_bg_s_" + (int)chaoData.rarity;
		}
		if (chaoData != null && chaoData.IsValidate)
		{
			m_chaoData = chaoData;
			m_setSprite.spriteName = ((chaoData.id == mainChaoId) ? "ui_chao_set_cursor_0" : ((chaoData.id != subChaoId) ? null : "ui_chao_set_cursor_1"));
			m_chaoTex.color = Color.white;
			m_chaoDefault.color = Color.white;
			m_chaoLevelLabel.text = TextUtility.GetTextLevel(chaoData.level.ToString());
			string str = m_chaoData.charaAtribute.ToString().ToLower();
			m_chaoTypeSprite.spriteName = "ui_chao_set_type_icon_" + str;
			if (m_bonusTypeSprite != null)
			{
				m_bonusTypeSprite.gameObject.SetActive(false);
			}
			m_bonusLabel.enabled = false;
			m_disabledSprite.SetActive(false);
			m_buttonScale.enabled = true;
		}
		else if (chaoData != null && !chaoData.IsValidate)
		{
			m_chaoData = chaoData;
			m_setSprite.spriteName = null;
			m_chaoTex.color = Color.black;
			m_chaoDefault.color = Color.black;
			m_chaoLevelLabel.text = string.Empty;
			string str2 = m_chaoData.charaAtribute.ToString().ToLower();
			m_chaoTypeSprite.spriteName = "ui_chao_set_type_icon_" + str2;
			if (m_bonusTypeSprite != null)
			{
				m_bonusTypeSprite.gameObject.SetActive(false);
			}
			m_bonusLabel.enabled = false;
			m_disabledSprite.SetActive(true);
			m_buttonScale.enabled = true;
		}
		else
		{
			m_chaoData = null;
			m_setSprite.spriteName = null;
			m_chaoTex.color = Color.black;
			m_chaoDefault.color = Color.black;
			m_chaoLevelLabel.text = string.Empty;
			m_chaoTypeSprite.spriteName = null;
			m_bonusTypeSprite.spriteName = null;
			m_bonusLabel.text = string.Empty;
			m_disabledSprite.SetActive(true);
			m_buttonScale.enabled = false;
		}
		m_chaoTex.alpha = 0.001f;
	}

	private void ChaoLoadFinishCallback(Texture tex)
	{
		m_chaoTextureData = tex;
	}

	private void ChaoReloadFinishCallback(Texture tex)
	{
		m_chaoTextureData = tex;
		m_chaoTex.mainTexture = m_chaoTextureData;
		m_drawDelay = -1f;
		m_loadingTime = -1f;
		m_isLoadCmp = true;
		m_checkDelay = 1f;
	}

	public void UpdateView(int id, int mainChaoId, int subChaoId, UIPanel parentPanel)
	{
		UpdateView(ChaoTable.GetChaoData(id), mainChaoId, subChaoId, parentPanel);
	}

	private void OnClick()
	{
		if (m_chaoData == null)
		{
			return;
		}
		ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
		if (window != null)
		{
			ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(m_chaoData);
			if (!m_chaoData.IsValidate)
			{
				chaoInfo.level = 0;
				chaoInfo.detail = m_chaoData.GetDetailLevelPlusSP(0);
				chaoInfo.name = TextManager.GetText(TextManager.TextType.TEXTTYPE_MILEAGE_MAP_COMMON, "Name", "name_question").text;
				chaoInfo.onMask = true;
				window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
			}
			else
			{
				window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WITH_BUTTON);
			}
		}
	}
}
