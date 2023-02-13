using SaveData;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public enum SourceId
	{
		NONE = -1,
		BGM_NORMAL = 0,
		BGM_BEGIN = 0,
		BGM_CROSSFADE = 1,
		SE = 2,
		BGM_END = 2,
		COUNT = 3
	}

	public enum PlayId
	{
		NONE
	}

	public class Playback
	{
		private CriAtomExPlayback m_atomExPlayback;

		public string cueName
		{
			get;
			private set;
		}

		public string cueSheet
		{
			get;
			private set;
		}

		public CriAtomExPlayback.Status status
		{
			get
			{
				return m_atomExPlayback.status;
			}
		}

		public Playback(CriAtomExPlayback playback, string cueName, string cueSheet)
		{
			m_atomExPlayback = playback;
			this.cueName = cueName;
			this.cueSheet = cueSheet;
		}

		public void Stop()
		{
			m_atomExPlayback.Stop();
		}

		public void Pause(bool sw)
		{
			m_atomExPlayback.Pause(sw);
		}
	}

	public class Source
	{
		private enum FadeType
		{
			NONE,
			OUT,
			OFF,
			IN
		}

		private static PlayId s_playIdBase;

		private float m_masterVolume = 1f;

		private bool m_isPaused;

		private FadeType m_fadeType;

		private float m_fadeTime;

		private Dictionary<PlayId, Playback> m_playbacks = new Dictionary<PlayId, Playback>();

		private CriAtomSource m_atomSource;

		private PlayId m_playId;

		public Dictionary<PlayId, Playback> playbacks
		{
			get
			{
				return m_playbacks;
			}
		}

		public PlayId playId
		{
			get
			{
				return m_playId;
			}
		}

		public CriAtomSource.Status status
		{
			get
			{
				return m_atomSource.status;
			}
		}

		public string cueName
		{
			get
			{
				return m_atomSource.cueName;
			}
			set
			{
				m_atomSource.cueName = value;
			}
		}

		public string cueSheet
		{
			get
			{
				return m_atomSource.cueSheet;
			}
			set
			{
				m_atomSource.cueSheet = value;
			}
		}

		public float volume
		{
			get
			{
				return m_atomSource.volume;
			}
			set
			{
				m_atomSource.volume = value;
			}
		}

		public bool loop
		{
			get
			{
				return m_atomSource.loop;
			}
			set
			{
				m_atomSource.loop = value;
			}
		}

		public float masterVolume
		{
			get
			{
				return m_masterVolume;
			}
			set
			{
				m_masterVolume = value;
				if (m_fadeType == FadeType.NONE)
				{
					volume = value;
				}
			}
		}

		public Source(CriAtomSource atomSource)
		{
			m_atomSource = atomSource;
		}

		private PlayId GeneratePlayId()
		{
			do
			{
				s_playIdBase = (PlayId)((int)(s_playIdBase + 1) % int.MaxValue);
			}
			while (s_playIdBase == PlayId.NONE || m_playbacks.ContainsKey(s_playIdBase));
			return s_playIdBase;
		}

		public PlayId Play(string cueName)
		{
			m_atomSource.volume = m_masterVolume;
			CriAtomExPlayback playback = m_atomSource.Play(cueName);
			m_playId = GeneratePlayId();
			m_playbacks.Add(m_playId, new Playback(playback, cueName, cueSheet));
			Removes();
			return m_playId;
		}

		public void Stop()
		{
			m_atomSource.Stop();
			FadeClear();
		}

		public void Pause(bool sw)
		{
			m_atomSource.Pause(sw);
			m_isPaused = sw;
		}

		public void Stop(PlayId playId)
		{
			if (m_playbacks.ContainsKey(playId))
			{
				m_playbacks[playId].Stop();
			}
		}

		public void Pause(PlayId playId, bool sw)
		{
			if (m_playbacks.ContainsKey(playId))
			{
				m_playbacks[playId].Pause(sw);
			}
		}

		private void Removes()
		{
			List<PlayId> list = new List<PlayId>();
			foreach (PlayId key in m_playbacks.Keys)
			{
				if (m_playbacks[key].status == CriAtomExPlayback.Status.Removed)
				{
					list.Add(key);
				}
			}
			foreach (PlayId item in list)
			{
				m_playbacks.Remove(item);
			}
		}

		public void FadeOutStart(float fadeTime)
		{
			FadeStart(FadeType.OUT, fadeTime, -1f);
		}

		public void FadeOffStart(float fadeTime)
		{
			FadeStart(FadeType.OFF, fadeTime, -1f);
		}

		public void FadeInStart(float fadeTime, float startVolume = -1f)
		{
			FadeStart(FadeType.IN, fadeTime, startVolume);
		}

		private void FadeStart(FadeType fadeType, float fadeTime, float startVolume)
		{
			if (!IsPlayingStatus(status))
			{
				return;
			}
			if (fadeTime == 0f)
			{
				FadeEnd(fadeType);
				return;
			}
			m_fadeType = fadeType;
			m_fadeTime = fadeTime;
			if (startVolume != -1f)
			{
				volume = startVolume;
			}
		}

		private void FadeEnd(FadeType fadeType)
		{
			if (fadeType == FadeType.OUT)
			{
				Stop();
				volume = m_masterVolume;
			}
			else
			{
				volume = GetTargetVolume(fadeType);
			}
			FadeClear();
		}

		private void FadeClear()
		{
			m_fadeType = FadeType.NONE;
			m_fadeTime = 0f;
		}

		private float GetTargetVolume(FadeType fadeType)
		{
			return (fadeType != FadeType.IN) ? 0f : m_masterVolume;
		}

		public void FixedUpdate()
		{
			if (m_fadeType != 0 && !m_isPaused)
			{
				float targetVolume = GetTargetVolume(m_fadeType);
				if (m_fadeTime > Time.fixedDeltaTime)
				{
					volume += (targetVolume - volume) * Time.fixedDeltaTime / m_fadeTime;
					m_fadeTime -= Time.fixedDeltaTime;
				}
				else
				{
					FadeEnd(m_fadeType);
				}
			}
		}
	}

