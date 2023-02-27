using Microsoft.Win32;
using PolomkaZakirova.DataBase;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolomkaZakirova.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public Client Client { get; set; }
        public List<Gender> Genders { get; set; }

        public List<Service> Services { get; set; }
        public ClientPage(Client client)
        {
            InitializeComponent();
            Client = client;
            Genders = bd_connection.connection.Gender.ToList();
            Services = bd_connection.connection.Service.ToList();
            if (client.ID == 0)
                btnDelete.Visibility = Visibility.Collapsed;

            DataContext = this;
        }
        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value == true)
            {
                Client.PhotoPath = File.ReadAllBytes(openFileDialog.FileName).ToString();
                ClientImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            bd_connection.connection.Client.Remove(Client);
            bd_connection.connection.SaveChanges();
            NavigationService.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(Client.FirstName))
                stringBuilder.AppendLine("Введите имя");
            if (string.IsNullOrEmpty(Client.LastName))
                stringBuilder.AppendLine("Введите фамилию");
            if (string.IsNullOrEmpty(Client.Phone))
                stringBuilder.AppendLine("Введите телефон");
            if (Client.Gender == null)
                stringBuilder.AppendLine("Выберите пол");

            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString());
                return;
            }

            bd_connection.connection.Client.Add(Client);
            bd_connection.connection.SaveChanges();
            NavigationService.GoBack();
        }

        private void lvServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        //    var service = lvServices.SelectedItem as ClientService;
        //    if (service != null)
        //    {
        //        if (!(bool)(new Windows.ChangeServiceWindow(service)).ShowDialog())
        //        {
        //            Client.ClientServices.Remove(service);
        //            lvServices.ItemsSource = Client.ClientServices;
        //            lvServices.Items.Refresh();
        //        };
        //    }
        }

        private void cbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var service = cbService.SelectedItem as Service;
            if (service != null)
            {
                Client.ClientService.Add(new ClientService { Client = Client, Service = service, StartTime = DateTime.Now });
                lvServices.ItemsSource = Client.ClientService;
                lvServices.Items.Refresh();
            }

        }

        private void cbService_TextChanged(object sender, TextChangedEventArgs e)
        {
            cbService.ItemsSource = Services.Where(p => p.Title.ToLower().Contains(cbService.Text.ToLower())).ToList();
            cbService.IsDropDownOpen = true;
        }
    }
}
