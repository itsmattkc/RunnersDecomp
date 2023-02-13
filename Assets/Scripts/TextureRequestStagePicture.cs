using UnityEngine;

public class TextureRequestStagePicture : TextureRequest
{
	private int m_stageIndex = 1;

	private string m_fileName;

	private UITexture m_uiTex;

	public TextureRequestStagePicture(int stageIndex, UITexture uiTex)
	{
		if (stageIndex >= 1)
		{
			m_stageIndex = stageIndex;
			m_fileName = "ui_tex_mile_w" + m_stageIndex.ToString("D2") + "A";
			m_uiTex = uiTex;
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
