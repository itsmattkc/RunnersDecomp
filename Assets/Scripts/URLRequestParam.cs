public class URLRequestParam
{
	private string mPropertyName;

	private string mValue;

	public string propertyName
	{
		get
		{
			return mPropertyName;
		}
	}

	public string value
	{
		get
		{
			return mValue;
		}
	}

	public URLRequestParam(string propertyName, string value)
	{
		mPropertyName = propertyName;
		mValue = value;
	}
}
