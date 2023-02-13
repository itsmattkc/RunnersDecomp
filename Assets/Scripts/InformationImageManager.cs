using App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Text;
using UnityEngine;

public class InformationImageManager : MonoBehaviour
{
	private class ImageData
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

		public string ImageId
		{
			get;
			set;
		}

		public string ImageUrl
		{
			get;
			set;
		}

		public string ImagePath
		{
			get;
			set;
		}

		public string ImageEtagPath
		{
			get;
			set;
		}

		public Texture2D Image
		{
			get;
			set;
		}

		public List<Action<Texture2D>> CallbackList
		{
			get;
			set;
		}

		public ImageData()
		{
			IsLoading = false;
			IsLoaded = false;
			ImageId = string.Empty;
			ImageUrl = string.Empty;
			ImagePath = string.Empty;
			Image = null;
			CallbackList = new List<Action<Texture2D>>(10);
		}

		public static IEnumerator DoFetchImages(ImageData imageData)
		{
			WWW www2 = new WWW(imageData.ImageUrl);
			yield return www2;
			imageData.IsLoading = false;
			imageData.IsLoaded = true;
			if (string.IsNullOrEmpty(www2.error) && www2.texture != null)
			{
				imageData.Image = www2.texture;
				int countCallback = imageData.CallbackList.Count;
				if (countCallback > 0)
				{
					for (int index = 0; index < countCallback; index++)
					{
						Action<Texture2D> callback = imageData.CallbackList[index];
						if (callback != null)
						{
							callback(imageData.Image);
						}
					}
				}
				SaveImageData(www2, imageData.Image, imageData.ImagePath, imageData.ImageEtagPath);
			}
			www2.Dispose();
			www2 = null;
		}

