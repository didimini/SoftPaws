using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SoftPaws
{
    class LoadFromDB
    {
        // метод загрузки данных combobox
        public static int LoadSinglAttribute(string request, int id)
        {
            sql.OpenConnection();

            SqlCommand command = new SqlCommand(request, sql.str);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                id = reader.GetInt32(0);
            }
            reader.Close();

            sql.CloseConnection();

            return id;
        }

        // метод загрузкии данных из List<> и Классов
        public static List<T> LoadMultAttributes<T>(string request, Func<int, string, T> createItem)
        {
            List<T> items = new List<T>();

            sql.OpenConnection();

            SqlCommand command = new SqlCommand(request, sql.str);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                T newItem = createItem(id, name);
                items.Add(newItem);
            }

            reader.Close();
            sql.CloseConnection();

            return items;
        }

    }

}
