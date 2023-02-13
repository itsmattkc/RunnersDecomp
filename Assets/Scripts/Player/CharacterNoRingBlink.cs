using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class CharacterNoRingBlink : MonoBehaviour
	{
		private const string ShaderName = "ykChrLine_dme1";

		private const string ChangeParamName = "_OutlineColor";

		private const float BlinkTime = 0.5f;

		private readonly Color RedColor = Color.red;

		private Color m_defaultColor;

		private Color m_nowColor;

		private List<Material> m_materialList = new List<Material>();

		private float m_timer;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		public void SetEnable()
		{
			m_nowColor = RedColor;
			UpdateColor(m_nowColor);
			base.enabled = true;
		}

		public void SetDisable()
		{
			UpdateColor(m_defaultColor);
			base.enabled = false;
		}

		private void Update()
		{
			float num = 0.25f;
			m_timer += Time.deltaTime;
			if (m_timer < num)
			{
				m_nowColor = Color.Lerp(RedColor, m_defaultColor, Mathf.Clamp(m_timer / num, 0f, 1f));
			}
			else
			{
				m_nowColor = Color.Lerp(m_defaultColor, RedColor, Mathf.Clamp((m_timer - num) / num, 0f, 1f));
			}
			if (m_timer >= 0.5f)
			{
				m_timer = Mathf.Max(m_timer - 0.5f, 0f);
			}
			UpdateColor(m_nowColor);
		}

		public void Setup(GameObject model)
		{
			foreach (Transform item in model.transform)
			{
				Renderer component = item.GetComponent<Renderer>();
				if (!(component != null))
				{
					continue;
				}
				Material[] materials = component.materials;
				Material[] array = materials;
				foreach (Material material in array)
				{
					if (material.HasProperty("_OutlineColor"))
					{
						m_materialList.Add(material);
					}
				}
			}
			if (m_materialList.Count > 0)
			{
				m_defaultColor = m_materialList[0].GetColor("_OutlineColor");
			}
		}

		public void UpdateColor(Color color)
		{
			foreach (Material material in m_materialList)
			{
				material.SetColor("_OutlineColor", color);
			}
		}
	}
}
