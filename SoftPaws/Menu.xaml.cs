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
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public string log;


        public Menu(string log)
        {
            InitializeComponent();
            // проверка пользователя и его должности
            this.log = log;
            sql.OpenConnection();
            SqlCommand command = new SqlCommand($"SELECT [Код_должности] " +
                $"FROM [dbo].[Сотрудники] " +
                $"WHERE Логин = '{log}'", sql.str);
            object result = command.ExecuteScalar();

            sql.CloseConnection();
            int postId = Convert.ToInt32(result);

            // запрет видимости кнопок для определнного пользователя
            if (postId == 2)
            {
                workersBtn.Visibility = Visibility.Collapsed;
                reportsBtn.Visibility = Visibility.Collapsed;
            }
            // развенуть полностьь окно
            WindowState = WindowState.Maximized; 
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void animalsBtn_Click(object sender, RoutedEventArgs e)
        {
            Animals animals = new Animals(log);
            animals.Show();
            this.Close();
        }

        private void Ankets_Click(object sender, RoutedEventArgs e)
        {
            Questionnaire questionnaire = new Questionnaire(log);
            questionnaire.Show();
            this.Close();
        }

        private void workersBtn_Click(object sender, RoutedEventArgs e)
        {
            Workers workers = new Workers(log);
            workers.Show();
            this.Close();
        }

        private void reportsBtn_Click(object sender, RoutedEventArgs e)
        {
            Reports reports = new Reports(log);
            reports.Show();
            this.Close();
        }
    }
}
