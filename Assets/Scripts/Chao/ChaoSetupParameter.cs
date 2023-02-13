using UnityEngine;

namespace Chao
{
	public class ChaoSetupParameter : MonoBehaviour
	{
		[SerializeField]
		private ChaoSetupParameterData m_data = new ChaoSetupParameterData();

		public ChaoSetupParameterData Data
		{
			get
			{
				return m_data;
			}
		}

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Vector3 localPosition = base.transform.localPosition;
			base.transform.localPosition = m_data.ColliCenter;
			Gizmos.DrawWireSphere(base.transform.position, m_data.ColliRadius);
			base.transform.localPosition = localPosition;
		}
	}
}
