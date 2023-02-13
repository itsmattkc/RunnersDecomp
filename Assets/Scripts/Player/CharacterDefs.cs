using GameScore;
using UnityEngine;

namespace Player
{
	public class CharacterDefs
	{
		public const int NumMaxTrick = 5;

		public static readonly string[] PhantomBodyName = new string[3]
		{
			"pha_laser",
			"pha_spin",
			"pha_asteroid"
		};

		public static readonly Vector3 BaseFrontTangent = new Vector3(1f, 0f, 0f);

		public static readonly Vector3 BaseRightTangent = new Vector3(0f, 0f, -1f);

		public static readonly int[] TrickScore = new int[5]
		{
			Data.Trick1,
			Data.Trick2,
			Data.Trick3,
			Data.Trick4,
			Data.Trick5
		};
	}
}
