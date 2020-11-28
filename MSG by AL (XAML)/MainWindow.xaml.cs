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
using MySql.Data.MySqlClient;
using ConnectionDB;

namespace MSG_by_AL__XAML_
{
    public partial class MainWindow : Window
    {
        //Создание объекта подключения к БД
        MySqlConnection connection = DBUtils.GetDBConnection();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                MessageBox.Show("Соединение установлено!", "Соединение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
