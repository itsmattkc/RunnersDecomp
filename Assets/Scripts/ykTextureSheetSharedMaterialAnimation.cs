using UnityEngine;

public class ykTextureSheetSharedMaterialAnimation : ykTextureSheetAnimation
{
	protected override Material GetMaterial()
	{
		return base.renderer.sharedMaterial;
	}

	protected override bool IsValidChange()
	{
		return false;
	}
}
