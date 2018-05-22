using System;
namespace studio.da.DMySQL
{
    public class TestTable : DTable
    {
        [IsPrimaryKey(true)]
        public int id;

        [ColumeFeatures("NOT NULL")]
        public string userName;

        public string passwd;

        public string msg;

    }
}
