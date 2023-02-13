using System;

namespace LitJson
{
	internal struct ArrayMetadata
	{
		private Type element_type;

		private bool is_array;

		private bool is_list;

		public Type ElementType
		{
			get
			{
				if (element_type == null)
				{
					return typeof(JsonData);
				}
				return element_type;
			}
			set
			{
				element_type = value;
			}
		}

		public bool IsArray
		{
			get
			{
				return is_array;
			}
			set
			{
				is_array = value;
			}
		}

		public bool IsList
		{
			get
			{
				return is_list;
			}
			set
			{
				is_list = value;
			}
		}
	}
}
