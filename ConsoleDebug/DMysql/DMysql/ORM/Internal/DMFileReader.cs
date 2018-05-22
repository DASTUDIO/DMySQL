using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace studio.da.DMySQL
{
    public class DMFileReader
    {
        protected StreamReader sr;

		public DMFileReader(string filePath) : this(filePath, DMySQL.DataBaseEncoding) {}

        public DMFileReader(string filePath, Encoding codingType)
        {
            sr = new StreamReader(filePath, codingType);
        }

        public Dictionary<string,string> Read()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string line;
            
            while((line = sr.ReadLine()) != null)
            {

                if (line.Length > 0)
                {
      //              if(line.Substring(0, 1) == @"#")
      //              {
						////SFLog.ERROR("fuck");
                    //    result.Add(SF_STRINGS.Licence,
                    //               line.Replace(@"#", ""));

                    //    line = "";

                    //    continue;
                    //}

                    if (line.Substring(0, 1) == @"#" )
                    {
                        line = "";

                        continue;

                    }
                }
                else 
                { 
                    continue; 
                }

				string[] strPair = line.Split('=');

                line = "";

                if (strPair.Length == 2)
                {
                    result.Add(
                        strPair[0].ToUpper().Replace("\"","").Replace("\'","").TrimStart().TrimEnd().Replace(";",""),
                        strPair[1].Replace("\"","").Replace("\'","").TrimStart().TrimEnd().Replace(";",""));
                }
                else
                {
                    continue; 
                }
            }

            sr.Close();

            return result;

        }

    }
}
