using UnityEngine;

public class TextureRequestEpisodeBanner : TextureRequest
{
	private static readonly int s_BannerCount = 2;

	private string m_fileName;

	private UITexture m_uiTex;

	public static int BannerCount
	{
		get
		{
			return s_BannerCount;
		}
		private set
		{
		}
	}

	public TextureRequestEpisodeBanner(int textureIndex, UITexture uiTex)
	{
		m_fileName = "ui_tex_mm_ep_" + (textureIndex + 1).ToString("D3");
		m_uiTex = uiTex;
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
