using UnityEngine;

#if UNITY_ANDROID
public class DisplayMetricsAndroid
{
	public static float Density
	{
		get;
		protected set;
	}

	public static int DensityDPI
	{
		get;
		protected set;
	}

	public static int HeightPixels
	{
		get;
		protected set;
	}

	public static int WidthPixels
	{
		get;
		protected set;
	}

	public static float ScaledDensity
	{
		get;
		protected set;
	}

	public static float XDPI
	{
		get;
		protected set;
	}

	public static float YDPI
	{
		get;
		protected set;
	}

	static DisplayMetricsAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (new AndroidJavaClass("android.util.DisplayMetrics"))
			{
				using (AndroidJavaObject androidJavaObject4 = new AndroidJavaObject("android.util.DisplayMetrics"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getWindowManager", new object[0]))
						{
							using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getDefaultDisplay", new object[0]))
							{
								androidJavaObject3.Call("getMetrics", androidJavaObject4);
								Density = androidJavaObject4.Get<float>("density");
								DensityDPI = androidJavaObject4.Get<int>("densityDpi");
								HeightPixels = androidJavaObject4.Get<int>("heightPixels");
								WidthPixels = androidJavaObject4.Get<int>("widthPixels");
								ScaledDensity = androidJavaObject4.Get<float>("scaledDensity");
								XDPI = androidJavaObject4.Get<float>("xdpi");
								YDPI = androidJavaObject4.Get<float>("ydpi");
							}
						}
					}
				}
			}
		}
	}
}
#endif
