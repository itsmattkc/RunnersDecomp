using UnityEngine;

[ExecuteInEditMode]
public class TenseEffect : MonoBehaviour
{
	[SerializeField]
	private string m_tenseTypeA = "DEFAULT";

	[SerializeField]
	private string m_tenseTypeB = "DEFAULT";

	private Color m_TenseColorA = Color.white;

	private Color m_TenseColorB = Color.white;

	private MaterialPropertyBlock m_MaterialProperty;

	private TenseEffectManager.Type m_tenseType;

	private void Start()
	{
		m_MaterialProperty = new MaterialPropertyBlock();
		m_TenseColorA = TenseEffectTable.GetItemData(m_tenseTypeA);
		m_TenseColorB = TenseEffectTable.GetItemData(m_tenseTypeB);
		if (TenseEffectManager.Instance != null)
		{
			m_tenseType = TenseEffectManager.Instance.GetTenseType();
		}
		Color color = (m_tenseType != 0) ? m_TenseColorB : m_TenseColorA;
		ModifyMaterialLightColor(color);
	}

	private void Update()
	{
	}

	private void ModifyMaterialLightColor(Color color)
	{
		if (m_MaterialProperty != null)
		{
			m_MaterialProperty.Clear();
			m_MaterialProperty.AddColor("_AmbientColor", color);
		}
		base.renderer.SetPropertyBlock(m_MaterialProperty);
	}
}
