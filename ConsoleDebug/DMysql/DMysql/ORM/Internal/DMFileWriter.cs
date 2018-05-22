using System;
using System.IO;
using System.Text;

namespace studio.da.DMySQL
{
    public class DMFileWriter
    {
		FileStream fs;

		protected StreamWriter sw;

        public DMFileWriter(string filePath)
        {
            fs = new FileStream(filePath, FileMode.OpenOrCreate);

            sw = new StreamWriter(fs);

        }

        public void Write(string content,bool isLine)
        {
            if (isLine)
            {
                sw.WriteLine(content);
            }
            else
            {
                sw.Write(content);
            }

            sw.Flush();

        }

        public void Finished()
        {
            sw.Close();

            fs.Close();

        }

    }
}
