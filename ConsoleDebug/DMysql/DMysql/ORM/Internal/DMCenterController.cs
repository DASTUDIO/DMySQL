using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using studio.da.DMySQL;

namespace studio.da.DMySQL
{
    public class DMCenterController
	{
        Dictionary<string, string> configs = new Dictionary<string, string>();

		public static Dictionary<string,string> Configs { get { return _Instance.configs; }}
        
        #region SingleTon

        private static DMCenterController _Instance = new DMCenterController();

        public static DMCenterController getInstance()
        {
            return _Instance;
        }

        private DMCenterController()
        {
            #region Read Config File

            Console.Write("DMySQL is Running at:");

            Console.Write(System.Environment.CurrentDirectory+"\n\n");

            if (File.Exists(System.Environment.CurrentDirectory + "/dmysql.config"))
            {
                try
                {
					DMFileReader reader = new DMFileReader(System.Environment.CurrentDirectory + "/dmysql.config");

                    configs = reader.Read();

                    if (configs == null)
                    {
                        configs = new Dictionary<string, string>();
                    }
                    else
                    {
                        Console.Write("---> Read Config File:");

						if (configs.Count <= 0)
						{
							Console.Write("No Configs");
							return;
						}

                        foreach (var configItem in configs)
                        {
                            Console.Write(configItem.Key + " => " + configItem.Value+"\n");
                        }
                    }
                }catch(Exception e)
                {
                    Console.Write("readFile Failed : "+e.Message,0);
                }
            
            }
            else
            {
                try
                {
					configs.Add("Error", "Can not find file : dmysql.config");

					DMFileWriter fw = new DMFileWriter(System.Environment.CurrentDirectory + "/dmysql.config");

                    fw.Write(@"# Add Your Config Here !" + "\n", true);


                    fw.Write(@"# " + DM_Strings.MySql_Host + " =", true);

                    fw.Write(@"# " + DM_Strings.Mysql_Port + " =", true);

                    fw.Write(@"# " + DM_Strings.MySql_DB + " =", true);

                    fw.Write(@"# " + DM_Strings.MySql_UserName + " =", true);

                    fw.Write(@"# " + DM_Strings.MySql_PassWord + " =\n", true);

                    fw.Write(@"# " + DM_Strings.rewrite_Table + " =\n", true);

              
                    fw.Finished();
                }
                catch(Exception e)
                {
                    Console.Write("Create File Error "+e.Message+e.StackTrace,0);
                }
            }

            #endregion

            Console.Write("Welcome to DMySQL\n");
           
        }

        #endregion
        
        #region Facade Methods

        public static string getCurrentPath()
        {
            return Environment.CurrentDirectory;
        }

		public void init(){}
        
        #endregion    

        #region getConfigFiles

        #region MySQL

        public static string getMySQLHost()
        {
            if (Configs.ContainsKey(DM_Strings.MySql_Host))
            {
                return Configs[DM_Strings.MySql_Host];
            }
            else
            {
                return null;
            }
        }

        public static string getMySQLPort()
        {
            if (Configs.ContainsKey(DM_Strings.Mysql_Port))
            {
                return Configs[DM_Strings.Mysql_Port];
            }
            else
            {
                return null;
            }
        }

		public static string getMySQLDB()
		{
            if (Configs.ContainsKey(DM_Strings.MySql_DB))
			{
                return Configs[DM_Strings.MySql_DB];
			}
			else
			{
				return null;
			}
		}

        public static string getMySQLUserName()
        {
            if (Configs.ContainsKey(DM_Strings.MySql_UserName))
            {
                return Configs[DM_Strings.MySql_UserName];
            }
            else
            {
                return null;
            }
        }

        public static string getMySQLPassWord()
        {
            if (Configs.ContainsKey(DM_Strings.MySql_PassWord))
            {
                return Configs[DM_Strings.MySql_PassWord];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #endregion

    }
}