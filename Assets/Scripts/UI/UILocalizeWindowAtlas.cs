using UnityEngine;

namespace UI
{
	[AddComponentMenu("Scripts/UI/UILocalizeWindowAtlas")]
	public class UILocalizeWindowAtlas : MonoBehaviour
	{
		[SerializeField]
		public string m_atlasName = "ui_mm_contents_word_Atlas_ja";

		private void Start()
		{
			base.enabled = false;
		}
	}
}
