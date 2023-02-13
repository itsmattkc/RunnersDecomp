using System;
using UnityEngine;

[ExecuteInEditMode]
public class AllocationStatus : MonoBehaviour
{
	public static bool hide;

	public bool show = true;

	public bool showFPS;

	public bool showInEditor;

	public string version = string.Empty;

	private int m_collectCount;

	private float lastCollect;

	private float lastCollectNum;

	private float delta;

	private float lastDeltaTime;

	private int allocRate;

	private int lastAllocMemory;

	private float lastAllocSet = -9999f;

	private int allocMem;

	private int collectAlloc;

	private int peakAlloc;

	public void Start()
	{
		base.useGUILayout = false;
		m_collectCount = GC.CollectionCount(0);
	}
}
