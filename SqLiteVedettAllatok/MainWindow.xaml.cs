using Microsoft.Data.Sqlite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqLiteVedettAllatok
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ConnectionString = "Data Source=vedett.db;";
        private SqliteConnection connection;

        private List<Vedett> dataList = [];

        public MainWindow()
        {
            InitializeComponent();
            connection = new SqliteConnection(ConnectionString);
            connection.Open();

            ReadFromDatabase();
            ReadBesoroloas();

        }

        public void ReadFromDatabase()
        {
            string queryText = """
                SELECT vedett_allat.id, vedett_allat.nev, ertek, ev, besorolas.nev AS osztaly
                FROM vedett_allat
                INNER JOIN besorolas ON vedett_allat.besorolas_id = besorolas.id
                """;

            SqliteCommand command = new SqliteCommand(queryText, connection);
            SqliteDataReader reader = command.ExecuteReader();

            dataList = [];

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nev = reader.GetString(1);
                int ertek = reader.GetInt32(2);
                int ev = reader.GetInt32(3);
                string osztaly = reader.GetString(4);
                dataList.Add(new Vedett(id, nev, ertek, ev, osztaly));
            }
            reader.Close();

            dataGrid.ItemsSource = dataList;
        }

        public void ReadBesoroloas()
        {
            string queryText = "Select nev from besorolas";
            SqliteCommand command = new SqliteCommand(queryText, connection);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nev = reader.GetString(0);
                osztalyCbx.Items.Add(nev);
            }
            reader.Close();
        }

        private void deleteBtn(object sender, RoutedEventArgs e)
        {
            List<Vedett> selectedItems = dataGrid.SelectedItems.Cast<Vedett>().ToList();
            if(selectedItems != null)
            {
                foreach (var item in selectedItems)
                {
                    dataList.Remove(item);
                    string deleteText = "DELETE FROM vedett_allat WHERE id = @id";
                    SqliteCommand deleteCommand = new SqliteCommand(deleteText, connection);
                    deleteCommand.Parameters.AddWithValue("@id", item.Id);

                    deleteCommand.ExecuteNonQuery();
                }
                dataGrid.Items.Refresh();
            }
        }

        private void osztalyCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filteredList = dataList.Where(x => x.Osztaly == osztalyCbx.SelectedItem.ToString());
            dataGrid.ItemsSource = filteredList.ToList();


            avgPriceLb.Content = $"Átlagos érték: {filteredList.Average(x => x.Ertek):F0}Ft";
        }

        private void resetBtn(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = dataList;
        }

        private void updateBtn(object sender, RoutedEventArgs e)
        {

            var lista = dataGrid.Items.Cast<Vedett>().ToList();

            var max = lista.Max(x => x.Ertek);
            var maxPriceList = lista.Where(x => x.Ertek == max).ToList();
            maxPriceLb.Content = "Legdrágább állat(ok):";
            maxPriceList.ForEach(x => maxPriceLb.Content += $"\n{x.Nev}");

        }
    }
}