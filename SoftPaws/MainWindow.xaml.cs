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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftPaws
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string log;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            log = Login.Text;
            string passw = PasswordOne.Password;

            sql.OpenConnection();

            SqlCommand command = new SqlCommand($"SELECT [Код_должности] " +
                $"FROM [dbo].[Сотрудники] " +
                $"WHERE Логин = '{log}' AND Пароль = '{passw}'", sql.str);
            object result = command.ExecuteScalar();

            sql.CloseConnection();

            // проврка и авторизация пользователей
            try
            {
                if (Login.Text == "" && PasswordOne.Password == "")
                {
                    MessageBox.Show("Введине логин и пароль");
                }
                else if (result != null)
                {
                    int postId = Convert.ToInt32(result);

                    if (postId == 1)
                    {
                        if (Login.Text == log && PasswordOne.Password == passw)
                        {
                            Menu menu = new Menu(log);
                            menu.Show();
                            this.Close();
                        }
                    }
                    else if (postId == 2)
                    {
                        if (Login.Text == log && PasswordOne.Password == passw)
                        {
                            Menu menu = new Menu(log);
                            menu.Show();
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                    Login.Text = "";
                    PasswordOne.Password = "";
                    PasswordTwo.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void showPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordOne.Visibility = Visibility.Collapsed;
            PasswordTwo.Visibility = Visibility.Visible;
            PasswordTwo.Text = PasswordOne.Password;
        }

        private void showPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordTwo.Visibility = Visibility.Collapsed;
            PasswordOne.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
