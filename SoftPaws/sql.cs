using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SoftPaws
{
    class sql
    {
        public static SqlConnection str;

        public static void OpenConnection()
        {
            str = new SqlConnection
            {
                ConnectionString = @"Data Source=LAPTOP-T86M4QDS;Initial Catalog=SoftPaws;Integrated Security=True"
            };
            str.Open();
        }

        public static void CloseConnection()
        {
            str.Close();
        }
    }
}
