using System.Collections.Generic;
using UnityEngine;

public class TextureAsyncLoadManager : MonoBehaviour
{
	private class TextureInfo
	{
		private enum State
		{
			IDLE,
			LOADING,
			LOADED,
			NUM
		}

		private string m_textureName;

		private Texture m_texture;

		private List<TextureRequest> m_requestList = new List<TextureRequest>();

		private int m_removeRequestCount;

		private GameObject m_gameObject;

		private ResourceSceneLoader m_loader;

		private State m_state;

		public bool Loaded
		{
			get
			{
				if (m_state == State.LOADED)
				{
					return true;
				}
				return false;
			}
			private set
			{
			}
		}

		public bool EnableRemove
		{
			get
			{
				if (!Loaded)
				{
					return false;
				}
				if (m_removeRequestCount <= 0)
				{
					return false;
				}
				return true;
			}
			private set
			{
			}
		}

		public TextureInfo(GameObject obj)
		{
			m_gameObject = obj;
		}

		public void RequestLoad(TextureRequest request)
		{
			if (request != null && !(m_gameObject == null))
			{
				if (string.IsNullOrEmpty(m_textureName))
				{
					m_textureName = request.GetFileName();
				}
				m_requestList.Add(request);
				if (m_state == State.LOADED)
				{
					request.LoadDone(m_texture);
				}
			}
		}

		public void RequestRemove(TextureRequest request)
		{
			if (request == null || m_gameObject == null)
			{
				return;
			}
			TextureRequest textureRequest = null;
			foreach (TextureRequest request2 in m_requestList)
			{
				if (request2 == null || request2 != request)
				{
					continue;
				}
				textureRequest = request2;
				break;
			}
			m_removeRequestCount++;
		}

		public void Load()
		{
			if (m_state == State.IDLE)
			{
				m_loader = m_gameObject.AddComponent<ResourceSceneLoader>();
				m_loader.AddLoadAndResourceManager(m_textureName, true, ResourceCategory.UI, false, false, m_textureName);
				m_state = State.LOADING;
			}
		}

		public void Update()
		{
			switch (m_state)
			{
			case State.IDLE:
				break;
			case State.LOADED:
				break;
			case State.LOADING:
				if (m_loader != null)
				{
					if (!m_loader.Loaded)
					{
						break;
					}
					m_texture = null;
					GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, m_textureName);
					AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
					m_texture = component.m_tex;
					for (int i = 0; i < m_requestList.Count; i++)
					{
						TextureRequest textureRequest = m_requestList[i];
						if (textureRequest != null)
						{
							textureRequest.LoadDone(m_texture);
						}
					}
					Object.Destroy(m_loader);
					m_state = State.LOADED;
				}
				else
				{
					m_state = State.LOADED;
				}
				break;
			}
		}

		public void Remove()
		{
			ResourceManager instance = ResourceManager.Instance;
			if (instance != null)
			{
				string[] removeList = new string[1]
				{
					m_requestList[0].GetFileName()
				};
				instance.RemoveResources(ResourceCategory.UI, removeList);
			}
		}
	}

	[SerializeField]
	private Texture m_charaDefaultTexture;

	[SerializeField]
	private Texture m_chaoDefaultTexture;

	private static TextureAsyncLoadManager m_instance;

	private Dictionary<string, TextureInfo> m_textureList = new Dictionary<string, TextureInfo>();

	private Queue<TextureInfo> m_loadQueue = new Queue<TextureInfo>();

	private bool m_DirtyFlag;

	public Texture CharaDefaultTexture
	{
		get
		{
			return m_charaDefaultTexture;
		}
		private set
		{
		}
	}

	public Texture ChaoDefaultTexture
	{
		get
		{
			return m_chaoDefaultTexture;
		}
		private set
		{
		}
	}

	public static TextureAsyncLoadManager Instance
	{
		get
		{
			return m_instance;
		}
		private set
		{
		}
	}

	public bool IsLoaded(TextureRequest request)
	{
		if (request == null)
		{
			return false;
		}
		string fileName = request.GetFileName();
		return IsLoaded(fileName);
	}

	public bool IsLoaded(string fileName)
	{
		TextureInfo value = null;
		if (m_textureList.TryGetValue(fileName, out value) && value.Loaded)
		{
			return true;
		}
		return false;
	}

	public void Request(TextureRequest request)
	{
		if (request == null || !request.IsEnableLoad())
		{
			return;
		}
		string fileName = request.GetFileName();
		TextureInfo value = null;
		if (m_textureList.TryGetValue(fileName, out value))
		{
			value.RequestLoad(request);
		}
		else
		{
			TextureInfo textureInfo = new TextureInfo(base.gameObject);
			textureInfo.RequestLoad(request);
			if (m_loadQueue.Count <= 0)
			{
				textureInfo.Load();
			}
			m_loadQueue.Enqueue(textureInfo);
			m_textureList.Add(fileName, textureInfo);
		}
		m_DirtyFlag = true;
	}

	public void Remove(TextureRequest request)
	{
		if (request != null && request.IsEnableLoad())
		{
			string fileName = request.GetFileName();
			TextureInfo value = null;
			if (m_textureList.TryGetValue(fileName, out value))
			{
				value.RequestRemove(request);
			}
		}
	}

	private void Start()
	{
		m_instance = this;
	}

	private void Update()
	{
		if (m_DirtyFlag)
		{
			UIPanel.SetDirty();
			m_DirtyFlag = false;
		}
		if (m_loadQueue.Count > 0)
		{
			TextureInfo textureInfo = m_loadQueue.Peek();
			textureInfo.Update();
			if (textureInfo.Loaded)
			{
				m_loadQueue.Dequeue();
				if (m_loadQueue.Count > 0)
				{
					TextureInfo textureInfo2 = m_loadQueue.Peek();
					textureInfo2.Load();
				}
			}
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, TextureInfo> texture in m_textureList)
		{
			string key = texture.Key;
			TextureInfo value = texture.Value;
			if (value != null && value.EnableRemove)
			{
				list.Add(key);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in list)
		{
			TextureInfo value2;
			if (m_textureList.TryGetValue(item, out value2))
			{
				value2.Remove();
			}
			m_textureList.Remove(item);
		}
	}
}
