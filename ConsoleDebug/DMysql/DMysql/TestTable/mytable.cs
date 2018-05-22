using System.Reflection;
namespace studio.da.DMySQL
{
    public class mytable : DTable
    {
        [IsPrimaryKey(true)]
        public int id;

        [CustomType(@"varchar(200)")]
        public string name;


    }
}
