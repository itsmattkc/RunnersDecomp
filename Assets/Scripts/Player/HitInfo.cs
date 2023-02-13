using UnityEngine;

namespace Player
{
	public struct HitInfo
	{
		public bool valid;

		public RaycastHit info;

		public void Reset()
		{
			valid = false;
		}

		public void Set(RaycastHit hit)
		{
			valid = true;
			info = hit;
		}

		public bool IsValid()
		{
			return valid;
		}
	}
}