#if UNITY_ANDROID
	private const string PLATFORM_DATA_PATH = "Android/";
#elif UNITY_IPHONE
	private const string PLATFORM_DATA_PATH = "iPhone/";
#else
	// Fallback
	private const string PLATFORM_DATA_PATH = "Android/";
#endif

	private const string ACF_FILE_PATH = PLATFORM_DATA_PATH + "Sonic_Runners_Sound.acf";

	[SerializeField]
	private bool m_isOutputPlayLog;

	private static SoundManager s_instance;

	private List<string> m_cueSheetNameList = new List<string>();

	private Source[] m_sources = new Source[3];

	private float m_bgmVolume = 1f;

	private float m_seVolume = 1f;

	private string proxyHost;

	private ushort proxyPort;

	public static float BgmVolume
	{
		get
		{
			return (!(s_instance != null)) ? 0f : s_instance.m_bgmVolume;
		}
		set
		{
			if (s_instance != null)
			{
				s_instance.m_bgmVolume = value;
				for (SourceId sourceId = SourceId.BGM_NORMAL; sourceId < SourceId.SE; sourceId++)
				{
					GetSource(sourceId).masterVolume = value;
				}
			}
		}
	}

	public static float SeVolume
	{
		get
		{
			return (!(s_instance != null)) ? 0f : s_instance.m_seVolume;
		}
		set
		{
			if (s_instance != null)
			{
				s_instance.m_seVolume = value;
				GetSource(SourceId.SE).masterVolume = value;
			}
		}
	}

	private void Start()
	{
		Initialize();
		s_instance = this;
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				BgmVolume = (float)systemdata.bgmVolume / 100f;
				SeVolume = (float)systemdata.seVolume / 100f;
			}
		}
	}

	private void Update()
	{
		base.gameObject.transform.position = new Vector3(-1000f, -1000f, 0f);
	}

	private void FixedUpdate()
	{
		if (!(s_instance == null))
		{
			for (SourceId sourceId = SourceId.BGM_NORMAL; sourceId < SourceId.SE; sourceId++)
			{
				GetSource(sourceId).FixedUpdate();
			}
		}
	}

	private void OnDestroy()
	{
		m_cueSheetNameList = new List<string>();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			SetProxyToCriFs();
		}
	}

	public static void AddTitleCueSheet()
	{
		AddCueSheet("BGM", "BGM_title.acb", "BGM_title_streamfiles.awb");
		AddCueSheet("SE", "se_runners_title.acb", null);
	}

	public static void AddMainMenuCommonCueSheet()
	{
		AddCueSheet("BGM", "BGM_menu.acb", "BGM_menu_streamfiles.awb", true);
		AddCueSheet("BGM_menu_v2", "BGM_menu_v2.acb", "BGM_menu_v2_streamfiles.awb", true);
		AddCueSheet("SE", "se_runners.acb", null, true);
	}

	public static void AddMainMenuEventCueSheet()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsInEvent())
		{
			AddEventBgmCueSheet(EventManager.Instance.Type);
			AddEventSeCueSheet(EventManager.Instance.Type);
		}
	}

	public static void AddStageCommonCueSheet()
	{
		AddCueSheet("SE", "se_runners.acb", null, true);
	}

	public static void AddStageCueSheet(string stageCueSheetName)
	{
		if (stageCueSheetName != null && stageCueSheetName != string.Empty)
		{
			AddCueSheet("BGM", stageCueSheetName + ".acb", stageCueSheetName + "_streamfiles.awb", true);
		}
		AddCueSheet("BGM_jingle", "BGM_jingle.acb", "BGM_jingle_streamfiles.awb", true);
		if (EventManager.Instance != null)
		{
			if (EventManager.Instance.IsSpecialStage())
			{
				AddEventSeCueSheet(EventManager.EventType.SPECIAL_STAGE);
			}
			if (EventManager.Instance.IsRaidBossStage())
			{
				AddEventSeCueSheet(EventManager.EventType.RAID_BOSS);
			}
		}
	}

	private static void AddEventBgmCueSheet(EventManager.EventType type)
	{
		if (EventCommonDataTable.Instance != null)
		{
			string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.MenuBgmFileName);
			if (data != null && data != string.Empty)
			{
				AddCueSheet("BGM_" + EventManager.GetEventTypeName(type), data + ".acb", data + "_streamfiles.awb", true);
			}
		}
	}

	private static void AddEventSeCueSheet(EventManager.EventType type)
	{
		if (EventCommonDataTable.Instance != null)
		{
			string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.SeFileName);
			if (data != null && data != string.Empty)
			{
				AddCueSheet("SE_" + EventManager.GetEventTypeName(type), data + ".acb", null, true);
			}
		}
	}

	private void Initialize()
	{
		CriWareErrorHandler criWareErrorHandler = Object.FindObjectOfType<CriWareErrorHandler>();
		if (criWareErrorHandler != null)
		{
			criWareErrorHandler.enabled = false;
		}
		CriAtomEx.RegisterAcf(null, Path.Combine(CriWare.streamingAssetsPath, PLATFORM_DATA_PATH + "Sonic_Runners_Sound.acf"));
		for (int i = 0; i < m_sources.Length; i++)
		{
			m_sources[i] = new Source(base.gameObject.AddComponent<CriAtomSource>());
		}
		SetProxyToCriFs();
	}

	private static bool IsPlayingStatus(CriAtomSource.Status status)
	{
		return status == CriAtomSource.Status.Playing || status == CriAtomSource.Status.Prep;
	}

	private static bool IsPlayingStatus(CriAtomExPlayback.Status status)
	{
		return status == CriAtomExPlayback.Status.Playing || status == CriAtomExPlayback.Status.Prep;
	}

	private static List<Playback> FindPlayingPlayback(string cueName, string cueSheetName)
	{
		List<Playback> list = new List<Playback>();
		Source[] sources = s_instance.m_sources;
		foreach (Source source in sources)
		{
			foreach (PlayId key in source.playbacks.Keys)
			{
				Playback playback = source.playbacks[key];
				if (IsPlayingStatus(playback.status) && playback.cueName == cueName && playback.cueSheet == cueSheetName)
				{
					list.Add(playback);
				}
			}
		}
		return list;
	}

	public static string[] GetCueSheetNameList()
	{
		if (s_instance == null)
		{
			return new string[0];
		}
		return s_instance.m_cueSheetNameList.ToArray();
	}

	public static bool ExistsCueSheet(string cueSheetName)
	{
		if (s_instance == null)
		{
			return false;
		}
		return s_instance.m_cueSheetNameList.Contains(cueSheetName);
	}

	public static CriAtomEx.CueInfo[] GetCueInfoList(string cueSheetName)
	{
		if (!ExistsCueSheet(cueSheetName))
		{
			return null;
		}
		CriAtomExAcb acb = CriAtom.GetAcb(cueSheetName);
		return (acb == null) ? null : acb.GetCueInfoList();
	}

	public static Source[] GetSourseList()
	{
		if (s_instance == null)
		{
			return new Source[0];
		}
		return s_instance.m_sources;
	}

	private static Source GetSource(SourceId sourceId)
	{
		return s_instance.m_sources[(int)sourceId];
	}

	private static PlayId Play(SourceId sourceId, string cueName, string cueSheetName, bool loopFlag = false)
	{
		Source source = GetSource(sourceId);
		if (sourceId >= SourceId.BGM_NORMAL && sourceId < SourceId.SE)
		{
			source.Stop();
		}
		if (!ExistsCueSheet(cueSheetName))
		{
			Debug.LogWarning("CueSheet " + cueSheetName + " not loaded.");
			return PlayId.NONE;
		}
		source.cueSheet = cueSheetName;
		source.cueName = cueName;
		source.loop = loopFlag;
		return source.Play(cueName);
	}

	private static void Change(SourceId sourceId, string cueName, string cueSheetName, bool loopFlag = false)
	{
		Source source = GetSource(sourceId);
		if (!IsPlayingStatus(source.status) || !(source.cueName == cueName))
		{
			Play(sourceId, cueName, cueSheetName, loopFlag);
		}
	}

	private static void Stop(SourceId sourceId)
	{
		GetSource(sourceId).Stop();
	}

	private static void Stop(SourceId sourceId, PlayId playId)
	{
		GetSource(sourceId).Stop(playId);
	}

	private static void Stop(SourceId sourceId, string cueName, string cueSheetName)
	{
		foreach (Playback item in FindPlayingPlayback(cueName, cueSheetName))
		{
			item.Stop();
		}
	}

	private static void Pause(SourceId sourceId, bool sw)
	{
		GetSource(sourceId).Pause(sw);
	}

	private static void PausePlaying(SourceId sourceId, bool sw)
	{
		Source source = GetSource(sourceId);
		foreach (PlayId key in source.playbacks.Keys)
		{
			Playback playback = source.playbacks[key];
			playback.Pause(sw);
		}
	}

	public static string GetDownloadURL()
	{
		return NetBaseUtil.AssetServerURL + "sound/" + PLATFORM_DATA_PATH;
	}

	public static string GetDownloadedDataPath()
	{
		return CriWare.installTargetPath + "/";
	}

	public static void AddCueSheet(string cueSheetName, string acbFile, string awbFile, bool isUrlLoad = false)
	{
		if (!(s_instance == null) && !ExistsCueSheet(cueSheetName))
		{
			s_instance.m_cueSheetNameList.Add(cueSheetName);
			string str = (!isUrlLoad) ? PLATFORM_DATA_PATH : GetDownloadedDataPath();
			string acbFile2 = (acbFile == null) ? null : (str + acbFile);
			string awbFile2 = (awbFile == null) ? null : (str + awbFile);
			CriAtomCueSheet criAtomCueSheet = CriAtom.AddCueSheet(cueSheetName, acbFile2, awbFile2);
		}
	}

	public static void RemoveCueSheet(string cueSheetName)
	{
		if (!(s_instance == null))
		{
			s_instance.m_cueSheetNameList.Remove(cueSheetName);
			CriAtom.RemoveCueSheet(cueSheetName);
		}
	}

	public static void BgmPlay(string cueName, string cueSheetName = "BGM", bool loop = false)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmPlay(" + cueName + ", " + cueSheetName + ")");
			Play(SourceId.BGM_NORMAL, cueName, cueSheetName, loop);
		}
	}

	public static void BgmChange(string cueName, string cueSheetName = "BGM")
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmChange(" + cueName + ", " + cueSheetName + ")");
			Change(SourceId.BGM_NORMAL, cueName, cueSheetName, true);
		}
	}

	public static void BgmStop()
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmStop()");
			Stop(SourceId.BGM_NORMAL);
			Stop(SourceId.BGM_CROSSFADE);
		}
	}

	public static void BgmPause(bool sw)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmPause(" + sw + ")");
			Pause(SourceId.BGM_NORMAL, sw);
			Pause(SourceId.BGM_CROSSFADE, sw);
		}
	}

	public static void BgmFadeOut(float fadeOutTime)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmFadeOut(" + fadeOutTime + ")");
			GetSource(SourceId.BGM_NORMAL).FadeOutStart(fadeOutTime);
			GetSource(SourceId.BGM_CROSSFADE).FadeOutStart(fadeOutTime);
		}
	}

	public static void BgmCrossFadePlay(string cueName, string cueSheetName = "BGM", float fadeOutTime = 0f)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmCrossFadePlay(" + cueName + ", " + cueSheetName + ", " + fadeOutTime + ")");
			GetSource(SourceId.BGM_NORMAL).FadeOffStart(fadeOutTime);
			Play(SourceId.BGM_CROSSFADE, cueName, cueSheetName, true);
		}
	}

	public static void ItemBgmCrossFadePlay(string cueName, string cueSheetName = "BGM", float fadeOutTime = 0f)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmCrossFadePlay(" + cueName + ", " + cueSheetName + ", " + fadeOutTime + ")");
			Source source = GetSource(SourceId.BGM_CROSSFADE);
			if (!IsPlayingStatus(source.status) || !(source.cueName == cueName))
			{
				GetSource(SourceId.BGM_NORMAL).FadeOffStart(fadeOutTime);
				Play(SourceId.BGM_CROSSFADE, cueName, cueSheetName, true);
			}
		}
	}

	public static void BgmCrossFadeStop(float fadeOutTime = 0f, float fadeInTime = 0f)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("BgmCrossFadeStop(" + fadeOutTime + ", " + fadeInTime + ")");
			GetSource(SourceId.BGM_CROSSFADE).FadeOutStart(fadeOutTime);
			GetSource(SourceId.BGM_NORMAL).FadeInStart(fadeInTime);
		}
	}

	public static PlayId SePlay(string cueName, string cueSheetName = "SE")
	{
		if (s_instance == null)
		{
			return PlayId.NONE;
		}
		OutputPlayLog("SePlay(" + cueName + ", " + cueSheetName + ")");
		return Play(SourceId.SE, cueName, cueSheetName);
	}

	public static void SeStop(string cueName, string cueSheetName = "SE")
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("SeStop(" + cueName + ", " + cueSheetName + ")");
			Stop(SourceId.SE, cueName, cueSheetName);
		}
	}

	public static void SeStop(PlayId playId)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog(string.Concat("SeStop(", playId, ")"));
			Stop(SourceId.SE, playId);
		}
	}

	public static void SePause(bool sw)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("SePause(" + sw + ")");
			Pause(SourceId.SE, sw);
		}
	}

	public static void SePausePlaying(bool sw)
	{
		if (!(s_instance == null))
		{
			OutputPlayLog("SePausePlaying(" + sw + ")");
			PausePlaying(SourceId.SE, sw);
		}
	}

	private void SetProxyToCriFs()
	{
		string host;
		ushort port;
		GetSystemProxy(out host, out port);
		if (host != proxyHost || port != proxyPort)
		{
			CriFsUtility.SetProxyServer(host, port);
			Debug.Log("SetProxyToCriFs: " + host + ":" + port);
			proxyHost = host;
			proxyPort = port;
		}
	}

	public static void SetProxyForDownloadData()
	{
		if (!(s_instance == null))
		{
			s_instance.SetProxyToCriFs();
		}
	}

	private static void GetSystemProxy(out string host, out ushort port)
	{
		Binding.Instance.GetSystemProxy(out host, out port);
	}

	private static void OutputPlayLog(string s)
	{
		if (s_instance != null && s_instance.m_isOutputPlayLog)
		{
			Debug.Log("SoundManager PlayLog: " + s);
		}
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
