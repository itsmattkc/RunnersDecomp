using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DrillTrack : MonoBehaviour
{
	private class History
	{
		public Vector3 m_Position = Vector3.zero;

		public float m_UVOffset;

		public bool m_Visible = true;
	}

	private class StripMeshData
	{
		public Vector3[] m_Vertices;

		public Vector2[] m_UV;

		public Color[] m_Colors;

		public int[] m_Triangles;
	}

	private const float TRACK_RADIUS = 0.4f;

	private const float THRESHOLD = 0.3f;

	private const int HISTORY_SIZE = 100;

	public GameObject m_Target;

	public Transform m_Camera;

	private bool m_disable;

	private float m_frnotOffset;

	private CircularBuffer<History> m_HistoryBuffer;

	private StripMeshData m_MeshData;

	public bool Disable
	{
		get
		{
			return m_disable;
		}
		set
		{
			m_disable = value;
		}
	}

	public float FrontOffset
	{
		set
		{
			m_frnotOffset = value;
		}
	}

	private void Start()
	{
		m_HistoryBuffer = new CircularBuffer<History>(100);
		m_MeshData = new StripMeshData();
		m_MeshData.m_Vertices = new Vector3[400];
		m_MeshData.m_UV = new Vector2[400];
		m_MeshData.m_Colors = new Color[400];
		m_MeshData.m_Triangles = new int[600];
	}

	private void Update()
	{
		if (m_Target != null)
		{
			Vector3 position = m_Target.transform.position;
			position += m_Target.transform.forward * m_frnotOffset;
			AddHistory(position);
		}
	}

	private void AddHistory(Vector3 position)
	{
		bool flag = false;
		if (m_HistoryBuffer.Size == 0)
		{
			flag = true;
		}
		else
		{
			History at = m_HistoryBuffer.GetAt(m_HistoryBuffer.Tail);
			if (Vector3.Distance(position, at.m_Position) > 0.3f)
			{
				flag = true;
			}
		}
		if (flag)
		{
			History history = new History();
			history.m_Position = position;
			if (m_HistoryBuffer.Size > 0)
			{
				History at2 = m_HistoryBuffer.GetAt(m_HistoryBuffer.Tail);
				float num = Vector3.Distance(position, at2.m_Position);
				history.m_UVOffset = at2.m_UVOffset + num / 0.8f;
			}
			if (m_disable)
			{
				history.m_Visible = false;
			}
			m_HistoryBuffer.Add(history);
			if (m_HistoryBuffer.Size > 1)
			{
				UpdateTrack();
			}
		}
	}

	private void UpdateTrack()
	{
		if (m_Camera == null)
		{
			return;
		}
		for (int i = 1; i < m_HistoryBuffer.Size; i++)
		{
			int index = (m_HistoryBuffer.Capacity + m_HistoryBuffer.Head + i - 1) % m_HistoryBuffer.Capacity;
			int index2 = (m_HistoryBuffer.Capacity + m_HistoryBuffer.Head + i) % m_HistoryBuffer.Capacity;
			History at = m_HistoryBuffer.GetAt(index);
			History at2 = m_HistoryBuffer.GetAt(index2);
			Vector3 position = at.m_Position;
			Vector3 position2 = at2.m_Position;
			Vector3 a = m_Camera.TransformDirection(Vector3.forward);
			Vector3 normalized = (position2 - position).normalized;
			Vector3 a2 = Vector3.Cross(normalized, -a);
			m_MeshData.m_Vertices[(i - 1) * 4] = position - a2 * 0.4f;
			m_MeshData.m_Vertices[(i - 1) * 4 + 1] = position + a2 * 0.4f;
			m_MeshData.m_Vertices[(i - 1) * 4 + 2] = position2 - a2 * 0.4f;
			m_MeshData.m_Vertices[(i - 1) * 4 + 3] = position2 + a2 * 0.4f;
			if (i > 1)
			{
				Vector3 vector = (m_MeshData.m_Vertices[(i - 2) * 4 + 2] + m_MeshData.m_Vertices[(i - 1) * 4]) / 2f;
				Vector3 vector2 = (m_MeshData.m_Vertices[(i - 2) * 4 + 3] + m_MeshData.m_Vertices[(i - 1) * 4 + 1]) / 2f;
				m_MeshData.m_Vertices[(i - 2) * 4 + 2] = vector;
				m_MeshData.m_Vertices[(i - 1) * 4] = vector;
				m_MeshData.m_Vertices[(i - 2) * 4 + 3] = vector2;
				m_MeshData.m_Vertices[(i - 1) * 4 + 1] = vector2;
			}
			m_MeshData.m_UV[(i - 1) * 4] = Vector2.right * at.m_UVOffset;
			m_MeshData.m_UV[(i - 1) * 4 + 1] = Vector2.up + Vector2.right * at.m_UVOffset;
			m_MeshData.m_UV[(i - 1) * 4 + 2] = Vector2.right * at2.m_UVOffset;
			m_MeshData.m_UV[(i - 1) * 4 + 3] = Vector2.up + Vector2.right * at2.m_UVOffset;
			m_MeshData.m_Colors[(i - 1) * 4] = ((!at.m_Visible) ? Color.clear : Color.white);
			m_MeshData.m_Colors[(i - 1) * 4 + 1] = ((!at.m_Visible) ? Color.clear : Color.white);
			m_MeshData.m_Colors[(i - 1) * 4 + 2] = ((!at2.m_Visible) ? Color.clear : Color.white);
			m_MeshData.m_Colors[(i - 1) * 4 + 3] = ((!at2.m_Visible) ? Color.clear : Color.white);
			m_MeshData.m_Triangles[(i - 1) * 6] = 0 + (i - 1) * 4;
			m_MeshData.m_Triangles[(i - 1) * 6 + 1] = 1 + (i - 1) * 4;
			m_MeshData.m_Triangles[(i - 1) * 6 + 2] = 2 + (i - 1) * 4;
			m_MeshData.m_Triangles[(i - 1) * 6 + 3] = 2 + (i - 1) * 4;
			m_MeshData.m_Triangles[(i - 1) * 6 + 4] = 1 + (i - 1) * 4;
			m_MeshData.m_Triangles[(i - 1) * 6 + 5] = 3 + (i - 1) * 4;
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = m_MeshData.m_Vertices;
		mesh.uv = m_MeshData.m_UV;
		mesh.colors = m_MeshData.m_Colors;
		mesh.triangles = m_MeshData.m_Triangles;
		mesh.RecalculateBounds();
	}
}
