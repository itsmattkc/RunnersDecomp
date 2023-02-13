using System.Collections.Generic;
using UnityEngine;

public class AtlasIntermediary : MonoBehaviour
{
	[SerializeField]
	private UIAtlas[] atlasList;

	private Dictionary<string, UIAtlas> m_atlasList;

	private static AtlasIntermediary m_instance;

	public static AtlasIntermediary instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool isInit
	{
		get
		{
			bool result = false;
			if (m_atlasList != null && m_atlasList.Count > 0)
			{
				result = true;
			}
			return result;
		}
	}

	public void Awake()
	{
		SetInstance();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void SetInstance()
	{
		if (m_instance == null)
		{
			m_instance = this;
			Init();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Init()
	{
		if (m_atlasList == null)
		{
			m_atlasList = new Dictionary<string, UIAtlas>();
			UIAtlas[] array = atlasList;
			foreach (UIAtlas uIAtlas in array)
			{
				m_atlasList.Add(uIAtlas.name, uIAtlas);
			}
		}
	}

	public UIAtlas GetAtlas(string atlasName)
	{
		UIAtlas result = null;
		if (!isInit)
		{
			Init();
		}
		if (m_atlasList.ContainsKey(atlasName))
		{
			result = m_atlasList[atlasName];
		}
		return result;
	}

	public UIAtlas GetAtlasServerItemId(int serverItemId)
	{
		UIAtlas result = null;
		string text = null;
		text = ServerItem.GetIdTypeAtlasName(new ServerItem((ServerItem.Id)serverItemId).idType);
		if (text != null && text != string.Empty)
		{
			result = GetAtlas(text);
		}
		return result;
	}

	public UIAtlas GetAtlasItemIdType(ServerItem.IdType idType)
	{
		UIAtlas result = null;
		string text = null;
		text = ServerItem.GetIdTypeAtlasName(idType);
		if (text != null && text != string.Empty)
		{
			result = GetAtlas(text);
		}
		return result;
	}

	public static List<string> GetSpriteNameList(UIAtlas atlas)
	{
		List<string> result = null;
		if (atlas != null)
		{
			result = new List<string>();
			List<UISpriteData> spriteList = atlas.spriteList;
			{
				foreach (UISpriteData item in spriteList)
				{
					result.Add(item.name);
				}
				return result;
			}
		}
		return result;
	}

	public static UISpriteData GetSpriteData(UIAtlas atlas, string spriteName)
	{
		UISpriteData result = null;
		if (atlas != null)
		{
			List<UISpriteData> spriteList = atlas.spriteList;
			{
				foreach (UISpriteData item in spriteList)
				{
					if (item.name == spriteName)
					{
						return item;
					}
				}
				return result;
			}
		}
		return result;
	}

	public bool SetSprite(ref UISprite target, ServerItem itemData, float scale = 1f)
	{
		bool result = false;
		if (target != null)
		{
			UIAtlas atlasItemIdType = GetAtlasItemIdType(itemData.idType);
			if (atlasItemIdType != null)
			{
				string serverItemSpriteName = itemData.serverItemSpriteName;
				if (serverItemSpriteName != null && serverItemSpriteName != string.Empty)
				{
					List<string> spriteNameList = GetSpriteNameList(atlasItemIdType);
					{
						foreach (string item in spriteNameList)
						{
							if (item == serverItemSpriteName)
							{
								UISpriteData spriteData = GetSpriteData(atlasItemIdType, serverItemSpriteName);
								target.atlas = atlasItemIdType;
								target.spriteName = serverItemSpriteName;
								if (scale >= 0f)
								{
									int num = spriteData.paddingLeft + spriteData.paddingRight;
									int num2 = spriteData.paddingTop + spriteData.paddingBottom;
									target.width = (int)((float)(spriteData.width + num) * scale);
									target.height = (int)((float)(spriteData.height + num2) * scale);
								}
								return true;
							}
						}
						return result;
					}
				}
			}
		}
		return result;
	}
}
