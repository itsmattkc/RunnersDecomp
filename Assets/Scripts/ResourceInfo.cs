using UnityEngine;

public class ResourceInfo
{
	public GameObject ResObject
	{
		get;
		set;
	}

	public bool DontDestroyOnChangeScene
	{
		get;
		set;
	}

	public ResourceCategory Category
	{
		get;
		set;
	}

	public string PathName
	{
		get;
		set;
	}

	public bool AssetBundle
	{
		get;
		set;
	}

	public bool Cashed
	{
		get;
		set;
	}

	public ResourceInfo(ResourceCategory category)
	{
		Category = category;
	}

	public void CopyTo(ResourceInfo dest)
	{
		dest.ResObject = ResObject;
		dest.DontDestroyOnChangeScene = DontDestroyOnChangeScene;
		dest.Category = Category;
		dest.PathName = PathName;
		dest.AssetBundle = AssetBundle;
		dest.Cashed = Cashed;
	}
}
