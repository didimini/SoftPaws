using Microsoft.Win32;
using SoftPaws.Classes;
using SoftPaws.Methods;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    /// Логика взаимодействия для Animals.xaml
    /// </summary>
    public partial class Animals : Window
    {
        public string log;
        public int postId, id;

        public int animalId;
        public int idForAtribOfAnimalCat, idForAtribOfAnimalVid, idForAtribOfAnimalStatus;
        public string requestCat, requestVid, requestStatus;

        public string imageFileName = "";

        List<AnimalsItems> animalsItem;
        List<CategoryItems> categoryItems;
        List<VidItems> vidItems;
        List<StatusItems> statusItems;

        AnimalsItems animalsSelect;
        CategoryItems categorySelect;
        VidItems vidSelect;
        StatusItems statusSelect;

        public DateTime currentDate = DateTime.Now;

        public Animals(string log)
        {
            InitializeComponent();
            this.log = log;
            WindowState = WindowState.Maximized;

            // проверка какой пользовател зашел в систему
            sql.OpenConnection();
            SqlCommand command = new SqlCommand($"SELECT [Код_должности] " +
                $"FROM [dbo].[Сотрудники] " +
                $"WHERE Логин = '{log}'", sql.str);
            object result = command.ExecuteScalar();
            sql.CloseConnection();
            postId = Convert.ToInt32(result);
            if (postId == 1)
            {
                addAnimal.Visibility = Visibility.Collapsed;
                editAnimalBtn.Visibility = Visibility.Collapsed;
                editImgBtn.Visibility = Visibility.Collapsed;
                BanOnEditing();
            }

            LoadDataCb();
            LoadAnimalsLV();
        }

        // блокировка элементов
        public void BanOnEditing()
        {
            NameInSheaterTB.IsEnabled = false;
            YearOldInSheaterTB.IsEnabled = false;
            CategInSheaterCB.IsEnabled = false;
            GanderMInSheaterTB.IsEnabled = false;
            GanderWInSheaterTB.IsEnabled = false;
            VidInSheaterTB.IsEnabled = false;
            WeaponPers.IsEnabled = false;
            BlohChB.IsEnabled = false;
            DegelchB.IsEnabled = false;
            PrivivChb.IsEnabled = false;
            SterizChB.IsEnabled = false;
        }
        // разблокировка элементов
        public void NotBanOnEditing()
        {
            NameInSheaterTB.IsEnabled = true;
            YearOldInSheaterTB.IsEnabled = true;
            CategInSheaterCB.IsEnabled = true;
            GanderMInSheaterTB.IsEnabled = true;
            GanderWInSheaterTB.IsEnabled = true;
            VidInSheaterTB.IsEnabled = true;
            WeaponPers.IsEnabled = true;
            BlohChB.IsEnabled = true;
            DegelchB.IsEnabled = true;
            PrivivChb.IsEnabled = true;
            SterizChB.IsEnabled = true;
        }

        // звгрузки в ComboBox данных
        public void LoadDataCb()
        {
            sql.OpenConnection();

            // категория
            categoryItems = LoadFromDB.LoadMultAttributes<CategoryItems>(
                "SELECT Код_категории_животного, Наименование_кат FROM [Категория_животного]",
                (id, name) => new CategoryItems(id, name));
            CategInSheaterCB.ItemsSource = categoryItems;
            AddCfaegInSheaterCB.ItemsSource = categoryItems;

            // порода
            vidItems = LoadFromDB.LoadMultAttributes<VidItems>(
                "SELECT Код_порода, Наименование_порода FROM [Порода]",
                (id, name) => new VidItems(id, name));
            VidInSheaterTB.ItemsSource = vidItems;
            AddVidInSheaterTB.ItemsSource = vidItems;

            // статус
            statusItems = LoadFromDB.LoadMultAttributes<StatusItems>(
                "SELECT Код_статуса, Наименование_статус FROM [Статус]",
                (id, name) => new StatusItems(id, name));
            WeaponPers.ItemsSource = statusItems;

            sql.CloseConnection();
        }

        // загрузка данных их бд в ListView
        public void LoadAnimalsLV()
        {
            sql.OpenConnection();

            List<AnimalsItems> results1 = new List<AnimalsItems>();
            SqlCommand command1 = new SqlCommand("SELECT ж.Код_животного, ж.Кличка, ж.Возраст, к.Наименование_кат, п.Наименование_пол, пр.Наименование_порода, с.Наименование_статус, ж.Фотография, " +
                "CASE WHEN ж.[Обработка_от_блох] = 1 " +
                "AND ж.[Дегельминтизация] = 1 " +
                "AND ж.[Прививки] = 1 " +
                "AND ж.[Стерилизация] = 1 " +
                "THEN '+' ELSE '-' END AS Здоровье " +
                "FROM Животное ж " +
                "INNER JOIN Категория_животного к ON ж.Код_категории_животного = к.Код_категории_животного " +
                "INNER JOIN Пол п ON ж.Код_пола = п.Код_пола " +
                "INNER JOIN Порода пр ON ж.Код_порода = пр.Код_порода " +
                "INNER JOIN Статус с ON ж.Код_статус = с.Код_статуса ", sql.str);
            SqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                AnimalsItems animalsItems = new AnimalsItems
                {
                    NumInSheater = reader1["Код_животного"].ToString(),
                    NameInSheater = reader1["Кличка"].ToString(),
                    YearoldInSheater = reader1["Возраст"].ToString(),
                    CategInSheater = reader1["Наименование_кат"].ToString(),
                    GenderInSheater = reader1["Наименование_пол"].ToString(),
                    VidInSheater = reader1["Наименование_порода"].ToString(),
                    StatusSheater = reader1["Наименование_статус"].ToString(),
                    HealtInSheater = reader1["Здоровье"].ToString(),
                    ImgInSheater = reader1["Фотография"].ToString(),
                    IsMale = Convert.ToString(reader1["Наименование_пол"].ToString() == "Мужской")
                };
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                animalsItems.FullImagePath = System.IO.Path.Combine(currentDirectory, "imgAnimals", animalsItems.ImgInSheater);
                results1.Add(animalsItems);
            }
            reader1.Close();
            command1.Dispose();

            inSheaterAnimalsLV.ItemsSource = results1;

            sql.CloseConnection();
        }

        // нажатие на список, выбор животного и выгрузка его данных в элементы
        // подробный просмотр информации о животном
        private void inSheaterAnimalsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sql.OpenConnection();

            editAnimalBtn.Visibility = Visibility.Visible;
            editImgBtn.Visibility = Visibility.Visible;

            AnimalsItems selectedAnimal = (AnimalsItems)inSheaterAnimalsLV.SelectedItem;
            if (selectedAnimal != null)
            {
                animalId = Convert.ToInt32(selectedAnimal.NumInSheater);

                sql.OpenConnection();
                requestStatus = $"SELECT Код_статус FROM Животное WHERE Код_животного = '{animalId}'";

                SqlCommand editAnimals = new SqlCommand($"SELECT ж.Код_животного, ж.Кличка, ж.Возраст, к.Наименование_кат, п.Наименование_пол, пр.Наименование_порода, с.Наименование_статус, ж.Обработка_от_блох, ж.Дегельминтизация, ж.Прививки, ж.Стерилизация, ж.Фотография " +
                        $"FROM Животное ж " +
                        $"INNER JOIN Категория_животного к ON ж.Код_категории_животного = к.Код_категории_животного " +
                        $"INNER JOIN Пол п ON ж.Код_пола = п.Код_пола " +
                        $"INNER JOIN Порода пр ON ж.Код_порода = пр.Код_порода " +
                        $"INNER JOIN Статус с ON ж.Код_статус = с.Код_статуса " +
                        $"WHERE ж.Код_животного = {animalId}", sql.str);
                SqlDataReader editReader = editAnimals.ExecuteReader();

                if (editReader.Read())
                {
                    NameInSheaterTB.Text = editReader["Кличка"].ToString();
                    YearOldInSheaterTB.Text = editReader["Возраст"].ToString();
                    string imagePath = editReader["Фотография"].ToString();
                    bool isMale = editReader["Наименование_пол"].ToString() == "Мужской";
                    bool isBloh = Convert.ToBoolean(editReader["Обработка_от_блох"]);
                    bool isDegel = Convert.ToBoolean(editReader["Дегельминтизация"]);
                    bool isPriviv = Convert.ToBoolean(editReader["Прививки"]);
                    bool isSteril = Convert.ToBoolean(editReader["Стерилизация"]);
                    editReader.Close();

                    // выгрузка изображения на форму
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string imageDestinationPath = System.IO.Path.Combine(currentDirectory, "imgAnimals");
                    imageSource.UriSource = new Uri(imageDestinationPath + @"\" + System.IO.Path.GetFileName(imagePath));
                    imageSource.EndInit();
                    ImgAnimals.Source = imageSource;
                    imageFileName = imagePath;

                    requestCat = $"SELECT Код_категории_животного FROM Животное WHERE Код_животного = '{animalId}'";
                    requestVid = $"SELECT Код_порода FROM Животное WHERE Код_животного = '{animalId}'";
                    requestStatus = $"SELECT Код_статус FROM Животное WHERE Код_животного = '{animalId}'";

                    string animalStatError = selectedAnimal.StatusSheater;
                    if (animalStatError == "погибло" || animalStatError == "усыновлено")
                    {
                        editAnimalBtn.Visibility = Visibility.Collapsed;
                        editImgBtn.Visibility = Visibility.Collapsed;
                        idForAtribOfAnimalCat = LoadFromDB.LoadSinglAttribute(requestCat, id);
                        idForAtribOfAnimalVid = LoadFromDB.LoadSinglAttribute(requestVid, id);
                        idForAtribOfAnimalStatus = LoadFromDB.LoadSinglAttribute(requestStatus, id);
                        // находим объект CategoryItems по его Id и устанавливаем его как выбранный элемент в ComboBox
                        CategoryItems selectedCategory = categoryItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalCat);
                        VidItems selectedVid = vidItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalVid);
                        StatusItems selectedStatus = statusItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalStatus);

                        CategInSheaterCB.SelectedItem = selectedCategory;
                        VidInSheaterTB.SelectedItem = selectedVid;
                        WeaponPers.SelectedItem = selectedStatus;

                        // Определяем, какой RadioButton должен быть выбран в зависимости от полученного пола
                        GanderMInSheaterTB.IsChecked = !isMale;
                        GanderWInSheaterTB.IsChecked = isMale;

                        // Устанавливаем значения CheckBox
                        BlohChB.IsChecked = isBloh;
                        DegelchB.IsChecked = isDegel;
                        PrivivChb.IsChecked = isPriviv;
                        SterizChB.IsChecked = isSteril;
                        BanOnEditing();
                    }
                    if (postId == 1)
                    {
                        editAnimalBtn.Visibility = Visibility.Collapsed;
                        editImgBtn.Visibility = Visibility.Collapsed;

                        idForAtribOfAnimalCat = LoadFromDB.LoadSinglAttribute(requestCat, id);
                        idForAtribOfAnimalVid = LoadFromDB.LoadSinglAttribute(requestVid, id);
                        idForAtribOfAnimalStatus = LoadFromDB.LoadSinglAttribute(requestStatus, id);
                        // находим объект CategoryItems по его Id и устанавливаем его как выбранный элемент в ComboBox
                        CategoryItems selectedCategory = categoryItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalCat);
                        VidItems selectedVid = vidItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalVid);
                        StatusItems selectedStatus = statusItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalStatus);

                        CategInSheaterCB.SelectedItem = selectedCategory;
                        VidInSheaterTB.SelectedItem = selectedVid;
                        WeaponPers.SelectedItem = selectedStatus;
                        // Определяем, какой RadioButton должен быть выбран в зависимости от полученного пола
                        GanderMInSheaterTB.IsChecked = !isMale;
                        GanderWInSheaterTB.IsChecked = isMale;
                        // Устанавливаем значения CheckBox
                        BlohChB.IsChecked = isBloh;
                        DegelchB.IsChecked = isDegel;
                        PrivivChb.IsChecked = isPriviv;
                        SterizChB.IsChecked = isSteril;
                    }
                    else if (animalStatError == "в приюте")
                    {
                        idForAtribOfAnimalCat = LoadFromDB.LoadSinglAttribute(requestCat, id);
                        idForAtribOfAnimalVid = LoadFromDB.LoadSinglAttribute(requestVid, id);
                        idForAtribOfAnimalStatus = LoadFromDB.LoadSinglAttribute(requestStatus, id);
                        // находим объект CategoryItems по его Id и устанавливаем его как выбранный элемент в ComboBox
                        CategoryItems selectedCategory = categoryItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalCat);
                        VidItems selectedVid = vidItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalVid);
                        StatusItems selectedStatus = statusItems.FirstOrDefault(item => item.Id == idForAtribOfAnimalStatus);

                        CategInSheaterCB.SelectedItem = selectedCategory;
                        VidInSheaterTB.SelectedItem = selectedVid;
                        WeaponPers.SelectedItem = selectedStatus;
                        // Определяем, какой RadioButton должен быть выбран в зависимости от полученного пола
                        GanderMInSheaterTB.IsChecked = !isMale;
                        GanderWInSheaterTB.IsChecked = isMale;
                        // Устанавливаем значения CheckBox
                        BlohChB.IsChecked = isBloh;
                        DegelchB.IsChecked = isDegel;
                        PrivivChb.IsChecked = isPriviv;
                        SterizChB.IsChecked = isSteril;
                        NotBanOnEditing();
                    }
                }
                editReader.Close();
            }
            sql.CloseConnection();
        }

        // переменная для запоминания пути к изображению
        // нажатие на кномпку чтобы выбрать изображение
        public string imagePath2;
        private void editImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath2 = openFileDialog.FileName;

                // Проверьте, что выбранный файл является изображением
                if (ImageMethod.IsImageFile(imagePath2))
                {
                    imageFileName = imagePath2;
                    // Создайте объект BitmapImage и установите его в элемент Image
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(imagePath2);
                    imageSource.EndInit();
                    ImgAnimals.Source = imageSource;
                }
                else
                {
                    MessageBox.Show("Выбранный файл не является изображением.");
                }
            }
        }

        // нажатие на кнопку чтобы измениь данные о животном
        private void editAnimalBtn_Click(object sender, RoutedEventArgs e)
        {

            int idCat = categorySelect.Id;
            int idVid = vidSelect.Id;
            int idStat = statusSelect.Id;
            string img = "";
            int genderAnimal;
            if (GanderWInSheaterTB.IsChecked == true && GanderMInSheaterTB.IsChecked == false)
                genderAnimal = 2;
            else
                genderAnimal = 1;

            // Если был выбран новый файл изображения, обновите imageFileName
            if (!string.IsNullOrWhiteSpace(imagePath2))
            {
                img = System.IO.Path.GetFileName(imagePath2);
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageDestinationPath = System.IO.Path.Combine(currentDirectory, "imgAnimals", img);

                if (imageDestinationPath == imagePath2) // совпадает ли путь с исходным путем изображения
                {
                    img = System.IO.Path.GetFileName(imagePath2);
                }
                else if (img == System.IO.Path.GetFileName(imageDestinationPath)) // совпадает ли имя файла изображения с именем файла в пути
                {
                    img = imageFileName;
                }
                else
                {
                    img = System.IO.Path.GetFileName(imagePath2);
                    System.IO.File.Copy(imagePath2, imageDestinationPath, true);
                }
                imagePath2 = null;
            }
            else  // Если изображение не менялось, используйте старое имя файла
            {
                img = imageFileName;
            }

            // Получаем выбранное животное из ListView
            AnimalsItems selectedAnimal = (AnimalsItems)inSheaterAnimalsLV.SelectedItem;
            if (selectedAnimal != null)
            {
                if (idStat == 2)
                {
                    MessageBox.Show($"Ошибка: Статус усыновить нельзя использовать при редактировании", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Вы действительно хотите выполнить это действие?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            sql.OpenConnection();

                            string updateQuery = $"UPDATE Животное SET " +
                                $"Кличка = '{NameInSheaterTB.Text}', " +
                                $"Возраст = {YearOldInSheaterTB.Text.Replace(',', '.')}, " +
                                $"[Код_пола] = '{genderAnimal}', " +
                                $"Код_категории_животного = {idCat}, " +
                                $"Код_порода = {idVid}, " +
                                $"Код_статус = {idStat}, " +
                                $"Обработка_от_блох = {Convert.ToInt32(BlohChB.IsChecked)}, " +
                                $"Дегельминтизация = {Convert.ToInt32(DegelchB.IsChecked)}, " +
                                $"Прививки = {Convert.ToInt32(PrivivChb.IsChecked)}, " +
                                $"Стерилизация = {Convert.ToInt32(SterizChB.IsChecked)}, " +
                                $"Фотография = '{img}' " +
                                $"WHERE Код_животного = {animalId} ";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, sql.str);
                            updateCommand.ExecuteNonQuery();

                            LoadAnimalsLV();

                            MessageBox.Show("Успех совершенной операции.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            sql.CloseConnection();

                            NameInSheaterTB.Text = "";
                            YearOldInSheaterTB.Text = "";
                            CategInSheaterCB.SelectedItem = null;
                            GanderMInSheaterTB.IsChecked = false;
                            GanderWInSheaterTB.IsChecked = false;
                            VidInSheaterTB.SelectedItem = null;
                            WeaponPers.SelectedItem = null;
                            BlohChB.IsChecked = false;
                            DegelchB.IsChecked = false;
                            PrivivChb.IsChecked = false;
                            SterizChB.IsChecked = false;
                            ImgAnimals.Source = null;
                            editAnimalBtn.Visibility = Visibility.Collapsed;
                            editImgBtn.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
                MessageBox.Show("Выберите животное для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }



        public string imagePath3;
        private void addImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                imagePath3 = openFileDialog.FileName;
                // Проверьте, что выбранный файл является изображением
                if (ImageMethod.IsImageFile(imagePath3))
                {
                    imageFileName = imagePath3;
                    // Создайте объект BitmapImage и установите его в элемент Image
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(imagePath3);
                    imageSource.EndInit();
                    AddImgAnimals.Source = imageSource;
                }
                else
                    MessageBox.Show("Выбранный файл не является изображением.");
            }
        }

        // кнопка добавления животного
        private void addAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            // проверка на заполенность полей
            if (AddNameInSheaterTB.Text == null || AddYearOldInSheaterTB.Text == null || categorySelect == null || vidSelect == null || (AddGanderWInSheaterTB.IsChecked == false && AddGanderMInSheaterTB.IsChecked == false))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idCat = categorySelect.Id;
            int idVid = vidSelect.Id;
            int idStat = 1;
            string img = "";
            int genderAnimal;
            int blohAnimal;
            int degelAnimal;
            int privivAnimal;
            int sterizAnimal;
            // пол
            if (AddGanderWInSheaterTB.IsChecked == true && AddGanderMInSheaterTB.IsChecked == false)
                genderAnimal = 2;
            else
                genderAnimal = 1;

            // Если был выбран новый файл изображения, обновите imageFileName
            if (!string.IsNullOrWhiteSpace(imagePath3))
            {
                img = System.IO.Path.GetFileName(imagePath3);
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageDestinationPath = System.IO.Path.Combine(currentDirectory, "imgAnimals", img);
                if (imageDestinationPath == imagePath3)
                {
                    img = System.IO.Path.GetFileName(imagePath3);
                }
                else
                {
                    img = System.IO.Path.GetFileName(imagePath3);
                    System.IO.File.Copy(imagePath3, imageDestinationPath, true);
                }

                imagePath3 = null;
            }
            else
                MessageBox.Show($"Ошибка: Изображени не выбрано", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите хотите выполнить это действие?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    sql.OpenConnection();

                    string countQuery = "SELECT COUNT(*) FROM Животное";
                    SqlCommand countCommand = new SqlCommand(countQuery, sql.str);
                    int rowCount = (int)countCommand.ExecuteScalar();

                    string inserIntoAniamlQuery = $"INSERT INTO Животное (Код_животного, Кличка, Возраст, [Код_пола], Код_категории_животного, Код_порода, Код_статус, Обработка_от_блох, Дегельминтизация, Прививки, Стерилизация, Фотография) " +
                     $"VALUES ('{rowCount + 1}', " +
                     $"'{AddNameInSheaterTB.Text}', " +
                     $"{AddYearOldInSheaterTB.Text.Replace(',', '.')}, " +
                     $"'{genderAnimal}', " +
                     $"{idCat}, " +
                     $"{idVid}, " +
                     $"{idStat}, " +
                     $"{Convert.ToInt32(AddBlohChB.IsChecked)}, " +
                     $"{Convert.ToInt32(AddDegelchB.IsChecked)}, " +
                     $"{Convert.ToInt32(AddPrivivChb.IsChecked)}, " +
                     $"{Convert.ToInt32(AddSterizChB.IsChecked)}, " +
                     $"'{img}')";
                    SqlCommand inserIntoCommand = new SqlCommand(inserIntoAniamlQuery, sql.str);
                    inserIntoCommand.ExecuteNonQuery();

                    string insertIntoCardQuery = $"INSERT INTO Карточка_животного (Код_карточки, Код_животного, Логин_сотрудника, Дата_поступления) " +
                             $"VALUES ('{rowCount + 1}', " +
                             $"'{rowCount + 1}', " +
                             $"'{log}', " +
                             $"'{currentDate:yyyy-MM-dd}')";
                    SqlCommand insertIntoCommand = new SqlCommand(insertIntoCardQuery, sql.str);
                    insertIntoCommand.ExecuteNonQuery();

                    LoadAnimalsLV();

                    MessageBox.Show("Успех совершенной операции.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    sql.CloseConnection();

                    AddNameInSheaterTB.Text = "";
                    AddYearOldInSheaterTB.Text = "";
                    AddCfaegInSheaterCB.SelectedItem = null;
                    AddGanderMInSheaterTB.IsChecked = false;
                    AddGanderWInSheaterTB.IsChecked = false;
                    AddVidInSheaterTB.SelectedItem = null;
                    AddBlohChB.IsChecked = false;
                    AddDegelchB.IsChecked = false;
                    AddPrivivChb.IsChecked = false;
                    AddSterizChB.IsChecked = false;
                    AddImgAnimals.Source = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
        // кнопка филбтра
        private void FiltrAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            FiltrAnimals filtr = new FiltrAnimals(log);
            filtr.AnimalsFiltered += Filtr_AnimalsFiltered; // Подписываемся на событие
            filtr.Show();
        }

        // Метод-обработчик события
        private void Filtr_AnimalsFiltered(object sender, List<AnimalsItems> filteredData)
        {
            // Обновляем ListView с использованием filteredData
            inSheaterAnimalsLV.ItemsSource = filteredData;
        }

        // при выборе из comboBox записывается id - для редактирования 
        private void CategInSheaterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categorySelect = CategInSheaterCB.SelectedItem as CategoryItems;
            if (categorySelect != null)
            {
                idForAtribOfAnimalCat = categorySelect.Id;
            }
        }

        private void BanFiltrAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadAnimalsLV();
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

        // при выборе из comboBox записывается id - для добавления 
        private void AddCfaegInSheaterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categorySelect = AddCfaegInSheaterCB.SelectedItem as CategoryItems;
            if (categorySelect != null)
            {
                idForAtribOfAnimalCat = categorySelect.Id;

                if (idForAtribOfAnimalCat == 1)
                {
                    vidItems = LoadFromDB.LoadMultAttributes<VidItems>(
                   "SELECT Код_порода, Наименование_порода FROM [Порода] " +
                   "WHERE Категория_животного = 1 ",
                   (id, name) => new VidItems(id, name));
                    AddVidInSheaterTB.ItemsSource = vidItems;
                }
                else if (idForAtribOfAnimalCat == 2)
                {
                    vidItems = LoadFromDB.LoadMultAttributes<VidItems>(
                   "SELECT Код_порода, Наименование_порода FROM [Порода] " +
                   "WHERE Категория_животного = 2 ",
                   (id, name) => new VidItems(id, name));
                    AddVidInSheaterTB.ItemsSource = vidItems;
                }
            }
        }

        // при выборе из comboBox записывается id - для добавления 
        private void AddVidInSheaterTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vidSelect = AddVidInSheaterTB.SelectedItem as VidItems;
            if (vidSelect != null)
            {
                idForAtribOfAnimalVid = vidSelect.Id;
            }
        }
    }
}
