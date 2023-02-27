using PolomkaZakirova.DataBase;
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
using PolomkaZakirova.DataBase;

namespace PolomkaZakirova.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientListPage.xaml
    /// </summary>
    public partial class ClientListPage : Page
    {
        public List<Client> Clients { get; set; }
        public List<Client> ClientsForFilters { get; set; }
        public List<Gender> Genders { get; set; }
        private int _page = 0;
        private int _count = 10;
        public Dictionary<string, Func<Client, object>> Sortings { get; set; }
        public ClientListPage(String title)
        {
            InitializeComponent();
            title = "Список клиентов";
            Clients = bd_connection.connection.Client.ToList();
            Genders = bd_connection.connection.Gender.ToList();
            
            Sortings = new Dictionary<string, Func<Client, object>>
            {
                {"Фамилия по убыванию", x => x.LastName },
                {"Фамилия по возрастанию", x => x.LastName },
                {"Дата регистрации по убыванию", x => x.RegistrationDate },
                {"Дата регистрации по возрастанию", x => x.RegistrationDate },
            };

            
            DataContext = this;
        }
        private void SetPageNumbers()
        {
            var pageCount = ClientsForFilters.Count % _count == 0 ? ClientsForFilters.Count / _count : ClientsForFilters.Count / _count + 1;
            spPageNumbers.Children.Clear();

            spPageNumbers.Children.Add(new TextBlock { Text = $"<" });
            for (int i = 0; i < pageCount; i++)
            {
                spPageNumbers.Children.Add(new TextBlock { Text = $"{i + 1}" });
            }
            spPageNumbers.Children.Add(new TextBlock { Text = $">" });
            foreach (var child in spPageNumbers.Children)
            {
                (child as UIElement).MouseDown += ClientsListPage_MouseDown;
            }
        }

        private void ClientsListPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pageCount = ClientsForFilters.Count % _count == 0 ? ClientsForFilters.Count / _count : ClientsForFilters.Count / _count + 1;
            var text = (sender as TextBlock).Text;
            if (text == "<")
            {
                if (_page > 0)
                    _page--;
            }
            else if (text == ">")
            {
                if (_page < pageCount - 1)
                    _page++;
            }
            else
            {
                _page = int.Parse(text);
            }
            Filter(false);
        }

        private void Filter(bool FilterChanged)
        {
            if (FilterChanged)
                _page = 0;

            var gender = (cbGender.SelectedItem as Gender);
            var searchText = tbSearch.Text.ToLower();
            var sort = cbSort.SelectedItem as string;

            

            if (gender != null && sort != null)
            {
                

                ClientsForFilters = gender.Client.Where(p => p.FirstName.ToLower().Contains(searchText)).ToList();
                if (sort.Contains("убыванию"))
                    ClientsForFilters.Reverse();

                

                lvClients.ItemsSource = ClientsForFilters.Skip(_page * _count).Take(_count);
                lvClients.Items.Refresh();
                SetPageNumbers();
            }
        }

        private void DataAccess_RefreshList()
        {
            Clients = bd_connection.connection.Client.ToList();
            lvClients.ItemsSource = Clients;
            lvClients.Items.Refresh();
            SetPageNumbers();
        }

        private void lvClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // var client = (Client)lvClients.SelectedItem;
           // if (client != null)
           //     NavigationService.Navigate(new ClientPage(client));
        }



        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter(true);
        }

        private void cbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(true);
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(false);
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
           // NavigationService.Navigate(new ClientPage(new Client { RegistrationDate = DateTime.Now }));
        }
    }
}
