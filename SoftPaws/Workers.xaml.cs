using SoftPaws.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoftPaws
{
    /// <summary>
    /// Логика взаимодействия для Workers.xaml
    /// </summary>
    public partial class Workers : Window
    {
        public string log;

        public Workers(string log)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            this.log = log;
            LaodWorkersLV();
        }

        public void LaodWorkersLV()
        {
            sql.OpenConnection();
            List<WorkersItems> results = new List<WorkersItems>();

            SqlCommand command = new SqlCommand("SELECT Логин, CONCAT (Фамилия, ' ', Имя, ' ', Отчество) AS ФИО, Код_должности, Адрес, Телефон, Пароль " +
                "FROM Сотрудники ", sql.str);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                WorkersItems workersItems = new WorkersItems
                {
                    Login = reader["Логин"].ToString(),
                    Password = reader["Пароль"].ToString(),
                    FIO = reader["ФИО"].ToString(),
                    Post = reader["Код_должности"].ToString(),
                    Adress = reader["Адрес"].ToString(),
                    Phone = reader["Телефон"].ToString()
                };
                results.Add(workersItems);
            }
            reader.Close();
            WorkersLV.ItemsSource = results;
            sql.CloseConnection();
        }


        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu(log);
            menu.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
