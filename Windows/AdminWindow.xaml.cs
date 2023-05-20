using EduInstitutesApp.Models;
using EduInstitutesApp.Pages;
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

namespace EduInstitutesApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new EduPage());
            Manager.MainFrame = MainFrame;
            
        }

        // Кнопка назад
        private void BtnBackClick(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
        // Кнопка навигации
        // Событие отрисовки страницы
        // Скрываем или показываем кнопку Назад 
        // Скрываем или показываем кнопки Для перехода к остальным страницам
        private void MainFrameContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
                BtnCategories.Visibility = Visibility.Collapsed;
                BtnWorkTime.Visibility = Visibility.Collapsed;
                BtnServices.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnBack.Visibility = Visibility.Collapsed;
                BtnCategories.Visibility = Visibility.Visible;
                BtnWorkTime.Visibility = Visibility.Visible;
                BtnServices.Visibility = Visibility.Visible;
            }
        }

        private void BtnCategoriesClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CategoriesPage());
        }

        private void BtnServicesClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());
        }

        private void BtnWorkTime_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkTimePage());
        }
    }
}
