using Microsoft.Maps.MapControl.WPF;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EduInstitutesApp.Models;
using System.IO;
using Microsoft.Win32;

namespace EduInstitutesApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEduPage.xaml
    /// </summary>
    public partial class AddEduPage : Page
    {
        //текущий товар
        private Pharmacy _currentItem = new Pharmacy();
        // путь к файлу
        private string _filePath = null;
        // название текущей главной фотографии
        private string _photoName = null;
        // текущая папка приложения
        private static string _currentDirectory = Directory.GetCurrentDirectory() + @"\Images\";

        class ComboBoxTypeItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
        }

        List<ComboBoxTypeItem> servicesItems = new List<ComboBoxTypeItem>();

        // метод для получения названий
        private string GetComboBoxItemContent(List<ComboBoxTypeItem> items)
        {
            List<string> s = new List<string>();

            foreach (ComboBoxTypeItem item in items)
            {
                if (item.Selected == true)
                {
                    s.Add(item.Name);

                }
            }
            return String.Join(", ", s);
        }
        // загрузка 

        private void LoadServices()
        {
            servicesItems.Clear();
            List<ServicePharmacy> xlist = PharmacyDBEntities.GetContext().ServicePharmacies.Where(i => i.PharmacyId == _currentItem.PharmacyId).ToList();
            List<Service> items = PharmacyDBEntities.GetContext().Services.ToList();

            List<int> datas = new List<int>();
            foreach (ServicePharmacy c in xlist)
            {
                datas.Add(c.ServiceId);
            }


            foreach (Service item in items)
            {
                if (datas.Contains(item.ServiceId))
                    servicesItems.Add(new ComboBoxTypeItem
                    {
                        Id = item.ServiceId,
                        Name = item.ServiceName,
                        Selected = true
                    });
                else
                    servicesItems.Add(new ComboBoxTypeItem
                    {
                        Id = item.ServiceId,
                        Name = item.ServiceName,
                        Selected = false
                    });
            }
            ComboServices.ItemsSource = null;
            ComboServices.ItemsSource = servicesItems;

        }
        //сохраytybt
        void SaveServices()
        {

            List<ServicePharmacy> delItems = PharmacyDBEntities.GetContext().ServicePharmacies.Where(i => i.PharmacyId == _currentItem.PharmacyId).ToList();

            List<ServicePharmacy> saveItems = new List<ServicePharmacy>();
            foreach (ComboBoxTypeItem item in servicesItems)
            {
                if (item.Selected == true)
                {
                    saveItems.Add(new ServicePharmacy
                    {
                        PharmacyId = _currentItem.PharmacyId,
                        ServiceId = item.Id,
                    });
                }
            }
            PharmacyDBEntities.GetContext().ServicePharmacies.RemoveRange(delItems);

            PharmacyDBEntities.GetContext().ServicePharmacies.AddRange(saveItems);

            PharmacyDBEntities.GetContext().SaveChanges();
        }


        // передача в AddClinicPage товара 
        public AddEduPage(Pharmacy selected)
        {
            InitializeComponent();
            // если передано null, то мы добавляем новый товар

            if (selected != null)
            {
                _currentItem = selected;
                TextBoxClinicId.Visibility = Visibility.Hidden;
                TextBlockClinicId.Visibility = Visibility.Hidden;
                int x = selected.PharmacyId;
                // загрузка комплементарных товаров
                MapZel.Children.Clear();

                Location pinLocation = new Location(_currentItem.Latitude, _currentItem.Longitude);

                Pushpin pin = new Pushpin();
                pin.Location = pinLocation;
                pin.ToolTip = $"{_currentItem.PharmacyName}\n{_currentItem.Address}";
                MapZel.Children.Add(pin);
               
                MapZel.Center = pinLocation;
                List<Pharmacy> goods = new List<Pharmacy>();

                _filePath = _currentDirectory + _currentItem.Photo;
            }
            DataContext = _currentItem;
            _photoName = _currentItem.Photo;
            LoadServices();
            //загрузка производителей
            ComboCategory.ItemsSource = PharmacyDBEntities.GetContext().Categories.ToList();
            ComboWorkTime.ItemsSource = PharmacyDBEntities.GetContext().WorkTimes.ToList();
        }

        private void IconSatelliteMode_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MapZel.Mode is AerialMode)
            {
                MapZel.Mode = new RoadMode();
            }
            else { MapZel.Mode = new AerialMode(true); }

        }
        // проверка полей
        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.PharmacyName))
                s.AppendLine("Поле название пустое");

            if (_currentItem.Category == null)
                s.AppendLine("Выберите производителя");


            if (string.IsNullOrWhiteSpace(_photoName))
                s.AppendLine("фото не выбрано пустое");
            return s;
        }

        // сохранение 
        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            StringBuilder _error = CheckFields();
            // если ошибки есть, то выводим ошибки в MessageBox
            // и прерываем выполнение 
            if (_error.Length > 0)
            {
                MessageBox.Show(_error.ToString());
                return;
            }


            // проверка полей прошла успешно
            if (_currentItem.PharmacyId == 0)
            {
                // добавление нового товара
                // формируем новое название файла картинки,
                // так как в папке может быть файл с тем же именем
                string photo = ChangePhotoName();
                // путь куда нужно скопировать файл
                string dest = _currentDirectory + photo;
                File.Copy(_filePath, dest);
                _currentItem.Photo = photo;
                _currentItem.Rate = Convert.ToDouble(RatingBarRate.Value);
                // добавляем товар в БД
                PharmacyDBEntities.GetContext().Pharmacies.Add(_currentItem);

           
                
            }
            try
            {
                if (_filePath != null)
                {

                    string photo = ChangePhotoName();
                    string dest = _currentDirectory + photo;
                    File.Copy(_filePath, dest);
                    _currentItem.Photo = photo;
                }
                _currentItem.Rate = Convert.ToDouble(RatingBarRate.Value);
                // Сохраняем изменения в БД
                PharmacyDBEntities.GetContext().SaveChanges();
                SaveServices();
                MessageBox.Show("Запись Изменена");
                // Возвращаемся на предыдущую форму
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        // загрузка фото 
        private void BtnLoadClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Диалог открытия файла
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                // диалог вернет true, если файл был открыт
                if (op.ShowDialog() == true)
                {
                    // проверка размера файла
                    // по условию файл дожен быть не более 2Мб.
                    FileInfo fileInfo = new FileInfo(op.FileName);
                    if (fileInfo.Length > (1024 * 1024 * 2))
                    {
                        // размер файла меньше 2Мб. Поэтому выбрасывается новое исключение
                        throw new Exception("Размер файла должен быть меньше 2Мб");
                    }
                    ImagePhoto.Source = new BitmapImage(new Uri(op.FileName));
                    _photoName = op.SafeFileName;
                    _filePath = op.FileName;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _filePath = null;
            }
        }
        //подбор имени файла
        string ChangePhotoName()
        {
            string x = _currentDirectory + _photoName;
            string photoname = _photoName;
            int i = 0;
            if (File.Exists(x))
            {
                while (File.Exists(x))
                {
                    i++;
                    x = _currentDirectory + i.ToString() + photoname;
                }
                photoname = i.ToString() + photoname;
            }
            return photoname;

        }
        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Location pinLocation = new Location(55.84589999, 48.5066666);
            MapZel.Center = pinLocation;
        }

        private void IconLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(MapZel);
            Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);

            MapZel.Center = pinLocation;

        }

        private void IconBottom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(MapZel);
            Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);

            MapZel.Center = pinLocation;
        }

        private void IconTop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(MapZel);
            Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);

            MapZel.Center = pinLocation;
        }

        private void IconRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(MapZel);
            Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);

            MapZel.Center = pinLocation;
        }

        private void IconLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            MaterialDesignThemes.Wpf.PackIcon packIcon = sender as MaterialDesignThemes.Wpf.PackIcon;
            packIcon.Opacity = 1;

        }

        private void IconLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            MaterialDesignThemes.Wpf.PackIcon packIcon = sender as MaterialDesignThemes.Wpf.PackIcon;
            packIcon.Opacity = 0.5;
        }

        private void MapZel_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void IconTop_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void MapZel_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MapZel.Children.Clear();

            Point mousePosition = e.GetPosition(MapZel);

            Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            //MapZel.Center = pinLocation;
            MapZel.Children.Add(pin);
            TextBoxLatitude.Text = pinLocation.Latitude.ToString();
            TextBoxLongitude.Text = pinLocation.Longitude.ToString();
            _currentItem.Latitude = pinLocation.Latitude;
            _currentItem.Longitude = pinLocation.Longitude;
        }

        private void MapZel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

          
        }

        private void MapZel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //// MapZel.Children.Clear();

            //Point mousePosition = e.GetPosition(MapZel);

            //Location pinLocation = MapZel.ViewportPointToLocation(mousePosition);
            //Pushpin pin = new Pushpin();
            //pin.Location = pinLocation;
            //MapZel.Center = pinLocation;
            //MapZel.Children.Add(pin);
            //TextBoxLatitude.Text = pinLocation.Latitude.ToString();
            //TextBoxLongitude.Text = pinLocation.Longitude.ToString();
            //_currentItem.Latitude = pinLocation.Latitude;
            //_currentItem.Longitude = pinLocation.Longitude;
            //TextBlockCoords.Text = $"{pinLocation.Latitude}: {pinLocation.Longitude}";
        }
    }
}

