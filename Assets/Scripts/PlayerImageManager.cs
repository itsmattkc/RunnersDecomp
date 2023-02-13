using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImageManager : MonoBehaviour
{
	private class PlayerImageData
	{
		public bool IsLoading
		{
			get;
			set;
		}

		public bool IsLoaded
		{
			get;
			set;
		}

		public string PlayerId
		{
			get;
			set;
		}

		public string PlayerImageUrl
		{
			get;
			set;
		}

		public Texture2D PlayerImage
		{
			get;
			set;
		}

		public List<Action<Texture2D>> CallbackList
		{
			get;
			set;
		}

		public PlayerImageData()
		{
			IsLoading = false;
			IsLoaded = false;
			PlayerId = string.Empty;
			PlayerImageUrl = string.Empty;
			PlayerImage = null;
			CallbackList = new List<Action<Texture2D>>(10);
		}

		public static IEnumerator DoFetchPlayerImages(PlayerImageData playerImageData)
		{
			WWW www2 = new WWW(playerImageData.PlayerImageUrl);
			yield return www2;
			if (www2.texture != null)
			{
				playerImageData.PlayerImage = www2.texture;
			}
			playerImageData.IsLoading = false;
			playerImageData.IsLoaded = true;
			int countCallback = playerImageData.CallbackList.Count;
			if (0 < countCallback)
			{
				for (int i = 0; i < countCallback; i++)
				{
					Action<Texture2D> callback = playerImageData.CallbackList[i];
					if (callback != null)
					{
						callback(playerImageData.PlayerImage);
					}
				}
			}
			www2.Dispose();
			www2 = null;
		}
	}

	private static PlayerImageManager mInstance;

	[SerializeField]
	private readonly int mMaxStorageCount = -1;

	[SerializeField]
	private Texture2D mDefaultPlayerImage;

	private Dictionary<string, PlayerImageData> mPlayerImageDataDic;

	private Queue<PlayerImageData> mPlayerImageQueue = new Queue<PlayerImageData>();

	public static PlayerImageManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	private int MaxStorageCount
	{
		get
		{
			return mMaxStorageCount;
		}
	}

	private bool IsExistStorageLimit
	{
		get
		{
			return 0 < mMaxStorageCount;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			if (IsExistStorageLimit)
			{
				mPlayerImageDataDic = new Dictionary<string, PlayerImageData>(MaxStorageCount);
			}
			else
			{
				mPlayerImageDataDic = new Dictionary<string, PlayerImageData>();
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public bool IsLoaded(string playerId)
	{
		PlayerImageData value;
		if (mPlayerImageDataDic.TryGetValue(playerId, out value))
		{
			return value.IsLoaded;
		}
		return false;
	}

	public bool IsLoading(string playerId)
	{
		PlayerImageData value;
		if (mPlayerImageDataDic.TryGetValue(playerId, out value))
		{
			return value.IsLoading;
		}
		return false;
	}

	public bool Load(string playerId, string url, Action<Texture2D> callback)
	{
		bool result = false;
		if (playerId != null && string.Empty != playerId)
		{
			PlayerImageData value;
			if (!mPlayerImageDataDic.TryGetValue(playerId, out value))
			{
				if (url != null && string.Empty != url)
				{
					value = new PlayerImageData();
					value.PlayerId = playerId;
					value.PlayerImageUrl = url;
					value.PlayerImage = mDefaultPlayerImage;
					value.IsLoading = false;
					value.IsLoaded = false;
					if (callback != null)
					{
						value.CallbackList.Add(callback);
					}
					mPlayerImageDataDic.Add(playerId, value);
					mPlayerImageQueue.Enqueue(value);
					result = true;
				}
			}
			else if (!value.IsLoaded)
			{
				if (callback != null)
				{
					value.CallbackList.Add(callback);
				}
			}
			else
			{
				if (callback != null)
				{
					callback(value.PlayerImage);
				}
				result = true;
			}
		}
		return result;
	}

	public void Dispose(string playerId, bool is_removeList = true)
	{
		PlayerImageData value;
		if (!mPlayerImageDataDic.TryGetValue(playerId, out value) || value == null || value.CallbackList == null)
		{
			return;
		}
		foreach (Action<Texture2D> callback in value.CallbackList)
		{
			if (callback != null)
			{
				callback(mDefaultPlayerImage);
			}
		}
		if (is_removeList)
		{
			mPlayerImageDataDic.Remove(playerId);
		}
		value.CallbackList.Clear();
	}

	private void Update()
	{
		if (mPlayerImageQueue.Count > 0)
		{
			PlayerImageData playerImageData = mPlayerImageQueue.Peek();
			if (playerImageData.IsLoaded)
			{
				mPlayerImageQueue.Dequeue();
			}
			else if (!playerImageData.IsLoading)
			{
				playerImageData.IsLoading = true;
				StartCoroutine(PlayerImageData.DoFetchPlayerImages(playerImageData));
			}
		}
	}

	public Texture2D GetPlayerImage(string playerId)
	{
		return GetPlayerImage(playerId, string.Empty, null);
	}

	public Texture2D GetPlayerImage(string playerId, string url, Action<Texture2D> callback)
	{
		if (Load(playerId, url, callback))
		{
			PlayerImageData playerImageData = mPlayerImageDataDic[playerId];
			return playerImageData.PlayerImage;
		}
		return mDefaultPlayerImage;
	}

	public Texture2D GetDefaultImage()
	{
		return mDefaultPlayerImage;
	}

	public void ClearPlayerImage(string playerId, bool is_removeList = true)
	{
		Dispose(playerId, is_removeList);
	}

	public void ClearAllPlayerImage()
	{
		foreach (KeyValuePair<string, PlayerImageData> item in mPlayerImageDataDic)
		{
			string playerId = item.Value.PlayerId;
			ClearPlayerImage(playerId, false);
		}
		mPlayerImageDataDic.Clear();
	}

	public static Texture2D GetPlayerDefaultImage()
	{
		Texture2D result = null;
		if (mInstance != null)
		{
			result = mInstance.GetDefaultImage();
		}
		return result;
	}
}
