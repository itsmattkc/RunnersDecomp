using System.Collections;
using UnityEngine;

public class HtmlLoaderASync : HtmlLoader
{
	protected override void OnSetup()
	{
	}

	private void Start()
	{
		StartCoroutine(WaitLoadAsync());
	}

	private IEnumerator WaitLoadAsync()
	{
		WWW www = GetWWW();
		while (!www.isDone)
		{
			yield return null;
		}
		base.IsEndLoad = true;
	}
}
