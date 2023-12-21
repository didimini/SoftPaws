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
using SoftPaws.Classes;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace SoftPaws
{
    /// <summary>
    /// Логика взаимодействия для FiltrAnimals.xaml
    /// </summary>
    public partial class FiltrAnimals : Window
    {
        public event EventHandler<List<AnimalsItems>> AnimalsFiltered;


        public string log;
        public List<AnimalsItems> animalsItemsFilt;

        List<CategoryItems> categoryItems;
        List<VidItems> vidItems;
        List<StatusItems> statusItems;

        CategoryItems categorySelect;
        VidItems vidSelect;
        StatusItems statusSelect;

        public int idForAtribOfAnimalCat, idForAtribOfAnimalVid, idForAtribOfAnimalStatus;


        public FiltrAnimals(string log)
        {
            InitializeComponent();
            this.log = log;
            LoadDataCb();
        }

        // звгрузки в CB данных
        public void LoadDataCb()
        {
            sql.OpenConnection();

            // категория
            categoryItems = LoadFromDB.LoadMultAttributes<CategoryItems>(
                "SELECT Код_категории_животного, Наименование_кат FROM [Категория_животного]",
                (id, name) => new CategoryItems(id, name));
            CategInSheaterTB.ItemsSource = categoryItems;

            // порода
            vidItems = LoadFromDB.LoadMultAttributes<VidItems>(
                "SELECT Код_порода, Наименование_порода FROM [Порода]",
                (id, name) => new VidItems(id, name));
            VidInSheaterTB.ItemsSource = vidItems;

            // статус
            statusItems = LoadFromDB.LoadMultAttributes<StatusItems>(
                "SELECT Код_статуса, Наименование_статус FROM [Статус]",
                (id, name) => new StatusItems(id, name));
            WeaponPers.ItemsSource = statusItems;

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

        private void FiltInput_Click(object sender, RoutedEventArgs e)
        {
            sql.OpenConnection();

            animalsItemsFilt = new List<AnimalsItems>();
            StringBuilder queryBuilder = new StringBuilder("SELECT ж.Код_животного, ж.Кличка, ж.Возраст, к.Наименование_кат, п.Наименование_пол,            пр.Наименование_порода, с.Наименование_статус, ж.Фотография, " +
                "CASE WHEN ж.[Обработка_от_блох] = 1 " +
            "AND ж.[Дегельминтизация] = 1 " +
            "AND ж.[Прививки] = 1 " +
            "AND ж.[Стерилизация] = 1 " +
            "THEN '+' ELSE '-' END AS Здоровье " +
            "FROM Животное ж " +
            "INNER JOIN Категория_животного к ON ж.Код_категории_животного = к.Код_категории_животного " +
            "INNER JOIN Пол п ON ж.Код_пола = п.Код_пола " +
            "INNER JOIN Порода пр ON ж.Код_порода = пр.Код_порода " +
            "INNER JOIN Статус с ON ж.Код_статус = с.Код_статуса ");


            if (idForAtribOfAnimalCat != 0)
                queryBuilder.Append($"WHERE ж.Код_категории_животного = {idForAtribOfAnimalCat} ");

            if (idForAtribOfAnimalVid != 0)
                queryBuilder.Append($"AND ж.Код_порода = {idForAtribOfAnimalVid} ");

            if (idForAtribOfAnimalStatus != 0)
                queryBuilder.Append($"AND ж.Код_статус = {idForAtribOfAnimalStatus} ");

            if (GanderMInSheaterTB.IsChecked == true)
                queryBuilder.Append("AND п.Наименование_пол = 'Женский' ");
            else if (GanderWInSheaterTB.IsChecked == true)
                queryBuilder.Append("AND п.Наименование_пол = 'Мужской' ");

            if (BlohChB.IsChecked == true)
                queryBuilder.Append("AND ж.[Обработка_от_блох] = 1 ");

            if (DegelchB.IsChecked == true)
                queryBuilder.Append("AND ж.[Дегельминтизация] = 1 ");

            if (PrivivChb.IsChecked == true)
                queryBuilder.Append("AND ж.[Прививки] = 1 ");

            if (SterizChB.IsChecked == true)
                queryBuilder.Append("AND ж.[Стерилизация] = 1 ");

            SqlCommand commandFilt = new SqlCommand(queryBuilder.ToString(), sql.str);
            SqlDataReader readerFilt = commandFilt.ExecuteReader();
            while (readerFilt.Read())
            {
                AnimalsItems animalsItemsFilt1 = new AnimalsItems
                {
                    NumInSheater = readerFilt["Код_животного"].ToString(),
                    NameInSheater = readerFilt["Кличка"].ToString(),
                    YearoldInSheater = readerFilt["Возраст"].ToString(),
                    CategInSheater = readerFilt["Наименование_кат"].ToString(),
                    GenderInSheater = readerFilt["Наименование_пол"].ToString(),
                    VidInSheater = readerFilt["Наименование_порода"].ToString(),
                    StatusSheater = readerFilt["Наименование_статус"].ToString(),
                    HealtInSheater = readerFilt["Здоровье"].ToString(),
                    ImgInSheater = readerFilt["Фотография"].ToString(),
                    IsMale = Convert.ToString(readerFilt["Наименование_пол"].ToString() == "Мужской")
                };

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                animalsItemsFilt1.FullImagePath = System.IO.Path.Combine(currentDirectory, "imgAnimals", animalsItemsFilt1.ImgInSheater);
                animalsItemsFilt.Add(animalsItemsFilt1);
            }
            readerFilt.Close();
            commandFilt.Dispose();

            AnimalsFiltered?.Invoke(this, animalsItemsFilt);

            sql.CloseConnection();

            this.Close();
        }

        private void CategInSheaterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categorySelect = CategInSheaterTB.SelectedItem as CategoryItems;
            if (categorySelect != null)
            {
                idForAtribOfAnimalCat = categorySelect.Id;
            }
        }

        // при выборе из comboBox записывается id - для редактирования 
        private void VidInSheaterTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vidSelect = VidInSheaterTB.SelectedItem as VidItems;
            if (vidSelect != null)
            {
                idForAtribOfAnimalVid = vidSelect.Id;
            }
        }

        // при выборе из comboBox записывается id - для редактирования 
        private void WeaponPers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statusSelect = WeaponPers.SelectedItem as StatusItems;
            if (statusSelect != null)
            {
                idForAtribOfAnimalStatus = statusSelect.Id;
            }
        }
    }
}
