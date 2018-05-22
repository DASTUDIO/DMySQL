using System;
namespace studio.da.DMySQL
{
    public class ORMAttributes { }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IsPrimaryKeyAttribute : Attribute
    {
        public bool isAutoIncrement = false;

        public IsPrimaryKeyAttribute(bool isAutoIncrement)
        {
            this.isAutoIncrement = isAutoIncrement;
        }

    }
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class CustomTypeAttribute : Attribute
	{
		public string type;
		
		public CustomTypeAttribute(string type)
		{
			this.type = " " + type + " ";
		}
	}

    [AttributeUsage(AttributeTargets.Field)]
    public class ColumeFeaturesAttribute : Attribute
    {
        public string Value = " ";

        public ColumeFeaturesAttribute(string features)
        {
            this.Value = " " + features + " ";
        }
    }

}

