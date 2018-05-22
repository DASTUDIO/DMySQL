using System;
using System.Reflection;
using System.Collections.Generic;

namespace studio.da.DMySQL
{
    public class DMySQL
    {
        protected static DMySQL _Instance = new DMySQL();

        protected DMSQLHelper _helper = new DMSQLHelper();

        static bool isInitialized = false;


        public static bool Init()
        {
            if (isInitialized) return true;
         
            Dictionary<string, string> configs = DMCenterController.Configs;

            if (configs == null)
            {
                Console.Write(" wrong config file path ",0);
                return false;
            }

            if (configs.ContainsKey(DM_Strings.MySql_Host) &&
                configs.ContainsKey(DM_Strings.Mysql_Port) &&
                configs.ContainsKey(DM_Strings.MySql_DB) &&
                configs.ContainsKey(DM_Strings.MySql_UserName) &&
                configs.ContainsKey(DM_Strings.MySql_PassWord)
               )
            {
                bool rewriteTable = false;

                if (configs.ContainsKey(DM_Strings.rewrite_Table))
                {
                    if (configs[DM_Strings.rewrite_Table].ToUpper() == "TRUE")
                    {
                        rewriteTable = true;
                    }

                }

                return _Instance.InitialORM(configs[DM_Strings.MySql_DB],
                                      configs[DM_Strings.MySql_Host],
                                      configs[DM_Strings.MySql_UserName],
                                      configs[DM_Strings.MySql_PassWord],
                                      configs[DM_Strings.Mysql_Port],
                                      rewriteTable);
            }
            else
            {
                Console.Write("Cannot Connect to Mysql , Config is UnCompleted !\n\n",0);

                return false;
            }
        }

        bool InitialORM(string db,string host,string userName,string passwd,string port,bool rewriteTable)
        {
            if (_Instance._helper.ConnectDatabase(db, host, userName, passwd, port, rewriteTable))
            {
                Assembly currentAssembly = Assembly.GetEntryAssembly();

                Type[] types = currentAssembly.GetTypes();

                foreach (var typeItem in types)
                {
                    _Instance._helper.CheckIfTable(typeItem);
                }

                isInitialized = true;

                return true;

            }
            else
            {
                return false;
            }

        }

	         
        public static bool AddItem<T>(params string[] keyValuePairs)
        {
			if (!isInitialized) Init();

            if(_Instance._helper.AddItem<T>(keyValuePairs))
            {
                Console.Write("Add Item Successful!");

                return true;

            }
            else
            {
                Console.Write("Add Item Failded");

                return false;

            }
        }


        public static bool DeleteItem<T>(params string[] conditionKVPairs)
        {
			if (!isInitialized) Init();
            
			if (_Instance._helper.DeleteItem<T>(conditionKVPairs))
            {
                if (conditionKVPairs == null || conditionKVPairs.Length <= 0)
                {
                    Console.Write("Delete Table Successful!");
                }
                else
                {
                    Console.Write("Delete Item Successful!");
                }

                return true;

            }
            else
            {
                Console.Write("Delete Failed");

                return false;

            }
        }


        public static bool ChangeItem<T>(string condition,params string[] keyValuePairs)
        {
			if (!isInitialized) Init();

            if(_Instance._helper.ChangeItem<T>(condition,keyValuePairs))
            {
                Console.Write("Change Item Successful! : " + keyValuePairs[0]+"...");

                return true;

			}
			else
			{
                Console.Write("Change Item Failed! ; " + keyValuePairs[0]+"...");

                return false;

			}
        }


		public static Dictionary<string, List<string>> QueryAllByCustomString(string Columns, string Tables, string Conditions,
													 string GroupBy, string HavingCondions, string OrderBy,
													 int Limit)
        {
			if (!isInitialized) Init();

            return(_Instance._helper.QueryAllByCustomString(Columns,Tables,Conditions,GroupBy,HavingCondions,OrderBy,Limit));
        }

        public static string QuerySingleItem<T>(string column,params string[] conditions)
        {
			if (!isInitialized) Init();

            string conditionStr = "";

            if (conditions != null && conditions.Length != 0)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i].IndexOf(@"=>") > -1)
                    {
                        string ckey = conditions[i].Substring(0, conditions[i].IndexOf(@"=>"));

                        string cvalue = conditions[i].Substring(conditions[i].IndexOf(@"=>") + 2);

                        conditionStr = conditionStr + (i == 0 ? "" : " AND ") +
                            ckey.TrimStart().TrimEnd().Replace("\'", "") + @"=" +
                                "\'" + cvalue.TrimStart().TrimEnd().Replace("\'", "") + "\'";
                    }
                    else
                    {
                        continue;
                    }
                }

                return QueryAllByCustomString(column, typeof(T).Name, conditionStr, null, null, null, -1)[column][0];
            
            }
            else
            {

                return QueryAllByCustomString(column, typeof(T).Name, null, null, null, null, -1)[column][0];
            
            }        
        }

		private DMySQL() { }

		public static System.Text.Encoding DataBaseEncoding = System.Text.Encoding.UTF8;

    }
}
