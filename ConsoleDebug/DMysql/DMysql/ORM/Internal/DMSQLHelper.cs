using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace studio.da.DMySQL
{
    public class DMSQLHelper
    {
        #region Elements

        internal MySqlConnection mc;

        internal string DbName;

        internal string DbHost;

        internal string userName;

        internal string passWd;

        internal string port;

        internal string connectConfig;

        internal bool rewriteTable;

        #endregion

        #region Facade

        internal bool ConnectDatabase(string db, string host, string username, string passwd, string port, bool rewriteTable)
        {
            this.rewriteTable = rewriteTable;

            this.DbName = db;

            this.DbHost = host;

            this.userName = username;

            this.passWd = passwd;

            this.port = port;

            this.connectConfig =
                string.Format("Database={0};Data Source={1};User Id={2};Password={3};pooling=false;CharSet=utf8;port={4}",
                              this.DbName,
                              this.DbHost,
                              this.userName,
                              this.passWd,
                              this.port);
            try
            {
                mc = new MySqlConnection(this.connectConfig);

                Console.Write("");

                Console.Write(@"*[DataBase Ready!]*" + "\n");

                return true;

            }
            catch (Exception e)
            {
                Console.Write("Cannot create MySQL connection " + e.Message + " at:" + this.GetType().FullName, 0);

                return false;

            }

        }

        internal bool CheckIfTable(Type t)
        {
            string sqlCommandStr = "CREATE TABLE IF NOT EXISTS " + t.Name + "(";

            int SQLColumnCount = 0;

            foreach (var interfaceType in t.GetInterfaces())
            {
                if (interfaceType == typeof(DTable))
                {

                    #region Check Old Table Feature  -> set SQLColumnCount

                    string sql = "SELECT COUNT(*) FROM information_schema. COLUMNS WHERE table_schema = \'" +
                        this.DbName + "\' AND table_name = \'" + t.Name + "\'";

                    try
                    {
                        mc.Open();

                        MySqlDataReader reader = new MySqlCommand(sql, mc).ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader.HasRows)
                            {
                                SQLColumnCount = reader.GetInt32("count(*)");
                            }
                        }

                        //Console.Write(SQLColumnCount.ToString());

                        mc.Close();

                    }
                    catch (Exception e)
                    {

                        Console.Write(e.Message);

                        mc.Close();

                    }

                    #endregion

                    #region delete table if update needed

                    FieldInfo[] fields = t.GetFields();      //get Fields

                    if (fields.Length != SQLColumnCount)      // 如果存在不同版本 个人版是直接删除 高级版才是alter表 个人版可以手动做
                    {
                        //string deleteSql = "drop TABLE " + t.Name;

                        string backUpSql = "RENAME TABLE " + t.Name +" TO "+(t.Name+DateTime.Now.ToFileTime().ToString())+";";

                        try
                        {
                            if (this.rewriteTable)
                            {
                                if (SQLColumnCount != 0)
                                {
                                    Console.Write("Backup old Table : " + t.Name);

                                    mc.Open();

                                    new MySqlCommand(backUpSql, mc).ExecuteNonQuery();

                                    mc.Close();
                                }

                                Console.Write("Create the Table : " + t.Name);
                            
                            }
                            else
                            {
                                Console.Write("Cannot Create Table : " + t.Name + "\nTable Name is alreally exist, And rewriteTable Function is not Actived",0);
                            }
                        }
                        //catch (Exception e)
                        catch
                        {
                            //Console.Write("Cannot delete old Table " + t.Name + " ERROR:" + e.Message + " at:" + this.GetType().FullName, 3306);

                            Console.Write("Create the Table : " + t.Name);

                            mc.Close();
                        }
                    }
                    else
                    {
                        Console.Write("Table no changed : " + t.Name );

                        return false;
                    }

                    #endregion

                    #region Create Table

                    foreach (FieldInfo fieldItem in fields)
                    {
                        #region Create SQL Phrase

                        sqlCommandStr = sqlCommandStr +
                                        fieldItem.Name;

                        #region check if custom type

                        bool isCustomType = false;

                        string customType = "";

                        foreach (var feature in fieldItem.GetCustomAttributes())
                        {
                            if (feature is CustomTypeAttribute)
                            {
                                isCustomType = true;

                                customType = ((CustomTypeAttribute)feature).type;
                            }
                        }

                        if (isCustomType && customType != "")
                        {
                            sqlCommandStr = sqlCommandStr + customType;
                        }
                        else
                        {
                            sqlCommandStr = sqlCommandStr + TypeMapping.GetSqlType(fieldItem.FieldType);
                        }

                        #endregion

                        foreach (var feature in fieldItem.GetCustomAttributes())
                        {
                            if (feature is IsPrimaryKeyAttribute)
                            {
                                sqlCommandStr = sqlCommandStr + DM_Strings.primaryKey;

                                if (((IsPrimaryKeyAttribute)feature).isAutoIncrement)
                                {
                                    sqlCommandStr = sqlCommandStr + DM_Strings.auto_Increment;
                                }

                            }

                            if (feature is ColumeFeaturesAttribute)
                            {
                                sqlCommandStr = sqlCommandStr + ((ColumeFeaturesAttribute)feature).Value;
                            }

                        }

                        sqlCommandStr = sqlCommandStr + ",";

                        #endregion
                    }

                    #region format sql phrase

                    sqlCommandStr = sqlCommandStr.Substring(0, sqlCommandStr.Length - 1);

                    sqlCommandStr = sqlCommandStr + ");";

                    #endregion

                    #region execute sql phrase

                    try
                    {
                        mc.Open();

                        //Console.Write("************" + sqlCommandStr);

                        new MySqlCommand(sqlCommandStr, mc).ExecuteNonQuery();

                        mc.Close();

                        return true;

                    }
                    catch (Exception e)
                    {
                        Console.Write("Cannot check or create Table " + t.Name + " ERROR:" + e.Message + " at:" + this.GetType().FullName, 3306);

                    }

                    #endregion

                    #endregion

                }

            }

            return false;

        }

        internal bool AddItem<T>(params string[] kvPairs)
        {
            if (kvPairs == null || kvPairs.Length <= 0)
            {
                return false;
            }

            string columnName = "";

            string columnValue = "";                    //去除引号

            foreach (var kvPair in kvPairs)
            {
                //string[] elements = kvPair.Split(':');

                string[] elements = new string[2];

                if (kvPair.IndexOf(@"=>") > -1)
                {
                    elements[0] = kvPair.Substring(0, kvPair.IndexOf(@"=>"));

                    elements[1] = kvPair.Substring(kvPair.IndexOf(@"=>") + 2);
                }
                else
                {
                    continue;
                }


                columnName = columnName +
                    elements[0].TrimStart().TrimEnd().Replace("\'", "")
                                   + ",";

                columnValue = columnValue + "\'" +
                    elements[1]
                    .TrimStart()
                    .TrimEnd()
                    .Replace("\'", "")
                    + "\',";

            }

            columnName = columnName.Substring(0, columnName.Length - 1);

            columnValue = columnValue.Substring(0, columnValue.Length - 1);


            string sqlCommandStr = String.Format("INSERT INTO {0}({1}) VALUES ({2});",
                                                 typeof(T).Name, columnName, columnValue);


            //Console.Write(sqlCommandStr);

            try
            {
                mc.Open();

                new MySqlCommand(sqlCommandStr, mc).ExecuteNonQuery();

                mc.Close();

                return true;
            }
            catch(Exception e)
            {
                Console.Write(e.Message,0);

                mc.Close();
            }

            return false;

        }

        internal bool DeleteItem<T>(params string[] kvConditions)
        {
            if (kvConditions == null || kvConditions.Length <= 0)
            {
                string deletetablesql = "DROP TABLE " + typeof(T).Name + ";";

                //Console.Write(deletetablesql);

				try
				{
					mc.Open();

					new MySqlCommand(deletetablesql, mc).ExecuteNonQuery();

					mc.Close();

					return true;
				}
				catch (Exception e)
				{
					Console.Write(e.Message, 0);

                    mc.Close();
				}

            }
            else
            {
                string conditions = "";

                string ckey;

                string cvalue;

                for (int i = 0; i < kvConditions.Length; i++)
                {
                    if (kvConditions[i].IndexOf(@"=>") > -1)
                    {
                        ckey = kvConditions[i].Substring(0, kvConditions[i].IndexOf(@"=>"));

                        cvalue = kvConditions[i].Substring(kvConditions[i].IndexOf(@"=>") + 2);

                        // 只用AND OR完全可以执行两次来实现

                        conditions = conditions + (i == 0 ? "" : " AND ") +
                            ckey.TrimStart().TrimEnd().Replace("\'", "") +
                                @" = " +
                                "\'" +
                                cvalue.TrimStart().TrimEnd().Replace("\'", "") +
                                "\'";

                    }
                    else
                    {
                        continue;
                    }

                }

                string deleteItemSql = string.Format("delete from {0} where {1};", typeof(T).Name, conditions);

                // Console.Write(deleteItemSql);

				try
				{
					mc.Open();
					
					new MySqlCommand(deleteItemSql, mc).ExecuteNonQuery();
					
					mc.Close();
					
					return true;

				}
				catch (Exception e)
				{
                    
					Console.Write(e.Message, 0);

                    mc.Close();

				}

            }

            return false;
        
        }

        internal bool ChangeItem<T>(string kvCondition, params string[] kvPairs)
        {
            // limit one condition ,because you can query id by multi-conditions,and then use id query the item

            string sqlCommandStr = "UPDATE " + typeof(T).Name + " SET ";

            string condition = "";

            string values = "";

            if (kvCondition != "" && kvCondition != null)
            {
                if (kvCondition.IndexOf(@"=>") > -1)
                {
                    string ckey = kvCondition.Substring(0, kvCondition.IndexOf(@"=>"));

                    string cvalue = kvCondition.Substring(kvCondition.IndexOf(@"=>") + 2);

                    condition = " " + ckey.TrimStart().TrimEnd().Replace("\'", "") + @"=" +
                                          "\'" + cvalue.TrimStart().TrimEnd().Replace("\'", "") + "\' ";

                }
            }

            if (kvPairs != null && kvPairs.Length != 0)
            {
                for (int i = 0; i < kvPairs.Length; i++)
                {
                    if (kvPairs[i].IndexOf(@"=>") > -1)
                    {
                        string kkey = kvPairs[i].Substring(0, kvPairs[i].IndexOf(@"=>"));

                        string vvalue = kvPairs[i].Substring(kvPairs[i].IndexOf(@"=>") + 2);

                        values = values + (i == 0 ? "" : @",")
                                + kkey.TrimStart().TrimEnd().Replace("\'", "") + @"="
                                     + "\'" + vvalue.TrimStart().TrimEnd().Replace("\'", "") + "\'";

                    }
                    else
                    {
                        continue;
                    }
                }
			}

            sqlCommandStr = sqlCommandStr + values + " WHERE " + condition + ";";

            //Console.Write(sqlCommandStr);

			try
			{
				mc.Open();

				new MySqlCommand(sqlCommandStr, mc).ExecuteNonQuery();

				mc.Close();

				return true;

			}
			catch (Exception e)
			{

				Console.Write(e.Message, 0);

				mc.Close();

			}

			return false;
        }


        internal Dictionary<string,List<string>> QueryAllByCustomString(string Columns, string Tables, string Conditions,
                                                     string GroupBy, string HavingCondions, string OrderBy,
                                                     int Limit)
        {

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();


            #region Construct SQL Phrase

            StringBuilder sqlCommandStr = new StringBuilder();

            sqlCommandStr.Append(string.Format("SELECT {0} FROM {1}", Columns, Tables));

            if (Conditions != null && Conditions != "")
            {
                sqlCommandStr.Append(" WHERE " + Conditions);
            }

            if (GroupBy != null && GroupBy != "")
            {
                sqlCommandStr.Append(" GROUP BY " + GroupBy);
            }

            if (HavingCondions != null && HavingCondions != "")
            {
                sqlCommandStr.Append(" HAVING " + HavingCondions);
            }

            if (OrderBy != null && OrderBy != "")
            {
                sqlCommandStr.Append(" ORDER BY " + OrderBy);
            }

            if (Limit > 0)
            {
                sqlCommandStr.Append(" LIMIT " + Limit);
            }

            string SQLString = sqlCommandStr.ToString() + ";";

            //Console.Write(SQLString);

            #endregion


            #region ExecuteSQLPhrase and get result

            string[] columnItems = Columns.Split(',');

            try
            {
                mc.Open();

                MySqlDataReader reader = new MySqlCommand(SQLString,mc).ExecuteReader();

                while(reader.Read())
                {
                    if(reader.HasRows)
                    {
                        foreach(var columnItem in columnItems)
                        {
                            if(!result.ContainsKey(columnItem))
                            {
                                result.Add(columnItem, new List<string>());
                            }

                            result[columnItem].Add(reader.GetString(columnItem));

                        }
                    }
                }

				reader.Close();

                mc.Close();

                return result;
            }
            catch(Exception e)
            {
                Console.Write(e.Message,0);

				try
				{
					mc.Close();
				}catch{}
                return null;
            }

            #endregion

        }

        #endregion

        internal DMSQLHelper() { }

    }
}
