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
using System.IO;

namespace SoftPaws
{
    /// <summary>
    /// Логика взаимодействия для Questionnaire.xaml
    /// </summary>
    public partial class Questionnaire : Window
    {
        public string log;
        public int idForAtribOfCard, idForAtribOfGuardisn;

        List<CardItems> cardItems;
        List<GuardianItems> guardianItems;

        CardItems cardSelect;
        GuardianItems guardianSelect;

        public Questionnaire(string log)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            this.log = log;

            LoaadQueLV();
            LoadDataCb();
            LoaadGuardianLV();
        }

        public void LoaadQueLV()
        {
            sql.OpenConnection();

            List<QuestionnaireItems> results1 = new List<QuestionnaireItems>();
            SqlCommand command1 = new SqlCommand("SELECT а.Код_анкеты, CONCAT(а.Код_карточки, ' - ', ж.Кличка) AS Карточка, CONCAT(о.Фамилия, ' ', о.Имя, ' ', о.Отчество) AS ФИО, о.Адрес_проживания, о.Телефон, а.Дата_выписки " +
                "FROM Анкета_на_усыновление а " +
                "INNER JOIN Потенциальный_опекун о ON а.Код_опекуна = о.Код_опекуна " +
                "INNER JOIN Карточка_животного к ON а.Код_карточки = к.Код_карточки " +
                "LEFT JOIN Животное ж ON к.Код_животного = ж.Код_животного ", sql.str);
            SqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                QuestionnaireItems queItems = new QuestionnaireItems
                {
                    NumQue = reader1["Код_анкеты"].ToString(),
                    CardQue = reader1["Карточка"].ToString(),
                    GuardianQue = reader1["ФИО"].ToString(),
                    AdresQue = reader1["Адрес_проживания"].ToString(),
                    PhoneQue = reader1["Телефон"].ToString(),
                    DateDischQue = reader1["Дата_выписки"].ToString()
                };

                // Добавляем элемент в список
                results1.Add(queItems);
            }
            reader1.Close();
            command1.Dispose();
            // Устанавливаем источник данных для ListView
            QuestionnariLV.ItemsSource = results1;

