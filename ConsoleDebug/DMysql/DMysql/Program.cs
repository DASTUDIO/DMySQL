using System;

using studio.da.DMySQL;

 class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

    		DMySQL.QuerySingleItem<mytable>("name", "email=>123");
			 
    		//DMySQL.AddItem <mytable>("x=>1");
        }
    }


