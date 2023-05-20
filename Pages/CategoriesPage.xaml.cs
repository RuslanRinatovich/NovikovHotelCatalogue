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
using EduInstitutesApp.Windows;
using System.Data.Entity;

namespace EduInstitutesApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        List<Category> categories;
        public CategoriesPage()
        {
            InitializeComponent();
        }


        void LoadData()
        {
            try
            {
                DtData.ItemsSource = null;
                //загрузка обновленных данных
                PharmacyDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                categories = PharmacyDBEntities.GetContext().Categories.OrderBy(p => p.CategoryName).ToList();
                DtData.ItemsSource = categories;
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //событие отображения данного Page
            // обновляем данные каждый раз когда активируется этот Page
            if (Visibility == Visibility.Visible)
            {
                LoadData();
            }
        }

        private void DataGridGoodLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                CategoryWindow1 window = new CategoryWindow1(new Category());
                if (window.ShowDialog() == true)
                {
                    PharmacyDBEntities.GetContext().Categories.Add(window.currentItem);
                    PharmacyDBEntities.GetContext().SaveChanges();
                    LoadData();
                    MessageBox.Show("Запись добавлена", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // если ни одного объекта не выделено, выходим
                if (DtData.SelectedItem == null) return;
                // получаем выделенный объект
                Category selected = DtData.SelectedItem as Category;


                CategoryWindow1 window = new CategoryWindow1(
                    new Category
                    {
                        CategoryId = selected.CategoryId,
                        CategoryName = selected.CategoryName,
                    }
                    );

                if (window.ShowDialog() == true)
                {
                    selected = PharmacyDBEntities.GetContext().Categories.Find(window.currentItem.CategoryId);
                    // получаем измененный объект
                    if (selected != null)
                    {

                        selected.CategoryId = window.currentItem.CategoryId;
                        selected.CategoryName = window.currentItem.CategoryName;
                        PharmacyDBEntities.GetContext().Entry(selected).State = EntityState.Modified;
                        PharmacyDBEntities.GetContext().SaveChanges();
                        LoadData();
                        MessageBox.Show("Запись изменена", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }

        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // если ни одного объекта не выделено, выходим
                if (DtData.SelectedItem == null) return;
                // получаем выделенный объект
                MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить запись? ", "Удаление", MessageBoxButton.OKCancel,
MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    Category deletedItem = DtData.SelectedItem as Category;


                    PharmacyDBEntities.GetContext().Pharmacies.Load();
                    var list = PharmacyDBEntities.GetContext().Pharmacies.Local;
                    int k = 0;
                    foreach (Pharmacy item in list)
                    {
                        if (item.CategoryId == deletedItem.CategoryId)
                            k++;
                    }
                    // MessageBox.Show(k.ToString());
                    if (k > 0)
                    {
                        MessageBox.Show("Ошибка удаления, есть связанные записи", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    PharmacyDBEntities.GetContext().Categories.Remove(deletedItem);
                    PharmacyDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Запись удалена", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка, есть связанные записи");
            }
            finally
            {
                LoadData();
            }
        }
    }
}