            sql.CloseConnection();
        }

        public void LoadDataCb()
        {
            sql.OpenConnection();

            //карточка
            cardItems = LoadFromDB.LoadMultAttributes<CardItems>(
               "SELECT к.Код_карточки, CONCAT(к.Код_карточки, ' - ', ж.Кличка) AS Карточка " +
               "FROM [Карточка_животного] к " +
               "INNER JOIN Животное ж ON к.Код_животного = ж.Код_животного " +
               "WHERE ж.Код_статус = 1 AND ж.Дегельминтизация = 1 AND ж.Обработка_от_блох = 1 AND ж.Прививки = 1 AND ж.Стерилизация = 1 ",
               (id, name) => new CardItems(id, name));
            CardQueTB.ItemsSource = cardItems;

            // опекун
            guardianItems = LoadFromDB.LoadMultAttributes<GuardianItems>(
                "SELECT Код_опекуна, CONCAT(Фамилия, ' ', Имя, ' ',  Отчество) AS ФИО " +
                "FROM [Потенциальный_опекун]",
                (id, name) => new GuardianItems(id, name));
            GuardianQueTB.ItemsSource = guardianItems;

            sql.CloseConnection();
        }

        public void LoaadGuardianLV()
        {
            sql.OpenConnection();

            List<GuardianItemsLV> results1 = new List<GuardianItemsLV>();
            SqlCommand command1 = new SqlCommand("SELECT TOP (1000) Код_опекуна, CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО, Адрес_проживания, Телефон, Паспортные_данные FROM [Потенциальный_опекун] ", sql.str);

            SqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                GuardianItemsLV guaItemsLV = new GuardianItemsLV
                {
                    NumGuardian = reader1["Код_опекуна"].ToString(),
                    fioGuardian = reader1["ФИО"].ToString(),
                    AdresGuardian = reader1["Адрес_проживания"].ToString(),
                    PhoneGuardian = reader1["Телефон"].ToString(),
                    PassporGuardian = reader1["Паспортные_данные"].ToString()
                };
                results1.Add(guaItemsLV);
            }
            reader1.Close();
            command1.Dispose();
            GuardianLV.ItemsSource = results1;
            sql.CloseConnection();
        }

        private void addQueBtn_Click(object sender, RoutedEventArgs e)
        {

            string dateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            // Получаем текущий каталог приложения
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Формируем путь к файлу шаблона договора
            string templatePath = System.IO.Path.Combine(currentDirectory, "Contracts", "Шаблон договора.docx");
            string outputPath = System.IO.Path.Combine(currentDirectory, "Contracts", $"Договор № {dateNow}.docx");

            string nameAnimal, yearOldAnimal, catAnimal, genderAnimal, vidAnimal, healtAnimal;
            string fioWorker;
            string fioGuardian, adressGuardian, passportGuardian, phoneGuardian;

            var currentDateQue = DateDischQueTB.Text;

            if (CardQueTB.SelectedItem == null || GuardianQueTB.SelectedItem == null || string.IsNullOrEmpty(currentDateQue))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idCard = cardSelect.Id;
            int idGuardiant = guardianSelect.Id;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выполнить это действие?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            { 
                try
                {
                    sql.OpenConnection();

                    SqlCommand command = new SqlCommand($"UPDATE Животное SET " +
                        $"Код_статус = {2} " +
                        $"WHERE Код_животного = {idCard}", sql.str);
                    command.ExecuteNonQuery();

                    string insertIntoCardQuery = $"INSERT INTO Анкета_на_усыновление (Код_карточки, Код_опекуна, Дата_выписки) " +
                             $"VALUES ('{idCard}', " +
                             $"'{idGuardiant}', " +
                             $"'{currentDateQue:yyyy-MM-dd}')";

                    SqlCommand insertIntoCommand = new SqlCommand(insertIntoCardQuery, sql.str);
                    insertIntoCommand.ExecuteNonQuery();

                    MessageBox.Show("Успех совершенной операции.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // для сотрудника
                    SqlCommand commandWorker = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО " +
                       $"FROM Сотрудники " +
                       $"WHERE Логин = '{log}' ", sql.str);
                    SqlDataReader readerWorker = commandWorker.ExecuteReader();
                    readerWorker.Read();
                    fioWorker = readerWorker["ФИО"].ToString();
                    readerWorker.Close();

                    // для животного
                    SqlCommand commandAnimal = new SqlCommand($"SELECT ж.Кличка, ж.Возраст, к.Наименование_кат, п.Наименование_пол, пр.Наименование_порода, " +
                        $"CASE WHEN ж.[Обработка_от_блох] = 1 " +
                        $"AND ж.[Дегельминтизация] = 1 " +
                        $"AND ж.[Прививки] = 1 " +
                        $"AND ж.[Стерилизация] = 1 " +
                        $"THEN '+' ELSE '-' END AS Здоровье " +
                        $"FROM Животное ж " +
                        $"INNER JOIN Категория_животного к ON ж.Код_категории_животного = к.Код_категории_животного " +
                        $"INNER JOIN Пол п ON ж.Код_пола = п.Код_пола " +
                        $"INNER JOIN Порода пр ON ж.Код_порода = пр.Код_порода " +
                        $"INNER JOIN Статус с ON ж.Код_статус = с.Код_статуса " +
                        $"WHERE Код_животного = {idCard} ", sql.str);
                    SqlDataReader readerAnimal = commandAnimal.ExecuteReader();
                    readerAnimal.Read();
                    nameAnimal = readerAnimal["Кличка"].ToString();
                    yearOldAnimal = readerAnimal["Возраст"].ToString();
                    catAnimal = readerAnimal["Наименование_кат"].ToString();
                    genderAnimal = readerAnimal["Наименование_пол"].ToString();
                    vidAnimal = readerAnimal["Наименование_порода"].ToString();
                    healtAnimal = readerAnimal["Здоровье"].ToString();
                    readerAnimal.Close();

                    // для опекуна
                    SqlCommand commandGuardian = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО, Адрес_проживания, Телефон, Паспортные_данные " +
                       $"FROM Потенциальный_опекун " +
                       $"WHERE Код_опекуна = {idGuardiant} ", sql.str);
                    SqlDataReader readerGuardian = commandGuardian.ExecuteReader();
                    readerGuardian.Read();
                    fioGuardian = readerGuardian["ФИО"].ToString();
                    adressGuardian = readerGuardian["Адрес_проживания"].ToString();
                    passportGuardian = readerGuardian["Телефон"].ToString();
                    phoneGuardian = readerGuardian["Паспортные_данные"].ToString();
                    readerGuardian.Close();

                    Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                    Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(templatePath);

                    doc.Content.Find.Execute(FindText: "{dateNow}", ReplaceWith: dateNow);
                    doc.Content.Find.Execute(FindText: "{currentDateQue}", ReplaceWith: currentDateQue);
                    doc.Content.Find.Execute(FindText: "{fioWorker}", ReplaceWith: fioWorker);
                    doc.Content.Find.Execute(FindText: "{fioGuardian}", ReplaceWith: fioGuardian);

                    doc.Content.Find.Execute(FindText: "{catAnimal}", ReplaceWith: catAnimal);
                    doc.Content.Find.Execute(FindText: "{vidAnimal}", ReplaceWith: vidAnimal);
                    doc.Content.Find.Execute(FindText: "{genderAnimal}", ReplaceWith: genderAnimal);
                    doc.Content.Find.Execute(FindText: "{yearOldAnimal}", ReplaceWith: yearOldAnimal);
                    doc.Content.Find.Execute(FindText: "{nameAnimal}", ReplaceWith: nameAnimal);
                    doc.Content.Find.Execute(FindText: "{healtAnimal}", ReplaceWith: healtAnimal);

                    doc.Content.Find.Execute(FindText: "{catAnimal}", ReplaceWith: catAnimal);
                    doc.Content.Find.Execute(FindText: "{nameAnimal}", ReplaceWith: nameAnimal);
                    doc.Content.Find.Execute(FindText: "{catAnimal}", ReplaceWith: catAnimal);
                    doc.Content.Find.Execute(FindText: "{catAnimal}", ReplaceWith: catAnimal);

                    doc.Content.Find.Execute(FindText: "{fioGuardian}", ReplaceWith: fioGuardian);
                    doc.Content.Find.Execute(FindText: "{adressGuardian}", ReplaceWith: adressGuardian);
                    doc.Content.Find.Execute(FindText: "{passportGuardian}", ReplaceWith: passportGuardian);
                    doc.Content.Find.Execute(FindText: "{phoneGuardian}", ReplaceWith: phoneGuardian);
                    doc.Content.Find.Execute(FindText: "{fioGuardian}", ReplaceWith: fioGuardian);

                    doc.SaveAs2(outputPath);
                    doc.Close();
                    wordApp.Quit();
                    System.Diagnostics.Process.Start(outputPath);

                    sql.CloseConnection();
                    LoaadQueLV();
                    LoadDataCb();
                    ImgAnimals.Source = null;
                    DateDischQueTB.Text = null;
                } 
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void addGuardianBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = FamilTB.Text;
            string surname = NameTB.Text;
            string patr = OtchTB.Text;
            string adress = AdressTB.Text;
            string phone = PhoneTB.Text;
            string passport = PasspotrTB.Text;

            // на заполненность полей
            if (name == "" || surname == "" || patr == "" || adress == "" || phone == "" || passport == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выполнить это действие?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    sql.OpenConnection();

                    string insertIntoCardQuery = $"INSERT INTO Потенциальный_опекун (Фамилия, Имя, Отчество, Адрес_проживания, Телефон, Паспортные_данные) " +
                             $"VALUES ('{name}', " +
                             $"'{surname}', " +
                             $"'{patr}', " +
                             $"'{adress}', " +
                             $"'+7-{phone}', " +
                             $"'{passport}')";

                    SqlCommand insertIntoCommand = new SqlCommand(insertIntoCardQuery, sql.str);
                    insertIntoCommand.ExecuteNonQuery();

                    MessageBox.Show("Успех совершенной операции.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    FamilTB.Text = null;
                    NameTB.Text = null;
                    OtchTB.Text = null;
                    AdressTB.Text = null;
                    PhoneTB.Text = null;
                    PasspotrTB.Text = null;

                    sql.CloseConnection();
                    LoaadGuardianLV();
                    LoadDataCb();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DocumntBtn_Click(object sender, RoutedEventArgs e)
        {
            string dateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            string templatePath = @"Contracts/Шаблон договора.docx";
            string outputPath = $@"Contracts/Договор № {dateNow}.docx";

            int idCard = cardSelect.Id;
            int idGuardiant = guardianSelect.Id;
            var currentDateQue = DateDischQueTB.Text;

            string nameAnimal, yearOldAnimal, catAnimal, genderAnimal, vidAnimal, healtAnimal;
            string fioWorker;
            string fioGuardian, adressGuardian, passportGuardian, phoneGuardian;

            // на заполненность полей
            if (CardQueTB.SelectedItem == null || GuardianQueTB.SelectedItem == null || DateDischQueTB == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                sql.OpenConnection();

                // для сотрудника
                SqlCommand commandWorker = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО " +
                   $"FROM Сотрудники " +
                   $"WHERE Логин = '{log}' ", sql.str);
                SqlDataReader readerWorker = commandWorker.ExecuteReader();
                readerWorker.Read();
                fioWorker = readerWorker["ФИО"].ToString();
                readerWorker.Close();

                // для животного
                SqlCommand command = new SqlCommand($"SELECT ж.Кличка, ж.Возраст, к.Наименование_кат, п.Наименование_пол, пр.Наименование_порода, " +
                    $"CASE WHEN ж.[Обработка_от_блох] = 1 " +
                    $"AND ж.[Дегельминтизация] = 1 " +
                    $"AND ж.[Прививки] = 1 " +
                    $"AND ж.[Стерилизация] = 1 " +
                    $"THEN '+' ELSE '-' END AS Здоровье " +
                    $"FROM Животное ж " +
                    $"INNER JOIN Категория_животного к ON ж.Код_категории_животного = к.Код_категории_животного " +
                    $"INNER JOIN Пол п ON ж.Код_пола = п.Код_пола " +
                    $"INNER JOIN Порода пр ON ж.Код_порода = пр.Код_порода " +
                    $"INNER JOIN Статус с ON ж.Код_статус = с.Код_статуса " +
                    $"WHERE Код_животного = '{idCard}' ", sql.str);
                SqlDataReader readerAnimal = command.ExecuteReader();
                readerAnimal.Read();
                nameAnimal = readerAnimal["Кличка"].ToString();
                yearOldAnimal = readerAnimal["Возраст"].ToString();
                catAnimal = readerAnimal["Наименование_кат"].ToString();
                genderAnimal = readerAnimal["Наименование_пол"].ToString();
                vidAnimal = readerAnimal["Наименование_порода"].ToString();
                healtAnimal = readerAnimal["Здоровье"].ToString();
                readerAnimal.Close();

                // для опекуна
                SqlCommand commandGuardian = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО, Адрес_проживания, Телефон, Паспортные_данные " +
                   $"FROM Потенциальный_опекун " +
                   $"WHERE Код_опекуна = '{idGuardiant}' ", sql.str);
                SqlDataReader readerGuardian = commandGuardian.ExecuteReader();
                readerGuardian.Read();
                fioGuardian = readerGuardian["ФИО"].ToString();
                adressGuardian = readerGuardian["Адрес_проживания"].ToString();
                passportGuardian = readerGuardian["Телефон"].ToString();
                phoneGuardian = readerGuardian["Паспортные_данные"].ToString();
                readerGuardian.Close();

                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(templatePath);

                doc.Content.Find.Execute(FindText: "{dateNow}", ReplaceWith: dateNow);
                doc.Content.Find.Execute(FindText: "{currentDateQue}", ReplaceWith: currentDateQue);
                doc.Content.Find.Execute(FindText: "{fioWorker}", ReplaceWith: fioWorker);

                doc.Content.Find.Execute(FindText: "{catAnimal}", ReplaceWith: catAnimal);
                doc.Content.Find.Execute(FindText: "{vidAnimal}", ReplaceWith: vidAnimal);
                doc.Content.Find.Execute(FindText: "{genderAnimal}", ReplaceWith: genderAnimal);
                doc.Content.Find.Execute(FindText: "{yearOldAnimal}", ReplaceWith: yearOldAnimal);
                doc.Content.Find.Execute(FindText: "{nameAnimal}", ReplaceWith: nameAnimal);
                doc.Content.Find.Execute(FindText: "{healtAnimal}", ReplaceWith: healtAnimal);

                doc.Content.Find.Execute(FindText: "{fioGuardian }", ReplaceWith: fioGuardian);
                doc.Content.Find.Execute(FindText: "{adressGuardian}", ReplaceWith: adressGuardian);
                doc.Content.Find.Execute(FindText: "{passportGuardian}", ReplaceWith: passportGuardian);
                doc.Content.Find.Execute(FindText: "{phoneGuardian}", ReplaceWith: phoneGuardian);

                doc.SaveAs2(outputPath);
                doc.Close();
                wordApp.Quit();
                System.Diagnostics.Process.Start(outputPath);

                sql.CloseConnection();
                LoaadQueLV();
                LoadDataCb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void CardQueTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string imagePath;
            cardSelect = CardQueTB.SelectedItem as CardItems;
            if (cardSelect != null)
            {
                idForAtribOfCard = cardSelect.Id;

                sql.OpenConnection();
                SqlCommand command = new SqlCommand($"SELECT Фотография FROM Животное WHERE Код_животного = '{idForAtribOfCard}'", sql.str);
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();
                imagePath = reader["Фотография"].ToString();
                reader.Close();
                // выгрузка изображения на форму
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageDestinationPath = System.IO.Path.Combine(currentDirectory, "imgAnimals", imagePath);

                imageSource.UriSource = new Uri(imageDestinationPath); // Проверьте путь к изображению
                imageSource.EndInit();
                ImgAnimals.Source = imageSource;

                sql.CloseConnection();
            }
        }

        private void GuardianQueTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            guardianSelect = GuardianQueTB.SelectedItem as GuardianItems;
            if (guardianSelect != null)
            {
                idForAtribOfGuardisn = guardianSelect.Id;
            }
        }

        private void PhoneTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли введенный символ цифрой
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true; // Если не является, отменяем ввод
            }
        }

        private void PhoneTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Форматируем номер телефона формат XXX-XXX-XXXX)
            if (PhoneTB.Text.Length == 3 || PhoneTB.Text.Length == 7)
            {
                PhoneTB.Text += "-";
                PhoneTB.CaretIndex = PhoneTB.Text.Length;
            }
        }
    }
}