		public static IEnumerator DoFetchCashImages(ImageData imageData, string etag, string lastModified)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers["If-Modified-Since"] = lastModified;
			headers["ETAG"] = etag;
			WWW www2 = new WWW(imageData.ImageUrl, null, headers);
			yield return www2;
			if (string.IsNullOrEmpty(www2.error) && www2.texture != null && www2.size > 0)
			{
				imageData.IsLoading = false;
				imageData.IsLoaded = true;
				imageData.Image = www2.texture;
				int countCallback = imageData.CallbackList.Count;
				if (countCallback > 0)
				{
					for (int index = 0; index < countCallback; index++)
					{
						Action<Texture2D> callback = imageData.CallbackList[index];
						if (callback != null)
						{
							callback(imageData.Image);
						}
					}
				}
				SaveImageData(www2, imageData.Image, imageData.ImagePath, imageData.ImageEtagPath);
			}
			else
			{
				Debug.Log("DoFetchCashImages new jpeg is non!!");
			}
			www2.Dispose();
			www2 = null;
		}
	}

	private const string BANNER_PREFIX = "ad_";

	private const string IMAGE_FOLDER = "/infoImage/";

	private const string IMAGE_EXTENSION = ".jpg";

	private const string PNG_EXTENSION = ".png";

	private const string IMAGE_ETG_EXTENSION = ".etag";

	private static InformationImageManager m_instance;

	private Dictionary<string, ImageData> m_imageDataDic;

	public static InformationImageManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			m_imageDataDic = new Dictionary<string, ImageData>();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		string savePath = getSavePath();
		if (!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}
		string etagFloderPath = GetEtagFloderPath();
		if (!Directory.Exists(etagFloderPath))
		{
			Directory.CreateDirectory(etagFloderPath);
		}
	}

	private string getSavePath()
	{
		return Application.persistentDataPath + "/infoImage/";
	}

	private IEnumerator LoadCashImage(ImageData imageData)
	{
		if (File.Exists(imageData.ImageEtagPath))
		{
			FileStream fs = new FileStream(imageData.ImageEtagPath, FileMode.Open);
			BinaryReader reader = new BinaryReader(fs);
			string text = reader.ReadString();
			string[] delimiter = new string[1]
			{
				"@@@"
			};
			string[] word = text.Split(delimiter, StringSplitOptions.None);
			string etag = word[0];
			string lastModified = word[1];
			reader.Close();
			yield return StartCoroutine(ImageData.DoFetchCashImages(imageData, etag, lastModified));
		}
		if (imageData.IsLoaded)
		{
			yield break;
		}
		LoadCashImage(imageData.ImagePath, ref imageData);
		imageData.IsLoading = false;
		imageData.IsLoaded = true;
		foreach (Action<Texture2D> callback in imageData.CallbackList)
		{
			if (callback != null)
			{
				callback(imageData.Image);
			}
		}
	}

	private void LoadCashImage(string filePath, ref ImageData imageData)
	{
		if (imageData != null)
		{
			imageData.Image = new Texture2D(0, 0, TextureFormat.ARGB32, false);
			imageData.Image.LoadImage(LoadImageData(filePath));
		}
	}

	private byte[] LoadImageData(string path)
	{
		FileStream input = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(input);
		byte[] result = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
		binaryReader.Close();
		return result;
	}

	private static void SaveImageData(WWW www, Texture2D texture, string path, string etagPath)
	{
		if (texture != null)
		{
			FileStream output = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(texture.EncodeToPNG());
			binaryWriter.Close();
		}
		if (www != null && www.responseHeaders.ContainsKey("ETAG") && www.responseHeaders.ContainsKey("LAST-MODIFIED"))
		{
			FileStream output2 = new FileStream(etagPath, FileMode.Create);
			BinaryWriter binaryWriter2 = new BinaryWriter(output2);
			binaryWriter2.Write(www.responseHeaders["ETAG"] + "@@@" + www.responseHeaders["LAST-MODIFIED"]);
			binaryWriter2.Close();
		}
	}

	public void DeleteImageData(string imageId)
	{
		string str = getSavePath() + "ad_" + imageId;
		string str2 = getSavePath() + imageId;
		string str3 = GetEtagFloderPath() + imageId;
		string str4 = GetEtagFloderPath() + "ad_" + imageId;
		for (int i = 0; i <= 10; i++)
		{
			string str5 = "_" + TextUtility.GetSuffix((Env.Language)i);
			string path = str + str5 + ".jpg";
			string path2 = str2 + str5 + ".jpg";
			string path3 = str + str5 + ".png";
			string path4 = str2 + str5 + ".png";
			string path5 = str4 + str5 + ".etag";
			string path6 = str3 + str5 + ".etag";
			if (ExistsFile(path))
			{
				File.Delete(path);
			}
			if (ExistsFile(path2))
			{
				File.Delete(path2);
			}
			if (ExistsFile(path5))
			{
				File.Delete(path5);
			}
			if (ExistsFile(path6))
			{
				File.Delete(path6);
			}
			if (ExistsFile(path3))
			{
				File.Delete(path3);
			}
			if (ExistsFile(path4))
			{
				File.Delete(path4);
			}
		}
	}

	private bool ExistsFile(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			return File.Exists(path);
		}
		return false;
	}

	private string GetCashedFilePath(string imageId)
	{
		if (!string.IsNullOrEmpty(imageId))
		{
			string suffixe = TextUtility.GetSuffixe();
			return getSavePath() + imageId + "_" + suffixe + ".jpg";
		}
		return null;
	}

	private string GetCashedEtagFilePath(string imageId)
	{
		if (!string.IsNullOrEmpty(imageId))
		{
			string suffixe = TextUtility.GetSuffixe();
			return GetEtagFloderPath() + imageId + "_" + suffixe + ".etag";
		}
		return null;
	}

	private string GetEtagFloderPath()
	{
		return getSavePath() + "etag/";
	}

	private string GetServerFileURL(string imageId)
	{
		if (!string.IsNullOrEmpty(imageId))
		{
			string text = "_" + TextUtility.GetSuffix(Env.language);
			return NetBaseUtil.InformationServerURL + "pictures/infoImage/" + imageId + text + ".jpg";
		}
		return null;
	}

	private string GetBannerName(string imageId)
	{
		return "ad_" + imageId;
	}

	public bool IsLoaded(string imageId)
	{
		ImageData value;
		if (m_imageDataDic.TryGetValue(imageId, out value))
		{
			return value.IsLoaded;
		}
		return false;
	}

	public bool IsLoading(string imageId)
	{
		ImageData value;
		if (m_imageDataDic.TryGetValue(imageId, out value))
		{
			return value.IsLoading;
		}
		return false;
	}

	public bool Load(string imageId, bool bannerFlag, Action<Texture2D> callback)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(imageId))
		{
			if (bannerFlag)
			{
				imageId = GetBannerName(imageId);
			}
			ImageData value = null;
			if (m_imageDataDic.TryGetValue(imageId, out value))
			{
				if (!value.IsLoaded)
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
						callback(value.Image);
					}
					result = true;
				}
			}
			else
			{
				value = new ImageData();
				value.ImageId = imageId;
				value.ImageUrl = GetServerFileURL(imageId);
				value.ImagePath = GetCashedFilePath(imageId);
				value.ImageEtagPath = GetCashedEtagFilePath(imageId);
				if (callback != null)
				{
					value.CallbackList.Add(callback);
				}
				value.IsLoading = true;
				value.IsLoaded = false;
				if (ExistsFile(value.ImagePath) && ExistsFile(value.ImageEtagPath))
				{
					m_imageDataDic.Add(imageId, value);
					StartCoroutine(LoadCashImage(value));
					result = true;
				}
				else
				{
					if (callback != null)
					{
						value.CallbackList.Add(callback);
					}
					m_imageDataDic.Add(imageId, value);
					StartCoroutine(ImageData.DoFetchImages(value));
					result = true;
				}
			}
		}
		return result;
	}

	public void ClearWinowImage()
	{
		if (m_imageDataDic.Count <= 0)
		{
			return;
		}
		Dictionary<string, ImageData>.KeyCollection keys = m_imageDataDic.Keys;
		List<string> list = new List<string>();
		foreach (string item in keys)
		{
			if (item.IndexOf("ad_") < 0)
			{
				list.Add(item);
			}
		}
		foreach (string item2 in list)
		{
			if (m_imageDataDic.ContainsKey(item2))
			{
				m_imageDataDic.Remove(item2);
			}
		}
	}

	public void ResetImage()
	{
		m_imageDataDic.Clear();
	}

	public void DeleteImageFiles()
	{
		ResetImage();
		string savePath = getSavePath();
		if (!Directory.Exists(savePath))
		{
			return;
		}
		string[] files = Directory.GetFiles(savePath, "*.png", SearchOption.AllDirectories);
		if (files != null && files.Length > 0)
		{
			string[] array = files;
			foreach (string path in array)
			{
				if (ExistsFile(path))
				{
					File.Delete(path);
				}
			}
		}
		string[] files2 = Directory.GetFiles(savePath, "*.jpg", SearchOption.AllDirectories);
		if (files2 != null && files2.Length > 0)
		{
			string[] array2 = files2;
			foreach (string path2 in array2)
			{
				if (ExistsFile(path2))
				{
					File.Delete(path2);
				}
			}
		}
		string etagFloderPath = GetEtagFloderPath();
		if (!Directory.Exists(etagFloderPath))
		{
			return;
		}
		string[] files3 = Directory.GetFiles(etagFloderPath, "*.etag", SearchOption.AllDirectories);
		if (files3 == null || files3.Length <= 0)
		{
			return;
		}
		string[] array3 = files3;
		foreach (string path3 in array3)
		{
			if (ExistsFile(path3))
			{
				File.Delete(path3);
			}
		}
	}

	public Texture2D GetImage(string imageId, bool bannerFlag, Action<Texture2D> callback)
	{
		if (Load(imageId, bannerFlag, callback))
		{
			ImageData imageData = m_imageDataDic[imageId];
			return imageData.Image;
		}
		return null;
	}
}
