using UnityEngine;

public class HtmlLoaderSync : HtmlLoader
{
	protected override void OnSetup()
	{
		WWW wWW;
		do
		{
			wWW = GetWWW();
		}
		while (!wWW.isDone);
		base.IsEndLoad = true;
	}
}
