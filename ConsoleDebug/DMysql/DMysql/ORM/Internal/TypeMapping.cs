using System;
using System.Collections.Generic;

namespace studio.da.DMySQL
{
    public class TypeMapping
    {
        protected static Dictionary<Type, string> Mapping = new Dictionary<Type, string>();

        protected TypeMapping() 
        {
            Mapping.Add(typeof(int)," int ");
            Mapping.Add(typeof(string)," text ");
            Mapping.Add(typeof(Int64)," bigint ");
            Mapping.Add(typeof(byte[])," binary ");
            Mapping.Add(typeof(bool)," bit ");
            Mapping.Add(typeof(DateTime)," datetime ");
            Mapping.Add(typeof(decimal)," decimal ");
            Mapping.Add(typeof(float)," float ");
            Mapping.Add(typeof(double)," decimal ");
            Mapping.Add(typeof(object), " Variant ");
            Mapping.Add(typeof(byte)," tinyint ");
        }

        protected static TypeMapping _Instance = new TypeMapping();

        public string getSQLType(Type t)
        {
            if(Mapping.ContainsKey(t))
            {
                return Mapping[t];
            }
            else
            {
                return " text "; 
            }
        }

        // 需要用到Instance的构造方法 所以还是要借助Instance对象成员方法的身份调用
        public static string GetSqlType(Type t)
        {
            return _Instance.getSQLType(t);
        }

    }
}
