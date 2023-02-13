using UnityEngine;

public class TextureRequestChara : TextureRequest
{
	private CharaType m_type;

	private string m_fileName;

	private UITexture m_uiTex;

	public TextureRequestChara(CharaType type, UITexture uiTex)
	{
		if (type != CharaType.UNKNOWN)
		{
			m_type = type;
			m_uiTex = uiTex;
			if (m_uiTex != null)
			{
				m_uiTex.SetTexture(TextureAsyncLoadManager.Instance.CharaDefaultTexture);
			}
			int num = (int)type;
			string[] prefixNameList = CharacterDataNameInfo.PrefixNameList;
			string str = prefixNameList[num];
			m_fileName = "ui_tex_player_" + num.ToString("D2") + "_" + str;
		}
	}

	public static void RemoveAllCharaTexture()
	{
		TextureAsyncLoadManager instance = TextureAsyncLoadManager.Instance;
		if (!(instance == null))
		{
			for (int i = 0; i < 29; i++)
			{
				CharaType type = (CharaType)i;
				TextureRequestChara request = new TextureRequestChara(type, null);
				instance.Remove(request);
			}
		}
	}

	public override void LoadDone(Texture tex)
	{
		if (!(m_uiTex == null))
		{
			m_uiTex.mainTexture = tex;
		}
	}

	public override bool IsEnableLoad()
	{
		if (string.IsNullOrEmpty(GetFileName()))
		{
			return false;
		}
		return true;
	}

	public override string GetFileName()
	{
		return m_fileName;
	}
}
