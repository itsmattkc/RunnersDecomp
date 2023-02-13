using UnityEngine;

public class FontManager : MonoBehaviour
{
	private UIFont m_uiFont;

	private bool m_loadedFont;

	private static FontManager instance;

	public static FontManager Instance
	{
		get
		{
			return instance;
		}
	}

	public bool IsNecessaryLoadFont()
	{
		return !m_loadedFont;
	}

	public void LoadResourceData()
	{
		if (!IsNecessaryLoadFont())
		{
			return;
		}
		GameObject gameObject = Resources.Load(GetResourceName()) as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, gameObject.transform.localPosition, Quaternion.identity) as GameObject;
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = base.gameObject.transform;
				m_uiFont = gameObject2.GetComponent<UIFont>();
				m_loadedFont = true;
			}
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void ReplaceFont()
	{
		if (!(m_uiFont != null))
		{
			return;
		}
		UIFont[] array = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		UIFont[] array2 = array;
		foreach (UIFont uIFont in array2)
		{
			if (uIFont != null && uIFont.name == "UCGothic_26_mix_reference")
			{
				uIFont.replacement = m_uiFont;
			}
		}
	}

	private string GetFontName()
	{
		return "UCGothic_26_mix";
	}

	private string GetResourceName()
	{
		return "Prefabs/Font/UCGothic_26_mix";
	}
}
