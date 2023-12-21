using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using SoftPaws.Classes;
using System.IO;
using System.Drawing;
using Xceed.Words.NET;
using Xceed.Document.NET;
using Microsoft.Office.Interop.Word;



namespace SoftPaws
{
    /// <summary>
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : System.Windows.Window
    {
        public string log;
        public DateTime currentDate = DateTime.Now;
        List<CardsItemsLV> results = new List<CardsItemsLV>();

        public Reports(string log)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            this.log = log;
            LoadCardsLV();
        }
        public void LoadCardsLV()
        {
            sql.OpenConnection();

            try
            {
                SqlCommand command = new SqlCommand("SELECT к.Код_карточки, ж.Кличка, CONCAT(с.Фамилия, ' ', с.Имя, ' ', с.Отчество) AS ФИО, к.Дата_поступления " +
                    "FROM Карточка_животного к " +
                    "INNER JOIN Животное ж ON к.Код_животного = ж.Код_животного " +
                    "INNER JOIN Сотрудники с ON к.Логин_сотрудника = с.Логин ", sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CardsItemsLV cardsItemsLV = new CardsItemsLV
                    {
                        Num = reader["Код_карточки"].ToString(),
                        Animal = reader["Кличка"].ToString(),
                        Worker = reader["ФИО"].ToString(),
                        Date = reader["Дата_поступления"].ToString()
                    };

                    results.Add(cardsItemsLV);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sql.CloseConnection();
            WorkersLV2.ItemsSource = results;
        }

        // нажатие на кнопку применить, чтобы посмотреть с какого по какой период были усыновлены животные
        private void FiltInput_Click(object sender, RoutedEventArgs e)
        {
            sql.OpenConnection();

            List<CardsItemsLV> results = new List<CardsItemsLV>();
            if (dateStartDP.SelectedDate == null && dateEndDP.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите начальную и конечную дату.", "Предупреждение");
                return; // Если даты не выбраны, завершаем метод, чтобы избежать выполнения дальнейших операций
            }

            try
            {
                string dateStart = dateStartDP.SelectedDate.Value.ToString("yyyy-MM-dd");
                string dateEnd = dateEndDP.SelectedDate.Value.ToString("yyyy-MM-dd");

                // запрос на животных в приюте в определенный период
                SqlCommand command = new SqlCommand("SELECT к.Код_карточки, ж.Кличка, CONCAT(с.Фамилия, ' ', с.Имя, ' ', с.Отчество) AS ФИО, к.Дата_поступления " +
                    "FROM Карточка_животного к " +
                    "INNER JOIN Животное ж ON к.Код_животного = ж.Код_животного " +
                    "INNER JOIN Сотрудники с ON к.Логин_сотрудника = с.Логин " +
                    "WHERE к.Дата_поступления BETWEEN @StartDate AND @EndDate", sql.str);

                command.Parameters.AddWithValue("@StartDate", dateStart);
                command.Parameters.AddWithValue("@EndDate", dateEnd);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CardsItemsLV cardsItemsLV = new CardsItemsLV
                    {
                        Num = reader["Код_карточки"].ToString(),
                        Animal = reader["Кличка"].ToString(),
                        Worker = reader["ФИО"].ToString(),
                        Date = reader["Дата_поступления"].ToString()
                    };
                    results.Add(cardsItemsLV);
                }
                WorkersLV2.ItemsSource = results;
                ReportAdopted.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            sql.CloseConnection();

            
        }

        // нажатие на кнопку формирования отчета
        private void ReportAdopted_Click(object sender, RoutedEventArgs e)
        {
            string dateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fio;
            string totalAnimals;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;            
            // путь к сохранению документа
            string outputPath = System.IO.Path.Combine(currentDirectory, "DocumetnsReportsInShelter", $"Отчет животных в приюте № {dateNow}.docx");

            string dateStart = dateStartDP.SelectedDate?.ToString("dd-MM-yyyy") ?? "N/A";
            string dateEnd = dateEndDP.SelectedDate?.ToString("dd-MM-yyyy") ?? "N/A";

            try
            {
                sql.OpenConnection();
                //фио сотрудника
                SqlCommand commandWorker = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество) AS ФИО " +
                    $"FROM Сотрудники " +
                    $"WHERE Логин = '{log}'", sql.str);
                SqlDataReader readerWorker = commandWorker.ExecuteReader();
                readerWorker.Read();
                fio = readerWorker["ФИО"].ToString();
                readerWorker.Close();

                SqlCommand commandTotalAnimals = new SqlCommand($"SELECT (COUNT(*) ) AS Количество_животных FROM  Карточка_животного " +
                    $"WHERE Дата_поступления BETWEEN '{dateStart}' AND '{dateEnd}' ", sql.str);
                SqlDataReader readerTotalAnimals = commandTotalAnimals.ExecuteReader();
                readerTotalAnimals.Read();
                totalAnimals = readerTotalAnimals["Количество_животных"].ToString();
                readerTotalAnimals.Close();

                // выгрузка данных из ListView
                List<CardsItemsLV> listViewData = GetListViewData();
                // создание документа с поомщью библиотеки DocX
                DocX docX = DocX.Create(outputPath);

                Xceed.Document.NET.Paragraph title = docX.InsertParagraph();
                title.Append("Документ по отчетности количества принятых животных в приюта SoftPaws").Bold().FontSize(14).Alignment = Alignment.center;

                Xceed.Document.NET.Paragraph period = docX.InsertParagraph();
                period.AppendLine().Append($"Период с {dateStart} по {dateEnd}").FontSize(14).Alignment = Alignment.left;

                // Добавление таблицы
                var wordTable = docX.AddTable(listViewData.Count + 1, 4);
                wordTable.Design = TableDesign.TableGrid;

                // Установка ширины столбцов
                float[] columnWidths = { 150f, 100f, 2000f, 100f }; // Замените значениями по умолчанию
                wordTable.SetWidths(columnWidths);

                for (int i = 0; i < 4; i++)
                {
                    wordTable.Rows[0].Cells[i].Paragraphs[0].Append(GetHeader(i)).Bold();
                }

                // Заполнение таблицы данными
                for (int i = 0; i < listViewData.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        wordTable.Rows[i + 1].Cells[j].Paragraphs[0].Append(GetCellContent(listViewData[i], j));
                    }
                }

                // Добавление таблицы в документ
                docX.InsertTable(wordTable);

                Xceed.Document.NET.Paragraph totalAnimalsWord = docX.InsertParagraph();
                totalAnimalsWord.AppendLine().Append($"Итого животных: {totalAnimals}").FontSize(14).Alignment = Alignment.left;

                // Добавление даты проверки
                Xceed.Document.NET.Paragraph checkDate = docX.InsertParagraph();
                checkDate.AppendLine().Append($"Дата проверки: {currentDate:yyyy-MM-dd}").FontSize(14).Alignment = Alignment.left;

                // Добавление сотрудника
                Xceed.Document.NET.Paragraph employee = docX.InsertParagraph();
                employee.AppendLine().Append($"Сотрудник: {fio}").FontSize(14).Alignment = Alignment.left;

                docX.Save();

                // Открытие сохраненного документа
                System.Diagnostics.Process.Start(outputPath);

                sql.CloseConnection();

                Reports reports = new Reports(log);
                reports.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // вывод данных из listView
        private List<CardsItemsLV> GetListViewData()
        {
            List<CardsItemsLV> data = new List<CardsItemsLV>();
            foreach (var item in WorkersLV2.Items)
            {
                CardsItemsLV listItem = item as CardsItemsLV;
                if (listItem != null)
                {
                    data.Add(listItem);
                }
            }
            return data;
        }

        // создание заголовков в таблице
        private string GetHeader(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "ЧИП животного";
                case 1:
                    return "Животное";
                case 2:
                    return "Сотрудник";
                case 3:
                    return "Дата поступления";
                default:
                    return string.Empty;
            }
        }

        // создание строк таблицы
        private string GetCellContent(CardsItemsLV item, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return item.Num;
                case 1:
                    return item.Animal;
                case 2:
                    return item.Worker;
                case 3:
                    return item.Date;
                default:
                    return string.Empty;
            }
        }


        // Обработчик события для изменения дат в DatePicker
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка, есть ли даты в обоих DatePicker
            bool hasStartDate = dateStartDP.SelectedDate != null;
            bool hasEndDate = dateEndDP.SelectedDate != null;

            // Если обе даты есть, то IsEnabled = true, иначе IsEnabled = false
            FiltInput.IsEnabled = hasStartDate && hasEndDate;
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
