using UnityEngine;

namespace Message
{
	public class MsgActivateBlock : MessageBase
	{
		public enum CheckPoint
		{
			None,
			First,
			Internal
		}

		public string m_stageName;

		public int m_blockNo;

		public int m_layerNo;

		public int m_activateID;

		public bool m_replaceStage;

		public Vector3 m_originPosition;

		public Quaternion m_originRotation;

		public CheckPoint m_checkPoint;

		public MsgActivateBlock(string stageName, int block, int layer, int activateID, Vector3 originPosition, Quaternion originrotation)
			: base(12299)
		{
			m_stageName = stageName;
			m_blockNo = block;
			m_layerNo = layer;
			m_activateID = activateID;
			m_originPosition = originPosition;
			m_originRotation = originrotation;
			m_replaceStage = false;
		}
	}
}
